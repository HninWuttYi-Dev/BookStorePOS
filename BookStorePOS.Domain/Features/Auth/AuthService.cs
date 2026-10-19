using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Domain.Features.RolePermission;
using BookStorePOS.Shared.Models.Auth;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Domain.Features.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IRolePermissionService _rolePermissionService;
    private readonly IPasswordHasher<TblUser> _passwordHasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        IConfiguration configuration,
        IRolePermissionService rolePermissionService,
        IPasswordHasher<TblUser> passwordHasher,
        ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _rolePermissionService = rolePermissionService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<LoginResponseModel> LoginAsync(LoginRequestModel request)
    {
        string rawIdentifier = !string.IsNullOrWhiteSpace(request.Phone) ? request.Phone : (request.Username ?? string.Empty);
        _logger.LogInformation("LoginAsync => Attempting login for identifier: {Identifier}", rawIdentifier);
        try
        {
            if (string.IsNullOrWhiteSpace(rawIdentifier) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new LoginResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.ValidationError,
                    Message = "Phone number and password are required."
                };
            }

            string identifier = rawIdentifier.Trim();
            string normalizedPhone = NormalizeMyanmarPhone(identifier);

            TblUser? user = null;

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                string reqPhone = NormalizeMyanmarPhone(request.Phone);
                user = await _context.TblUsers
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(x =>
                        !x.IsDeleted &&
                        ((!string.IsNullOrEmpty(reqPhone) && x.Phone == reqPhone) || x.Phone == request.Phone.Trim()));
            }
            else
            {
                user = await _context.TblUsers
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(x =>
                        !x.IsDeleted &&
                        ((!string.IsNullOrEmpty(normalizedPhone) && x.Phone == normalizedPhone) ||
                         x.Phone == identifier ||
                         x.Username == identifier));
            }

            if (user is null)
            {
                _logger.LogWarning("LoginAsync => User not found for identifier: {Identifier}", identifier);
                return new LoginResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.Unauthorized,
                    Message = "Invalid phone number or password."
                };
            }

            PasswordVerificationResult verification;
            try
            {
                verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? string.Empty, request.Password);
            }
            catch
            {
                verification = PasswordVerificationResult.Failed;
            }

            // Fallback for legacy plain-text passwords (e.g. unhashed initial demo accounts)
            if (verification == PasswordVerificationResult.Failed && user.PasswordHash == request.Password)
            {
                verification = PasswordVerificationResult.SuccessRehashNeeded;
            }

            if (verification == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("LoginAsync => Password verification failed for identifier: {Identifier}", identifier);
                return new LoginResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.Unauthorized,
                    Message = "Invalid phone number or password."
                };
            }

            if (verification == PasswordVerificationResult.SuccessRehashNeeded)
            {
                _logger.LogInformation("LoginAsync => Upgrading legacy/outdated password hash for user: {Username}", user.Username);
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
                await _context.SaveChangesAsync();
            }

            if (user.Role != null && user.Role.IsDeleted)
            {
                _logger.LogWarning("LoginAsync => User's assigned role is deactivated for identifier: {Identifier}", identifier);
                return new LoginResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.Unauthorized,
                    Message = "User's role is deactivated."
                };
            }

            var roleName = user.Role?.RoleName ?? "";
            var permissions = await _rolePermissionService.GetUserPermissionsAsync(user.UserId);
            var token = GenerateToken(user, roleName, permissions);
            var refreshToken = await GenerateAndSaveRefreshTokenAsync(user.UserId);

            _logger.LogInformation("LoginAsync => User {Username} logged in successfully with role: {RoleName}", user.Username, roleName);

            return new LoginResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Login successful.",
                AccessToken = token,
                RefreshToken = refreshToken,
                Role = roleName,
                Username = user.Username,
                FullName = user.FullName,
                Permissions = permissions
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoginAsync => Error during login for identifier: {Identifier}", rawIdentifier);
            return new LoginResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = $"An error occurred during login: {ex.Message}"
            };
        }
    }

    public async Task<RegisterResponseModel> RegisterCustomerAsync(RegisterRequestModel request)
    {
        _logger.LogInformation("RegisterCustomerAsync => Attempting registration for username: {Username}", request.Username);
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return new RegisterResponseModel { IsSuccess = false, Type = EnumResType.ValidationError, Message = "Username is required." };

            if (string.IsNullOrWhiteSpace(request.FullName))
                return new RegisterResponseModel { IsSuccess = false, Type = EnumResType.ValidationError, Message = "Full Name is required." };

            if (string.IsNullOrWhiteSpace(request.Phone))
                return new RegisterResponseModel { IsSuccess = false, Type = EnumResType.ValidationError, Message = "Phone number is required." };

            if (string.IsNullOrWhiteSpace(request.Password))
                return new RegisterResponseModel { IsSuccess = false, Type = EnumResType.ValidationError, Message = "Password is required." };

            if (request.Username.Contains(' ') || request.Username.Any(char.IsWhiteSpace))
            {
                _logger.LogWarning("RegisterCustomerAsync => Username contains spaces: {Username}", request.Username);
                return new RegisterResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.ValidationError,
                    Message = "Username cannot contain spaces."
                };
            }

            if (request.Username.Trim().Length < 2 || request.Username.Trim().Length > 30)
            {
                return new RegisterResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.ValidationError,
                    Message = "Username must be between 2 and 30 characters long."
                };
            }

            string username = request.Username.Trim();
            string phone = NormalizeMyanmarPhone(request.Phone);
            string? email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

            if (!Regex.IsMatch(phone, @"^09\d{9}$"))
            {
                _logger.LogWarning("RegisterCustomerAsync => Invalid phone number format: {Phone}", request.Phone);
                return new RegisterResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.ValidationError,
                    Message = "Invalid phone number. Please enter a valid Myanmar phone number (e.g. 09789456123)."
                };
            }

            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                _logger.LogWarning("RegisterCustomerAsync => Invalid email format: {Email}", email);
                return new RegisterResponseModel { IsSuccess = false, Type = EnumResType.ValidationError, Message = "Invalid email format." };
            }

            // Uniqueness checks
            bool usernameExists = await _context.TblUsers.AnyAsync(x => x.Username == username && !x.IsDeleted);
            if (usernameExists)
            {
                _logger.LogWarning("RegisterCustomerAsync => Username already exists: {Username}", username);
                return new RegisterResponseModel { IsSuccess = false, Type = EnumResType.Conflict, Message = "Username is already taken." };
            }

            bool phoneExists = await _context.TblUsers.AnyAsync(x => x.Phone == phone && !x.IsDeleted);
            if (phoneExists)
            {
                _logger.LogWarning("RegisterCustomerAsync => Phone already registered: {Phone}", phone);
                return new RegisterResponseModel { IsSuccess = false, Type = EnumResType.Conflict, Message = "Phone number is already registered." };
            }

            if (!string.IsNullOrEmpty(email))
            {
                bool emailExists = await _context.TblUsers.AnyAsync(x => x.Email == email && !x.IsDeleted);
                if (emailExists)
                {
                    _logger.LogWarning("RegisterCustomerAsync => Email already registered: {Email}", email);
                    return new RegisterResponseModel { IsSuccess = false, Type = EnumResType.Conflict, Message = "Email is already registered." };
                }
            }

            // Find or create "Customer" role
            var customerRole = await _context.TblRoles
                .FirstOrDefaultAsync(r => r.RoleName.ToLower() == "customer" && !r.IsDeleted);

            if (customerRole == null)
            {
                _logger.LogInformation("RegisterCustomerAsync => Creating default 'Customer' role in database");
                customerRole = new TblRole
                {
                    RoleName = "Customer",
                    IsDeleted = false,
                    CreateAt = DateTime.Now
                };
                _context.TblRoles.Add(customerRole);
                await _context.SaveChangesAsync();

                // Ensure default customer permissions exist and are mapped
                var defaultPermissions = new[] { "Book.View", "Order.Create", "Order.View" };
                foreach (var permName in defaultPermissions)
                {
                    var perm = await _context.TblPermissions
                        .FirstOrDefaultAsync(p => p.PermissionName == permName && !p.IsDeleted);

                    if (perm == null)
                    {
                        perm = new TblPermission
                        {
                            PermissionName = permName,
                            IsDeleted = false,
                            CreateAt = DateTime.Now
                        };
                        _context.TblPermissions.Add(perm);
                        await _context.SaveChangesAsync();
                    }

                    bool hasLink = await _context.TblRolePermissions.AnyAsync(rp =>
                        rp.RoleId == customerRole.RoleId && rp.PermissionId == perm.PermissionId);

                    if (!hasLink)
                    {
                        _context.TblRolePermissions.Add(new TblRolePermission
                        {
                            RoleId = customerRole.RoleId,
                            PermissionId = perm.PermissionId
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            var newUser = new TblUser
            {
                Username = username,
                FullName = request.FullName.Trim(),
                Phone = phone,
                Email = email,
                RoleId = customerRole.RoleId,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

            _context.TblUsers.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("RegisterCustomerAsync => Customer registered successfully. UserId: {UserId}, Username: {Username}", newUser.UserId, newUser.Username);

            var permissions = await _rolePermissionService.GetUserPermissionsAsync(newUser.UserId);
            var token = GenerateToken(newUser, customerRole.RoleName, permissions);
            var refreshToken = await GenerateAndSaveRefreshTokenAsync(newUser.UserId);

            return new RegisterResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Registration successful.",
                AccessToken = token,
                RefreshToken = refreshToken,
                Role = customerRole.RoleName,
                Username = newUser.Username,
                FullName = newUser.FullName,
                Permissions = permissions
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RegisterCustomerAsync => Error registering customer {Username}", request.Username);
            return new RegisterResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = $"An error occurred while registering: {ex.Message}"
            };
        }
    }

    private string GenerateToken(TblUser user, string roleName, List<string> permissions)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, roleName)
        };

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "BookStorePOS_Dev_SecretKey_AtLeast32Characters!")
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var expireMinutes = Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "15");

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "BookStorePOS",
            audience: _configuration["Jwt:Audience"] ?? "BookStorePOS",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    public async Task<RefreshTokenResponseModel> RefreshTokenAsync(RefreshTokenRequestModel request)
    {
        _logger.LogInformation("RefreshTokenAsync => Processing token refresh request.");
        try
        {
            if (request is null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return new RefreshTokenResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.ValidationError,
                    Message = "Refresh token is required."
                };
            }

            string rawToken = request.RefreshToken.Trim();

            var existingToken = await _context.TblRefreshTokens
                .Include(t => t.User)
                    .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(t => t.Token == rawToken);

            if (existingToken is null)
            {
                _logger.LogWarning("RefreshTokenAsync => Refresh token not found in database.");
                return new RefreshTokenResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.Unauthorized,
                    Message = "Invalid refresh token."
                };
            }

            if (existingToken.RevokeAt is not null)
            {
                _logger.LogWarning("RefreshTokenAsync => Attempted reuse of revoked refresh token for UserId: {UserId}", existingToken.UserId);
                return new RefreshTokenResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.Unauthorized,
                    Message = "Refresh token has been revoked."
                };
            }

            if (existingToken.ExpiresAt <= DateTime.UtcNow)
            {
                _logger.LogWarning("RefreshTokenAsync => Refresh token expired at {ExpiresAt} for UserId: {UserId}", existingToken.ExpiresAt, existingToken.UserId);
                return new RefreshTokenResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.Unauthorized,
                    Message = "Refresh token has expired. Please log in again."
                };
            }

            var user = existingToken.User;
            if (user is null || user.IsDeleted)
            {
                _logger.LogWarning("RefreshTokenAsync => User account is deleted or null for token.");
                return new RefreshTokenResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.Unauthorized,
                    Message = "User account not found or is deactivated."
                };
            }

            if (user.Role != null && user.Role.IsDeleted)
            {
                _logger.LogWarning("RefreshTokenAsync => User role is deactivated for user: {Username}", user.Username);
                return new RefreshTokenResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.Unauthorized,
                    Message = "User role is deactivated."
                };
            }

            // ROTATE: Revoke the old refresh token
            existingToken.RevokeAt = DateTime.UtcNow;

            // ROTATE: Issue a new refresh token for the same user
            var refreshTokenLifetimeDays = Convert.ToDouble(_configuration["Jwt:RefreshTokenExpireDays"] ?? "7");
            var newRefreshTokenValue = Ulid.NewUlid().ToString();
            var newRefreshToken = new TblRefreshToken
            {
                UserId = user.UserId,
                Token = newRefreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenLifetimeDays),
                CreateAt = DateTime.UtcNow,
                RevokeAt = null
            };
            _context.TblRefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            // Generate new short-lived access token with same claims & permissions
            var roleName = user.Role?.RoleName ?? "";
            var permissions = await _rolePermissionService.GetUserPermissionsAsync(user.UserId);
            var newAccessToken = GenerateToken(user, roleName, permissions);

            _logger.LogInformation("RefreshTokenAsync => Token rotated successfully for UserId: {UserId}, Username: {Username}", user.UserId, user.Username);

            return new RefreshTokenResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Token refreshed successfully.",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue,
                Role = roleName,
                Username = user.Username,
                FullName = user.FullName,
                Permissions = permissions
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RefreshTokenAsync => Unexpected error during token refresh");
            return new RefreshTokenResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = $"An error occurred during token refresh: {ex.Message}"
            };
        }
    }

    public async Task<Result> LogoutAsync(string? refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Result.Ok("Logged out successfully.");
            }

            var tokenRecord = await _context.TblRefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken.Trim() && t.RevokeAt == null);

            if (tokenRecord is not null)
            {
                tokenRecord.RevokeAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _logger.LogInformation("LogoutAsync => Revoked refresh token for UserId: {UserId}", tokenRecord.UserId);
            }

            return Result.Ok("Logged out successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogoutAsync => Error revoking refresh token");
            return Result.Fail($"An error occurred during logout: {ex.Message}", EnumResType.SystemError);
        }
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(int userId)
    {
        var refreshTokenLifetimeDays = Convert.ToDouble(_configuration["Jwt:RefreshTokenExpireDays"] ?? "7");
        var tokenValue = Ulid.NewUlid().ToString();
        var refreshToken = new TblRefreshToken
        {
            UserId = userId,
            Token = tokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenLifetimeDays),
            CreateAt = DateTime.UtcNow,
            RevokeAt = null
        };
        _context.TblRefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return tokenValue;
    }

    private static string NormalizeMyanmarPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        string p = phone.Trim().Replace(" ", "").Replace("-", "");
        if (p.StartsWith("+959"))
            p = "09" + p.Substring(4);
        else if (p.StartsWith("959"))
            p = "09" + p.Substring(3);
        else if (p.StartsWith("9") && p.Length == 10)
            p = "0" + p;
        else if (p.Length == 9 && !p.StartsWith("0"))
            p = "09" + p;
        return p;
    }
}

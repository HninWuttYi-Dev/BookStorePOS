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
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Domain.Features.RolePermission;
using BookStorePOS.Shared.Models.Auth;

namespace BookStorePOS.Domain.Features.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IRolePermissionService _rolePermissionService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        IConfiguration configuration,
        IRolePermissionService rolePermissionService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _rolePermissionService = rolePermissionService;
        _logger = logger;
    }

    public async Task<LoginResponseModel> LoginAsync(LoginRequestModel request)
    {
        _logger.LogInformation("LoginAsync => Attempting login for username: {Username}", request.Username);
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new LoginResponseModel
                {
                    isSuccess = false,
                    Message = "Username and password are required."
                };
            }

            var user = await _context.TblUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Username == request.Username.Trim() &&
                    x.PasswordHash == request.Password);

            if (user is null)
            {
                _logger.LogWarning("LoginAsync => Invalid credentials for username: {Username}", request.Username);
                return new LoginResponseModel
                {
                    isSuccess = false,
                    Message = "Invalid username or password."
                };
            }

            if (user.Role != null && user.Role.IsDeleted)
            {
                _logger.LogWarning("LoginAsync => User's assigned role is deactivated for username: {Username}", request.Username);
                return new LoginResponseModel
                {
                    isSuccess = false,
                    Message = "User's role is deactivated."
                };
            }

            var roleName = user.Role?.RoleName ?? "";
            var permissions = await _rolePermissionService.GetUserPermissionsAsync(user.UserId);
            var token = GenerateToken(user, roleName, permissions);

            _logger.LogInformation("LoginAsync => User {Username} logged in successfully with role: {RoleName}", user.Username, roleName);

            return new LoginResponseModel
            {
                isSuccess = true,
                Message = "Login successful.",
                AccessToken = token,
                Role = roleName,
                Permissions = permissions
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoginAsync => Error during login for username: {Username}", request.Username);
            return new LoginResponseModel
            {
                isSuccess = false,
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
                return new RegisterResponseModel { isSuccess = false, Message = "Username is required." };

            if (string.IsNullOrWhiteSpace(request.FullName))
                return new RegisterResponseModel { isSuccess = false, Message = "Full Name is required." };

            if (string.IsNullOrWhiteSpace(request.Phone))
                return new RegisterResponseModel { isSuccess = false, Message = "Phone number is required." };

            if (string.IsNullOrWhiteSpace(request.Password))
                return new RegisterResponseModel { isSuccess = false, Message = "Password is required." };

            string username = request.Username.Trim().Replace(" ", "");
            string phone = request.Phone.Trim().Replace(" ", "").Replace("-", "");
            string? email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

            // Myanmar phone normalization
            if (phone.StartsWith("+959"))
                phone = "09" + phone.Substring(4);
            else if (phone.StartsWith("959"))
                phone = "09" + phone.Substring(3);
            else if (phone.StartsWith("9") && phone.Length == 10)
                phone = "0" + phone;
            else if (phone.Length == 9)
                phone = "09" + phone;

            if (!Regex.IsMatch(phone, @"^09\d{9}$"))
            {
                _logger.LogWarning("RegisterCustomerAsync => Invalid phone number format: {Phone}", request.Phone);
                return new RegisterResponseModel
                {
                    isSuccess = false,
                    Message = "Invalid phone number. Please enter a valid Myanmar phone number (e.g. 09789456123)."
                };
            }

            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                _logger.LogWarning("RegisterCustomerAsync => Invalid email format: {Email}", email);
                return new RegisterResponseModel { isSuccess = false, Message = "Invalid email format." };
            }

            // Uniqueness checks
            bool usernameExists = await _context.TblUsers.AnyAsync(x => x.Username == username && !x.IsDeleted);
            if (usernameExists)
            {
                _logger.LogWarning("RegisterCustomerAsync => Username already exists: {Username}", username);
                return new RegisterResponseModel { isSuccess = false, Message = "Username is already taken." };
            }

            bool phoneExists = await _context.TblUsers.AnyAsync(x => x.Phone == phone && !x.IsDeleted);
            if (phoneExists)
            {
                _logger.LogWarning("RegisterCustomerAsync => Phone already registered: {Phone}", phone);
                return new RegisterResponseModel { isSuccess = false, Message = "Phone number is already registered." };
            }

            if (!string.IsNullOrEmpty(email))
            {
                bool emailExists = await _context.TblUsers.AnyAsync(x => x.Email == email && !x.IsDeleted);
                if (emailExists)
                {
                    _logger.LogWarning("RegisterCustomerAsync => Email already registered: {Email}", email);
                    return new RegisterResponseModel { isSuccess = false, Message = "Email is already registered." };
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
                PasswordHash = request.Password,
                RoleId = customerRole.RoleId,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };

            _context.TblUsers.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("RegisterCustomerAsync => Customer registered successfully. UserId: {UserId}, Username: {Username}", newUser.UserId, newUser.Username);

            var permissions = await _rolePermissionService.GetUserPermissionsAsync(newUser.UserId);
            var token = GenerateToken(newUser, customerRole.RoleName, permissions);

            return new RegisterResponseModel
            {
                isSuccess = true,
                Message = "Registration successful.",
                AccessToken = token,
                Role = customerRole.RoleName,
                Permissions = permissions
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RegisterCustomerAsync => Error registering customer {Username}", request.Username);
            return new RegisterResponseModel
            {
                isSuccess = false,
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

        var expireMinutes = Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "60");

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "BookStorePOS",
            audience: _configuration["Jwt:Audience"] ?? "BookStorePOS",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}

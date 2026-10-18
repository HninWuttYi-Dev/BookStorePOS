using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.User;

namespace BookStorePOS.Domain.Features.User;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext db, ILogger<UserService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<UserModel>> CreateUserAsync(UserCreateRequestModel request)
    {
        _logger.LogInformation("Create User Async => Creating user");
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return Result<UserModel>.ValidationError("Username is required.");

            if (request.Username.Contains(' ') || request.Username.Any(char.IsWhiteSpace))
                return Result<UserModel>.ValidationError("Username cannot contain spaces.");

            if (string.IsNullOrWhiteSpace(request.FullName))
                return Result<UserModel>.ValidationError("Full Name is required.");

            if (string.IsNullOrWhiteSpace(request.Phone))
                return Result<UserModel>.ValidationError("Phone number is required.");

            string username = request.Username.Trim();
            string phone = request.Phone.Trim().Replace(" ", "").Replace("-", "");
            string? email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

            // Normalize Myanmar phone number to 09XXXXXXXXX
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
                return Result<UserModel>.ValidationError("Invalid phone number. Please enter a valid Myanmar phone number.");
            }

            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return Result<UserModel>.ValidationError("Invalid email format.");
            }

            // Validate Username uniqueness
            bool usernameExists = await _db.TblUsers
                .AnyAsync(x => x.Username == username && !x.IsDeleted);
            
            if (usernameExists)
                return Result<UserModel>.Conflict("Username is already taken.");

            // Validate Phone uniqueness
            bool phoneExists = await _db.TblUsers
                .AnyAsync(x => x.Phone == phone && !x.IsDeleted);

            if (phoneExists)
                return Result<UserModel>.Conflict("Phone number is already registered.");

            // Validate Email uniqueness if provided
            if (!string.IsNullOrEmpty(email))
            {
                bool emailExists = await _db.TblUsers
                    .AnyAsync(x => x.Email == email && !x.IsDeleted);

                if (emailExists)
                    return Result<UserModel>.Conflict("Email is already registered.");
            }

            // Password validation for staff account
            string password = !string.IsNullOrWhiteSpace(request.Password) ? request.Password.Trim() 
                            : (!string.IsNullOrWhiteSpace(request.PasswordHash) ? request.PasswordHash.Trim() : "");
            if (string.IsNullOrWhiteSpace(password))
            {
                return Result<UserModel>.ValidationError("Password is required for staff account.");
            }
            if (password.Length < 3)
            {
                return Result<UserModel>.ValidationError("Password must be at least 3 characters.");
            }

            // Role assignment
            int? roleId = request.RoleId;
            string roleName = "Staff";
            if (roleId.HasValue && roleId.Value > 0)
            {
                var role = await _db.TblRoles.FirstOrDefaultAsync(r => r.RoleId == roleId.Value && !r.IsDeleted);
                if (role == null)
                {
                    return Result<UserModel>.ValidationError("Selected role does not exist.");
                }
                if (role.RoleName.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                {
                    return Result<UserModel>.ValidationError("Staff cannot be assigned the Customer system role.");
                }
                roleName = role.RoleName;
            }
            else
            {
                var defaultRole = await _db.TblRoles.FirstOrDefaultAsync(r => r.RoleName == "Staff" && !r.IsDeleted);
                roleId = defaultRole?.RoleId;
                if (defaultRole != null) roleName = defaultRole.RoleName;
            }

            var newUser = new TblUser
            {
                Username = username,
                FullName = request.FullName.Trim(),
                Phone = phone,
                Email = email,
                PasswordHash = password,
                RoleId = roleId,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };

            _db.TblUsers.Add(newUser);
            await _db.SaveChangesAsync();

            var userModel = new UserModel
            {
                UserId = newUser.UserId,
                Username = newUser.Username,
                FullName = newUser.FullName,
                Phone = newUser.Phone,
                Email = newUser.Email,
                RoleId = newUser.RoleId,
                RoleName = roleName,
                IsDeleted = newUser.IsDeleted,
                CreatedAt = newUser.CreatedAt,
                UpdatedAt = newUser.UpdatedAt
            };

            _logger.LogInformation("Create User Async => User created successfully");

            return Result<UserModel>.Success(userModel, "Staff user created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Create User Async => Failed to create user");
            return Result<UserModel>.SystemError("An unexpected error occurred: " + ex.Message);
        }
    }

    public async Task<UserListResponseModel> GetUsersAsync(UserListRequestModel requestModel)
    {
        _logger.LogInformation("Get Users Async => Fetching all users");
        try
        {
            var query = _db.TblUsers
                .AsNoTracking()
                .Include(x => x.Role)
                .Where(x => !x.IsDeleted && (x.Role == null || x.Role.RoleName != "Customer"));

            if (!string.IsNullOrWhiteSpace(requestModel.Search))
            {
                var term = requestModel.Search.Trim().ToLower();
                query = query.Where(x => x.Username.ToLower().Contains(term) 
                                      || x.FullName.ToLower().Contains(term) 
                                      || x.Phone.Contains(term));
            }
            else if (!string.IsNullOrWhiteSpace(requestModel.Username))
            {
                query = query.Where(x => x.Username.Contains(requestModel.Username));
            }

            if (requestModel.RoleId.HasValue && requestModel.RoleId.Value > 0)
            {
                query = query.Where(x => x.RoleId == requestModel.RoleId.Value);
            }

            int count = await query.CountAsync();
            int limit = requestModel.Limit > 0 ? requestModel.Limit : 10;
            int page = requestModel.Page > 0 ? requestModel.Page : 1;
            var totalPages = (int)Math.Ceiling(count / (double)limit);
            if (totalPages == 0) totalPages = 1;

            var users = await query
                .OrderByDescending(x => x.UserId)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(x => new UserModel
                {
                    UserId = x.UserId,
                    Username = x.Username,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    Email = x.Email,
                    RoleId = x.RoleId,
                    RoleName = x.Role != null ? x.Role.RoleName : "Staff",
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            _logger.LogInformation("Get Users Async => Users fetched successfully");
            return new UserListResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Users retrieved successfully.",
                Data = users,
                Page = page,
                Limit = limit,
                Count = count,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Get Users Async => Failed to fetch users");
            return new UserListResponseModel { IsSuccess = false, Type = EnumResType.SystemError, Message = "An unexpected error occurred: " + ex.Message };
        }
    }

    public async Task<Result<UserModel>> GetUserByIdAsync(int userId)
    {
        _logger.LogInformation("Get User Async => Fetching user by Id");
        try
        {
            var user = await _db.TblUsers
                .AsNoTracking()
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted);

            if (user == null)
            {
                _logger.LogWarning("Get User By Id Async => User is not found");
                return Result<UserModel>.NotFound("User not found.");
            }

            var userModel = new UserModel
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role != null ? user.Role.RoleName : "Staff",
                IsDeleted = user.IsDeleted,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            _logger.LogInformation("Get User By Id Async => User fetched successfully");
            return Result<UserModel>.Success(userModel, "User retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Get User By Id Async => Failed to fetch user");
            return Result<UserModel>.SystemError("An unexpected error occurred: " + ex.Message);
        }
    }

    public async Task<Result<UserModel>> UpdateUserAsync(UserUpdateRequestModel request)
    {
        _logger.LogInformation("Update User Async => Updating user");
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return Result<UserModel>.ValidationError("Username is required.");

            if (request.Username.Contains(' ') || request.Username.Any(char.IsWhiteSpace))
                return Result<UserModel>.ValidationError("Username cannot contain spaces.");

            if (string.IsNullOrWhiteSpace(request.FullName))
                return Result<UserModel>.ValidationError("Full Name is required.");

            if (string.IsNullOrWhiteSpace(request.Phone))
                return Result<UserModel>.ValidationError("Phone number is required.");

            var user = await _db.TblUsers.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == request.UserId && !x.IsDeleted);

            if (user == null)
            {
                _logger.LogWarning("Update User Async => User is not found");
                return Result<UserModel>.NotFound("User not found.");
            }

            string username = request.Username.Trim();
            string phone = request.Phone.Trim().Replace(" ", "").Replace("-", "");
            string? email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

            // Normalize Myanmar phone number to 09XXXXXXXXX
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
                return Result<UserModel>.ValidationError("Invalid phone number. Please enter a valid Myanmar phone number.");
            }

            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return Result<UserModel>.ValidationError("Invalid email format.");
            }

            // Validate Username uniqueness (excluding current user)
            bool usernameExists = await _db.TblUsers
                .AnyAsync(x => x.Username == username && x.UserId != request.UserId && !x.IsDeleted);
            
            if (usernameExists)
                return Result<UserModel>.Conflict("Username is already taken.");

            // Validate Phone uniqueness (excluding current user)
            bool phoneExists = await _db.TblUsers
                .AnyAsync(x => x.Phone == phone && x.UserId != request.UserId && !x.IsDeleted);

            if (phoneExists)
                return Result<UserModel>.Conflict("Phone number is already registered.");

            // Validate Email uniqueness if provided (excluding current user)
            if (!string.IsNullOrEmpty(email))
            {
                bool emailExists = await _db.TblUsers
                    .AnyAsync(x => x.Email == email && x.UserId != request.UserId && !x.IsDeleted);

                if (emailExists)
                    return Result<UserModel>.Conflict("Email is already registered.");
            }

            // Update password if provided
            string newPassword = !string.IsNullOrWhiteSpace(request.Password) ? request.Password.Trim() 
                               : (!string.IsNullOrWhiteSpace(request.PasswordHash) ? request.PasswordHash.Trim() : "");
            if (!string.IsNullOrEmpty(newPassword))
            {
                if (newPassword.Length < 3)
                {
                    return Result<UserModel>.ValidationError("New password must be at least 3 characters.");
                }
                user.PasswordHash = newPassword;
            }

            // Update role if provided
            string roleName = user.Role != null ? user.Role.RoleName : "Staff";
            if (request.RoleId.HasValue && request.RoleId.Value > 0)
            {
                var role = await _db.TblRoles.FirstOrDefaultAsync(r => r.RoleId == request.RoleId.Value && !r.IsDeleted);
                if (role != null && !role.RoleName.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                {
                    user.RoleId = request.RoleId.Value;
                    roleName = role.RoleName;
                }
            }

            user.Username = username;
            user.FullName = request.FullName.Trim();
            user.Phone = phone;
            user.Email = email;
            user.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            var userModel = new UserModel
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = roleName,
                IsDeleted = user.IsDeleted,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            _logger.LogInformation("Update User Async => User updated successfully");

            return Result<UserModel>.Success(userModel, "Staff user updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Update User Async => Failed to update user");
            return Result<UserModel>.SystemError("An unexpected error occurred: " + ex.Message);
        }
    }

    public async Task<Result<UserModel>> DeleteUserAsync(int userId)
    {
        _logger.LogInformation("Delete User Async => Deleting user");
        try
        {
            var user = await _db.TblUsers.FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted);
            
            if (user == null)
            {
                _logger.LogWarning("Delete User Async => User is not found");
                return Result<UserModel>.NotFound("User not found.");
            }

            if (user.Username.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                return Result<UserModel>.ValidationError("The system admin account cannot be deleted.");
            }

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.Now;
            
            await _db.SaveChangesAsync();
            
            _logger.LogInformation("Delete User Async => User deleted successfully");

            return Result<UserModel>.Success(null, "Staff user deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Delete User Async => Failed to delete user");
            return Result<UserModel>.SystemError("An unexpected error occurred: " + ex.Message);
        }
    }
}

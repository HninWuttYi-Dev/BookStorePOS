using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using BookStorePOS.Database.AppDbContextModels;
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

    public async Task<UserCreateResponseModel> CreateUserAsync(UserCreateRequestModel request)
    {
        _logger.LogInformation("Create User Async => Creating user");
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return new UserCreateResponseModel { isSuccess = false, Message = "Username is required." };

            if (string.IsNullOrWhiteSpace(request.FullName))
                return new UserCreateResponseModel { isSuccess = false, Message = "Full Name is required." };

            if (string.IsNullOrWhiteSpace(request.Phone))
                return new UserCreateResponseModel { isSuccess = false, Message = "Phone number is required." };

            string username = request.Username.Trim().Replace(" ", "");
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
                return new UserCreateResponseModel { isSuccess = false, Message = "Invalid phone number. Please enter a valid Myanmar phone number." };
            }

            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return new UserCreateResponseModel { isSuccess = false, Message = "Invalid email format." };
            }

            // Validate Username uniqueness
            bool usernameExists = await _db.TblUsers
                .AnyAsync(x => x.Username == username && !x.IsDeleted);
            
            if (usernameExists)
                return new UserCreateResponseModel { isSuccess = false, Message = "Username is already taken." };

            // Validate Phone uniqueness
            bool phoneExists = await _db.TblUsers
                .AnyAsync(x => x.Phone == phone && !x.IsDeleted);

            if (phoneExists)
                return new UserCreateResponseModel { isSuccess = false, Message = "Phone number is already registered." };

            // Validate Email uniqueness if provided
            if (!string.IsNullOrEmpty(email))
            {
                bool emailExists = await _db.TblUsers
                    .AnyAsync(x => x.Email == email && !x.IsDeleted);

                if (emailExists)
                    return new UserCreateResponseModel { isSuccess = false, Message = "Email is already registered." };
            }

            var newUser = new TblUser
            {
                Username = username,
                FullName = request.FullName.Trim(),
                Phone = phone,
                Email = email,
                PasswordHash = request.PasswordHash, // Might be hashed in controller/API later
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
                IsDeleted = newUser.IsDeleted,
                CreatedAt = newUser.CreatedAt,
                UpdatedAt = newUser.UpdatedAt
            };

            _logger.LogInformation("Create User Async => User created successfully");

            return new UserCreateResponseModel
            {
                isSuccess = true,
                Message = "User created successfully.",
                Data = userModel
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Create User Async => Failed to create user");
            return new UserCreateResponseModel { isSuccess = false, Message = "An unexpected error occurred." };
        }
    }

    public async Task<UserListResponseModel> GetUsersAsync()
    {
        _logger.LogInformation("Get Users Async => Fetching all users");
        try
        {
            var users = await _db.TblUsers
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Select(x => new UserModel
                {
                    UserId = x.UserId,
                    Username = x.Username,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    Email = x.Email,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            _logger.LogInformation("Get Users Async => Users fetched successfully");
            return new UserListResponseModel
            {
                isSuccess = true,
                Message = "Users retrieved successfully.",
                Data = users
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Get Users Async => Failed to fetch users");
            return new UserListResponseModel { isSuccess = false, Message = "An unexpected error occurred." };
        }
    }

    public async Task<UserGetByIdResponseModel> GetUserByIdAsync(int userId)
    {
        _logger.LogInformation("Get User Async => Fetching user by Id");
        try
        {
            var user = await _db.TblUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted);

            if (user == null)
            {
                _logger.LogWarning("Get User By Id Async => User is not found");
                return new UserGetByIdResponseModel { isSuccess = false, Message = "User not found." };
            }

            var userModel = new UserModel
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,
                IsDeleted = user.IsDeleted,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            _logger.LogInformation("Get User By Id Async => User fetched successfully");
            return new UserGetByIdResponseModel
            {
                isSuccess = true,
                Message = "User retrieved successfully.",
                Data = userModel
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Get User By Id Async => Failed to fetch user");
            return new UserGetByIdResponseModel { isSuccess = false, Message = "An unexpected error occurred." };
        }
    }

    public async Task<UserUpdateResponseModel> UpdateUserAsync(UserUpdateRequestModel request)
    {
        _logger.LogInformation("Update User Async => Updating user");
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return new UserUpdateResponseModel { isSuccess = false, Message = "Username is required." };

            if (string.IsNullOrWhiteSpace(request.FullName))
                return new UserUpdateResponseModel { isSuccess = false, Message = "Full Name is required." };

            if (string.IsNullOrWhiteSpace(request.Phone))
                return new UserUpdateResponseModel { isSuccess = false, Message = "Phone number is required." };

            var user = await _db.TblUsers.FirstOrDefaultAsync(x => x.UserId == request.UserId && !x.IsDeleted);

            if (user == null)
            {
                _logger.LogWarning("Update User Async => User is not found");
                return new UserUpdateResponseModel { isSuccess = false, Message = "User not found." };
            }

            string username = request.Username.Trim().Replace(" ", "");
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
                return new UserUpdateResponseModel { isSuccess = false, Message = "Invalid phone number. Please enter a valid Myanmar phone number." };
            }

            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return new UserUpdateResponseModel { isSuccess = false, Message = "Invalid email format." };
            }

            // Validate Username uniqueness (excluding current user)
            bool usernameExists = await _db.TblUsers
                .AnyAsync(x => x.Username == username && x.UserId != request.UserId && !x.IsDeleted);
            
            if (usernameExists)
                return new UserUpdateResponseModel { isSuccess = false, Message = "Username is already taken." };

            // Validate Phone uniqueness (excluding current user)
            bool phoneExists = await _db.TblUsers
                .AnyAsync(x => x.Phone == phone && x.UserId != request.UserId && !x.IsDeleted);

            if (phoneExists)
                return new UserUpdateResponseModel { isSuccess = false, Message = "Phone number is already registered." };

            // Validate Email uniqueness if provided (excluding current user)
            if (!string.IsNullOrEmpty(email))
            {
                bool emailExists = await _db.TblUsers
                    .AnyAsync(x => x.Email == email && x.UserId != request.UserId && !x.IsDeleted);

                if (emailExists)
                    return new UserUpdateResponseModel { isSuccess = false, Message = "Email is already registered." };
            }

            user.Username = username;
            user.FullName = request.FullName.Trim();
            user.Phone = phone;
            user.Email = email;
            if (!string.IsNullOrEmpty(request.PasswordHash))
            {
                user.PasswordHash = request.PasswordHash;
            }
            user.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            var userModel = new UserModel
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,
                IsDeleted = user.IsDeleted,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            _logger.LogInformation("Update User Async => User updated successfully");

            return new UserUpdateResponseModel
            {
                isSuccess = true,
                Message = "User updated successfully.",
                Data = userModel
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Update User Async => Failed to update user");
            return new UserUpdateResponseModel { isSuccess = false, Message = "An unexpected error occurred." };
        }
    }

    public async Task<UserDeleteResponseModel> DeleteUserAsync(int userId)
    {
        _logger.LogInformation("Delete User Async => Deleting user");
        try
        {
            var user = await _db.TblUsers.FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted);
            
            if (user == null)
            {
                _logger.LogWarning("Delete User Async => User is not found");
                return new UserDeleteResponseModel { isSuccess = false, Message = "User not found." };
            }

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.Now;
            
            await _db.SaveChangesAsync();
            
            _logger.LogInformation("Delete User Async => User deleted successfully");

            return new UserDeleteResponseModel
            {
                isSuccess = true,
                Message = "User deleted successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Delete User Async => Failed to delete user");
            return new UserDeleteResponseModel { isSuccess = false, Message = "An unexpected error occurred." };
        }
    }
}

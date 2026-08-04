using System;
using System.Collections.Generic;
using System.Linq;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Domain.Models;

namespace BookStorePOS.Domain.Features.UserFeatures;

public class UserService
{
    private readonly AppDbContext _db;
    
    public UserService()
    {
        _db = new AppDbContext();
    }

    public UserListResponseModel GetUsers(UserListRequestModel requestModel)
    {
        try
        {
            var lst = _db.Users.ToList();
            List<UserModel> users = new List<UserModel>();
            foreach (var item in lst)
            {
                UserModel user = new UserModel
                {
                    UserId = item.UserId,
                    Name = item.Name,
                    Email = item.Email,
                    Role = item.Role
                };
                users.Add(user);
            }

            return new UserListResponseModel
            {
                isSuccess = true,
                Message = "Users fetched successfully",
                Data = users
            };
        }
        catch (Exception ex)
        {
            return new UserListResponseModel
            {
                isSuccess = false,
                Message = ex.ToString()
            };
        }
    }

    public UserEditResponseModel GetUser(UserEditRequestModel requestModel)
    {
        try
        {
            var item = _db.Users.FirstOrDefault(x => x.UserId == requestModel.UserId);
            if (item is null)
            {
                return new UserEditResponseModel
                {
                    isSuccess = false,
                    Message = "User is not found"
                };
            }
            return new UserEditResponseModel
            {
                isSuccess = true,
                Message = "User fetched successfully",
                Data = new UserModel
                {
                    UserId = item.UserId,
                    Name = item.Name,
                    Email = item.Email,
                    Role = item.Role
                }
            };
        }
        catch (Exception ex)
        {
            return new UserEditResponseModel
            {
                isSuccess = false,
                Message = ex.ToString()
            };
        }
    }

    public UserCreateResponseModel CreateUser(UserCreateRequestModel requestModel)
    {
        try
        {
            User user = new User
            {
                Name = requestModel.Name,
                Email = requestModel.Email,
                PasswordHash = requestModel.Password,
                Role = requestModel.Role ?? "Customer",
                CreatedAt = DateTime.Now
            };
            _db.Users.Add(user);
            _db.SaveChanges();
            
            return new UserCreateResponseModel
            {
                isSuccess = true,
                Message = "Created new user successfully",
                Data = user.UserId
            };
        }
        catch (Exception ex)
        {
            return new UserCreateResponseModel
            {
                isSuccess = false,
                Message = "Failed to create user: " + ex.Message
            };
        }
    }

    public UserPatchResponseModel UpdateUser(UserPatchRequestModel requestModel)
    {
        try
        {
            var item = _db.Users.FirstOrDefault(x => x.UserId == requestModel.UserId);
            if (item is null)
            {
                return new UserPatchResponseModel
                {
                    isSuccess = false,
                    Message = "User doesn't exist"
                };
            }
            if (!string.IsNullOrEmpty(requestModel.Name))
            {
                item.Name = requestModel.Name;
            }
            if (!string.IsNullOrEmpty(requestModel.Email))
            {
                item.Email = requestModel.Email;
            }
            if (!string.IsNullOrEmpty(requestModel.Password))
            {
                item.PasswordHash = requestModel.Password;
            }
            if (!string.IsNullOrEmpty(requestModel.Role))
            {
                item.Role = requestModel.Role;
            }
            
            _db.SaveChanges();

            return new UserPatchResponseModel
            {
                isSuccess = true,
                Message = "Updated user successfully"
            };
        }
        catch (Exception ex)
        {
            return new UserPatchResponseModel
            {
                isSuccess = false,
                Message = "Failed to update user: " + ex.Message
            };
        }
    }

    public UserDeleteResponseModel DeleteUser(UserDeleteRequestModel requestModel)
    {
        try
        {
            var item = _db.Users.FirstOrDefault(x => x.UserId == requestModel.UserId);
            if (item is null)
            {
                return new UserDeleteResponseModel
                {
                    isSuccess = false,
                    Message = "User is not found"
                };
            }
            _db.Users.Remove(item);
            _db.SaveChanges();
            
            return new UserDeleteResponseModel
            {
                isSuccess = true,
                Message = "User is deleted successfully"
            };
        }
        catch (Exception ex)
        {
            return new UserDeleteResponseModel
            {
                isSuccess = false,
                Message = "Failed to delete user: " + ex.Message
            };
        }
    }
}

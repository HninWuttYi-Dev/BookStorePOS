using System;
using System.Collections.Generic;

namespace BookStorePOS.Shared.Models.User;

public class UserListRequestModel
{
    public string? Username { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}

public class UserListResponseModel
{
    public bool isSuccess { get; set; }
    public string Message { get; set; } = null!;
    public List<UserModel> Data { get; set; } = new List<UserModel>();
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Count { get; set; }
    public int TotalPages { get; set; }
}

public class UserModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

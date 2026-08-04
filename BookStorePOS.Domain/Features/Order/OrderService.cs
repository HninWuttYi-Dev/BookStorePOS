using System;
using System.Collections.Generic;
using System.Linq;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Domain.Models.Order;

namespace BookStorePOS.Domain.Features.Order;

public class OrderService
{
    private readonly AppDbContext _db;

    public OrderService()
    {
        _db = new AppDbContext();
    }

    public OrderListResponseModel GetOrders(OrderListRequestModel requestModel)
    {
        try
        {
            var query = _db.Orders.AsQueryable();

            if (requestModel.UserId.HasValue)
            {
                query = query.Where(o => o.UserId == requestModel.UserId.Value);
            }

            var lst = query.ToList();
            List<OrderModel> orders = new List<OrderModel>();
            foreach (var item in lst)
            {
                orders.Add(new OrderModel
                {
                    OrderId = item.OrderId,
                    UserId = item.UserId,
                    OrderDate = item.OrderDate,
                    TotalPrice = item.TotalPrice
                });
            }

            return new OrderListResponseModel
            {
                isSuccess = true,
                Message = "Orders fetched successfully",
                Data = orders
            };
        }
        catch (Exception ex)
        {
            return new OrderListResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch orders: " + ex.Message
            };
        }
    }

    public OrderEditResponseModel GetOrder(OrderEditRequestModel requestModel)
    {
        try
        {
            var item = _db.Orders.FirstOrDefault(x => x.OrderId == requestModel.OrderId);
            if (item is null)
            {
                return new OrderEditResponseModel
                {
                    isSuccess = false,
                    Message = "Order is not found"
                };
            }
            return new OrderEditResponseModel
            {
                isSuccess = true,
                Message = "Order fetched successfully",
                Data = new OrderModel
                {
                    OrderId = item.OrderId,
                    UserId = item.UserId,
                    OrderDate = item.OrderDate,
                    TotalPrice = item.TotalPrice
                }
            };
        }
        catch (Exception ex)
        {
            return new OrderEditResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch order: " + ex.Message
            };
        }
    }

    public OrderCreateResponseModel CreateOrder(OrderCreateRequestModel requestModel)
    {
        try
        {
            var order = new Database.AppDbContextModels.Order
            {
                UserId = requestModel.UserId,
                OrderDate = DateTime.Now,
                TotalPrice = requestModel.TotalPrice
            };
            _db.Orders.Add(order);
            _db.SaveChanges();

            return new OrderCreateResponseModel
            {
                isSuccess = true,
                Message = "Created new order successfully",
                Data = new OrderModel
                {
                    OrderId = order.OrderId,
                    UserId = order.UserId,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice
                }
            };
        }
        catch (Exception ex)
        {
            return new OrderCreateResponseModel
            {
                isSuccess = false,
                Message = "Failed to create order: " + ex.Message
            };
        }
    }

    public OrderDeleteResponseModel DeleteOrder(OrderDeleteRequestModel requestModel)
    {
        try
        {
            var item = _db.Orders.FirstOrDefault(x => x.OrderId == requestModel.OrderId);
            if (item is null)
            {
                return new OrderDeleteResponseModel
                {
                    isSuccess = false,
                    Message = "Order is not found"
                };
            }

            _db.Orders.Remove(item);
            _db.SaveChanges();

            return new OrderDeleteResponseModel
            {
                isSuccess = true,
                Message = "Order is deleted successfully",
                Data = new OrderModel
                {
                    OrderId = item.OrderId,
                    UserId = item.UserId,
                    OrderDate = item.OrderDate,
                    TotalPrice = item.TotalPrice
                }
            };
        }
        catch (Exception ex)
        {
            return new OrderDeleteResponseModel
            {
                isSuccess = false,
                Message = "Failed to delete order: " + ex.Message
            };
        }
    }
}

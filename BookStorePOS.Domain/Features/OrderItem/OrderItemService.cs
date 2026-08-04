using System;
using System.Collections.Generic;
using System.Linq;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Domain.Models.OrderItem;

namespace BookStorePOS.Domain.Features.OrderItem;

public class OrderItemService
{
    private readonly AppDbContext _db;

    public OrderItemService()
    {
        _db = new AppDbContext();
    }

    public OrderItemListResponseModel GetOrderItemsByOrderId(OrderItemListRequestModel requestModel)
    {
        try
        {
            var lst = _db.OrderItems.Where(oi => oi.OrderId == requestModel.OrderId).ToList();
            List<OrderItemModel> items = new List<OrderItemModel>();
            foreach (var item in lst)
            {
                items.Add(new OrderItemModel
                {
                    OrderItemId = item.OrderItemId,
                    OrderId = item.OrderId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Subtotal = item.Subtotal
                });
            }

            return new OrderItemListResponseModel
            {
                isSuccess = true,
                Message = "Order items fetched successfully",
                Data = items
            };
        }
        catch (Exception ex)
        {
            return new OrderItemListResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch order items: " + ex.Message
            };
        }
    }

    public OrderItemCreateResponseModel AddOrderItem(OrderItemCreateRequestModel requestModel)
    {
        try
        {
            var orderItem = new Database.AppDbContextModels.OrderItem
            {
                OrderId = requestModel.OrderId,
                BookId = requestModel.BookId,
                Quantity = requestModel.Quantity,
                UnitPrice = requestModel.UnitPrice
            };
            _db.OrderItems.Add(orderItem);
            _db.SaveChanges();

            return new OrderItemCreateResponseModel
            {
                isSuccess = true,
                Message = "Added order item successfully",
                Data = new OrderItemModel
                {
                    OrderItemId = orderItem.OrderItemId,
                    OrderId = orderItem.OrderId,
                    BookId = orderItem.BookId,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.UnitPrice,
                    Subtotal = orderItem.Subtotal
                }
            };
        }
        catch (Exception ex)
        {
            return new OrderItemCreateResponseModel
            {
                isSuccess = false,
                Message = "Failed to add order item: " + ex.Message
            };
        }
    }

    public OrderItemDeleteResponseModel RemoveOrderItem(OrderItemDeleteRequestModel requestModel)
    {
        try
        {
            var item = _db.OrderItems.FirstOrDefault(x => x.OrderItemId == requestModel.OrderItemId);
            if (item is null)
            {
                return new OrderItemDeleteResponseModel
                {
                    isSuccess = false,
                    Message = "Order item is not found"
                };
            }

            _db.OrderItems.Remove(item);
            _db.SaveChanges();

            return new OrderItemDeleteResponseModel
            {
                isSuccess = true,
                Message = "Order item is removed successfully",
                Data = new OrderItemModel
                {
                    OrderItemId = item.OrderItemId,
                    OrderId = item.OrderId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Subtotal = item.Subtotal
                }
            };
        }
        catch (Exception ex)
        {
            return new OrderItemDeleteResponseModel
            {
                isSuccess = false,
                Message = "Failed to remove order item: " + ex.Message
            };
        }
    }
}

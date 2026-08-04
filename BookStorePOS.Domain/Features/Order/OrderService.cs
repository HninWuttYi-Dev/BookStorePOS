using System;
using System.Collections.Generic;
using System.Linq;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Domain.Models.Order;
using Microsoft.EntityFrameworkCore;

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

            var lst = query.ToList();
            List<OrderModel> orders = new List<OrderModel>();
            foreach (var item in lst)
            {
                orders.Add(new OrderModel
                {
                    OrderId = item.OrderId,
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
            var item = _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(x => x.OrderId == requestModel.OrderId);
                
            if (item is null)
            {
                return new OrderEditResponseModel
                {
                    isSuccess = false,
                    Message = "Order is not found"
                };
            }

            var orderModel = new OrderModel
            {
                OrderId = item.OrderId,
                OrderDate = item.OrderDate,
                TotalPrice = item.TotalPrice
            };

            foreach(var oi in item.OrderItems)
            {
                orderModel.Items.Add(new OrderItemModel
                {
                    OrderItemId = oi.OrderItemId,
                    OrderId = oi.OrderId,
                    BookId = oi.BookId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal
                });
            }

            return new OrderEditResponseModel
            {
                isSuccess = true,
                Message = "Order fetched successfully",
                Data = orderModel
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
        using var transaction = _db.Database.BeginTransaction();
        try
        {
            var order = new Database.AppDbContextModels.Order
            {
                OrderDate = DateTime.Now,
                TotalPrice = 0 
            };
            
            _db.Orders.Add(order);
            _db.SaveChanges(); 

            decimal orderTotal = 0;
            var orderModelItems = new List<OrderItemModel>();

            foreach (var item in requestModel.Items)
            {
                var book = _db.Books.FirstOrDefault(b => b.BookId == item.BookId && !b.IsDeleted);
                if (book == null)
                {
                    throw new Exception($"Book with ID {item.BookId} not found or is deleted.");
                }

                if (book.StockQuantity < item.Quantity)
                {
                    throw new Exception($"Insufficient stock for Book '{book.Title}'. Available: {book.StockQuantity}");
                }

                book.StockQuantity -= item.Quantity;

                decimal subtotal = book.Price * item.Quantity;
                orderTotal += subtotal;

                var orderItem = new Database.AppDbContextModels.OrderItem
                {
                    OrderId = order.OrderId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = book.Price,
                    Subtotal = subtotal
                };
                
                _db.OrderItems.Add(orderItem);

                orderModelItems.Add(new OrderItemModel
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = book.Price,
                    Subtotal = subtotal
                });
            }

            order.TotalPrice = orderTotal;
            _db.SaveChanges();
            transaction.Commit();

            for(int i = 0; i < orderModelItems.Count; i++)
            {
                var dbItem = _db.OrderItems.FirstOrDefault(oi => oi.OrderId == order.OrderId && oi.BookId == orderModelItems[i].BookId);
                if(dbItem != null) orderModelItems[i].OrderItemId = dbItem.OrderItemId;
                orderModelItems[i].OrderId = order.OrderId;
            }

            return new OrderCreateResponseModel
            {
                isSuccess = true,
                Message = "Created new order successfully",
                Data = new OrderModel
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    Items = orderModelItems
                }
            };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return new OrderCreateResponseModel
            {
                isSuccess = false,
                Message = "Failed to create order: " + ex.Message
            };
        }
    }
}

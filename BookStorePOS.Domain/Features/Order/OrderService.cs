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

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<OrderListResponseModel> GetOrdersAsync(OrderListRequestModel requestModel)
    {
        try
        {
            var lst = await _db.TblOrders.ToListAsync();
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

    public async Task<OrderGetByIdResponseModel> GetOrder(OrderGetByIdRequestModel requestModel)
    {
        try
        {
            var item = await _db.TblOrders
                .Include(o => o.TblOrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(x => x.OrderId == requestModel.OrderId);

            if (item is null)
            {
                return new OrderGetByIdResponseModel
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

            foreach (var oi in item.TblOrderItems)
            {
                orderModel.Items.Add(new OrderItemModel
                {
                    OrderItemId = oi.OrderItemId,
                    OrderId = oi.OrderId,
                    BookId = oi.BookId,
                    BookTitle = oi.Book.Title,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal ?? 0
                });
            }

            return new OrderGetByIdResponseModel
            {
                isSuccess = true,
                Message = "Order fetched successfully",
                Data = orderModel
            };
        }
        catch (Exception ex)
        {
            return new OrderGetByIdResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch order: " + ex.Message
            };
        }
    }

    public async Task<OrderCreateResponseModel> CreateOrder(OrderCreateRequestModel requestModel)
    {
        try
        {
            TblOrder order = new TblOrder
            {
                OrderDate = DateTime.Now,
                TotalPrice = 0
            };

            // 1. Save the order FIRST to generate the OrderId
            _db.TblOrders.Add(order);
            await _db.SaveChangesAsync();

            decimal orderTotal = 0;
            var orderModelItems = new List<OrderItemModel>();

            foreach (var item in requestModel.Items)
            {
                var book = _db.TblBooks.FirstOrDefault(b => b.BookId == item.BookId && !b.IsDeleted);
                if (book == null)
                {
                    throw new Exception($"Book with ID {item.BookId} not found or is deleted.");
                }

                if (book.StockQuantity < item.Quantity)
                {
                    throw new Exception($"Insufficient stock for Book '{book.Title}'. Available: {book.StockQuantity}");
                }

                // Deduct stock
                book.StockQuantity -= item.Quantity;

                decimal subtotal = book.Price * item.Quantity;
                orderTotal += subtotal;

                // Create OrderItem using the generated OrderId
                TblOrderItem orderItem = new TblOrderItem
                {
                    OrderId = order.OrderId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = book.Price,
                    Subtotal = subtotal
                };

                // 2. Add directly to _db exactly as you learned
                _db.TblOrderItems.Add(orderItem);

                orderModelItems.Add(new OrderItemModel
                {
                    BookId = item.BookId,
                    BookTitle = book.Title,
                    Quantity = item.Quantity,
                    UnitPrice = book.Price,
                    Subtotal = subtotal
                });
            }

            // 3. Update the total price and save everything else
            order.TotalPrice = orderTotal;
            await _db.SaveChangesAsync();

            // Populate the OrderItemId for the response
            int i = 0;
            foreach (var modelItem in orderModelItems)
            {
                var dbItem = await _db.TblOrderItems
                            .FirstOrDefaultAsync(oi =>
                             oi.OrderId == order.OrderId
                             && oi.BookId == modelItem.BookId);
                if (dbItem != null)
                {
                    modelItem.OrderItemId = dbItem.OrderItemId;
                }
                modelItem.OrderId = order.OrderId;
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
            return new OrderCreateResponseModel
            {
                isSuccess = false,
                Message = "Failed to create order: " + ex.Message
            };
        }
    }
}

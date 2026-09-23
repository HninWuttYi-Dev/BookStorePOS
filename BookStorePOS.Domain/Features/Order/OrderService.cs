using System;
using System.Collections.Generic;
using System.Linq;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Domain.Models.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.Domain.Features.Order;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    private readonly ILogger<OrderService> _logger;

    public OrderService(AppDbContext db, ILogger<OrderService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<OrderListResponseModel> GetOrdersAsync(OrderListRequestModel requestModel)
    {
        _logger.LogInformation("Get Orders Async => Fetching all orders");
        try
        {
            var lst = await _db.TblOrders
                    .AsNoTracking()
                    .ToListAsync();
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

            _logger.LogInformation("Get Orders Async => Orders fetched successfully");
            return new OrderListResponseModel
            {
                isSuccess = true,
                Message = "Orders fetched successfully",
                Data = orders
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Orders Async => Failed to fetch orders {Message}", ex.Message);
            return new OrderListResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch orders: " + ex.Message
            };
        }
    }

    public async Task<OrderGetByIdResponseModel> GetOrder(OrderGetByIdRequestModel requestModel)
    {
        _logger.LogInformation("Get Order Async => Fetching order by Id");
        try
        {
            var item = await _db.TblOrders
                .AsNoTracking()
                .Include(o => o.TblOrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(x => x.OrderId == requestModel.OrderId);

            if (item is null)
            {
                _logger.LogWarning("Get Order Async => Order is not found");
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

            _logger.LogInformation("Get Order Async => Order fetched successfully");
            return new OrderGetByIdResponseModel
            {
                isSuccess = true,
                Message = "Order fetched successfully",
                Data = orderModel
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Get Order Async => Failed to fetch order {Message}", ex.Message);
            return new OrderGetByIdResponseModel
            {
                isSuccess = false,
                Message = "Failed to fetch order: " + ex.Message
            };
        }
    }

    public async Task<OrderCreateResponseModel> CreateOrder(OrderCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Order Async => Creating order");

        try
        {
            if (requestModel.Items == null || !requestModel.Items.Any())
            {
                _logger.LogWarning("Create Order Async => Order items are required");
                return new OrderCreateResponseModel
                {
                    isSuccess = false,
                    Message = "Order must contain at least one item."
                };
            }

            foreach (var item in requestModel.Items)
            {
                if (item.Quantity <= 0)
                {
                    return new OrderCreateResponseModel
                    {
                        isSuccess = false,
                        Message = "Quantity must be greater than 0."
                    };
                }

                var book = await _db.TblBooks
                    .FirstOrDefaultAsync(b => b.BookId == item.BookId && !b.IsDeleted);

                if (book == null)
                {
                    return new OrderCreateResponseModel
                    {
                        isSuccess = false,
                        Message = $"Book with ID {item.BookId} not found or is deleted."
                    };
                }

                if (book.StockQuantity < item.Quantity)
                {
                    return new OrderCreateResponseModel
                    {
                        isSuccess = false,
                        Message = $"Insufficient stock for '{book.Title}'. Available: {book.StockQuantity}"
                    };
                }
            }

            // 3. Now create the order (all items are valid)
            var order = new TblOrder
            {
                OrderDate = DateTime.Now,
                TotalPrice = 0
            };

            _db.TblOrders.Add(order);
            await _db.SaveChangesAsync();   // get OrderId

            decimal orderTotal = 0;
            var orderModelItems = new List<OrderItemModel>();

            foreach (var item in requestModel.Items)
            {
                var book = await _db.TblBooks
                    .FirstOrDefaultAsync(b => b.BookId == item.BookId && !b.IsDeleted);

                book.StockQuantity -= item.Quantity;

                decimal subtotal = book.Price * item.Quantity;
                orderTotal += subtotal;

                var orderItem = new TblOrderItem
                {
                    OrderId = order.OrderId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = book.Price,
                    Subtotal = subtotal
                };

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

            order.TotalPrice = orderTotal;
            await _db.SaveChangesAsync();

            // optional: fill OrderItemId if you need it

            _logger.LogInformation("Create Order Async => Order is created successfully");
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
            _logger.LogWarning(ex, "Create Order Async => Failed to create order");
            return new OrderCreateResponseModel
            {
                isSuccess = false,
                Message = "Failed to create order: " + ex.Message
            };
        }
    }
}

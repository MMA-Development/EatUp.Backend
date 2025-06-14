﻿using EatUp.Orders.Models;
using System.Linq.Expressions;

namespace EatUp.Orders.DTO
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid VendorId { get; set; }
        public Guid FoodPackageId { get; set; }
        public string FoodPackageTitle { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public string? PaymentId { get; set; }
        public double Price { get; internal set; }
        public int Quantity { get; set; }
        public DateTime? CreatedAt { get; set; }

        public static Expression<Func<Order, OrderDTO>> FromEntity = (order) =>
             new OrderDTO
             {
                 UserId = order.UserId,
                 UserName = order.UserName,
                 VendorId = order.VendorId,
                 FoodPackageId = order.FoodPackageId,
                 FoodPackageTitle = order.FoodPackageTitle,
                 PaymentStatus = order.PaymentStatus,
                 PaymentId = order.PaymentId,
                 Price = order.Price,
                 Quantity = order.Quantity,
                 CreatedAt = order.CreatedAt,
                 Id = order.Id,
             };
    }
}

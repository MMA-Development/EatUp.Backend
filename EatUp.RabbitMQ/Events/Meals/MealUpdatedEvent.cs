﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EatUp.RabbitMQ.Events.Meals
{
    public class MealUpdatedEvent: IEvent
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double OriginalPrice { get; set; }
        public Guid VendorId { get; set; }
    }
}

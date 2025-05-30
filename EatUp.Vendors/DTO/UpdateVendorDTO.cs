﻿using EatUp.RabbitMQ.Events.Vendor;
using EatUp.Vendors.Models;

namespace EatUp.Vendors.DTO
{
    public class UpdateVendorDTO
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public void Merge(Vendor vendorFromDb)
        {
            vendorFromDb.Email = Email;
            vendorFromDb.Name = Name;
            vendorFromDb.Location = new NetTopologySuite.Geometries.Point(Longitude, Latitude)
            {
                SRID = 4326
            };
        }

        public VendorUpdatedEvent ToEvent(Guid id)
        {
            return new VendorUpdatedEvent
            {
                Id = id,
                Name = Name,
                Longitude = Longitude,
                Latitude = Latitude
            };
        }
    }
}

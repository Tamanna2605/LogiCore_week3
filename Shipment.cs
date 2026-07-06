using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Tamanna.Models
{
    public class Shipment
    {
        public int ShipmentId { get; set; }

        public string TrackingId { get; set; }

        public string Origin { get; set; }

        public string Destination { get; set; }

        public string Status { get; set; }

        public DateTime ETA { get; set; }

        public int Progress { get; set; }
        public int ClientId { get; set; }

    }
}
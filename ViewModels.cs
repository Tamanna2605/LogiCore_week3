using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Tamanna.Models
{
    public class DashboardViewModel
    {
        public int TotalShipments { get; set; }
        public int Delivered { get; set; }
        public int InTransit { get; set; }
        public int Pending { get; set; }
        public double SuccessRate { get; set; }
        public double AverageProgress { get; set; }
        public int TotalClients { get; set; }
        public List<Shipment> RecentShipments { get; set; }
    }
}
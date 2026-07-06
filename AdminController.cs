using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tamanna.Models;
using System.Data.SqlClient;
using System.Configuration;

using System.Web.Mvc;

namespace Tamanna.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Temporary login
            if (model.Username == "admin" &&
                model.Password == "admin123")
            {
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid username or password";

            return View(model);
        }
        public ActionResult Dashboard()
        {
            DashboardViewModel model = new DashboardViewModel();
            model.RecentShipments = new List<Shipment>();

            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                // TOTAL SHIPMENTS
                SqlCommand cmdTotal = new SqlCommand(
                    "SELECT COUNT(*) FROM Shipments", con);
                model.TotalShipments = (int)cmdTotal.ExecuteScalar();

                // IN TRANSIT
                SqlCommand cmdTransit = new SqlCommand(
                    "SELECT COUNT(*) FROM Shipments WHERE Status='In Transit'", con);
                model.InTransit = (int)cmdTransit.ExecuteScalar();

                // DELIVERED
                SqlCommand cmdDelivered = new SqlCommand(
                    "SELECT COUNT(*) FROM Shipments WHERE Status='Delivered'", con);
                model.Delivered = (int)cmdDelivered.ExecuteScalar();

                // PENDING
                SqlCommand cmdPending = new SqlCommand(
                    "SELECT COUNT(*) FROM Shipments WHERE Status='Pending'", con);
                model.Pending = (int)cmdPending.ExecuteScalar();
                SqlCommand cmdClients = new SqlCommand(
    "SELECT COUNT(*) FROM Clients", con);

                model.TotalClients =
                    Convert.ToInt32(cmdClients.ExecuteScalar());

                // RECENT SHIPMENTS (THIS NEEDS LOOP)
                SqlCommand cmdRecent = new SqlCommand(
                    "SELECT TOP 5 TrackingId, Origin, Destination, Status FROM Shipments ORDER BY ShipmentId DESC",
                    con);

                SqlDataReader dr = cmdRecent.ExecuteReader();

                while (dr.Read())
                {
                    model.RecentShipments.Add(new Shipment
                    {
                        TrackingId = dr["TrackingId"].ToString(),
                        Origin = dr["Origin"].ToString(),
                        Destination = dr["Destination"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }

                dr.Close();
            }

            return View(model);
        }

        public ActionResult Clients()
        {
            List<Client> list = new List<Client>();

            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Clients", con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new Client
                    {
                        ClientId = Convert.ToInt32(dr["ClientId"]),
                        ClientName = dr["ClientName"].ToString(),
                        CompanyName = dr["CompanyName"].ToString(),
                        Email = dr["Email"].ToString(),
                        Phone = dr["Phone"].ToString(),
                        Address = dr["Address"].ToString()
                    });
                }
            }

            return View(list);
        }

        public ActionResult AddClient()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddClient(Client client)
        {
            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = @"
INSERT INTO Clients
(
    ClientName,
    CompanyName,
    Email,
    Phone,
    Address,
    Password
)
VALUES
(
    @ClientName,
    @CompanyName,
    @Email,
    @Phone,
    @Address,
    @Password
)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ClientName", client.ClientName);
                cmd.Parameters.AddWithValue("@CompanyName", client.CompanyName);
                cmd.Parameters.AddWithValue("@Email", client.Email);
                cmd.Parameters.AddWithValue("@Phone", client.Phone);
                cmd.Parameters.AddWithValue("@Address", client.Address);
                cmd.Parameters.AddWithValue("@Password", client.Password);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Clients");
        }
        [HttpPost]
        public ActionResult EditClient(Client client)
        {
            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = @"
        UPDATE Clients
        SET
            ClientName=@ClientName,
            CompanyName=@CompanyName,
            Email=@Email,
            Phone=@Phone,
            Address=@Address
        WHERE ClientId=@ClientId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ClientId", client.ClientId);
                cmd.Parameters.AddWithValue("@ClientName", client.ClientName);
                cmd.Parameters.AddWithValue("@CompanyName", client.CompanyName);
                cmd.Parameters.AddWithValue("@Email", client.Email);
                cmd.Parameters.AddWithValue("@Phone", client.Phone);
                cmd.Parameters.AddWithValue("@Address", client.Address);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Clients");
        }
        public ActionResult EditClient(int id)
        {
            Client client = new Client();

            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Clients WHERE ClientId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    client.ClientId = Convert.ToInt32(dr["ClientId"]);
                    client.ClientName = dr["ClientName"].ToString();
                    client.CompanyName = dr["CompanyName"].ToString();
                    client.Email = dr["Email"].ToString();
                    client.Phone = dr["Phone"].ToString();
                    client.Address = dr["Address"].ToString();
                }
            }

            return View(client);
        }
        public ActionResult DeleteClient(int id)
        {
            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Clients WHERE ClientId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Clients");
        }
        public ActionResult Shipments()
        {
            List<Shipment> list = new List<Shipment>();

            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Shipments", con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new Shipment
                    {
                        ShipmentId = Convert.ToInt32(dr["ShipmentId"]),
                        TrackingId = dr["TrackingId"].ToString(),
                        Origin = dr["Origin"].ToString(),
                        Destination = dr["Destination"].ToString(),
                        Status = dr["Status"].ToString(),
                        Progress = Convert.ToInt32(dr["Progress"])
                    });
                }
            }

            return View(list);
        }
        public ActionResult AddShipment()
        {
            List<Client> clients = new List<Client>();

            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd =
                    new SqlCommand("SELECT ClientId, CompanyName FROM Clients", con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    clients.Add(new Client
                    {
                        ClientId = Convert.ToInt32(dr["ClientId"]),
                        CompanyName = dr["CompanyName"].ToString()
                    });
                }
            }

            ViewBag.Clients = clients;

            return View();
        }
       
        [HttpPost]
        public ActionResult AddShipment(Shipment shipment)
        {
            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = @"INSERT INTO Shipments
(TrackingId, Origin, Destination,
 Status, ETA, Progress, ClientId)
 VALUES
(@TrackingId, @Origin, @Destination,
 @Status, @ETA, @Progress, @ClientId)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@TrackingId", shipment.TrackingId);
                cmd.Parameters.AddWithValue("@Origin", shipment.Origin);
                cmd.Parameters.AddWithValue("@Destination", shipment.Destination);
                cmd.Parameters.AddWithValue("@Status", shipment.Status);
                cmd.Parameters.AddWithValue("@ETA", shipment.ETA);
                cmd.Parameters.AddWithValue("@Progress", shipment.Progress);
                cmd.Parameters.AddWithValue("@ClientId", shipment.ClientId);
               
                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Shipments");
        }
        public ActionResult Reports()
        {
            DashboardViewModel model = new DashboardViewModel();
            model.RecentShipments = new List<Shipment>();

            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                // Total Shipments
                SqlCommand cmdTotal = new SqlCommand(
                    "SELECT COUNT(*) FROM Shipments", con);
                model.TotalShipments = Convert.ToInt32(cmdTotal.ExecuteScalar());

                // Delivered
                SqlCommand cmdDelivered = new SqlCommand(
                    "SELECT COUNT(*) FROM Shipments WHERE Status='Delivered'", con);
                model.Delivered = Convert.ToInt32(cmdDelivered.ExecuteScalar());

                // In Transit
                SqlCommand cmdTransit = new SqlCommand(
                    "SELECT COUNT(*) FROM Shipments WHERE Status='In Transit'", con);
                model.InTransit = Convert.ToInt32(cmdTransit.ExecuteScalar());

                // Pending
                SqlCommand cmdPending = new SqlCommand(
                    "SELECT COUNT(*) FROM Shipments WHERE Status='Pending'", con);
                model.Pending = Convert.ToInt32(cmdPending.ExecuteScalar());
                SqlCommand cmdClients = new SqlCommand(
    "SELECT COUNT(*) FROM Clients", con);

                model.TotalClients =
                    Convert.ToInt32(cmdClients.ExecuteScalar());

                // Success Rate
                if (model.TotalShipments > 0)
                {
                    model.SuccessRate =
                        (double)model.Delivered * 100 /
                        model.TotalShipments;
                }

                // Average Progress
                SqlCommand cmdAvg = new SqlCommand(
                    "SELECT AVG(CAST(Progress AS FLOAT)) FROM Shipments", con);

                object avg = cmdAvg.ExecuteScalar();

                model.AverageProgress =
                    avg != DBNull.Value ? Convert.ToDouble(avg) : 0;

                // Recent Shipments
                SqlCommand cmdRecent = new SqlCommand(
                    @"SELECT TOP 10 TrackingId,
                     Origin,
                     Destination,
                     Status
              FROM Shipments
              ORDER BY ShipmentId DESC", con);

                SqlDataReader dr = cmdRecent.ExecuteReader();

                while (dr.Read())
                {
                    model.RecentShipments.Add(new Shipment
                    {
                        TrackingId = dr["TrackingId"].ToString(),
                        Origin = dr["Origin"].ToString(),
                        Destination = dr["Destination"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }

                dr.Close();
            }

            return View(model);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }
        public ActionResult DeleteShipment(int id)
        {
            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query =
                    "DELETE FROM Shipments WHERE ShipmentId=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Shipments");
        }
        public ActionResult EditShipment(int id)
        {
            Shipment shipment = new Shipment();

            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Shipments WHERE ShipmentId=@Id", con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    shipment.ShipmentId = Convert.ToInt32(dr["ShipmentId"]);
                    shipment.TrackingId = dr["TrackingId"].ToString();
                    shipment.Origin = dr["Origin"].ToString();
                    shipment.Destination = dr["Destination"].ToString();
                    shipment.Status = dr["Status"].ToString();
                    shipment.ETA = Convert.ToDateTime(dr["ETA"]);
                    shipment.Progress = Convert.ToInt32(dr["Progress"]);
                }
            }

            return View(shipment);
        }
        [HttpPost]
        public ActionResult EditShipment(Shipment shipment)
        {
            string conStr = ConfigurationManager
                .ConnectionStrings["LogiCoreConnection"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = @"
        UPDATE Shipments
        SET TrackingId=@TrackingId,
            Origin=@Origin,
            Destination=@Destination,
            Status=@Status,
            ETA=@ETA,
            Progress=@Progress
        WHERE ShipmentId=@ShipmentId";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ShipmentId", shipment.ShipmentId);
                cmd.Parameters.AddWithValue("@TrackingId", shipment.TrackingId);
                cmd.Parameters.AddWithValue("@Origin", shipment.Origin);
                cmd.Parameters.AddWithValue("@Destination", shipment.Destination);
                cmd.Parameters.AddWithValue("@Status", shipment.Status);
                cmd.Parameters.AddWithValue("@ETA", shipment.ETA);
                cmd.Parameters.AddWithValue("@Progress", shipment.Progress);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Shipments");
        }
    }
}
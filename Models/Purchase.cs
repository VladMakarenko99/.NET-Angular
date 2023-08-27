using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace API.Models
{
    public class Purchase
    {
        [Required(ErrorMessage = "The 'service' field is required.")]
        public Service Service { get; set; }

        [Required(ErrorMessage = "The 'steamId' field is required.")]
        public string SteamId { get; set; }

        public Purchase(Service service, string steamId)
        {
            this.Service = service;
            this.SteamId = steamId;
        }
    }
    public class Order
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "The 'amount' field is required.")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "The 'steamId' field is required.")]
        public string SteamId { get; set; }

        public string Status { get; set; }

        public DateTime Date { get; set; }
        public Order(string Id, double Amount, string SteamId, string Status, DateTime Date)
        {
            this.Id = Id;
            this.Amount = Amount;
            this.SteamId = SteamId;
            this.Status = Status;
            this.Date = Date;
        }
        public static Order CreateWithCurrentDateTimeInKyiv(string Id, double Amount, string SteamId, string Status)
        {
            var currentDateTimeUtc = DateTime.UtcNow;
            return new Order(Id, Amount, SteamId, Status, currentDateTimeUtc);
        }
    }
    public class TopUpPurchase
    {
        [Required(ErrorMessage = "The 'amount' field is required.")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "The 'steamId' field is required.")]
        public string SteamId { get; set; }

        public TopUpPurchase(double amount, string steamId)
        {
            this.Amount = amount;
            this.SteamId = steamId;
        }
    }
}
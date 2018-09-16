using System;

namespace DataDomainService.Models
{
    public partial class VehicleStatusTrans
    {
        public int Id { get; set; }
        public DateTime? PingTime { get; set; }
        public string Status { get; set; }
        public int? VehicleId { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}

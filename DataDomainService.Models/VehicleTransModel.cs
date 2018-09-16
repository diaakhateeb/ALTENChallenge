namespace DataDomainService.Models
{
    public class VehicleTransModel
    {
        public int VehicleId { get; set; }
        public string VehicleCode { get; set; }
        public string VehicleRegistrationNumber { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
    }
}

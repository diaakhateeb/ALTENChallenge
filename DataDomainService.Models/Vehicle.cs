using System.Collections.Generic;

namespace DataDomainService.Models
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            VehicleStatusTrans = new HashSet<VehicleStatusTrans>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string RegNumber { get; set; }
        public int? CustomerId { get; set; }

        public Customer Customer { get; set; }
        public ICollection<VehicleStatusTrans> VehicleStatusTrans { get; set; }
    }
}

using System.Collections.Generic;

namespace DataDomainService.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Vehicle = new HashSet<Vehicle>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public ICollection<Vehicle> Vehicle { get; set; }
    }
}

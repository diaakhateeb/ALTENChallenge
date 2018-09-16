using DataDomainService.Customer.Handlers;
using DataDomainService.Customer.Interfaces;
using DataDomainService.Patterns.Factory.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataDomainService.Customer.UnitTest
{
    [TestClass]
    public class CustomerUnitTest
    {
        private readonly ICustomerContextRepository _dbContextRepo;

        public CustomerUnitTest()
        {
            _dbContextRepo = new CustomerContextFactory<CustomerContextRepository>().GetInstance();
        }

        [TestMethod]
        public void send_signal_to_all_customer_vehicles()
        {
            var vehiclesList = _dbContextRepo.PingVehiclesInQueue(0);
            Assert.AreNotEqual(vehiclesList.Count, 0);
        }

        [TestMethod]
        public void send_signal_to_non_existed_customer_vehicles_return_not_null()
        {
            // customer Id 99 is not existed.
            var vehiclesList = _dbContextRepo.PingVehiclesInQueue(99);
            Assert.IsNotNull(vehiclesList);
        }

        [TestMethod]
        public void get_associated_vehicles_not_null()
        {
            // customer Id 99 is not existed.
            var vehiclesList = _dbContextRepo.GetAssociatedVehicles(99);
            Assert.IsNotNull(vehiclesList);
        }
    }
}

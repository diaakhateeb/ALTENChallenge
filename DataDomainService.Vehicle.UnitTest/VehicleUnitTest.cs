using DataDomainService.Patterns.Factory.Handlers;
using DataDomainService.Vehicle.Handlers;
using DataDomainService.Vehicle.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataDomainService.Vehicle.UnitTest
{
    [TestClass]
    public class VehicleUnitTest
    {
        private readonly IVehicleContextRepository _dbContextRepo;

        public VehicleUnitTest()
        {
            _dbContextRepo = new VehicleContextFactory<VehicleContextRepository>().GetInstance();
        }

        [TestMethod]
        public void send_signal_to_all_vehicles()
        {
            var vehiclesList = _dbContextRepo.PingVehiclesInQueue(0);
            Assert.AreNotEqual(vehiclesList.Count, 0);
        }

        [TestMethod]
        public void send_signal_to_non_existed_vehicle_return_not_null()
        {
            // vehicle Id 60 is not existed.
            var vehiclesList = _dbContextRepo.PingVehiclesInQueue(60);
            Assert.IsNotNull(vehiclesList);
        }

        [TestMethod]
        public void unassociate_vehicles_return_not_null()
        {
            var vehicle = _dbContextRepo.UnassociateVehicle(1);
            Assert.IsNotNull(vehicle);
        }

        [TestMethod]
        public void get_associated_vehicles_not_null_if_no_result()
        {
            var vehiclesList = _dbContextRepo.GetUnassociatedVehicles();
            Assert.IsNotNull(vehiclesList);
        }
    }
}

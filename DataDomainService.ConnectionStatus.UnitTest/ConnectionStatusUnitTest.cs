using DataDomainService.ConnectionStatus.Handlers;
using DataDomainService.ConnectionStatus.Interfaces;
using DataDomainService.Patterns.Factory.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataDomainService.ConnectionStatus.UnitTest
{
    [TestClass]
    public class ConnectionStatusUnitTest
    {
        private readonly IVehiclePingStatusContextRepository _contextRepo;

        public ConnectionStatusUnitTest()
        {
            _contextRepo = new ConnectionStatusFactory<VehiclePingStatusContextRepository>().GetInstance();
        }

        [TestMethod]
        public void ping_vehicle_status_only_on()
        {
            var vehicleList = _contextRepo.PingVehicleInQueue(Enums.VehicleStatusEnum.On);

            vehicleList.ForEach(v => { Assert.AreEqual(v.Status, "On"); });
        }

        [TestMethod]
        public void ping_vehicle_status_only_off()
        {
            var vehicleList = _contextRepo.PingVehicleInQueue(Enums.VehicleStatusEnum.Off);

            vehicleList.ForEach(v => { Assert.AreEqual(v.Status, "Off"); });
        }
    }
}

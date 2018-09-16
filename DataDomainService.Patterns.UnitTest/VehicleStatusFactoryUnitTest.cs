using DataDomainService.Patterns.Factory.Handlers;
using DataDomainService.Vehicle.Handlers;
using DataDomainService.Vehicle.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataDomainService.Patterns.UnitTest
{
    [TestClass]
    public class VehicleStatusFactoryUnitTest
    {
        IVehicleContextRepository _dbContextRepo;

        [TestMethod]
        public void create_vehicle_context_repository_object_using_factory_pattern()
        {
            _dbContextRepo = new VehicleContextFactory<VehicleContextRepository>().GetInstance();

            Assert.IsNotNull(_dbContextRepo);

        }
    }
}

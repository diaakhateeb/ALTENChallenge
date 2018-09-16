using DataDomainService.ConnectionStatus.Handlers;
using DataDomainService.ConnectionStatus.Interfaces;
using DataDomainService.Patterns.Factory.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataDomainService.Patterns.UnitTest
{
    [TestClass]
    public class ConnectionStatusFactoryUnitTest
    {
        IVehiclePingStatusContextRepository _dbContextRepo;

        [TestMethod]
        public void create_connection_status_context_repository_object_using_factory_pattern()
        {
            _dbContextRepo = new ConnectionStatusFactory<VehiclePingStatusContextRepository>().GetInstance();

            Assert.IsNotNull(_dbContextRepo);

        }
    }
}

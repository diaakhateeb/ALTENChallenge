using DataDomainService.ConnectionStatus.Handlers;
using DataDomainService.ConnectionStatus.Interfaces;
using DataDomainService.Models;
using DataDomainService.Patterns.Factory.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQEventBus;
using System.Collections.Generic;

namespace RabbitMqServiceUnitTests
{
    [TestClass]
    public class RabbitMQEventBusUnitTest
    {
        private readonly IVehiclePingStatusContextRepository _contextRepo;
        private readonly MqService _mqServiceBus;

        public RabbitMQEventBusUnitTest()
        {
            _contextRepo = new ConnectionStatusFactory<VehiclePingStatusContextRepository>().GetInstance();
            _mqServiceBus = new MqService();
        }

        [TestMethod]
        public void publish_vehicle_signal_transactions_to_rabbitmq()
        {
            // PingVehicleInQueue pushes signals data to the queue.
            var vehiclesSignalData = _contextRepo.PingVehicleInQueue(_contextRepo.GenerateSignal());
            Assert.IsNotNull(vehiclesSignalData);
        }

        [TestMethod]
        public void subscribe_to_vehicle_signal_transactions_rabbitmq()
        {
            // PingVehicleInQueue pushes signals data to the queue.
            var vehiclesSignalResult = _mqServiceBus.Subscribe<List<VehicleTransModel>>
                ("PingSignal_VehicleStatusTrans");
            Assert.IsNotNull(vehiclesSignalResult);
        }

    }
}

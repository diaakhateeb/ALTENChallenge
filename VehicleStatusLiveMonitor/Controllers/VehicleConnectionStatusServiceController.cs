using DataDomainService.ConnectionStatus.Enums;
using DataDomainService.ConnectionStatus.Handlers;
using DataDomainService.ConnectionStatus.Interfaces;
using DataDomainService.Models;
using DataDomainService.Patterns.Factory.Handlers;
using Logging.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;
using System;
using System.Collections.Generic;

namespace VehicleStatusLiveMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleConnectionStatusServiceController : Controller
    {
        private readonly IVehiclePingStatusContextRepository _contextRepo;
        private readonly MqService _mqServiceBus;
        private readonly CustomLogger _logger;
        public VehicleConnectionStatusServiceController()
        {
            _contextRepo = new ConnectionStatusFactory<VehiclePingStatusContextRepository>().GetInstance();
            _mqServiceBus = new MqService();
            _logger = new CustomLogger();
        }

        [HttpGet("[action]")]
        public IEnumerable<VehicleTransModel> GetConnectionStatusTransById(int status = -1)
        {
            try
            {
                _contextRepo.PingVehicleInQueue((VehicleStatusEnum)status);
                return _mqServiceBus.Subscribe<List<VehicleTransModel>>("PingSignal_VehicleStatusTrans");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message, "VehicleConnectionStatusServiceController");
                return default(IEnumerable<VehicleTransModel>);
            }
        }
    }
}
using DataDomainService.Customer.Handlers;
using DataDomainService.Models;
using DataDomainService.Patterns.Factory.Handlers;
using DataDomainService.Vehicle.Handlers;
using Logging.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VehicleStatusLiveMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleServiceController : Controller
    {
        private readonly VehicleContextRepository _vehicleContextRepo;
        private readonly CustomerContextRepository _customerContextRepo;
        private readonly MqService _mqServiceBus;
        private readonly CustomLogger _logger;
        public VehicleServiceController()
        {
            _vehicleContextRepo = new VehicleContextFactory<VehicleContextRepository>().GetInstance();
            _customerContextRepo = new CustomerContextFactory<CustomerContextRepository>().GetInstance();
            _mqServiceBus = new MqService();
            _logger = new CustomLogger();
        }

        [HttpGet("[action]")]
        public dynamic GetVehicles()
        {
            var vehiclesWithCustomer = new List<dynamic>();

            foreach (var v in _vehicleContextRepo.GenericsDbContext.GetAll())
            {
                vehiclesWithCustomer.Add(new
                {
                    v.Id,
                    v.Code,
                    v.RegNumber,
                    CustomerName = v.CustomerId != null ? _customerContextRepo.GenericsDbContext.Find(v.CustomerId).Name : string.Empty
                });
            }

            return vehiclesWithCustomer;
        }

        [HttpGet("[action]")]
        public IEnumerable<Vehicle> GetVehiclesExt()
        {
            return _vehicleContextRepo.GenericsDbContext.GetAll().Where(v => v.CustomerId != null).ToList();
        }

        [HttpGet("[action]")]
        public Vehicle GetVehicle(string id)
        {
            return _vehicleContextRepo.GenericsDbContext.Find(int.Parse(id));
        }

        [HttpGet("[action]")]
        public IEnumerable<VehicleTransModel> GetVehicleTransById(int id)
        {
            try
            {
                _vehicleContextRepo.PingVehiclesInQueue(id);
                return _mqServiceBus.Subscribe<List<VehicleTransModel>>("Vehicle_VehicleStatusTrans");
            }
            catch (Exception exp)
            {
                _logger.Log(LogLevel.Error, exp.Message, "VehicleServiceController");
                return default(IEnumerable<VehicleTransModel>);
            }
        }

        [HttpGet("[action]")]
        public dynamic AddVehicle(string code, string regNum, string customerId)
        {
            var vehicle = _vehicleContextRepo.GenericsDbContext.Add(new Vehicle
            {
                Code = code,
                RegNumber = regNum,
                CustomerId = int.Parse(customerId)
            });
            _vehicleContextRepo.GenericsDbContext.SaveChanges();

            return new
            {
                vehicle.Id,
                Code = code,
                RegNumber = regNum,
                CustomerId = customerId,
                CustomerName = _customerContextRepo.GenericsDbContext.Find(int.Parse(customerId)).Name
            };
        }

        [HttpGet("[action]")]
        public Vehicle EditVehicle(string id, string code, string regNum, string customerId)
        {
            var vehicle = _vehicleContextRepo.GenericsDbContext.Find(int.Parse(id));
            if (vehicle == null) return default(Vehicle);

            vehicle.Code = code;
            vehicle.RegNumber = regNum;
            vehicle.CustomerId = int.Parse(customerId);

            _vehicleContextRepo.GenericsDbContext.Edit(vehicle);
            _vehicleContextRepo.GenericsDbContext.SaveChanges();

            return vehicle;
        }

        [HttpGet("[action]")]
        public Vehicle DeleteVehicle(string id)
        {
            return _vehicleContextRepo.UnassociateVehicle(int.Parse(id));
        }

        [HttpGet("[action]")]
        public IEnumerable<dynamic> GetUnassociatedVehicles()
        {
            var vehicles = _vehicleContextRepo.GetUnassociatedVehicles();
            var resultList = new List<dynamic>();

            foreach (var v in vehicles)
            {
                resultList.Add(new
                {
                    v.Id,
                    v.Code
                });
            }

            return resultList;
        }

    }
}
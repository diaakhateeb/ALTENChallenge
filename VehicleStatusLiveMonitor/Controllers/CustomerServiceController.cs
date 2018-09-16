using DataDomainService.Customer.Handlers;
using DataDomainService.Customer.Interfaces;
using DataDomainService.Models;
using DataDomainService.Patterns.Factory.Handlers;
using DataDomainService.Vehicle.Handlers;
using DataDomainService.Vehicle.Interfaces;
using Logging.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQEventBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VehicleStatusLiveMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerServiceController : Controller
    {
        private readonly ICustomerContextRepository _dbContextRepo;
        private readonly IVehicleContextRepository _dbVehicleContextRepo;
        private readonly MqService _mqServiceBus;
        private readonly CustomLogger _logger;
        public CustomerServiceController()
        {
            _logger = new CustomLogger();
            _dbContextRepo = new CustomerContextFactory<CustomerContextRepository>().GetInstance();
            _dbVehicleContextRepo = new VehicleContextFactory<VehicleContextRepository>().GetInstance();
            _mqServiceBus = new MqService();
        }
        [HttpGet("[action]")]
        public IEnumerable<Customer> GetCustomers()
        {
            return _dbContextRepo.GenericsDbContext.GetAll();
        }

        [HttpGet("[action]")]
        public Customer GetCustomer(string id)
        {
            return _dbContextRepo.GenericsDbContext.Find(int.Parse(id));
        }

        [HttpGet("[action]")]
        public IEnumerable<VehicleTransModel> GetCustomerTransById(int id = 0)
        {
            try
            {
                _dbContextRepo.PingVehiclesInQueue(id);
                return _mqServiceBus.Subscribe<List<VehicleTransModel>>("Customer_VehicleStatusTrans");
            }
            catch (Exception exp)
            {
                _logger.Log(LogLevel.Error, exp.Message, "CustomerServiceController");
                return default(IEnumerable<VehicleTransModel>);
            }
        }

        [HttpGet("[action]")]
        public Customer AddCustomer(string name, string address)
        {
            var customer = _dbContextRepo.GenericsDbContext.Add(new Customer
            {
                Name = name,
                Address = address
            });

            _dbContextRepo.GenericsDbContext.SaveChanges();
            return customer;
        }

        [HttpPost("[action]")]
        public Customer EditCustomer(object data)
        {
            var custData = (JObject)JsonConvert.DeserializeObject(data.ToString());
            var customer = _dbContextRepo.GenericsDbContext.Find(custData.SelectToken("id").Value<int>());
            if (customer == null) return default(Customer);

            customer.Name = custData.SelectToken("name").Value<string>();
            customer.Address = custData.SelectToken("address").Value<string>();
            _dbContextRepo.GenericsDbContext.SaveChanges();

            using (var db = _dbVehicleContextRepo.GenericsDbContext)
            {
                string[] ids;

                var currVehIds = custData.SelectToken("currentVehiclesIds").Value<string>();
                if (!string.IsNullOrEmpty(currVehIds))
                {
                    ids = JsonConvert.DeserializeObject(currVehIds).ToString().Split(",");
                    foreach (var i in ids)
                    {
                        var vehicle = db.Find(int.Parse(i));
                        if (vehicle != null) vehicle.CustomerId = null;
                    }
                }

                var newVehIds = custData.SelectToken("newVehiclesIds").Value<string>();
                if (!string.IsNullOrEmpty(newVehIds))
                {
                    ids = JsonConvert.DeserializeObject(newVehIds).ToString().Split(",");
                    foreach (var i in ids)
                    {
                        var vehicle = db.Find(int.Parse(i));
                        if (vehicle != null) vehicle.CustomerId = customer.Id;
                    }
                }
                db.SaveChanges();
            }

            return customer;
        }

        [HttpGet("[action]")]
        public Customer DeleteCustomer(string id)
        {
            Debug.WriteLine("inside delete");
            var customer = _dbContextRepo.GenericsDbContext.Find(int.Parse(id));
            Debug.WriteLine(customer);
            if (customer == null) return default(Customer);
            Debug.WriteLine("b4 delete fires");
            _dbContextRepo.GenericsDbContext.Delete(customer);
            _dbContextRepo.GenericsDbContext.SaveChanges();
            return customer;
        }

        [HttpGet("[action]")]
        public IEnumerable<Vehicle> GetAssociatedVehicles(string id)
        {
            return _dbContextRepo.GetAssociatedVehicles(int.Parse(id));
        }
    }
}
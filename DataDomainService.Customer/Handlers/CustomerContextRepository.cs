using DataDomainService.ConnectionStatus.Interfaces;
using DataDomainService.Customer.Interfaces;
using DataDomainService.GenericsContext.Handlers;
using DataDomainService.GenericsContext.Interfaces;
using DataDomainService.Models;
using Logging.Handlers;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataDomainService.Customer.Handlers
{
    /// <summary>
    /// The Customer repository class.
    /// </summary>
    public class CustomerContextRepository : ICustomerContextRepository
    {
        private readonly IGenericsDbContextRepository<Models.Customer> _genericsCustomerDbContextRepo;
        private readonly IGenericsDbContextRepository<Models.Vehicle> _vehicleContextRepo;
        private readonly IGenericsDbContextRepository<VehicleStatusTrans> _genericsVehicleTransDbContextRepo;
        private readonly IVehiclePingStatusContextRepository _genericsConnectionStatusRepo;
        private readonly MqService _serviceBusQueue;
        private readonly CustomLogger _logger;
        /// <summary>
        /// Creates CustomerContextRepository instance.
        /// </summary>
        /// <param name="genericsDbContextRepo">Customer generics database context object.</param>
        /// <param name="vehicleContextRepo">Vehicle generics database context object.</param>
        /// <param name="genericsVehicleTransDbContextRepo">Signal transactions generics database context object.</param>
        /// <param name="genericsConnectionStatusRepo">Vehicle ping signal context object.</param>
        /// <param name="serviceBusQueue">RabbitMQ object</param>
        public CustomerContextRepository(IGenericsDbContextRepository<Models.Customer> genericsDbContextRepo,
            IGenericsDbContextRepository<Models.Vehicle> vehicleContextRepo,
            IGenericsDbContextRepository<VehicleStatusTrans> genericsVehicleTransDbContextRepo,
            IVehiclePingStatusContextRepository genericsConnectionStatusRepo,
            MqService serviceBusQueue)
        {
            _genericsCustomerDbContextRepo = genericsDbContextRepo;
            _vehicleContextRepo = vehicleContextRepo;
            _genericsVehicleTransDbContextRepo = genericsVehicleTransDbContextRepo;
            _genericsConnectionStatusRepo = genericsConnectionStatusRepo;
            _serviceBusQueue = serviceBusQueue;
            _logger = new CustomLogger();
        }
        /// <summary>
        /// Sends checking signal to Customer's vehicle and enqueue the result using RabbitMQ.
        /// </summary>
        /// <param name="cid">Customer Id</param>
        public List<VehicleTransModel> PingVehiclesInQueue(int cid)
        {
            try
            {
                var vehicleTransList = new List<VehicleTransModel>();

                // 1- ping vehicles of this customer/all and get signal statuses back.
                var vehiclesSignalStatuses =
                    cid != 0
                        ? _vehicleContextRepo.GetAll().Where(v => v.CustomerId != null && v.CustomerId == cid)
                        : _vehicleContextRepo.GetAll().Where(v => v.CustomerId != null);

                // 2- post to DB.
                foreach (var v in vehiclesSignalStatuses)
                {
                    var vse = _genericsConnectionStatusRepo.GenerateSignal();
                    _genericsVehicleTransDbContextRepo.Add(new VehicleStatusTrans
                    {
                        PingTime = DateTime.Now,
                        Status = vse.ToString(),
                        VehicleId = v.Id
                    });
                    _genericsVehicleTransDbContextRepo.SaveChanges();
                    vehicleTransList.Add(new VehicleTransModel
                    {
                        VehicleId = v.Id,
                        VehicleCode = v.Code,
                        VehicleRegistrationNumber = v.RegNumber,
                        CustomerId = v.CustomerId,
                        CustomerName = _genericsCustomerDbContextRepo.Find(v.CustomerId).Name,
                        Status = vse.ToString()
                    });
                }

                // Publish list to EventBus queue.
                _serviceBusQueue.Publish("Customer_VehicleStatusTrans", vehicleTransList);

                return vehicleTransList;
            }
            catch (NullReferenceException nullExp)
            {
                _logger.Log(LogLevel.Error, nullExp.Message, nullExp.Source);
                throw;
            }
            catch (ObjectDisposedException objectDisponseExp)
            {
                _logger.Log(LogLevel.Error, objectDisponseExp.Message, objectDisponseExp.Source);
                throw;
            }
            catch (NotSupportedException notSuppExp)
            {
                _logger.Log(LogLevel.Error, notSuppExp.Message, notSuppExp.Source);
                throw;
            }
            catch (Exception exp)
            {
                _logger.Log(LogLevel.Error, exp.Message, exp.Source);
                throw;
            }
        }
        /// <summary>
        /// Gets Customer's Vehicles.
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <returns>Returns Vehicle collection object.</returns>
        public IEnumerable<Models.Vehicle> GetAssociatedVehicles(int customerId)
        {
            return _vehicleContextRepo.GetAll().Where(v => v.CustomerId == customerId).ToList();
        }
        /// <summary>
        /// Customer generics database context object.
        /// </summary>
        public GenericsDbContextRepository<Models.Customer> GenericsDbContext =>
            (GenericsDbContextRepository<Models.Customer>)_genericsCustomerDbContextRepo;
    }
}

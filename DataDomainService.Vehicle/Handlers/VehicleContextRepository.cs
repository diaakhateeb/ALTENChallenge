using DataDomainService.ConnectionStatus.Interfaces;
using DataDomainService.GenericsContext.Handlers;
using DataDomainService.GenericsContext.Interfaces;
using DataDomainService.Models;
using DataDomainService.Vehicle.Interfaces;
using Logging.Handlers;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataDomainService.Vehicle.Handlers
{
    /// <summary>
    /// /// Vehicle repository context class.
    /// </summary>
    public class VehicleContextRepository : IVehicleContextRepository
    {
        private readonly IGenericsDbContextRepository<Models.Vehicle> _genericsVehicleDbContextRepo;
        private readonly IGenericsDbContextRepository<Customer> _genericsCustomerDbContextRepo;
        private readonly IVehiclePingStatusContextRepository _vehiclePingStatusRepository;
        private readonly IGenericsDbContextRepository<VehicleStatusTrans> _genericsVehicleTransDbContextRepo;
        private readonly MqService _serviceBusQueue;
        CustomLogger _logger;
        /// <summary>
        /// Creates VehicleContextRepository instance.
        /// </summary>
        /// <param name="genericsVehicleDbContextRepo">Vehicle generics database context object.</param>
        /// <param name="genericsCustomerDbContextRepo">Customer generics database context object.</param>
        /// <param name="vehiclePingStatusRepository">Signal transactions repository object.</param>
        /// <param name="genericsVehicleTransDbContextRepo">Vehicle transactions generics database context object.</param>
        /// <param name="mqService">RabbitMQ Object.</param>
        public VehicleContextRepository(IGenericsDbContextRepository<Models.Vehicle> genericsVehicleDbContextRepo,
            IGenericsDbContextRepository<Customer> genericsCustomerDbContextRepo,
            IVehiclePingStatusContextRepository vehiclePingStatusRepository,
            IGenericsDbContextRepository<VehicleStatusTrans> genericsVehicleTransDbContextRepo,
            MqService mqService)
        {
            _genericsVehicleDbContextRepo = genericsVehicleDbContextRepo;
            _genericsCustomerDbContextRepo = genericsCustomerDbContextRepo;
            _vehiclePingStatusRepository = vehiclePingStatusRepository;
            _genericsVehicleTransDbContextRepo = genericsVehicleTransDbContextRepo;
            _serviceBusQueue = mqService;
            _logger = new CustomLogger();
        }
        /// <summary>
        /// Sends checking signal to vehicle and enqueue the result using RabbitMQ.
        /// </summary>
        /// <param name="vid">Vehicle Id.</param>
        public List<VehicleTransModel> PingVehiclesInQueue(int vid)
        {
            try
            {
                var vehicleTransList = new List<VehicleTransModel>();

                // 1- ping vehicles and get signal statuses back.
                var vehiclesSignalStatuses =
                    vid != 0
                        ? _genericsVehicleDbContextRepo.GetAll().Where(v => v.CustomerId != null && v.Id == vid)
                        : _genericsVehicleDbContextRepo.GetAll().Where(v => v.CustomerId != null);

                // 2- post to DB.
                foreach (var v in vehiclesSignalStatuses)
                {
                    var vse = _vehiclePingStatusRepository.GenerateSignal();

                    _genericsVehicleTransDbContextRepo.Add(new VehicleStatusTrans
                    {
                        PingTime = DateTime.Now,
                        Status = vse.ToString(),
                        VehicleId = v.Id
                    });
                    _genericsVehicleTransDbContextRepo.SaveChanges();
                    // Prepare object for UI.
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
                _serviceBusQueue.Publish("Vehicle_VehicleStatusTrans", vehicleTransList);

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
        /// Vendor generics operations (CRUD) property.
        /// </summary>
        public GenericsDbContextRepository<Models.Vehicle> GenericsDbContext =>
            (GenericsDbContextRepository<Models.Vehicle>)_genericsVehicleDbContextRepo;
        /// <summary>
        /// Detach Vehicle from Customer.
        /// </summary>
        /// <param name="vid">Vehicle Id.</param>
        /// <returns>Returns Vehicle object.</returns>
        public Models.Vehicle UnassociateVehicle(int vid)
        {
            var vehicle = _genericsVehicleDbContextRepo.Find(vid);
            if (vehicle == null) return default(Models.Vehicle);
            vehicle.CustomerId = null;
            _genericsVehicleDbContextRepo.SaveChanges();

            return vehicle;

        }
        /// <summary>
        /// Gets unassociated vehicles.
        /// </summary>
        /// <returns>Returns collection of unassociated Vehicles.</returns>
        public IEnumerable<Models.Vehicle> GetUnassociatedVehicles()
        {
            return _genericsVehicleDbContextRepo.GetAll().Where(v => v.CustomerId == null).ToList();
        }
    }
}

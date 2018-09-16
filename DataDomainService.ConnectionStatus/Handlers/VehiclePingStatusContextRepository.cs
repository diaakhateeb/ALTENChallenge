using DataDomainService.ConnectionStatus.Enums;
using DataDomainService.ConnectionStatus.Interfaces;
using DataDomainService.GenericsContext.Interfaces;
using DataDomainService.Models;
using Logging.Handlers;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataDomainService.ConnectionStatus.Handlers
{
    /// <summary>
    /// A Vehicle repository that is responsible for sending checking signal.
    /// </summary>
    public class VehiclePingStatusContextRepository : IVehiclePingStatusContextRepository
    {
        private readonly IGenericsDbContextRepository<Customer> _genericsCustomerDbContextRepo;
        private readonly IGenericsDbContextRepository<Vehicle> _genericsVehicleDbContextRepo;
        private readonly IGenericsDbContextRepository<VehicleStatusTrans> _genericsVehicleTransDbContextRepo;
        private readonly MqService _serviceBusQueue;
        CustomLogger _logger;
        /// <summary>
        /// Creates VehiclePingStatusContextRepository instance.
        /// </summary>
        /// <param name="genericsDbContextRepo">Generic database context instance for Customer.</param>
        /// <param name="genericsVehicleDbContextRepo">Generic database context instance for Vehicle.</param>
        /// <param name="genericsVehicleTransDbContextRepo">Generic database context instance for signal transactions.</param>
        /// <param name="serviceBusQueue">RabbitMQ object.</param>
        public VehiclePingStatusContextRepository(IGenericsDbContextRepository<Customer> genericsDbContextRepo,
            IGenericsDbContextRepository<Vehicle> genericsVehicleDbContextRepo,
            IGenericsDbContextRepository<VehicleStatusTrans> genericsVehicleTransDbContextRepo,
            MqService serviceBusQueue)
        {
            _genericsCustomerDbContextRepo = genericsDbContextRepo;
            _genericsVehicleDbContextRepo = genericsVehicleDbContextRepo;
            _genericsVehicleTransDbContextRepo = genericsVehicleTransDbContextRepo;
            _serviceBusQueue = serviceBusQueue;

            _logger = new CustomLogger();
        }

        /// <summary>
        /// Sends checking signal to vehicle and enqueue the result using RabbitMQ.
        /// </summary>
        /// <param name="status">Status Enum value.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// /// <exception cref="ObjectDisposedException"></exception>
        /// /// <exception cref="NotSupportedException"></exception>
        /// /// <exception cref="Exception"></exception>
        public List<VehicleTransModel> PingVehicleInQueue(VehicleStatusEnum status)
        {
            var vehicleTransList = new List<VehicleTransModel>();
            try
            {
                // 1- ping vehicles of this customer/all and get signal statuses back.
                var vehiclesSignalStatuses = _genericsVehicleDbContextRepo.GetAll().Where(v => v.CustomerId != null);

                // 2- post to DB.
                foreach (var v in vehiclesSignalStatuses)
                {
                    var vse = GenerateSignal();
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

                var vehicleTransModels = status.ToString() != "-1"
                    ? vehicleTransList.Where(vt => vt.Status.ToString() == status.ToString()).ToList()
                    : vehicleTransList;

                // Publish list to EventBus queue.
                _serviceBusQueue.Publish("PingSignal_VehicleStatusTrans", vehicleTransModels);

                return vehicleTransModels;
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
        /// Fires Vehicle checking signal and get result.
        /// </summary>
        /// <returns>Signal result enum value.</returns>
        public VehicleStatusEnum GenerateSignal()
        {
            var rand = new Random();
            return (VehicleStatusEnum)rand.Next(0, 2);
        }
    }
}

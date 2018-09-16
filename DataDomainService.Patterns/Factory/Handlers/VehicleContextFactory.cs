using DataDomainService.ConnectionStatus.Handlers;
using DataDomainService.GenericsContext.Handlers;
using DataDomainService.Models;
using DataDomainService.Patterns.Factory.Interfaces;
using DataDomainService.Vehicle.Interfaces;
using Logging.Handlers;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;
using Unity;
using Unity.Exceptions;
using Unity.Injection;

namespace DataDomainService.Patterns.Factory.Handlers
{
    /// <summary>
    /// Vehicle factory interface.
    /// </summary>
    /// <typeparam name="T">Generic type to get instance of.</typeparam>
    public class VehicleContextFactory<T> : IVehicleContextFactory<T> where T : IVehicleContextRepository
    {
        private IUnityContainer _container;
        private CustomLogger _logger;
        /// <summary>
        /// Gets instance of the specified generic type.
        /// </summary>
        /// <returns>Returns specified type object.</returns>
        /// <exception cref="ResolutionFailedException"></exception>
        public T GetInstance()
        {
            try
            {
                if (_container != null) return (T)_container.Resolve<IVehicleContextRepository>();

                _container = new UnityContainer();
                var dbContextObject = new ALTENStockholmChallengeContext();
                var vehicleGenericsContextRepoObject = new GenericsDbContextRepository<Models.Vehicle>(dbContextObject);
                var customerGenericsContextRepoObject = new GenericsDbContextRepository<Models.Customer>(dbContextObject);
                var vehicleStatusTransGenericsContextRepoObject = new GenericsDbContextRepository<Models.VehicleStatusTrans>(dbContextObject);
                var mqService = new MqService();
                var genericsConnectionStatusRepo = new VehiclePingStatusContextRepository(
                    customerGenericsContextRepoObject,
                    vehicleGenericsContextRepoObject,
                    vehicleStatusTransGenericsContextRepoObject,
                    mqService);

                _container.RegisterType<IVehicleContextRepository, T>(new InjectionConstructor(
                    vehicleGenericsContextRepoObject,
                    customerGenericsContextRepoObject,
                    genericsConnectionStatusRepo,
                    vehicleStatusTransGenericsContextRepoObject,
                    mqService));

                return (T)_container.Resolve<IVehicleContextRepository>();
            }
            catch (ResolutionFailedException resFailExp)
            {
                _logger = new CustomLogger();
                _logger.Log(LogLevel.Error, resFailExp.Message, "VehicleContextFactory..." + typeof(T));
                return default(T);
            }
        }
    }
}

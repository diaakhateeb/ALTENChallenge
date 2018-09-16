using DataDomainService.ConnectionStatus.Handlers;
using DataDomainService.Customer.Interfaces;
using DataDomainService.GenericsContext.Handlers;
using DataDomainService.Models;
using DataDomainService.Patterns.Factory.Interfaces;
using Logging.Handlers;
using Microsoft.Extensions.Logging;
using RabbitMQEventBus;
using Unity;
using Unity.Exceptions;
using Unity.Injection;

namespace DataDomainService.Patterns.Factory.Handlers
{
    /// <summary>
    /// Customer factory interface.
    /// </summary>
    /// <typeparam name="T">Generic type to get instance of.</typeparam>
    public class CustomerContextFactory<T> : ICustomerContextFactory<T> where T : ICustomerContextRepository
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
                if (_container != null) return (T)_container.Resolve<ICustomerContextRepository>();

                _container = new UnityContainer();
                var dbContextObject = new ALTENStockholmChallengeContext();
                var customerGenericsContextRepoObject = new GenericsDbContextRepository<Models.Customer>(dbContextObject);
                var vehicleGenericsContextRepoObject = new GenericsDbContextRepository<Models.Vehicle>(dbContextObject);
                var vehicleStatusTransGenericsContextRepoObject = new GenericsDbContextRepository<Models.VehicleStatusTrans>(dbContextObject);
                var mqService = new MqService();
                var genericsConnectionStatusRepo = new VehiclePingStatusContextRepository(
                    customerGenericsContextRepoObject,
                    vehicleGenericsContextRepoObject,
                    vehicleStatusTransGenericsContextRepoObject,
                    mqService);

                _container.RegisterType<ICustomerContextRepository, T>(
                    new InjectionConstructor(customerGenericsContextRepoObject,
                        vehicleGenericsContextRepoObject, vehicleStatusTransGenericsContextRepoObject,
                        genericsConnectionStatusRepo, mqService));

                return (T)_container.Resolve<ICustomerContextRepository>();
            }
            catch (ResolutionFailedException resFailExp)
            {
                _logger = new CustomLogger();
                _logger.Log(LogLevel.Error, resFailExp.Message, "CustomerContextFactory..." + typeof(T));
                return default(T);
            }
        }
    }
}

using DataDomainService.GenericsContext.Handlers;
using DataDomainService.Models;
using System.Collections.Generic;

namespace DataDomainService.Customer.Interfaces
{
    /// <summary>
    /// Customer repository context interface.
    /// </summary>
    public interface ICustomerContextRepository
    {
        /// <summary>
        /// Sends checking signal to customer's vehicle and enqueue the result using RabbitMQ.
        /// </summary>
        /// <param name="cid">Customer Id.</param>
        List<VehicleTransModel> PingVehiclesInQueue(int cid);
        /// <summary>
        /// Customer generics operations (CRUD) property.
        /// </summary>
        GenericsDbContextRepository<Models.Customer> GenericsDbContext { get; }
        /// <summary>
        /// Gets specific Customer Vehicles.
        /// </summary>
        /// <param name="customerId">Customer Id.</param>
        /// <returns>Returns Vehicle collection object</returns>
        IEnumerable<Models.Vehicle> GetAssociatedVehicles(int customerId);
    }
}

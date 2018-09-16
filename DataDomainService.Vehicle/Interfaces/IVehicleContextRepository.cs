using DataDomainService.GenericsContext.Handlers;
using DataDomainService.Models;
using System.Collections.Generic;

namespace DataDomainService.Vehicle.Interfaces
{
    /// <summary>
    /// Vehicle repository context interface.
    /// </summary>
    public interface IVehicleContextRepository
    {
        /// <summary>
        /// Sends checking signal to vehicle and enqueue the result using RabbitMQ.
        /// </summary>
        /// <param name="vid">Vehicle Id.</param>
        List<VehicleTransModel> PingVehiclesInQueue(int vid);
        /// <summary>
        /// Vendor generics operations (CRUD) property.
        /// </summary>
        GenericsDbContextRepository<Models.Vehicle> GenericsDbContext { get; }
        /// <summary>
        /// Detach Vehicle from Customer.
        /// </summary>
        /// <param name="vid">Vehicle Id.</param>
        /// <returns>Returns Vehicle object.</returns>
        Models.Vehicle UnassociateVehicle(int vid);
        /// <summary>
        /// Gets unassociated vehicles.
        /// </summary>
        /// <returns>Returns collection of unassociated Vehicles.</returns>
        IEnumerable<Models.Vehicle> GetUnassociatedVehicles();
    }
}
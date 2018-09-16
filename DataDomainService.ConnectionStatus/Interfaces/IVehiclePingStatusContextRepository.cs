using DataDomainService.ConnectionStatus.Enums;
using DataDomainService.Models;
using System.Collections.Generic;

namespace DataDomainService.ConnectionStatus.Interfaces
{
    /// <summary>
    /// Vehicle ping status Interface.
    /// </summary>
    public interface IVehiclePingStatusContextRepository
    {
        /// <summary>
        /// Sends checking signal to vehicle and enqueue the result using RabbitMQ.
        /// </summary>
        /// <param name="status">Status Enum value.</param>
        List<VehicleTransModel> PingVehicleInQueue(VehicleStatusEnum status);
        /// <summary>
        /// Fires Vehicle checking signal and get result.
        /// </summary>
        /// <returns>Signal result enum value.</returns>
        VehicleStatusEnum GenerateSignal();
    }
}

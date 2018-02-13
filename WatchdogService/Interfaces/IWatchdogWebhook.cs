using Microsoft.ServiceFabric.WatchdogService.Models;
using System;
using System.Collections.Generic;
using System.Fabric.Health;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.WatchdogService.Interfaces
{
    public interface IWatchdogWebhook
    {
        /// <summary>
        /// Gets or sets the Webhook URL.
        /// </summary>
        string WebhookUrl { get; set; }

        /// <summary>
        /// Gets or sets the timeout period for alerts
        /// </summary>
        TimeSpan AlertTimeout { get; set; }

        /// <summary>
        /// Gets or sets the Cluster Alert Status to prevent repeating the same alerts
        /// </summary>
        Dictionary<string, HealthDataAlertState> ClusterAlertStatus { get; set; }

        /// <summary>
        /// Gets or sets the Node Alert Status to prevent repeating the same alerts
        /// </summary>
        Dictionary<string, HealthDataAlertState> NodeAlertStatus { get; set; }

        /// <summary>
        /// Gets or sets the Application Alert Status to prevent repeating the same alerts
        /// </summary>
        Dictionary<string, HealthDataAlertState> ApplicationAlertStatus { get; set; }

        /// <summary>
        /// Sends specific error message to webhook for integration
        /// </summary>
        /// <param name="cluster">Cluster where the error is happening</param>
        /// <param name="healthState">Health State for cluster</param>
        /// <param name="cancellationToken">CancellationToken instance</param>
        Task ReportClusterError(string cluster, HealthState healthState, CancellationToken cancellationToken);

        /// <summary>
        /// Sends specific error message to webhook for integration
        /// </summary>
        /// <param name="cluster">Cluster where the error is happening</param>
        /// <param name="node">Node where the error is happening</param>
        /// <param name="healthState">Health State for specific node</param>
        /// <param name="cancellationToken">CancellationToken instance</param>
        Task ReportNodeError(string cluster, string node, HealthState healthState, CancellationToken cancellationToken);

        /// <summary>
        /// Sends specific error message to webhook for integration
        /// </summary>
        /// <param name="cluster">Cluster where the error is happening</param>
        /// <param name="node">Node where the error is happening</param>
        /// <param name="application">Application where the error is happening</param>
        /// <param name="healthState">Health State for specific node</param>
        /// <param name="cancellationToken">CancellationToken instance</param>
        /// <returns></returns>
        Task ReportApplicationError(string cluster,
            string node,
            string application,
            HealthState healthState,
            CancellationToken cancellationToken);

        /// <summary>
        /// Makes a POST request to the specific Webhook endpoint
        /// </summary>
        /// <param name="healthData">Custom class that contains all the properties needed to create an alert</param>
        Task PostMessage(HealthData healthData);
    }
}

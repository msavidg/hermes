using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.ServiceProcess;
using System.Threading.Tasks;
using Hermes.Common.Interfaces;
using NServiceBus;

namespace Hermes.EndpointLoadBalancer.Service
{
    static class Program
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;
        private const string CacheKey = "endpointRegistrations";

        public async static Task Main(string[] args)
        {
            NServiceBus.Logging.LogManager.Use<NLogFactory>();

            Cache[CacheKey] = new List<IEndpointRegistration>();

            var host = new Host();

            // pass this command line option to run as a windows service
            if (args.Contains("--run-as-service"))
            {
                using (var windowsService = new WindowsService(host))
                {
                    ServiceBase.Run(windowsService);
                    return;
                }
            }

            Console.Title = host.EndpointName;

            var tcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, e) => { e.Cancel = true; tcs.SetResult(null); };

            await host.Start();
            await Console.Out.WriteLineAsync("Press Ctrl+C to exit...");

            await tcs.Task;
            await host.Stop();
        }
    }
}

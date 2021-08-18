using Hermes.Common.Interfaces;
using NLog.Extensions.Logging;
using NServiceBus.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Hermes.EndpointLoadBalancer.Service
{
    static class Program
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;
        private const string CacheKey = "endpointRegistrations";

        public async static Task Main(string[] args)
        {

            Microsoft.Extensions.Logging.ILoggerFactory extensionsLoggerFactory = new NLogLoggerFactory();
            NServiceBus.Logging.ILoggerFactory nservicebusLoggerFactory = new ExtensionsLoggerFactory(loggerFactory: extensionsLoggerFactory);
            NServiceBus.Logging.LogManager.UseFactory(loggerFactory: nservicebusLoggerFactory);

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

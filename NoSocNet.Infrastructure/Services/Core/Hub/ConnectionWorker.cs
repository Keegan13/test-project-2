//using Microsoft.Extensions.Hosting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace NoSocNet.Infrastructure.Services.Hub
//{
//    public class ConnectionCleanup : BackgroundService
//    {
//        protected int secondsInterval { get; set; } = 1;

//        //private readonly HubBase hub;

//        //public ConnectionCleanup(HubBase hub)
//        //{
//        //    this.hub = hub;
//        //}

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            //do
//            //{
//            //    await Task.Delay(1000 * secondsInterval, stoppingToken);
//            //    DropUnusedConnections();
//            //}
//            //while (!stoppingToken.IsCancellationRequested);
//        }

//        protected virtual IEnumerable<ValueTuple<string, List<Guid>>> DropUnusedConnections()
//        {
//            return Enumerable.Empty<ValueTuple<string, List<Guid>>>();
//        }
//    }
//}
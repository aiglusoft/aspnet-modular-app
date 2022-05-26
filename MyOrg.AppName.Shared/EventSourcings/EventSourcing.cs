using Autofac;
using MyOrg.AppName.Shared.EventSourcing.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOrg.AppName.Shared.EventSourcing
{
    public static class EventSourcing
    {
        public static void AddEventSourcing(this ContainerBuilder builder)
        {
            builder.RegisterType<AggregateStore>()
                   .As<IAggregateStore>()
                   .InstancePerRequest();

        }
    }
}

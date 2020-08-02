using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace VoterApp
{
    internal static class ContainerConfiguration
    {
        public static IContainer GetConfiguredContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainWindow>()
                .SingleInstance()
                .OnActivated(x => x.Instance.DataContext = x.Context.Resolve<AppViewModel>());

            builder.RegisterType<AppViewModel>()
                .SingleInstance();

            builder.Register(f => new Nethereum.Web3.Web3("https://kovan.infura.io/v3/9b3e6ee424c647a09069c69770048b58"))
                .SingleInstance();

            return builder.Build();
        }
    }
}

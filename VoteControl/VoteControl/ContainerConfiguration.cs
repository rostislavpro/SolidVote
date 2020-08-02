using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace VoteControl
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

            builder.Register(f => new Telegram.Bot.TelegramBotClient("1354821244:AAGH-6ghs8o21fsWmb-3o7ZW8aj0KOKnMEE"))
                .SingleInstance();

            var privateKeyHexString = "b929edff20d2647cb762f4f9021e5675e41bb3abacdba5b84dc3939a14508dae";
            var privateKey = Enumerable.Range(0, privateKeyHexString.Length / 2).Select(x => Convert.ToByte(privateKeyHexString.Substring(x * 2, 2), 16)).ToArray();

            builder.Register(f => new Nethereum.Web3.Web3(new Nethereum.Web3.Accounts.Account(privateKey), "https://kovan.infura.io/v3/9b3e6ee424c647a09069c69770048b58"))
                .SingleInstance();

            return builder.Build();
        }
    }
}

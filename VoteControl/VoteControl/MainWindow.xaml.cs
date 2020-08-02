using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace VoteControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow(AppViewModel viewModel)
        {
            InitializeComponent();

            //var Bot = new TelegramBotClient("1262730239:AAEFpk70OnOL0rck5yH12Ym7TrNhZY0OVFc");
            //var me = Bot.GetMeAsync().Result;

            //Bot.OnMessage += Bot_OnMessage;
            //Bot.OnUpdate += Bot_OnUpdate;

            //Bot.StartReceiving(new UpdateType[0]);
            viewModel.StartWorkerThread();
        }
    }
}

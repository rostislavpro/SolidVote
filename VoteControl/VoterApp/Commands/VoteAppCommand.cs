using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VoterApp.Commands
{
    public class VoteAppCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action handler;

        public VoteAppCommand(Action handler)
        {
            this.handler = handler;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.handler();
        }
    }
}

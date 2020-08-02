using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VoteControl.Commands
{
    public class VoteControlCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action handler;

        public VoteControlCommand(Action handler)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    class ShipOrderCommand : ICommand, ICommandFactory
    {
        public void Execute()
        {
            
        }

        public string CommandName
        {
            get { return "ShipOrderCommand"; }
        }

        public string Description
        {
            get { return "Ship Order"; }
        }

        public ICommand MakeCommand(string[] arguments)
        {
            return new ShipOrderCommand();
        }
    }
}

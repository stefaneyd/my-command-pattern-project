using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public class CreateOrderCommand : ICommand, ICommandFactory
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }

        public string CommandName
        {
            get { return "CreateOrder"; }
        }

        public string Description
        {
            get { return "Creates new Orders"; }
        }

        public ICommand MakeCommand(string[] arguments)
        {
            return new CreateOrderCommand();
        }
    }
}

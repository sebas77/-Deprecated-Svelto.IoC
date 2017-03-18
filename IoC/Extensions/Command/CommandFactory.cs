using Svelto.Command;

namespace Svelto.IoC
{
    public class CommandFactory:ICommandFactory
    {
        [Inject] public IContainer container { set; private get; }

        public CommandFactory()
        {
            _commandFactory = new Command.CommandFactory(cmd => container.Inject(cmd));
        }

        public TCommand Build<TCommand>() where TCommand : ICommand, new()
        {
            return _commandFactory.Build<TCommand>();
        }

        Command.CommandFactory _commandFactory;
    }
}

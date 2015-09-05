using System;
using System.Collections.Generic;

namespace Svelto.Command
{
    sealed public class CommandFactory : ICommandFactory
    {
        public CommandFactory()
        {}

        public CommandFactory(Action<ICommand> onNewCommand)
        {
            _onNewCommand = onNewCommand;
        }

        public TCommand Build<TCommand>() where TCommand : ICommand, new()
        {
            TCommand command = new TCommand();

            _commandPool.AddCommand(command);

            OnNewCommand(command);

            return command;
        }

        public TCommand Build<TCommand>(params object[] args) where TCommand : ICommand
        {
            Type commandClass = typeof(TCommand);

            TCommand command = (TCommand)Activator.CreateInstance(commandClass, args);

            _commandPool.AddCommand(command);

            OnNewCommand(command);

            return command;
        }

        void OnNewCommand(ICommand command)
        {
            if (_onNewCommand != null)
                _onNewCommand(command);
        }

        Action<ICommand>    _onNewCommand;
        CommandPool         _commandPool = new CommandPool();

        private class CommandPool
        {
            Dictionary<Type, ICommand> _command = new Dictionary<Type, ICommand>();

            public TCommand GetCommand<TCommand>(out bool commandIsNew) where TCommand : ICommand, new()
            {
                Type type = typeof(TCommand);

                ICommand command = null;

                if (_command.TryGetValue(type, out command) == true)
                {
                    commandIsNew = false;

                    return (TCommand)command;
                }

                command = new TCommand();

                _command[type] = command;

                commandIsNew = true;

                return (TCommand)command;
            }

            public void AddCommand<TCommand>(TCommand command) where TCommand : ICommand
            {
                Type type = typeof(TCommand);

                _command[type] = command;
            }
        }
    }
}

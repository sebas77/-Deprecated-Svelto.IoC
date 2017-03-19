namespace Svelto.Command
{
    interface ICommandFactory
    {
        TCommand Build<TCommand>() where TCommand : ICommand, new();
    }
}


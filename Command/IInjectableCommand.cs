namespace Svelto.Command
{
    public interface IInjectableCommand : ICommand
    {
        ICommand Inject(object dependency);
    }

    public interface IInjectableCommand<T> : IInjectableCommand
    {}

    public interface IMultiInjectableCommand : ICommand
	{
	    ICommand Inject(params object[] dependencies);
	}
}

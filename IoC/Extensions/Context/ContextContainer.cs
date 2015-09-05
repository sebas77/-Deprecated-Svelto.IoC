#region

using System;
using Svelto.Context;

#endregion

namespace Svelto.IoC.Extensions.Context
{
    public class ContextContainer: Container
	{
		public ContextContainer(IContextNotifer contextNotifier)
		{
            _contextNotifier = contextNotifier;
        }

        override protected void OnInstanceGenerated<TContractor>(TContractor instance)
        {
            if (instance is IWaitForFrameworkInitialization)
                _contextNotifier.AddFrameworkInitializationListener(instance as IWaitForFrameworkInitialization);

            if (instance is IWaitForFrameworkDestruction)
                _contextNotifier.AddFrameworkDestructionListener(instance as IWaitForFrameworkDestruction);
        }

        IContextNotifer _contextNotifier;
    }
}


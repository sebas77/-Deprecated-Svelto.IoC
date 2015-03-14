using System;

namespace Svelto.IoC
{
	public interface ICompositionRoot
	{
		IContainer container { get; }
	}
}


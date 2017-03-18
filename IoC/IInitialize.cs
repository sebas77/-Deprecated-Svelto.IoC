using System;

namespace Svelto.IoC
{
    interface IInitialize
	{
		void OnDependenciesInjected();
	}
}

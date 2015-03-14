using System;

namespace Svelto.IoC
{
	internal interface IInitialize
	{
		void OnDependenciesInjected();
	}
}

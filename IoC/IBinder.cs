using System;

namespace Svelto.IoC
{
	public interface IBinder<Contractor> where Contractor:class
	{
		void AsSingle<T>(T istance) where T:class, Contractor;
		void AsSingle<T>() where T:Contractor, new();
        void ToFactory<T>(IProvider<T> provider) where T : class, Contractor;
        			
		void Bind<ToBind>(IInternalContainer container) where ToBind:class;
	}
}


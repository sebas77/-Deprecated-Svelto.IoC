using UnityEngine;
using System;

namespace Svelto.IoC
{
	public class MonoBehaviourFactory: IMonoBehaviourFactory
	{
		IContainer _container;

		public MonoBehaviourFactory(IContainer container)
		{
			_container = container;
		}
		
		public M Build<M>(Func<M> constructor) where M:MonoBehaviour
		{
			M mb = (M)constructor();
			
			_container.Inject(mb);
						
			return mb;
		}
	}
}

using System;
using UnityEngine;

namespace Svelto.IoC
{
	public interface IMonoBehaviourFactory
	{
		M Build<M>(Func<M> constructor) where M:MonoBehaviour;
	}
}


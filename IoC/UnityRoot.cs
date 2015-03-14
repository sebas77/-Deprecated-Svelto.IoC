using System.Collections;
using UnityEngine;
using Svelto.IoC;

public class UnityRoot:MonoBehaviour
{}

public class UnityRoot<T>: UnityRoot where T:class, ICompositionRoot, new()
{
	void Awake()
	{
		_applicationRoot = new T();
		
		Init();
	}

	virtual protected void Init()
	{
		MonoBehaviour[] behaviours = this.transform.GetComponentsInChildren<MonoBehaviour>(true);

        foreach (MonoBehaviour component in behaviours)
            _applicationRoot.container.Inject(component);
	}
	
	private T _applicationRoot = null;
}

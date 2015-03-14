#region

using Svelto.IoC;
using UnityEngine;

#endregion

public class PrefabHolder:MonoBehaviour, IInitialize
{
	[Inject] internal IGameObjectFactory factory { set; private get; }
	
	public GameObject 	prefab;
	public bool			createAtStartup;
	public bool			hideOnStartup;
		
	public void OnDependenciesInjected()
	{
		factory.AddPrefab(prefab, prefab.name, gameObject);
		
		if (createAtStartup)
		{
			GameObject go = factory.Build(prefab.name);
			
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			
			if (hideOnStartup)
				go.SetActive(false);
		}
	}
}

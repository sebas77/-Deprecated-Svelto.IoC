using System;
using UnityEngine;
using System.Collections.Generic;

namespace Svelto.IoC
{
	public class GameObjectFactory: IGameObjectFactory
	{
		IContainer _container;
		
		private System.Collections.Generic.Dictionary<string, GameObject[]> prefabs;
		
		public GameObjectFactory(IContainer container)
		{
			_container = container;
			prefabs = new Dictionary<string, GameObject[]>();
		} 
		
		public void AddPrefab(GameObject prefab, string type, GameObject parent)
		{
		 	GameObject[] objects = new GameObject[2];
		 	
		 	objects[0] = prefab; objects[1] = parent;
			
			prefabs.Add(type, objects);
		}
		
		public GameObject Build(string type)
		{
			DesignByContract.Check.Require(prefabs.ContainsKey(type), "IGameObjectFactory - Invalid Prefab Type");
			
			GameObject go = Build(prefabs[type][0]);
			
			Vector3 scale = go.transform.localScale;
			Quaternion rotation = go.transform.localRotation;
			Vector3 position = go.transform.localPosition;
			
			prefabs[type][1].transform.gameObject.SetActive(true);
				
			go.transform.parent = prefabs[type][1].transform;
						
			go.transform.localPosition = position;
			go.transform.localRotation = rotation;
			go.transform.localScale = scale;
			
			return go;
		}

		public GameObject Build(GameObject go)
		{
			GameObject copy = GameObject.Instantiate(go) as GameObject;
			MonoBehaviour[] components = copy.GetComponentsInChildren<MonoBehaviour>(true);
			
			for (int i = 0; i < components.Length; ++i)
				_container.Inject(components[i]);

			return copy;
		}
	}
}


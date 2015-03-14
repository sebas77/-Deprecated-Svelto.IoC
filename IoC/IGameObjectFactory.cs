using UnityEngine;

namespace Svelto.IoC
{
    public interface IGameObjectFactory
    {
        void AddPrefab(GameObject prefab, string type, GameObject parent);

        GameObject Build(string type);
        GameObject Build(GameObject go);
    }
}

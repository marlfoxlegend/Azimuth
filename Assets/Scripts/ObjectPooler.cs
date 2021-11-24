using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class PooledObject
        {
            public int numberToPool = 10;
            public GameObject objectToPool;
            public string newObjectName;
        }

        public List<PooledObject> pool = new List<PooledObject>();

        Dictionary<string, List<GameObject>> aggregatePools = new Dictionary<string, List<GameObject>>();

        private void OnEnable()
        {
            foreach (PooledObject obj in pool)
            {
                if (string.IsNullOrEmpty(obj.newObjectName))
                {
                    Debug.LogWarning($"{nameof(PooledObject)}<{obj.objectToPool.name}> has empty or null name.");
                    continue;
                }
                aggregatePools.Add(obj.newObjectName, new List<GameObject>());
                CreatePooledObjects(obj);
            }
        }

        private void CreatePooledObjects(PooledObject pooledObject)
        {
            for (int i = 0; i < pooledObject.numberToPool; i++)
            {
                var createdObj = Instantiate(pooledObject.objectToPool, transform, true);
                createdObj.SetActive(false);
                createdObj.name = $"{pooledObject.newObjectName} {i}";
                aggregatePools[pooledObject.newObjectName].Add(createdObj);
            }
        }

        public GameObject GetObjectFromPool(string objectName)
        {
            if (aggregatePools.ContainsKey(objectName))
            {
                List<GameObject> objectsInPool = aggregatePools[objectName];
                for (int i = 0; i < objectsInPool.Count; i++)
                {
                    if (!objectsInPool[i].activeInHierarchy)
                    {
                        return objectsInPool[i];
                    }
                }
            }
            return null;
        }

        public void ReturnObjectToPool(GameObject go)
        {
            if (go.transform.parent == transform)
            {
                go.SetActive(false);
            }
            else
            {
                Destroy(go);
            }
        }
    }
}

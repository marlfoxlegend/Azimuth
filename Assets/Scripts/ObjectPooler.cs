using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
    [Serializable]
    public class PooledObject
    {
        public int numberToPool;
        public GameObject objectToPool;
        public string newObjectName;
        public readonly List<GameObject> objects = new List<GameObject>();

        public PooledObject(GameObject objectToPool, int numberToPool = 20, string newObjectName = null)
        {
            this.numberToPool = numberToPool;
            this.objectToPool = objectToPool;
            this.newObjectName = newObjectName ?? objectToPool.tag;
        }

        public void BuildPool(ObjectPooler pooler, int startIndex = 0)
        {
            for (int i = 0; i < numberToPool; i++)
            {
                var obj = GameObject.Instantiate(objectToPool, pooler.transform, true);
                obj.SetActive(false);
                obj.name = $"{newObjectName}_{i + startIndex:00#}";
                objects.Add(obj);
            }
        }
    }

    public class ObjectPooler : MonoBehaviour
    {
        private List<PooledObject> _pools;

        private void Awake()
        {
            _pools = new List<PooledObject>();
        }

        private void OnEnable()
        {
            if (_pools.Count == 0)
            {
                Init();
            }
        }

        private void Init()
        {
            foreach (PooledObject po in _pools)
            {
                if (po.objects.Count > 0)
                    continue;
                po.BuildPool(this);
            }
        }

        private void CreatePooledObjects(PooledObject pooledObject, int startIndex)
        {
            var goPool = new List<GameObject>();
            for (int i = 0; i < pooledObject.numberToPool; i++)
            {
                var createdObj = Instantiate(pooledObject.objectToPool, transform, true);
                createdObj.SetActive(false);
                createdObj.name = $"{pooledObject.newObjectName}_{i + startIndex:00#}";
                goPool.Add(createdObj);
            }
            return goPool;
        }

        public GameObject GetObjectFromPool(GameObject go)
        {
            var obj = _pools.SingleOrDefault(po => po.objectToPool == go);
            if (obj != null)
            {
                return obj.objects.FirstOrDefault(go => !go.activeInHierarchy);
            }
            return null;
        }

        public void ReturnObjectToPool(GameObject go)
        {
            var obj = _pools.SingleOrDefault(po => po.objectToPool == go);
            if (obj != null)
            {
                if (!obj.objects.Contains(go))
                {
                    Debug.LogWarning(
                        $"Attempted to return an invalid object: {go.name} into pool: {go}");
                    return;
                }
                go.SetActive(false);
            }
        }

        public void AddPool(PooledObject pooledObject)
        {
            if (!_pools.Contains(pooledObject))
            {
                pooledObject.BuildPool(this);
                _pools.Add(pooledObject);
            }
        }
    }
}

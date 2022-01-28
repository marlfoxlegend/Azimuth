using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
    public enum PoolCategory
    {
        PlayerLaser, EnemyLaser, EnemyLargeLaser
    }

    [Serializable]
    public class PooledObject
    {
        public PoolCategory category;
        public int numberToPool;
        public GameObject objectToPool;
        public string name;
    }

    public class ObjectPooler : MonoBehaviour
    {
        [SerializeField] private List<PooledObject> _pooledObjects = new List<PooledObject>();
        private Dictionary<PoolCategory, List<GameObject>> _pools;

        private void Awake()
        {
            _pools = new Dictionary<PoolCategory, List<GameObject>>();
            Init();
        }

        private void Init()
        {
            foreach (PooledObject po in _pooledObjects)
            {
                _pools[po.category] = BuildPool(po);
            }
        }

        private List<GameObject> BuildPool(PooledObject po)
        {
            List<GameObject> pool = new List<GameObject>();
            for (int i = 0; i < po.numberToPool; i++)
            {
                var obj = Instantiate(po.objectToPool, transform, true);
                obj.SetActive(false);
                obj.name = $"{po.name}_{i:00#}";
                pool.Add(obj);
            }
            return pool;
        }

        private void IncreasePoolCapacity(PooledObject po)
        {
            var pool = _pools[po.category];
            var offset = pool.Count;
            for (int i = 0; i < po.numberToPool; i++)
            {
                var obj = Instantiate(po.objectToPool, transform, true);
                obj.SetActive(false);
                obj.name = $"{po.name}_{i + offset:00#}";
                pool.Add(obj);
            }
        }

        public GameObject GetObjectFromPool(PoolCategory category)
        {
            GameObject obj = null;
            if (_pools.ContainsKey(category))
            {
                obj = _pools[category].FirstOrDefault(go => !go.activeInHierarchy);
                if (obj == null)
                {
                    var pooledObj = _pooledObjects.Single(po => po.category == category);
                    IncreasePoolCapacity(pooledObj);
                    obj = _pools[category].FirstOrDefault(go => !go.activeInHierarchy);
                }
            }
            return obj;
        }

        public void ReturnObjectToPool(GameObject go)
        {
            try
            {
                var category = _pools.First(kv => kv.Value.Contains(go)).Key;
                go.SetActive(false);
            }
            catch (InvalidOperationException)
            {
                Debug.LogWarning($"Attempted to return an invalid object: {go.name} into pool.");
            }
        }
    }
}


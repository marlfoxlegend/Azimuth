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
			public string newObjectName = string.Empty;
		}

		public List<PooledObject> pool = new List<PooledObject>();

		Dictionary<string, List<GameObject>> aggregatePools = new Dictionary<string, List<GameObject>>();

		private void OnEnable()
		{
			foreach (var obj in pool)
			{
				aggregatePools.Add(obj.objectToPool.tag, new List<GameObject>());
				CreatePooledObjects(obj);
			}
		}

		private void CreatePooledObjects(PooledObject pooledObject)
		{
			for (int i = 0; i < pooledObject.numberToPool; i++)
			{
				var createdObj = Instantiate(pooledObject.objectToPool, transform, true);
				createdObj.SetActive(false);
				var replacedName = pooledObject.newObjectName != string.Empty ?
					$"{pooledObject.newObjectName} {i}" : $"{pooledObject.objectToPool.name} {i}";
				createdObj.name = replacedName;
				aggregatePools[createdObj.tag].Add(createdObj);
			}
		}

		public GameObject GetObjectFromPool(string objectTag)
		{
			if (aggregatePools.ContainsKey(objectTag))
			{
				List<GameObject> objectsInPool = aggregatePools[objectTag];
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
			if (aggregatePools.ContainsKey(go.tag))
			{
				go.SetActive(false);
			}
		}
	}
}

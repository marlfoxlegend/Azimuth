using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	public class LaserDestroyer : MonoBehaviour
	{
        private ObjectPooler _pooler;

		private void Awake()
		{
			_pooler = FindObjectOfType<ObjectPooler>();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
            var laser = collision.gameObject.GetComponent<Laser>();
            if (laser != null)
            {
                _pooler.ReturnObjectToPool(laser.gameObject);
            }
		}
	}
}

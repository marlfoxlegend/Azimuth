using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	public class LaserDestroyer : MonoBehaviour
	{
		public string laserLayerName = "Lasers";

		ObjectPooler pooler;

		private void Awake()
		{
			pooler = FindObjectOfType<ObjectPooler>();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			var layer = collision.gameObject.layer;
			if (LayerMask.NameToLayer(laserLayerName) == layer)
			{
				pooler.ReturnObjectToPool(collision.gameObject);
			}
		}
	}
}

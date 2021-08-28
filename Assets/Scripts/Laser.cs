using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	public class Laser : MonoBehaviour
	{
		ObjectPooler pooler;

		private void Awake()
		{
			pooler = FindObjectOfType<ObjectPooler>();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			pooler.ReturnObjectToPool(gameObject);
		}
	}
}

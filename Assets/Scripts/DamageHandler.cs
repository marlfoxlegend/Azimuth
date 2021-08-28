using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	public class DamageHandler : MonoBehaviour
	{
		public int health = 100;
		public bool destroyOnDeath = false;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			var dmg = collision.gameObject.GetComponent<Damager>();
			if (dmg)
			{
				health = Mathf.Max(0, health - dmg.GetDamageAmount());
			}
		}

		private void Update()
		{
			if (health == 0)
			{
				Die();
			}
		}

		private void Die()
		{
			if (destroyOnDeath)
			{
				Destroy(gameObject);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	public class Enemy : MonoBehaviour, IDestroyable
	{
		[SerializeField] float maxFireDelay = 5f;
		[SerializeField] int rewardPoints = 50;
		[SerializeField] LaserShooter laserGun;
		[SerializeField] int health = 100;

		private void OnEnable()
		{
			_ = StartCoroutine(RandomFireLaser());
		}

		private void Update()
		{
			if (GameManagement.Instance.PlayState != GameManagement.CurrentState.Playing)
			{
				CeaseFire();
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			var dmg = collision.gameObject.GetComponent<Damager>();
			if (dmg)
			{
				TakeDamage(dmg);
			}
		}

		private IEnumerator RandomFireLaser()
		{
			do
			{
				var wait = UnityEngine.Random.Range(0f, maxFireDelay);
				yield return new WaitForSeconds(wait);
				if (GetComponent<EnemyPathing>().FacingPlayer)
				{
					laserGun.ShootLaser();
				}
			} while (GameManagement.Instance.PlayState == GameManagement.CurrentState.Playing);
		}

		private void Explode()
		{
			Destroy(gameObject);
		}

		private void CeaseFire() => StopCoroutine(nameof(RandomFireLaser));

		public void TakeDamage(Damager otherDamager)
		{
			health -= otherDamager.GetDamageAmount();
			if (health <= 0)
			{
				GameManagement.Instance.baseStats.score += rewardPoints;
				Explode();
			}
		}
	}
}

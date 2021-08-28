using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	public class Damager : MonoBehaviour
	{
		[SerializeField] int damageAmount;

		public void Damage(Health health) => health.CurrentHealth -= damageAmount;

		public int GetDamageAmount() => damageAmount;
	}
}

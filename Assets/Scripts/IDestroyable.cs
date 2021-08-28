using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	public interface IHealth
	{
		public int CurrentHealth { get; }

		public void ChangeHealth(int amount);
	}

	public interface IDestroyable
	{
		public void TakeDamage(Damager damager);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	public interface IDestroyable
	{
		public void TakeDamage(int damageAmount);
        public int GetHealth();
        public void Destroyed();
	}
}

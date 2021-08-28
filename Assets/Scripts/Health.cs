using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	public class Health : MonoBehaviour
	{
		[SerializeField] int currentHealth = 0;

		public int CurrentHealth
		{
			get => currentHealth;
			set => currentHealth = Mathf.Max(value, 0);
		}
	}
}

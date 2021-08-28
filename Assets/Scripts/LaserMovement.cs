using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	public class LaserMovement : MonoBehaviour
	{
		[SerializeField] float laserSpeed = 10;

		Rigidbody2D rb;
		ObjectPooler pooler;

		public Transform ShootDirection { get; set; }

		private void OnEnable()
		{
			rb = GetComponent<Rigidbody2D>();
			pooler = FindObjectOfType<ObjectPooler>();
		}

		private void MoveLaser()
		{
			Vector3 pos = transform.position;
			Vector3 force = new Vector3(0f, laserSpeed * Time.fixedDeltaTime, 0f);
			pos += transform.rotation * force;
			rb.MovePosition(pos);
		}

		private void OnBecameInvisible()
		{
			pooler.ReturnObjectToPool(gameObject);
		}

		private void FixedUpdate()
		{
			MoveLaser();
		}
	}
}

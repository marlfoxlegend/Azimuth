using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Azimuth
{
	public class PlayerController : MonoBehaviour, IDestroyable
	{
		[System.Serializable]
		public class PowerUpStat
		{
			public Stats modifiers;
			public string powerUpId;
			public float elapsedTime;
			public float maxTime;
		}

		[System.Serializable]
		public struct Stats
		{
			public int health;
			public int score;
			public float shipSpeed;
			public float laserCoolDown;
			public int guns;

			public static Stats operator +(Stats a, Stats b)
			{
				return new Stats
				{
					health = a.health + b.health,
					score = a.score + b.score,
					shipSpeed = a.shipSpeed + b.shipSpeed,
					laserCoolDown = a.laserCoolDown + b.laserCoolDown,
					guns = a.guns + b.guns
				};
			}
		}

		//[SerializeField] float movementSpeed = 10f;
		//[SerializeField] GameObject thrusters;
		//[SerializeField] float laserCoolDownPeriod = 1;
		//[SerializeField] float fireReleaseTime = 0f;

		[Range(0f, 1f)] public float maxYBoundary;
		[Min(0.01f)] public float rotateSpeed = 1f;
		public Stats baseStats;

		Rigidbody2D rb;
		SpriteRenderer spriteRenderer;
		List<LaserShooter> shooters;
		AzimuthControls controls;
		List<PowerUpStat> powerUps = new List<PowerUpStat>();
		Stats activeStats;
		Vector2 maxBoundary;
		Vector2 minBoundary;
		Coroutine firingLasers = null;

		private void Awake()
		{
			rb = GetComponent<Rigidbody2D>();
			spriteRenderer = GetComponent<SpriteRenderer>();
			shooters = new List<LaserShooter>(GetComponentsInChildren<LaserShooter>());
			controls = new AzimuthControls();
			SetPlayerBoundaries();
		}

		private void OnEnable()
		{
			baseStats = GameManagement.Instance.baseStats;
			controls.SpaceShooter.Fire.performed += OnFire;
			controls.SpaceShooter.Fire.canceled += OnFireCeased;
		}

		private void OnDisable()
		{
			SetPlayerControl(false);
		}

		private void FixedUpdate()
		{
			MovePlayer(controls.SpaceShooter.Move.ReadValue<Vector2>());
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			var dmg = collision.gameObject.GetComponent<Damager>();
			if (dmg)
			{
				TakeDamage(dmg);
			}
		}

		private IEnumerator FireLasers()
		{
			while (true)
			{
				foreach (var shooter in shooters)
				{
					if (shooter.gameObject.activeInHierarchy)
					{
						shooter.ShootLaser();
					}
				}
				yield return new WaitForSeconds(GameManagement.Instance.baseStats.laserCoolDown);
			}
		}

		public void OnFire(InputAction.CallbackContext context) => firingLasers = StartCoroutine(FireLasers());

		public void OnFireCeased(InputAction.CallbackContext context)
		{
			if (firingLasers != null)
			{
				StopCoroutine(firingLasers);
			}
		}

		private void MovePlayer(Vector3 movement)
		{
			// ROTATE player
			//var rot = transform.rotation.eulerAngles.z;
			//rot += movement.x * rotateSpeed * Time.fixedDeltaTime;
			//rb.MoveRotation(rot);

			// MOVE player
			var pos = transform.position;
			movement *= GameManagement.Instance.baseStats.shipSpeed * Time.fixedDeltaTime;
			pos += movement;
			//var velocity = new Vector3(0f, movement.y * gameManager.gameStats.shipSpeed * Time.fixedDeltaTime, 0f);
			//pos += transform.rotation * velocity;

			// Clamp movement to defined boundaries
			pos.x = Mathf.Clamp(pos.x, minBoundary.x, maxBoundary.x);
			pos.y = Mathf.Clamp(pos.y, minBoundary.y, maxBoundary.y);

			rb.MovePosition(pos);
		}

		private void SetPlayerBoundaries()
		{
			// Boundaries are screen edge offset by player sprite in world units
			var shipRect = spriteRenderer.sprite.rect;
			var ppu = spriteRenderer.sprite.pixelsPerUnit;
			var offset = new Vector3(shipRect.width, shipRect.height, 0f) / ppu / 2.0f;

			minBoundary = Camera.main.ViewportToWorldPoint(Vector2.zero) + offset;
			maxBoundary = Camera.main.ViewportToWorldPoint(new Vector2(1f, maxYBoundary)) - offset;
		}

		public void TakeDamage(Damager otherDamager)
		{
			GameManagement.Instance.baseStats.health -= otherDamager.GetDamageAmount();
			if (GameManagement.Instance.baseStats.health <= 0)
			{
				Explode();
			}
		}

		private void Explode()
		{
			gameObject.SetActive(false);
			GameManagement.Instance.FinishLevel(GameManagement.CurrentState.Lost);
		}

		public void SetPlayerControl(bool takesInput = true)
		{
			if (takesInput)
			{
				controls.SpaceShooter.Enable();
			}
			else
			{
				controls.SpaceShooter.Disable();
			}
		}
	}
}

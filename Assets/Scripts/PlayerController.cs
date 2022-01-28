using System;
using System.Collections;
using System.Collections.Generic;
using Azimuth.Events;
using UnityEngine;

namespace Azimuth
{
    public class PlayerController : MonoBehaviour, IDestroyable
    {
        [Serializable]
        public class PowerUpStat
        {
            public Stats modifiers;
            public string powerUpId;
            public float elapsedTime;
            public float maxTime;
        }

        [Serializable]
        public class Stats
        {
            public int health;
            public float shipSpeed;
            public float laserCoolDown;
            public int activeLaserGuns;
            public int shieldDefense;

            #region Constructors
            public Stats()
            {
            }

            public Stats(Stats stats) : this()
            {
                AddToStats(stats);
            }
            #endregion

            public static Stats operator +(Stats a, Stats b)
            {
                return new Stats()
                {
                    health = a.health + b.health,
                    shipSpeed = a.shipSpeed + b.shipSpeed,
                    laserCoolDown = a.laserCoolDown + b.laserCoolDown,
                    activeLaserGuns = a.activeLaserGuns + b.activeLaserGuns,
                    shieldDefense = a.shieldDefense + b.shieldDefense
                };
            }

            public void AddToStats(Stats stats)
            {
                health += stats.health;
                shipSpeed += stats.shipSpeed;
                laserCoolDown += stats.laserCoolDown;
                activeLaserGuns += stats.activeLaserGuns;
                shieldDefense += stats.shieldDefense;
            }
        }

        public event EventHandler<LevelFinishedEventArgs> onPlayerDestroyed;
        public event EventHandler<PlayerHealthEventArgs> onHealthChanged;

        private const string Horizontal = "Horizontal";
        private const string Vertical = "Vertical";
        private const string Fire = "Fire";

        [SerializeField] private Stats _baseStats;
        [SerializeField] [Range(0f, 1f)] private float _maxYBoundary;

        [Header("Ship FX")]
        [SerializeField] private AudioClip _playerExplosionClip;
        [SerializeField] [Range(0f, 1f)] private float _explosionVolume = 1f;
        [SerializeField] private GameObject _explosionFx;
        [SerializeField] [Range(1f, 10f)] private float _explosionDuration = 3f;


        public Stats stats;

        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private List<LaserShooter> _shooters;
        private List<PowerUpStat> _powerUps = new List<PowerUpStat>();
        private Vector2 _maxBoundary;
        private Vector2 _minBoundary;
        private bool _firingLasers = false;

        private void Awake()
        {
            stats = new Stats(_baseStats);
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _shooters = new List<LaserShooter>(GetComponentsInChildren<LaserShooter>());
            SetPlayerBoundaries();
        }

        private void OnEnable()
        {
            GameManager.Instance.maxHealth = _baseStats.health;
        }

        private void OnDisable()
        {
            SetPlayerControl(false);
        }

        private void Update()
        {
            if (Input.GetButton(Fire))
            {
                StartFiringLaser();
            }
            else
            {
                StopFiringLaser();
            }
        }

        private void FixedUpdate()
        {
            MoveShip(GetMovement());
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var dmg = collision.gameObject.GetComponent<IDestroyer>();
            if (dmg != null)
            {
                dmg.DamageTarget(this);
            }
        }

        private Vector3 GetMovement()
        {
            float x = Input.GetAxis(Horizontal);
            float y = Input.GetAxis(Vertical);
            return new Vector3(x, y, 0);
        }

        private IEnumerator FiringLasers()
        {
            while (_firingLasers)
            {
                for (var i = 0; i < stats.activeLaserGuns; i++)
                {
                    _shooters[i].ShootLaser();
                }
                yield return new WaitForSeconds(stats.laserCoolDown);
            }
        }

        private void SetPlayerBoundaries()
        {
            // Boundaries are screen edge offset by player sprite in world units
            var shipRect = _spriteRenderer.sprite.rect;
            var ppu = _spriteRenderer.sprite.pixelsPerUnit;
            var offset = new Vector3(shipRect.width, shipRect.height, 0f) / ppu / 2.0f;

            _minBoundary = Camera.main.ViewportToWorldPoint(Vector2.zero) + offset;
            _maxBoundary = Camera.main.ViewportToWorldPoint(new Vector2(1f, _maxYBoundary)) - offset;
        }

        public void SetPlayerControl(bool allowPlayerControl = true)
        {
            _rb.simulated = allowPlayerControl;
        }

        public void StartFiringLaser()
        {
            if (!_firingLasers)
            {
                _firingLasers = true;
                _ = StartCoroutine(FiringLasers());
            }
        }

        public void StopFiringLaser()
        {
            _firingLasers = false;
            StopCoroutine(FiringLasers());
        }

        public void MoveShip(Vector3 movement)
        {
            var pos = transform.position;
            pos += stats.shipSpeed * Time.fixedDeltaTime * movement;

            // Clamp movement to defined boundaries
            pos.x = Mathf.Clamp(pos.x, _minBoundary.x, _maxBoundary.x);
            pos.y = Mathf.Clamp(pos.y, _minBoundary.y, _maxBoundary.y);
            _rb.MovePosition(pos);
        }

        public void TakeDamage(int damageAmount)
        {
            var remainingShields = stats.shieldDefense - damageAmount;
            int damageRemaining;
            if (remainingShields >= 0)
            {
                stats.shieldDefense = remainingShields;
                damageRemaining = 0;
            }
            else
            {
                stats.shieldDefense = 0;
                damageRemaining = Mathf.Abs(remainingShields);
            }

            int oldHealth = stats.health;
            stats.health = Mathf.Max(oldHealth - damageRemaining, 0);
            var e = new PlayerHealthEventArgs(_baseStats.health, stats.health, oldHealth);
            GameManager.Instance.UpdateHealth(e);
            //var handler = onHealthChanged;
            //if (handler != null)
            //{
            //    handler(this, e);
            //}

            if (stats.health <= 0)
            {
                Destroyed();
            }
        }

        public void Destroyed()
        {
            SetPlayerControl(false);
            gameObject.SetActive(false);
            if (_explosionFx != null)
            {
                GameObject explosion = Instantiate(_explosionFx, transform.position, Quaternion.identity);
                AudioSource.PlayClipAtPoint(_playerExplosionClip,
                                            Camera.main.transform.position,
                                            _explosionVolume);
                Destroy(explosion, _explosionDuration);
            }

            GameManager.Instance.FinishLevel(false);
        }

        public int GetHealth() => stats.health;

        public int GetMaxHealth() => _baseStats.health;
    }
}

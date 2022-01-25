using System;
using System.Collections;
using System.Collections.Generic;
using Azimuth.Events;
using UnityEngine;

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
            public float shipSpeed;
            public float laserCoolDown;
            public int activeLaserGuns;
            public int shieldDefense;

            public static Stats operator +(Stats a, Stats b)
            {
                return new Stats
                {
                    health = a.health + b.health,
                    shipSpeed = a.shipSpeed + b.shipSpeed,
                    laserCoolDown = a.laserCoolDown + b.laserCoolDown,
                    activeLaserGuns = a.activeLaserGuns + b.activeLaserGuns,
                    shieldDefense = a.shieldDefense + b.shieldDefense
                };
            }
        }

        public event EventHandler onPlayerDestroyed;
        public event EventHandler<PlayerHealthEventArgs> onHealthChanged;

        private const string Horizontal = "Horizontal";
        private const string Vertical = "Vertical";
        private const string Fire = "Fire";

        [SerializeField] private Stats _baseStats;
        [SerializeField] private int _health = 100;
        [SerializeField] private int _score = 0;
        [SerializeField] [Range(0f, 1f)] private float _maxYBoundary;

        [Header("Ship FX")]
        [SerializeField] private AudioClip _playerExplosionClip;
        [SerializeField] [Range(0f, 1f)] private float _explosionVolume = 1f;
        [SerializeField] private GameObject _explosionFx;
        [SerializeField] [Range(1f, 10f)] private float _explosionDuration = 3f;


        [Tooltip("READ ONLY")] public Stats stats;

        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private List<LaserShooter> _shooters;
        private List<PowerUpStat> _powerUps = new List<PowerUpStat>();
        private Vector2 _maxBoundary;
        private Vector2 _minBoundary;
        private Coroutine _firingLasers = null;

        public Stats BaseStats => _baseStats;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _shooters = new List<LaserShooter>(GetComponentsInChildren<LaserShooter>());
            SetPlayerBoundaries();
        }

        private void OnEnable()
        {
            stats = _baseStats;
        }

        private void OnDisable()
        {
            SetPlayerControl(false);
        }

        private void Update()
        {
            if (Input.GetButtonDown(Fire))
            {
                Debug.Log($"Fire button held: {Input.GetButtonDown(Fire)}");
                FireLaser();
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
            while (true)
            {
                for (var i = 0; i < stats.activeLaserGuns; i++)
                {
                    Debug.Log($"Firing from {_shooters[i].name}.", this);
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

        public void FireLaser()
        {
            if (_firingLasers == null)
            {
                _firingLasers = StartCoroutine(FiringLasers());
            }
            else
            {
                StopCoroutine(_firingLasers);
            }
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
            var damage = damageAmount - stats.shieldDefense;
            stats.shieldDefense = damage >= 0 ? 0 : Mathf.Abs(damage);

            int damageAfterShields = Mathf.Max(damage, 0);
            int oldHealth = stats.health;
            int newHealth = Mathf.Max(oldHealth - damageAfterShields, 0);

            stats.health = newHealth;
            var e = new PlayerHealthEventArgs(_baseStats.health, newHealth, oldHealth);
            
            onHealthChanged?.Invoke(this, e);
            
            if (stats.health <= 0)
            {
                Destroyed();
            }
        }

        public void Destroyed()
        {
            _spriteRenderer.enabled = false;
            SetPlayerControl(false);
            StopAllCoroutines();
            if (_explosionFx != null)
            {
                GameObject explosion = Instantiate(_explosionFx, transform.position, Quaternion.identity);
                AudioSource.PlayClipAtPoint(_playerExplosionClip,
                                            Camera.main.transform.position,
                                            _explosionVolume);
                Destroy(explosion, _explosionDuration);
            }
            GameManager.Instance.FinishLevel
        }

        public int GetHealth() => _health;
    }
}

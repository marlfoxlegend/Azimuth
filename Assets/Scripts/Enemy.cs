using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
    public class Enemy : MonoBehaviour, IDestroyer, IDestroyable, ISpawnable<EnemySpawner>
    {
        [SerializeField] [Min(0f)] private int _rewardPoints = 50;
        [SerializeField] [Min(0f)] private int _health = 100;
        [SerializeField] [Min(0f)] private int _contactDamage = 100;
        
        [Header("Ship")]
        [SerializeField] private LaserShooter _laserGun;
        [SerializeField] [Min(0f)] private float _maxFireDelay = 5f;
        [SerializeField] private bool _destroyOnImpact = true;

        [Header("Ship FX")]
        [SerializeField] private GameObject _explosionFx;
        [SerializeField] private AudioClip _explosionClip;
        [SerializeField] [Range(0f, 1f)] private float _explosionVolume = 1f;
        [SerializeField] [Range(1f, 10f)] private float _explosionDuration = 3f;

        private bool _firing = false;

        private EnemySpawner _spawner;

        private void OnBecameVisible()
        {
            if (!_firing)
            {
                _ = StartCoroutine(RandomFireLaser());
                _firing = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var dmg = collision.gameObject.GetComponent<IDestroyer>();
            if (dmg != null)
            {
                dmg.DamageTarget(this);
            }
        }

        private IEnumerator RandomFireLaser()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0f, _maxFireDelay));
                _laserGun.ShootLaser();
            }
        }

        public void CompletedPath()
        {
            gameObject.SetActive(false);
            _spawner.RemoveSpawn(this, false);
        }

        public void CeaseFire()
        {
            StopCoroutine(RandomFireLaser());
            _firing = false;
        }

        public void TakeDamage(int damageAmount)
        {
            _health -= damageAmount;
            if (_health <= 0)
            {
                Destroyed();
            }
        }

        public int GetDamageAmount() => _contactDamage;

        public void DamageTarget(IDestroyable target)
        {
            target.TakeDamage(_contactDamage);
            if (_destroyOnImpact)
            {
                Destroyed();
            }
        }

        public int GetHealth() => _health;

        public void Destroyed()
        {
            gameObject.SetActive(false);
            if (_explosionFx)
            {
                var explosion = Instantiate(_explosionFx, transform.position, Quaternion.identity);
                AudioSource.PlayClipAtPoint(_explosionClip,
                                            Camera.main.transform.position,
                                            _explosionVolume);
                Destroy(explosion, _explosionDuration);
            }

            _spawner.RemoveSpawn(this, true);
        }

        public void SetSprite(Sprite sprite)
        {
            SpriteRenderer sp = GetComponent<SpriteRenderer>();
            PolygonCollider2D collider2D = GetComponent<PolygonCollider2D>();
            
            sp.sprite = sprite;
            List<Vector2> shapes = new List<Vector2>();
            if (sp.sprite.GetPhysicsShape(0, shapes) > 0)
            {
                collider2D.points = shapes.ToArray();
            }
        }

        public int RewardPoints() => _rewardPoints;

        public EnemySpawner GetSpawner() => _spawner;

        public void SetSpawner(EnemySpawner spawner) => _spawner = spawner;
    }
}

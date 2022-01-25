using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
    public class Enemy : MonoBehaviour, IDestroyer, IDestroyable
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

        private Coroutine _firing;


        private void OnBecameVisible()
        {
            if (_firing == null)
            {
                _firing = StartCoroutine(RandomFireLaser());
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

        private void OnBecameInvisible()
        {
            if (_firing != null)
            {
                StopCoroutine(_firing);
            }
        }

        private void OnDestroy()
        {
        }

        private IEnumerator RandomFireLaser()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0f, _maxFireDelay));
                _laserGun.ShootLaser();
            }
        }

        public void CeaseFire() => StopCoroutine(_firing);

        public void TakeDamage(int damageAmount)
        {
            _health -= damageAmount;
            if (_health <= 0)
            {
                // TODO: add to game session's score
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

            var destroyedEventArgs = new Events.EnemyDestroyedGameEvent(_rewardPoints);
            EventManager.Instance.TriggerEvent(this, destroyedEventArgs);
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

        private void RewardPoints()
        {
            GameManager.Instance.AddToScore(_rewardPoints);
        }
    }
}

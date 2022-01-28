using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
    public class Laser : MonoBehaviour, IDestroyer
    {
        [SerializeField] private int _damageAmount = 50;
        [SerializeField] private bool _moveDownward = false;
        [SerializeField] private bool _rotate = false;
        [SerializeField] [Min(0f)] private float _rotationSpeed = 0f;
        [SerializeField] [Range(0f,100f)] private float _laserSpeed = 10f;

        private Rigidbody2D _rb;

        private void OnEnable()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            MoveLaser();
        }

        private void MoveLaser()
        {
            Vector2 direction = _moveDownward ? Vector2.down : Vector2.up;
            Vector2 pos = _rb.position;
            pos += _laserSpeed * Time.fixedDeltaTime * direction;
            _rb.MovePosition(pos);

            if (_rotate)
            {
                var rot = _rb.rotation;
                rot += _rotationSpeed * Time.fixedDeltaTime;
                _rb.MoveRotation(rot);
            }
        }

        public int GetDamageAmount()
        {
            return _damageAmount;
        }

        public void DamageTarget(IDestroyable target)
        {
            target.TakeDamage(_damageAmount);
            FindObjectOfType<ObjectPooler>().ReturnObjectToPool(gameObject);
        }
    }
}

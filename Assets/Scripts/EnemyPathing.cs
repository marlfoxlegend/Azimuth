using System.Collections.Generic;
using UnityEngine;


namespace Azimuth
{
    public class EnemyPathing : MonoBehaviour
    {
        [SerializeField] private float _speed = 3f;

        private Rigidbody2D _r2d;
        private Enemy _enemy;
        private PathCreation.VertexPath _vertexPath;
        private float _dst = 0f;


        private void Awake()
        {
            _r2d = GetComponent<Rigidbody2D>();
            _enemy = GetComponent<Enemy>();
        }

        private void FixedUpdate()
        {
            MoveAlongPath();
        }

        private void LateUpdate()
        {
            if (transform.position == _vertexPath.GetPoint(_vertexPath.NumPoints - 1))
            {
                gameObject.SetActive(false);
                EventManager.Instance.TriggerEvent(_enemy,
                                                   new Events.EnemyDestroyedGameEvent(0));
            }
        }

        public void SetPath(PathCreation.VertexPath vertexPath) => _vertexPath = vertexPath;

        private void MoveAlongPath()
        {
            _dst += Time.fixedDeltaTime * _speed;
            var pos = _vertexPath.GetPointAtDistance(_dst, PathCreation.EndOfPathInstruction.Stop);

            _r2d.MovePosition(pos);
        }

        private Vector3 FindDirection(Vector3 pos)
        {
            var heading = pos - transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance;
            return direction;
        }

    }
}

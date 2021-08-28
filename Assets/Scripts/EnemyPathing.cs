using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Azimuth
{
	public class EnemyPathing : MonoBehaviour
	{
		[SerializeField] float enemySpeed = 10f;

		public bool FacingPlayer { get; private set; } = false;
		public delegate void TravelMethod();

		TravelMethod travel;
		VertexPath path;
		PlayerController player;
		float dst = 0;

		private void Start()
		{
			player = FindObjectOfType<PlayerController>();
			
		}

		private void Update()
		{
			TravelPath();
			//if (transform.parent.childCount == 1)
			//{
			//	var diff = _pos != transform.position && _rot != transform.rotation;
			//	if (diff)
			//	{
			//		_pos = transform.position;
			//		_rot = transform.rotation;
			//		Debug.Log($"{name} --    POSITION: ({_pos})    ROTATION: ({_rot.eulerAngles})");
			//	}
			//}
		}

		public void SetPath(VertexPath vertexPath) => path = vertexPath;

		private void TravelPath()
		{
			if (player)
			{
				MoveTowardsTarget();
				RotateTowardsPlayer();
			}
			else
			{
				MoveAlongPath();
			}
		}

		private void MoveAlongPath()
		{
			dst += Time.deltaTime * enemySpeed;
			var pos = path.GetPointAtDistance(dst, EndOfPathInstruction.Stop);
			var direction = path.GetDirectionAtDistance(dst, EndOfPathInstruction.Stop).normalized;
			var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

			transform.position = pos;
			transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

			if (transform.position == path.GetPoint(path.NumPoints - 1))
			{
				Destroy(gameObject);
			}
		}

		private void RotateTowardsPlayer()
		{
			if (!player)
			{
				return;
			}
			Vector3 direction = FindDirection(player.gameObject.transform.position);
			var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			FacingPlayer = angle <= 0;
			angle = Mathf.Abs(angle) * -1;
			transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
		}

		private Vector3 FindDirection(Vector3 pos)
		{
			var heading = pos - transform.position;
			var distance = heading.magnitude;
			var direction = heading / distance;
			return direction;
		}

		private void MoveTowardsTarget()
		{
			dst += Time.deltaTime * enemySpeed;
			var pos = path.GetPointAtDistance(dst, EndOfPathInstruction.Stop);
			transform.position = pos;

			if (transform.position == path.GetPoint(path.NumPoints - 1))
			{
				Destroy(gameObject);
			}
		}
	}
}

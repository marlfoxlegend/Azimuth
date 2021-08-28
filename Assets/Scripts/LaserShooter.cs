using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	public class LaserShooter : MonoBehaviour
	{
		[SerializeField] string pooledObjectName = "Laser";
		[SerializeField] AudioClip[] shootAudioClips;
		[Range(0f, 1f)] [SerializeField] float volumeShootAudio = 1f;

		ObjectPooler pooler;

		private void OnEnable()
		{
			pooler = FindObjectOfType<ObjectPooler>();
		}

		public void ShootLaser()
		{
			var laser = pooler.GetObjectFromPool(pooledObjectName);
			if (laser)
			{
				laser.transform.rotation = transform.rotation;
				laser.transform.position = transform.position;

				laser.SetActive(true);

				PlayLaserSound();
			}
		}

		private void PlayLaserSound()
		{
			var index = UnityEngine.Random.Range(0, shootAudioClips.Length);
			AudioSource.PlayClipAtPoint(
				shootAudioClips[index],
				Camera.main.transform.position,
				volumeShootAudio);
		}
	}
}

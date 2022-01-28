using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
    public class LaserShooter : MonoBehaviour
    {
        public PoolCategory laserCategory;
        [SerializeField] private AudioClip[] _laserClips;
        [SerializeField] [Range(0f, 1f)] private float _laserVolume = 1f;

        private ObjectPooler _pooler;


        private void OnEnable()
        {
            _pooler = FindObjectOfType<ObjectPooler>();
        }

        public void ShootLaser()
        {
            var laser = _pooler.GetObjectFromPool(laserCategory);
            if (!laser)
            {
                Debug.LogWarning($"No object returned from pool {laserCategory}.", this);
                return;
            }
            laser.transform.SetPositionAndRotation(transform.position, transform.rotation);
            laser.SetActive(true);
            PlayLaserAudio();
        }

        private void PlayLaserAudio()
        {
            int index = Random.Range(0, _laserClips.Length);
            Vector3 position = new Vector3(transform.position.x,
                                           transform.position.y,
                                           Camera.main.transform.position.z);
            AudioSource.PlayClipAtPoint(_laserClips[index], position, _laserVolume);
        }
    }
}

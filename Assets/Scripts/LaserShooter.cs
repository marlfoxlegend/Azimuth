using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
    public class LaserShooter : MonoBehaviour
    {
        public GameObject pooledLaser;
        [SerializeField] private int _laserPoolAmount = 10;
        [SerializeField] private AudioClip[] _laserClips;
        [SerializeField] [Range(0f, 1f)] private float _laserVolume = 1f;

        private ObjectPooler _pooler;


        private void OnEnable()
        {
            FindObjectOfType<ObjectPooler>()
                .AddPool(new PooledObject(pooledLaser,
                                          objectToPool: pooledLaser.tag));
        }

        public void ShootLaser()
        {
            var laser = _pooler.GetObjectFromPool(pooledLaser);
            if (!laser)
            {
                Debug.LogWarning($"No object with name {pooledLaser} found in {nameof(ObjectPooler)}.", gameObject);
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

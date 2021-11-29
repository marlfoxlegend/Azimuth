using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Azimuth.Events;
using UnityEngine;

namespace Azimuth
{
    public class EnemySpawner : MonoBehaviour, ISubscriber
    {
        [SerializeField] private List<Wave> _waves;
        [SerializeField] [Range(0, 20)] private int _loopCount = 1;

        private bool _waveCompleted = false;
        private bool _spawningCompleted;
        private int _spawns;

        private void OnEnable()
        {
            EventManager.PlayerEventHandler += OnNotify;
            EventManager.EnemyDestroyedHandler += OnNotify;
        }

        private void OnDisable()
        {
            EventManager.PlayerEventHandler -= OnNotify;
            EventManager.EnemyDestroyedHandler -= OnNotify;
        }

        private void Start()
        {
            _spawns = _waves.Sum(w => w.NumberOfEnemies * _loopCount);
            _ = StartCoroutine(StartSpawning());
        }

        private IEnumerator StartSpawning()
        {
            for (int i = 0; i < _loopCount; i++)
            {
                yield return StartCoroutine(SpawnWaves());
            }
            _spawningCompleted = true;
        }

        private IEnumerator SpawnWaves()
        {
            foreach (Wave wave in _waves)
            {
                _waveCompleted = false;
                yield return new WaitForSeconds(wave.TimeBetweenWaveSpawn);
                yield return StartCoroutine(SpawnWaveEnemies(wave));
            }
        }

        private IEnumerator SpawnWaveEnemies(Wave wave)
        {
            PathCreation.VertexPath path = wave.PathCreators[UnityEngine.Random.Range(0, wave.PathCreators.Count)].path;
            for (int i = 0; i < wave.NumberOfEnemies; i++)
            {
                var enemy = Instantiate(wave.EnemyInWave,
                                        path.GetPoint(0),
                                        Quaternion.identity,
                                        transform);
                enemy.GetComponent<EnemyPathing>().SetPath(path);
                enemy.SetSprite(wave.Sprite);
                yield return new WaitForSeconds(wave.TimeBetweenEnemySpawn);
            }
            _waveCompleted = true;
        }

        private void StopSpawning()
        {
            Debug.Log($"{nameof(EnemySpawner)} has stopped spawning.", this);

            StopAllCoroutines();
        }

        private void SpawnKilled(Enemy enemy)
        {
            Destroy(enemy.gameObject);
            if (transform.childCount == 0)
            {
                _waveCompleted = true;
            }
        }

        public void OnNotify(GameEventType eventType, object sender, GameEventArgs args)
        {
            switch (eventType)
            {
                case GameEventType.EnemyDestroyed:
                    SpawnKilled((Enemy)sender);
                    break;
                case GameEventType.PlayerDestroyed:
                    StopAllCoroutines();
                    break;
                default:
                    break;
            }
        }

        public void OnNotify(object sender, PlayerGameEvent playerDestroyed)
        {
            Debug.Log($"{nameof(EnemySpawner)} notified about {playerDestroyed}");
            if (playerDestroyed.PlayerDestroyed)
            {
                StopSpawning();
            }
        }

        public void OnNotify(object sender, EnemyDestroyedGameEvent enemyDestroyed)
        {
            Debug.Log($"{nameof(EnemySpawner)} notified about {enemyDestroyed}");
        }
    }
}
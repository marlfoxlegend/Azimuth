using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Azimuth.Events;
using UnityEngine;

namespace Azimuth
{
    public class EnemySpawner : MonoBehaviour
    {
        public event EventHandler<EnemyDestroyedEventArgs> onEnemyDestroyed;

        [SerializeField] private List<Wave> _waves;
        [SerializeField] [Range(0, 20)] private int _loopCount = 1;

        private readonly HashSet<Enemy> _enemies = new HashSet<Enemy>();
        private bool _waveCompleted = false;
        private bool _spawningCompleted;

        private void Start()
        {
            _ = StartCoroutine(StartSpawning());
        }

        private void LateUpdate()
        {
            var cleared = AllSpawnCleared();
            if (cleared)
            {
                GameManager.Instance.TriggerLevelFinish(cleared);
            }
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
            int index = UnityEngine.Random.Range(0, wave.PathCreators.Count);
            PathCreation.VertexPath path = wave.PathCreators[index].path;
            for (int i = 0; i < wave.NumberOfEnemies; i++)
            {
                var enemy = Instantiate(wave.EnemyInWave,
                                        path.GetPoint(0),
                                        Quaternion.identity,
                                        transform);
                enemy.GetComponent<EnemyPathing>().SetPath(path);
                enemy.SetSprite(wave.Sprite).SetSpawner(this);
                _enemies.Add(enemy);
                yield return new WaitForSeconds(wave.TimeBetweenEnemySpawn);
            }
            _waveCompleted = true;
        }

        public void StopSpawning()
        {
            Debug.Log($"{nameof(EnemySpawner)} has stopped spawning.", this);

            StopAllCoroutines();
        }

        public void RemoveSpawn(Enemy enemy, bool isDestroyed)
        {
            _ = _enemies.Remove(enemy);
            enemy.gameObject.SetActive(false);
            var e = new EnemyDestroyedEventArgs(enemy.RewardPoints(), isDestroyed);
            onEnemyDestroyed?.Invoke(enemy, e);
        }

        private IEnumerator CheckAllSpawnRemoved()
        {
            while (true)
            {
                if (AllSpawnCleared())
                {
                    yield break;
                }
            }
        }
        public bool AllSpawnCleared()
        {
            return _enemies.Count == 0 && _spawningCompleted;
        }
    }
}
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
        public event EventHandler<LevelFinishedEventArgs> onSpawningFinished;

        [SerializeField] private List<Wave> _waves;
        [SerializeField] [Range(0, 20)] private int _loopCount = 1;

        private readonly List<Enemy> _enemies = new List<Enemy>();
        private bool _spawningCompleted;

        private void Start()
        {
            _spawningCompleted = false;
            _ = StartCoroutine(StartSpawning());
        }

        private void LateUpdate()
        {
            if (AllSpawnCleared())
            {
                GameManager.Instance.FinishLevel(true);
                //var handler = onSpawningFinished;
                //if (handler != null)
                //{
                //    handler(this, new LevelFinishedEventArgs(true));
                //}
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
                enemy.SetSpawner(this);
                enemy.SetSprite(wave.Sprite);
                _enemies.Add(enemy);
                yield return new WaitForSeconds(wave.TimeBetweenEnemySpawn);
            }
        }

        public void StopSpawning()
        {
            Debug.Log($"{nameof(EnemySpawner)} has stopped spawning.", this);

            StopAllCoroutines();
        }

        public void RemoveSpawn(Enemy enemy, bool isDestroyed)
        {
            //var e = new EnemyDestroyedEventArgs(enemy.RewardPoints(), isDestroyed);
            //var handler = onEnemyDestroyed;
            //if (handler != null)
            //{
            //    handler(enemy, e);
            //}
            _ = _enemies.Remove(enemy);
            if (isDestroyed)
            {
                GameManager.Instance.AddToScore(enemy.RewardPoints());
            }
        }

        public bool AllSpawnCleared()
        {
            return _spawningCompleted && _enemies.Count == 0;
        }
    }
}
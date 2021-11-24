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

        private bool _allSpawnDestroyed = false;
        private int _spawned = 0;

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEventType.PlayerDestroyed, this);
            EventManager.Instance.Subscribe(GameEventType.EnemyDestroyed, this);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveAllSubscriber(this);
        }

        private void Start()
        {
            _ = StartCoroutine(StartSpawning());
        }

        private IEnumerator StartSpawning()
        {
            for (int i = 0; i < _loopCount; i++)
            {
                _allSpawnDestroyed = false;
                yield return StartCoroutine(SpawnWaves());
            }
            yield return new WaitUntil(() => _allSpawnDestroyed);
            Debug.Log($"{nameof(EnemySpawner)} has finished spawning and {nameof(_spawned)} == {_spawned}.", this);
            EventManager.Instance.TriggerEvent(GameEventType.SpawningCompleted, this, (GameEventArgs)EventArgs.Empty);
        }

        private IEnumerator SpawnWaves()
        {
            foreach (Wave wave in _waves)
            {
                _spawned += wave.NumberOfEnemies;
                yield return StartCoroutine(SpawnWaveEnemies(wave));
                yield return new WaitForSeconds(wave.TimeBetweenWaveSpawn);
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
        }

        private void StopSpawning()
        {
            StopAllCoroutines();
        }

        private void SpawnKilled(Enemy enemy)
        {
            _spawned--;
            Destroy(enemy.gameObject);
            if (_spawned <= 0)
            {
                _allSpawnDestroyed = true;
            }
        }

        public void OnNotify(object sender, GameEventArgs args, GameEventType eventType)
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
    }
}
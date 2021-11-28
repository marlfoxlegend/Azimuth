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
            //EventManager.Instance.Subscribe(GameEventType.PlayerDestroyed, this);
            //EventManager.Instance.Subscribe(GameEventType.EnemyDestroyed, this);
            EventManager.PlayerEventHandler += OnNotify;
            EventManager.EnemyDestroyedHandler += OnNotify;
        }

        private void OnDisable()
        {
            EventManager.PlayerEventHandler -= OnNotify;
            EventManager.EnemyDestroyedHandler -= OnNotify;
            //EventManager.Instance.RemoveSubscriberAll(this);
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
            Debug.Log($"{nameof(EnemySpawner)} has finished spawning and {nameof(_spawned)} == {_spawned}.", this);

            StopAllCoroutines();
            EventManager.Instance.TriggerEvent(this, new LevelCompletedGameEvent());
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
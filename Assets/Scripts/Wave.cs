using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Azimuth
{
    [CreateAssetMenu(fileName = "New Wave", menuName = "Wave", order = 11)]
    public class Wave : ScriptableObject
    {
        [SerializeField] private Enemy _enemyInWave;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _numberOfEnemies;
        [SerializeField] private float _timeBetweenEnemySpawn;
        [SerializeField] private float _timeBetweenWaveSpawn;
        [SerializeField] private List<PathCreator> _pathCreators;

        public Enemy EnemyInWave => _enemyInWave;
        public Sprite Sprite => _sprite;
        public int NumberOfEnemies => _numberOfEnemies;
        public float TimeBetweenEnemySpawn => _timeBetweenEnemySpawn;
        public float TimeBetweenWaveSpawn => _timeBetweenWaveSpawn;
        public List<PathCreator> PathCreators => _pathCreators;
    }
}

using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	[CreateAssetMenu(fileName = "New Wave", menuName = "Wave", order = 11)]
	public class Wave : ScriptableObject
	{
		//[Range(1, 5)] public int numberOfRows = 5;
		//[Range(0.1f, 5f)] public float spaceBetweenRows = 2;
		//[Range(1, 20)] public int numberOfColumns = 8;
		//[Range(0.1f, 5f)] public float shipPadding = 2;
		//public List<GameObject> enemyInRow;

		[SerializeField] GameObject enemyInWave;
		[SerializeField] int numberOfEnemies;
		[SerializeField] float timeBetweenSpawn;
		[SerializeField] float delayWaveStart;
		[SerializeField] List<PathCreator> pathCreators;

		public GameObject EnemyInWave => enemyInWave;
		public int NumberOfEnemies => numberOfEnemies;
		public float TimeBetweenSpawn => timeBetweenSpawn;
		public float DelayWaveStart => delayWaveStart;
		public List<PathCreator> PathCreators => pathCreators;
	}
}

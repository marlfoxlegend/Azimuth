using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	public class EnemySpawner : MonoBehaviour
	{
		[SerializeField] List<Wave> waves;
		[SerializeField] int loopCount = 1;

		Vector3[,] placements;
		Transform spawnArea;
		float startXPos;

		private void Awake()
		{
		}

		private void Start()
		{
			_ = StartCoroutine(SpawnWaves());
		}

		private IEnumerator CheckRemaining()
		{
			while (true)
			{
				if (transform.childCount == 0)
				{
					GameManagement.Instance.FinishLevel(GameManagement.CurrentState.Won);
					yield break;
				}
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator SpawnWaves()
		{
			yield return new WaitUntil(() => GameManagement.Instance.PlayState == GameManagement.CurrentState.Playing);
			for (int i = 0; i < loopCount; i++)
			{
				foreach (var wave in waves)
				{
					//yield return new WaitForSeconds(wave.DelayWaveStart);
					yield return StartCoroutine(SpawnWaveEnemies(wave));
					_ = StartCoroutine(CheckRemaining());
					//yield return new WaitWhile(() => transform.childCount >= 1);
				}
				//yield return new WaitUntil(() => transform.childCount == 0);
			}
		}

		private IEnumerator SpawnWaveEnemies(Wave wave)
		{
			var pathIndex = UnityEngine.Random.Range(0, wave.PathCreators.Count);
			var pth = wave.PathCreators[pathIndex];
			for (int i = 0; i < wave.NumberOfEnemies; i++)
			{
				var enemyPrefab = Instantiate(wave.EnemyInWave, pth.path.GetPoint(0), Quaternion.identity, transform);
				var enemy = enemyPrefab.GetComponent<EnemyPathing>();
				enemy.SetPath(pth.path);
				yield return new WaitForSeconds(wave.TimeBetweenSpawn);
			}
		}

		//private void CreateRows()
		//{
		//	// Get ship width and height in world units
		//	var ship = wave.enemyInRow[0].GetComponentInChildren<SpriteRenderer>().sprite;
		//	var shipWidth = ship.rect.width / ship.pixelsPerUnit;
		//	var shipHeight = ship.rect.height / ship.pixelsPerUnit;

		//	// Each ship is placed 1 ship width/height plus padding
		//	var shipOffset = new Vector3(shipWidth + wave.shipPadding, shipHeight + wave.spaceBetweenRows);
		//	shipOffset.y *= -1;

		//	// Origin is not (0,0) so begin in negative X
		//	startXPos = shipOffset.x * (wave.numberOfColumns - 1) / -2f;

		//	for (int row = 0; row < wave.numberOfRows; row++)
		//	{
		//		for (int col = 0; col < wave.numberOfColumns; col++)
		//		{
		//			var x = startXPos + (shipOffset.x * col);
		//			var y = shipOffset.y * (row + 1);
		//			placements[row, col] = new Vector3(x, y);
		//		}
		//	}
		//}

		//private void InitializeWave()
		//{
		//	for (int row = 0; row < wave.numberOfRows; row++)
		//	{
		//		var enemy = wave.enemyInRow[row];
		//		var numCols = placements.GetLength(1);
		//		for (int col = 0; col < numCols; col++)
		//		{
		//			Vector3 position = placements[row, col];
		//			var instEnemy = Instantiate(enemy, transform);
		//			instEnemy.transform.localPosition = position;
		//		}
		//	}
		//}
	}
}
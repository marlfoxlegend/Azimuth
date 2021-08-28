using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Azimuth
{
	public class GameManagement : MonoBehaviour
	{
		public enum CurrentState
		{
			Preparing,
			Playing,
			Lost,
			Won
		}

		[SerializeField] Image fadeImage;
		[SerializeField] float fadeTimer;
		[SerializeField] TextMeshProUGUI scoreDisplay;
		[SerializeField] Slider healthDisplay;
		public PlayerController.Stats baseStats;

		public static GameManagement Instance { get; private set; }

		public CurrentState PlayState { get; private set; } = CurrentState.Preparing;


		PlayerController.Stats finalStats = new PlayerController.Stats();
		PlayerController player;
		EnemySpawner enemySpawner;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else if (Instance != this)
			{
				gameObject.SetActive(false);
				Destroy(gameObject);
			}
			DontDestroyOnLoad(gameObject);
		}

		private void OnEnable()
		{
			player = FindObjectOfType<PlayerController>();
			enemySpawner = FindObjectOfType<EnemySpawner>();
			SceneManager.sceneLoaded += NewLevelLoaded;

		}

		private void Update()
		{
			healthDisplay.value = baseStats.health;
			scoreDisplay.text = baseStats.score.ToString("000000000");
		}

		private void OnDestroy()
		{
			SceneManager.sceneLoaded -= NewLevelLoaded;
		}

		public void SetUpdatedStats(PlayerController.Stats updated) => finalStats = updated;

		private void NewLevelLoaded(Scene loaded, LoadSceneMode mode)
		{
			player.SetPlayerControl(false);
			_ = StartCoroutine(FadeIntoLevel());

			if (loaded.name == "Level 1")
			{
				healthDisplay.maxValue = baseStats.health;
				healthDisplay.value = healthDisplay.maxValue;
			}
			else if (loaded.name == "Game Over")
			{
				healthDisplay.gameObject.SetActive(false);
				scoreDisplay.text = baseStats.score.ToString();
			}
			else
			{
				player.baseStats = baseStats;
			}
		}

		private IEnumerator FadeIntoLevel()
		{
			for (float a = fadeTimer; a >= 0f; a -= Time.deltaTime)
			{
				var color = fadeImage.color;
				var alpha = a / fadeTimer;
				color.a = alpha;
				fadeImage.color = color;
				yield return null;
			}
			PlayState = CurrentState.Playing;
			player.SetPlayerControl();
		}

		private IEnumerator FadeOutOfLevel()
		{
			for (float a = 0f; a <= fadeTimer; a += Time.deltaTime)
			{
				var color = fadeImage.color;
				var alpha = a / fadeTimer;
				color.a = alpha;
				fadeImage.color = color;
				yield return null;
			}
			PlayState = CurrentState.Preparing;
		}

		public void FinishLevel(CurrentState endState)
		{
			PlayState = endState;
			_ = StartCoroutine(FadeOutOfLevel());
			if (PlayState == CurrentState.Lost)
			{
				_ = StartCoroutine(GameOver());
			}
			else if (PlayState == CurrentState.Won)
			{
				_ = StartCoroutine(BeginNextLevel());
			}
		}

		private IEnumerator GameOver()
		{
			yield return new WaitUntil(() => PlayState == CurrentState.Preparing);
			Destroy(gameObject);
			SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
		}

		private IEnumerator BeginNextLevel()
		{
			yield return new WaitUntil(() => PlayState == CurrentState.Preparing);
			var index = SceneManager.GetActiveScene().buildIndex;
			Destroy(gameObject);
			SceneManager.LoadScene(index);
		}
	}
}

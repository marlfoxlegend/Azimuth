using S = System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;
using Azimuth.Events;

namespace Azimuth
{
    public class GameManager : MonoBehaviour
    {
        public enum GameState
        {
            Loading,
            Playing,
            Unloading
        }

        private const string ScoreFormat = "{0:00000000#}";

#pragma warning disable IDE0044 // Add readonly modifier
        [SerializeField] private Image _fadeImage;
        [SerializeField] [Min(0.1f)] private float _fadeTimer;
        [SerializeField] private TextMeshProUGUI _scoreDisplay;
        [SerializeField] private Slider _healthDisplay;
        [SerializeField] private TextMeshProUGUI _levelDisplay;
#pragma warning restore IDE0044 // Add readonly modifier

        [NonSerialized] public int maxHealth;
        private int _score;
        private int _highScore;
        private int _level;
        private bool _isFading = false;
        private bool _reset = false;
        internal PlayerController player;
        internal EnemySpawner spawner;
        private static GameManager s_manager;

        public static GameManager Instance
        {
            get
            {
                if (!s_manager)
                {
                    s_manager = FindObjectOfType<GameManager>();
                    if (!s_manager)
                    {
                        Debug.LogError($"Error no {nameof(GameManager)} object found.");
                        return null;
                    }
                }
                return s_manager;
            }
        }

        public GameState PlayState { get; private set; } = GameState.Loading;


        private void Awake()
        {
            if (FindObjectsOfType<GameManager>().Length > 1)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += NewLevelLoaded;
            Init();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= NewLevelLoaded;
        }

        private void Init()
        {
            player = FindObjectOfType<PlayerController>();
            spawner = FindObjectOfType<EnemySpawner>();
        }

        private void NewLevelLoaded(Scene loaded, LoadSceneMode mode)
        {
            _levelDisplay.text = loaded.name;
            _scoreDisplay.text = string.Format(ScoreFormat, _score);
            _healthDisplay.value = 1f;
            _ = StartCoroutine(FadeIntoLevel());
        }

        private IEnumerator FadeIntoLevel()
        {
            _isFading = true;
            _levelDisplay.gameObject.SetActive(true);
            _fadeImage.gameObject.SetActive(true);

            float a = 0f;
            float t = 0f;
            while (t < 1f)
            {
                Color color = _fadeImage.color;
                t = Mathf.InverseLerp(0f, _fadeTimer, a);
                t = Mathf.Clamp01(t);
                color.a = 1f - t;

                _fadeImage.color = color;

                a += Time.deltaTime;
                yield return null;
            }
            _levelDisplay.gameObject.SetActive(false);
            _fadeImage.gameObject.SetActive(false);
            _isFading = false;
            Debug.Assert(!_levelDisplay.gameObject.activeInHierarchy);
        }

        private IEnumerator FadeOutOfLevel()
        {
            _isFading = true;
            _fadeImage.gameObject.SetActive(true);
            float a = 0f;
            float t = 0f;
            while (t < 1f)
            {
                Color color = _fadeImage.color;
                t = Mathf.InverseLerp(0f, _fadeTimer, a);
                t = Mathf.Clamp01(t);
                color.a = t;

                _fadeImage.color = color;
                a += Time.deltaTime;
                yield return null;
            }
            _isFading = false;
        }

        public void FinishLevel(object sender, LevelFinishedEventArgs e)
        {
            _ = e.PlayerWon ? StartCoroutine(LevelWon()) : StartCoroutine(LevelLost());
        }

        public void FinishLevel(bool playerWon)
        {
            _ = playerWon ? StartCoroutine(LevelWon()) : StartCoroutine(LevelLost());
        }

        private IEnumerator LevelWon()
        {
            yield return StartCoroutine(FadeOutOfLevel());
            _level = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(_level);
        }

        private IEnumerator LevelLost()
        {
            yield return StartCoroutine(FadeOutOfLevel());
            Debug.Log($"{nameof(FadeOutOfLevel)} has completed.");
            ResetGame(false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void AddToScore(object sender, EnemyDestroyedEventArgs e)
        {
            if (!e.DestroyedByPlayer)
                return;
            _score += e.Points;
            _scoreDisplay.text = string.Format(ScoreFormat, _score);
        }

        public void AddToScore(int points)
        {
            _score += points;
            _scoreDisplay.text = string.Format(ScoreFormat, _score);
        }

        public void UpdateHealth(object sender, PlayerHealthEventArgs e)
        {
            _healthDisplay.value = e.HealthValue / e.MaxHealth;
        }

        public void UpdateHealth(PlayerHealthEventArgs e)
        {
            _healthDisplay.value = e.HealthValue / e.MaxHealth;
        }

        public void ResetGame(bool returnToMenu = false)
        {
            if (returnToMenu)
            {
                Destroy(gameObject);
                return;
            }
            _highScore = Mathf.Max(_score, _highScore);
            _score = 0;
            _level = 0;
            _reset = true;
        }
    }
}

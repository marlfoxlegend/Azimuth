using S = System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

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

        private const string ScoreFormat = "000000000";

        [SerializeField] private Image _fadeImage;
        [SerializeField] [Min(0.1f)] private float _fadeTimer;
        [SerializeField] private TextMeshProUGUI _scoreDisplay;
        [SerializeField] private Slider _healthDisplay;
        [SerializeField] private TextMeshProUGUI _levelDisplay;

        private int _maxHealth;
        private static GameManager s_manager;
        public static GameManager Instance
        {
            get
            {
                if (! s_manager)
                {
                    s_manager = FindObjectOfType<GameManager>();
                    if (! s_manager)
                    {
                        Debug.LogError($"{nameof(GameManager)} could not be found. Make sure one is included in the scene.", s_manager);
                        return null;
                    }
                }
                return s_manager;
            }
        }

        public GameState PlayState { get; private set; } = GameState.Loading;


        private void Awake()
        {
            if (!s_manager || s_manager == this)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += NewLevelLoaded;
            EventManager.LevelCompletedHandler += FinishLevel;
            _maxHealth = FindObjectOfType<PlayerController>().BaseStats.health;
        }

        private void OnDisable()
        {
            EventManager.LevelCompletedHandler -= FinishLevel;
            SceneManager.sceneLoaded -= NewLevelLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= NewLevelLoaded;
        }

        private void NewLevelLoaded(Scene loaded, LoadSceneMode mode)
        {
            _levelDisplay.text = loaded.name;
            _ = StartCoroutine(FadeIntoLevel());
        }

        private IEnumerator FadeIntoLevel()
        {
            if (_fadeImage.color.a != 1f)
            {
                Color c = _fadeImage.color;
                c.a = 1f;
                _fadeImage.color = c;
            }

            float t, a = 0f;
            _fadeImage.gameObject.SetActive(true);
            _levelDisplay.gameObject.SetActive(true);

            while (true)
            {
                Color color = _fadeImage.color;
                t = Mathf.InverseLerp(0f, _fadeTimer, a);
                t = Mathf.Clamp01(t);
                color.a = 1f - t;

                _fadeImage.color = color;

                if (t >= 1f)
                {
                    break;
                }

                a += Time.deltaTime;
                yield return null;
            }
            _levelDisplay.gameObject.SetActive(false);
            _fadeImage.gameObject.SetActive(false);
            PlayState = GameState.Playing;
            FindObjectOfType<PlayerController>().SetPlayerControl();
        }

        private IEnumerator FadeOutOfLevel()
        {
            PlayState = GameState.Playing;
            if (_fadeImage.color.a != 0f)
            {
                Color c = _fadeImage.color;
                c.a = 0f;
                _fadeImage.color = c;
            }

            float t, a = 0f;
            _fadeImage.gameObject.SetActive(true);

            while (true)
            {
                Color color = _fadeImage.color;
                t = Mathf.InverseLerp(0f, _fadeTimer, a);
                t = Mathf.Clamp01(t);
                color.a = t;

                _fadeImage.color = color;

                if (t >= 1f)
                {
                    break;
                }

                a += Time.deltaTime;
                yield return null;
            }
            PlayState = GameState.Loading;
        }

        private void FinishLevel(object sender, Events.LevelCompletedGameEvent levelCompleted)
        {
            PlayState = GameState.Unloading;
            if (sender is PlayerController player)
            {
                LevelWon();
            }
            else
            {
                LevelLost();
            }
        }

        public void LevelWon()
        {
            _ = StartCoroutine(BeginNextLevel());
        }

        public void LevelLost()
        {
            _ = StartCoroutine(GameOver());
        }

        private IEnumerator GameOver()
        {
            yield return StartCoroutine(FadeOutOfLevel());
            yield return new WaitUntil(() => PlayState == GameState.Loading);
            SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
        }

        private IEnumerator BeginNextLevel()
        {
            yield return StartCoroutine(FadeOutOfLevel());
            yield return new WaitUntil(() => PlayState == GameState.Loading);
            var index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index);
        }

        public void SetScore(int amount)
        {
            _scoreDisplay.text = string.Format(ScoreFormat, amount);
        }
    }
}

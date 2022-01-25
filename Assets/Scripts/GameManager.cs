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

        public event EventHandler GameEventHandler;
        public event EventHandler<PlayerGameEventArgs> PlayerGameEventHandler;
        private const string ScoreFormat = "000000000";

#pragma warning disable IDE0044 // Add readonly modifier
        [SerializeField] private Image _fadeImage;
        [SerializeField] [Min(0.1f)] private float _fadeTimer;
        [SerializeField] private TextMeshProUGUI _scoreDisplay;
        [SerializeField] private Slider _healthDisplay;
        [SerializeField] private TextMeshProUGUI _levelDisplay;
#pragma warning restore IDE0044 // Add readonly modifier

        private int _maxHealth;
        private int _score;
        private int _level;
        private bool _isFading = false;
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
            if (s_manager == null)
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
            ConnectEvents();
        }

        private void OnDisable()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            SceneManager.sceneLoaded += NewLevelLoaded;
            _maxHealth = FindObjectOfType<PlayerController>().BaseStats.health;
        }

        private void DisconnectEvents()
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
            _isFading = !_isFading;
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
            _isFading = false;
        }

        private IEnumerator FadeOutOfLevel()
        {
            _isFading = true;
            s_manager.PlayState = GameState.Playing;
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
            _isFading = false;
            _fadeImage.gameObject.SetActive(false);
        }

        public void FinishLevel(object sender)
        {
            s_manager.PlayState = GameState.Unloading;
            _ = StartCoroutine(FadeOutOfLevel());
            if (sender is PlayerController player)
            {
                player.SetPlayerControl(false);
                _ = LevelLost();
            }
            else if (sender is EnemySpawner spawner)
            {
                LevelWon();
            }
        }

        public IEnumerator LevelWon()
        {
            _level = SceneManager.GetActiveScene().buildIndex;
            yield return new WaitUntil(() => !_isFading);
            SceneManager.LoadScene(_level);
        }

        public IEnumerator LevelLost()
        {
            _ = StartCoroutine(GameOver());
            _level = SceneManager.sceneCountInBuildSettings - 1;
            yield return new WaitUntil(() => !_isFading);
            SceneManager.LoadScene(_level);
        }

        private IEnumerator GameOver()
        {
            yield return StartCoroutine(FadeOutOfLevel());
            yield return new WaitUntil(() => PlayState == GameState.Loading);
        }

        public void AddToScore(int amount)
        {
            _scoreDisplay.text = string.Format(ScoreFormat, amount);
        }
    }
}

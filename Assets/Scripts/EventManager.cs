using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using Azimuth.Events;

namespace Azimuth
{
    public class EventManager : MonoBehaviour
    {
        public delegate void HandleGameEvent<in TGameEvent>(TGameEvent gameEvent);

        public static event EventHandler<PlayerGameEvent> PlayerEventHandler;
        public static event EventHandler<LevelCompletedGameEvent> LevelCompletedHandler;
        public static event EventHandler<EnemyDestroyedGameEvent> EnemyDestroyedHandler;
        public static event EventHandler<GameEvent> AllEnemiesCleared;

        private static EventManager s_eventManager;
        private static Dictionary<GameEventType, List<ISubscriber>> s_events;

        public static EventManager Instance
        {
            get
            {
                if (!s_eventManager)
                {
                    s_eventManager = FindObjectOfType<EventManager>();
                    if (!s_eventManager)
                    {
                        Debug.LogError($"No {nameof(EventManager)} found in the scene. " +
                            $"Include one {nameof(GameObject)} with a {nameof(EventManager)} script.");
                    }
                    else
                    {
                        s_eventManager.Init();
                    }
                }
                return s_eventManager;
            }
        }

        private void Init()
        {
            if (s_events == null)
            {
                s_events = new Dictionary<GameEventType, List<ISubscriber>>();
            }
        }

        public void TriggerEvent(object sender, PlayerGameEvent gameEvent)
        {
            Debug.Log($"Triggering {nameof(PlayerGameEvent)}.");
            PlayerEventHandler?.Invoke(sender, gameEvent);
        }
        public void TriggerEvent(object sender, EnemyDestroyedGameEvent gameEvent)
        {
            Debug.Log($"Triggering {nameof(EnemyDestroyedGameEvent)}.");
            EnemyDestroyedHandler?.Invoke(sender, gameEvent);
        }
        public void TriggerEvent(object sender, LevelCompletedGameEvent gameEvent)
        {
            Debug.Log($"Triggering {nameof(LevelCompletedHandler)}.");
            LevelCompletedHandler?.Invoke(sender, gameEvent);
        }

        public void Subscribe(GameEventType gameEventType, ISubscriber subscriber)
        {
            if (s_events.ContainsKey(gameEventType))
            {
                s_events[gameEventType].Add(subscriber);
            }
            else
            {
                s_events.Add(gameEventType, new List<ISubscriber>() { subscriber });
            }
            Debug.Log($"{((MonoBehaviour)subscriber).name} subscribed to {gameEventType}.");
        }

        public bool RemoveSubscriber(GameEventType gameEventType, ISubscriber subscriber)
        {
            Debug.Log($"{((MonoBehaviour)subscriber).name} attempting to unsubscribe from {gameEventType}.");
            
            bool removed = true;
            if (s_events.ContainsKey(gameEventType) && s_events[gameEventType].Contains(subscriber))
            {
                removed = s_events[gameEventType].Remove(subscriber);
            }
            return removed;
        }

        public bool RemoveSubscriberAll(ISubscriber subscriber)
        {
            Debug.Log($"{((MonoBehaviour)subscriber).name} attempting to unsubscribe from all events.");
            
            bool allRemoved = true;
            foreach (GameEventType eventType in s_events.Keys)
            {
                if (s_events[eventType].Contains(subscriber))
                {
                    allRemoved = s_events[eventType].Remove(subscriber);
                }
            }
            return allRemoved;
        }
    }

    public enum GameEventType
    {
        LevelCompleted,
        LevelStarted,
        EnemyDestroyed,
        PlayerDestroyed,
        SpawningCompleted,
        PowerupCollected
    }

    public interface ISubscriber
    {
        public void OnNotify(GameEventType eventType, object sender, GameEventArgs args);
    }
}
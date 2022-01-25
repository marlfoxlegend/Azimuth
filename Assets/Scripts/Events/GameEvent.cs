using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth.Events
{
    public class GameEvent
    {
    }
    public class LevelCompletedGameEvent
    {
        protected Action finishTask;
        public Action FinishTask
        {
            get => finishTask;
            set
            {
                if (finishTask == null)
                {
                    finishTask = value;
                }
                else
                {
                    finishTask += value;
                }
            }
        }
    }

    public struct PlayerGameEvent
    {
        public int PlayerScore { get; }
        public int PlayerHealth { get; }
        public bool PlayerDestroyed { get; }

        public PlayerGameEvent(int playerScore, int playerHealth, bool playerDestroyed) : this()
        {
            PlayerScore = playerScore;
            PlayerHealth = playerHealth;
            PlayerDestroyed = playerDestroyed;
        }
    }

    public struct EnemyDestroyedGameEvent
    {
        public int Points { get; }

        public EnemyDestroyedGameEvent(int points): this()
        {
            Points = points;
        }
    }

    public class GameEventArgs : EventArgs
    {
        public static new GameEventArgs Empty => EventArgs.Empty as GameEventArgs;
    }
    public class PlayerDestroyedEventArgs : GameEventArgs
    {
        public int PlayerScore { get; }
        public int PlayerHealth { get; }
        public PlayerDestroyedEventArgs(int playerScore, int playerHealth)
        {
            PlayerScore = playerScore;
            PlayerHealth = playerHealth;
        }
    }
    public class PlayerGameEventArgs:EventArgs
    {
        public readonly bool winner;

        public PlayerGameEventArgs(bool winner)
        {
            this.winner = winner;
        }
    }
    public class PlayerEventArgs : EventArgs
    {
        public enum PlayerEvent
        {
            Destroyed, HealthChange
        }
        public readonly PlayerEvent playerEvent;

        public PlayerEventArgs(PlayerEvent playerEvent)
        {
            this.playerEvent = playerEvent;
        }
    }
    public class PlayerHealthEventArgs : EventArgs
    {
        public readonly float maxHealth;
        public readonly float healthValue;
        public readonly float delta;

        public PlayerHealthEventArgs(float maxHealth, float healthValue, float oldValue)
        {
            this.maxHealth = maxHealth;
            this.healthValue = healthValue;
            delta = healthValue - oldValue;
        }
    }
    public class EnemyDestroyedEventArgs : GameEventArgs
    {
        public int Points { get; }
        public bool DestroyedByPlayer { get; }

        public EnemyDestroyedEventArgs(int points, bool destroyedByPlayer)
        {
            Points = points;
            DestroyedByPlayer = destroyedByPlayer;
        }
    }
}

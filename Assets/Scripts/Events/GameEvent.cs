using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth.Events
{
    public class LevelFinishedEventArgs : EventArgs
    {
        public bool PlayerWon { get; }

        public LevelFinishedEventArgs(bool playerWon)
        {
            PlayerWon = playerWon;
        }
    }
    public class PlayerStateEventArgs : EventArgs
    {
        public enum PlayerEventType
        {
            Destroyed, PowerUpCollected
        }
        
        public PlayerEventType PlayerEvent { get; }
        public PlayerController.PowerUpStat PowerUp { get; }

        public PlayerStateEventArgs(
            PlayerEventType playerEventType, PlayerController.PowerUpStat powerUp = null)
        {
            PlayerEvent = playerEventType;
            PowerUp = powerUp;
        }
    }
    public class PlayerHealthEventArgs : EventArgs
    {
        public float MaxHealth { get; }
        public float HealthValue { get; }
        public float OldHealthValue { get; }

        public PlayerHealthEventArgs(float maxHealth, float healthValue, float oldValue)
        {
            MaxHealth = maxHealth;
            HealthValue = healthValue;
            OldHealthValue = oldValue;
        }
    }
    public class EnemyDestroyedEventArgs : EventArgs
    {
        public int Points { get; }
        public bool DestroyedByPlayer { get; }

        public EnemyDestroyedEventArgs(int points, bool destroyedByPlayer = true)
        {
            Points = points;
            DestroyedByPlayer = destroyedByPlayer;
        }
    }
}

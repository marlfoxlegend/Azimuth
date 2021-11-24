using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth.Events
{
    public class GameEventArgs : EventArgs
    {
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

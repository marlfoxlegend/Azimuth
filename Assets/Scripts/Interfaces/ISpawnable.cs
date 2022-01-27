using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
    public interface ISpawnable<T> where T : EnemySpawner
    {
        public T GetSpawner();
        public void SetSpawner(T spawner);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimuth
{
    public interface IDestroyer
    {
        public int GetDamageAmount();
        public void DamageTarget(IDestroyable target);
    }
}

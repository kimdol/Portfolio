using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ARPG.Core
{
    public interface IDamagable
    {
        // Properties
        bool IsAlive
        {
            get;
        }

        void TakeDamage(int damage, GameObject attacker, GameObject hitEffect);
    }
}

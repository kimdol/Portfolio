using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.Characters
{
    public class ProjectileAttackBehaviour : AttackBehaviour
    {
        public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
        {
            if (target == null)
            {
                return;
            }

            Vector3 projectilePosition = startPoint?.position ?? transform.position;

            if (effectPrefab != null)
            {
                GameObject projectileGO = GameObject.Instantiate<GameObject>(effectPrefab, 
                    projectilePosition, 
                    Quaternion.identity);
                Projectile projectile = projectileGO.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.owner = this.gameObject;
                    projectile.target = target;
                    projectile.attackBehaviour = this;
                }
            }
            calcCoolTime = 0.0f;
        }
    }
}

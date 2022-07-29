using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.Characters
{
    public class RandomMoveProjectileAttackBehaviour : AttackBehaviour
    {
        [Range(0, 360)]
        public float angle = 30f;
        public int projectileCount = 3;

        public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
        {
            if (target == null)
            {
                return;
            }

            Vector3 projectilePosition = startPoint?.position ?? transform.position;
            float totalAngle = angle * (projectileCount / 2);

            for (int i = 0; i < projectileCount; i++)
            {
                if (effectPrefab != null)
                {
                    GameObject projectileGO = GameObject.Instantiate<GameObject>(effectPrefab,
                        projectilePosition,
                        Quaternion.identity);
                    projectileGO.transform.localEulerAngles = new Vector3(0, totalAngle - angle * i, 0);
                    Projectile projectile = projectileGO.GetComponent<Projectile>();
                    if (projectile != null)
                    {
                        projectile.owner = this.gameObject;
                        projectile.target = target;
                        projectile.attackBehaviour = this;
                    }
                }
            }
            
            calcCoolTime = 0.0f;
        }
    }
}


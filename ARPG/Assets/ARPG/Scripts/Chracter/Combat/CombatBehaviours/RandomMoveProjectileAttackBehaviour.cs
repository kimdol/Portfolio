using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.Characters
{
    public class RandomMoveProjectileAttackBehaviour : AttackBehaviour
    {
        [Range(0, 360)]
        public float stepAngle = 30f;
        public int projectileCount = 3;

        public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
        {
            if (target == null)
            {
                return;
            }

            Vector3 projectilePosition = startPoint?.position ?? transform.position;
            Quaternion projectileRotation = transform.rotation;
            float totalAngle = stepAngle * (projectileCount - 1);

            for (int i = 0; i < projectileCount; i++)
            {
                if (effectPrefab != null)
                {
                    GameObject projectileGO = GameObject.Instantiate<GameObject>(effectPrefab,
                        projectilePosition,
                        projectileRotation);

                    projectileGO.transform.Rotate(0, totalAngle / 2 - stepAngle * i, 0);

                    Projectile_RandomMove projectile = projectileGO.GetComponent<Projectile_RandomMove>();
                    if (projectile != null)
                    {
                        projectile.owner = this.gameObject;
                        projectile.target = target;
                        projectile.attackBehaviour = this;
                        projectile.index = i;
                    }
                }
            }
            
            calcCoolTime = 0.0f;
        }
    }
}


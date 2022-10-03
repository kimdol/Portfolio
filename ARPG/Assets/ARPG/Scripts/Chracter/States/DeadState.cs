using ARPG.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEditor.Experimental.TerrainAPI;
using UnityEditorInternal;
using UnityEngine;

namespace ARPG.AI
{
    [Serializable]
    public class DeadState : State<EnemyController>
    {
        private Animator animator;

        protected int isAliveHash = Animator.StringToHash("IsAlive");

        private bool isPatrol = false;

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(isAliveHash, false);

            if (context is EnemyController_Patrol)
            {
                isPatrol = true;
            }
        }

        public override void Update(float deltaTime)
        {
            if (stateMachine.ElapsedTimeInState > 3.0f)
            {
                if (isPatrol)
                {
                    GameObject.Destroy(context.gameObject);
                }
                else
                {
                    context.OnDead();
                }
            }
        }

        public override void OnExit()
        {
        }
    }
}

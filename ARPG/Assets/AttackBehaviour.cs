using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace ARPG.Characters
{
    //[Serializable]
    //public enum AttackType : int
    //{
    //    Melee,
    //    Range,
    //}

    public abstract class AttackBehaviour : MonoBehaviour
    {
        #region Variables

#if UNITY_EDITOR
        [Multiline]
        public string developmentDescription = "";
#endif  // UNITY_EDITOR

        public int animationIndex;

        //public AttackType type;
        public int priority;

        public int damage;
        public float range = 3f;

        [SerializeField]
        private float coolTime;

        public GameObject effectPrefab;

        protected float calcCoolTime = 0.0f;

        [HideInInspector]
        public LayerMask targetMask;

        [SerializeField]
        public bool IsAvailable => calcCoolTime >= coolTime;

        #endregion Variables

        protected virtual void Start()
        {
            calcCoolTime = coolTime;
        }

        // Update is called once per frame
        protected void Update()
        {
            if (calcCoolTime < coolTime)
            {
                calcCoolTime += Time.deltaTime;
            }
        }

        public abstract void ExecuteAttack(GameObject target = null, Transform startPoint = null);
    }
}

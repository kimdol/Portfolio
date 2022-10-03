using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ARPG.Characters
{
    public class Actor : MonoBehaviour
    {
        [SerializeField]
        protected bool isDead = false;

        public bool IsDead
        {
            get
            {
                return isDead;
            }
        }


        // Start is called before the first frame update
        protected virtual void Start()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {
            UpdateActor();
        }

        protected virtual void UpdateActor()
        {

        }

        public virtual void OnDead()
        {
            Debug.Log(name + " OnDead");
            isDead = true;
        }
    }
}
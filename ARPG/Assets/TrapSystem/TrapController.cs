using ARPG.Characters;
using ARPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.TrapSystem
{
    public class TrapController : MonoBehaviour
    {
        #region Variables
        public float damageInterval = 0.5f;
        public float damageDuration = 5f;
        public int damage = 5;

        private float calcDuration = 0.0f;

        [SerializeField]
        private ParticleSystem effect;

        private IDamagable damagable;
        #endregion Variables

        #region Unity Methods
        private void Update()
        {
            calcDuration -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            damagable = other.GetComponent<IDamagable>();
            if (damagable != null)
            {
                calcDuration = damageDuration;

                effect.Play();
                StartCoroutine(ProcessDamage());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            damagable = null;
            StopAllCoroutines();
            effect.Stop();
        }
        #endregion Unity Methods

        #region Methods
        IEnumerator ProcessDamage()
        {
            while (calcDuration > 0 && damagable != null)
            {
                damagable.TakeDamage(damage, null, null);

                yield return new WaitForSeconds(damageInterval);
            }

            damagable = null;
            effect.Stop();
        }
        #endregion Methods
    }
}

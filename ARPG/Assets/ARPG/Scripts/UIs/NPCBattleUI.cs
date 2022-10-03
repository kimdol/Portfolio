using UnityEngine;
using UnityEngine.UI;

namespace ARPG.UIs
{
    public class NPCBattleUI : MonoBehaviour
    {
        #region Variables

        private Slider hpSlider;

        [SerializeField]
        private GameObject damageTextPrefab;

        #endregion Variables

        #region Properties
        public float MinimumValue
        {
            get => hpSlider.minValue;
            set
            {
                hpSlider.minValue = value;
            }
        }

        public float MaximumValue
        {
            get => hpSlider.maxValue;
            set
            {
                hpSlider.maxValue = value;
            }
        }

        public float Value
        {
            get => hpSlider.value;
            set
            {
                hpSlider.value = value;
            }
        }

        #endregion Properties

        #region Unity Methods
        private void Awake()
        {
            hpSlider = gameObject.GetComponentInChildren<Slider>();
        }

        private void OnEnable()
        {
            GetComponent<Canvas>().enabled = true;
        }

        private void OnDisable()
        {
            GetComponent<Canvas>().enabled = false;
        }

        #endregion Unity Methods

        #region Damage Methods
        public void TakeDamage(int damage)
        {
            if (damageTextPrefab != null)
            {
                GameObject damageTextGO = Instantiate(damageTextPrefab, transform);
                DamageText damageText = damageTextGO.GetComponent<DamageText>();
                if (damageText == null)
                {
                    Destroy(damageTextGO);
                }

                damageText.Damage = damage;
            }
        }
        #endregion Damage Methods
    }
}

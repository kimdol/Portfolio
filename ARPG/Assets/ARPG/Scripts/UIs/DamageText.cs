using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ARPG.UIs
{
    public class DamageText : MonoBehaviour
    {
        #region Variables
        private TextMeshProUGUI textMeshPro;

        public float destroyDelayTime = 1.0f;

        #endregion Variables

        #region Properties

        public int Damage
        {
            get
            {
                if (textMeshPro != null)
                {
                    return int.Parse(textMeshPro.text);
                }

                return 0;
            }
            set
            {
                if (textMeshPro != null)
                {
                    textMeshPro.text = value.ToString();
                }
            }
        }

        #endregion Properties

        #region Unity Methods
        private void Awake()
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }
        #endregion Unity Methods

        private void Start()
        {
            Destroy(gameObject, destroyDelayTime);
        }
    }
}

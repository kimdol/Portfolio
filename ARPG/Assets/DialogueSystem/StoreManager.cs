using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPG.StoreSystem
{
    public class StoreManager : MonoBehaviour
    {
        #region Variables
        private static StoreManager instance;

        public Text nameText;

        public Animator animator = null;

        public Transform blockInputPannel;


        public event Action OnStartSale;
        public event Action OnEndSale;
        #endregion Variables

        #region Properties
        public static StoreManager Instance => instance;
        #endregion Properties

        #region Unity Methods
        private void Awake()
        {
            instance = this;
        }

        #endregion Unity Methods

        #region Methods

        public void StartSale(string name)
        {
            OnStartSale?.Invoke();
            blockInputPannel.gameObject.SetActive(true);

            animator?.SetBool("IsOpen", true);

            nameText.text = name;
        }

        public void FinishSale()
        {
            EndSale();
        }

        void EndSale()
        {
            animator?.SetBool("IsOpen", false);

            blockInputPannel.gameObject.SetActive(false);
            OnEndSale?.Invoke();
        }

        #endregion Methods
    }
}

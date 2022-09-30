using ARPG.Characters;
using ARPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.StoreSystem
{
    public class StoreNPC : MonoBehaviour, IInteractable
    {
        #region Variables

        public string name;

        bool isStartSale = false;

        GameObject interactGO;

        #endregion Variables

        #region Unity Methods

        private void OnDisable()
        {
            StoreManager.Instance.OnEndSale -= OnEndSale;
        }

        #endregion Unity Methods

        #region IInteractable Interface

        [SerializeField]
        private float distance = 2.0f;

        public float Distance => distance;

        public void Interact(GameObject other)
        {
            float calcDistance = Vector3.Distance(other.transform.position, transform.position);
            if (calcDistance > distance)
            {
                return;
            }

            if (isStartSale)
            {
                return;
            }

            this.interactGO = other;

            StoreManager.Instance.OnEndSale += OnEndSale;
            isStartSale = true;

            StoreManager.Instance.StartSale(name);
        }

        public void StopInteract(GameObject other)
        {
            isStartSale = false;

            PlayerCharacter playerCharacter = other?.GetComponent<PlayerCharacter>();
            if (playerCharacter)
            {
                playerCharacter.RemoveTarget();
            }
        }

        #endregion IInteractable Interface

        #region Methods
        private void OnEndSale()
        {
            StopInteract(interactGO);
        }
        #endregion Methods
    }
}

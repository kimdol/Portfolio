using ARPG.Characters;
using ARPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.DialogueSystem
{
    public class DialogueNPC : MonoBehaviour, IInteractable
    {
        #region Variables

        [SerializeField]
        Dialogue dialogue;

        bool isStartDialogue = false;

        GameObject interactGO;

        #endregion Variables

        #region Unity Methods
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

            if (isStartDialogue)
            {
                return;
            }

            this.interactGO = other;

            DialogueManager.Instance.OnEndDialogue += OnEndDialogue;
            isStartDialogue = true;

            DialogueManager.Instance.StartDialogue(dialogue);
        }

        public void StopInteract(GameObject other)
        {
            isStartDialogue = false;

            PlayerCharacter playerCharacter = other?.GetComponent<PlayerCharacter>();
            if (playerCharacter)
            {
                playerCharacter.RemoveTarget();
            }
        }

        #endregion IInteractable Interface

        #region Methods
        private void OnEndDialogue()
        {
            StopInteract(interactGO);
        }
        #endregion Methods
    }
}

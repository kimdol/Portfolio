using ARPG.Characters;
using ARPG.Core;
using ARPG.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.QuestSystem
{
    public class QuestNPC : MonoBehaviour, IInteractable
    {
        #region Variables
        public QuestObject questObject;

        public Dialogue readyDialogue;
        public Dialogue acceptedDialogue;
        public Dialogue completedDialogue;

        bool isStartQuestDialogue = false;
        GameObject interactGO = null;

        [SerializeField]
        private GameObject questEffectGO;
        [SerializeField]
        private GameObject questRewardGO;

        #endregion Variables

        #region Unity Methods
        private void Start()
        {
            questEffectGO.SetActive(false);
            questRewardGO.SetActive(false);
            if (questObject.status == QuestStatus.None)
            {
                questEffectGO.SetActive(true);
            }
            else if (questObject.status == QuestStatus.Completed)
            {
                questRewardGO.SetActive(true);
            }

            QuestManager.Instance.OnCompletedQuest += OnCompletedQuest;
        }
        #endregion Unity Methods

        #region IInteractable Interface
        public float distance = 2.0f;
        public float Distance => distance;

        public void Interact(GameObject other)
        {
            float calcDistance = Vector3.Distance(other.transform.position, transform.position);
            if (calcDistance > Distance)
            {
                return;
            }

            if (isStartQuestDialogue)
            {
                return;
            }

            interactGO = other;
            isStartQuestDialogue = true;

            if (questObject.status == QuestStatus.None)
            {
                DialogueManager.Instance.StartDialogue(readyDialogue);
                questObject.status = QuestStatus.Accepted;
            }
            else if (questObject.status == QuestStatus.Accepted)
            {
                DialogueManager.Instance.StartDialogue(acceptedDialogue);
            }
            else if (questObject.status == QuestStatus.Completed)
            {
                // Reward quest에 대한 처리함
                DialogueManager.Instance.StartDialogue(completedDialogue);
                questObject.status = QuestStatus.Rewarded;
                questEffectGO.SetActive(false);
                questRewardGO.SetActive(false);
            }


            DialogueManager.Instance.OnEndDialogue += OnEndDialogue;
        }

        public void StopInteract(GameObject other)
        {
            isStartQuestDialogue = false;

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

        private void OnCompletedQuest(QuestObject questObject)
        {
            if (questObject.data.id == this.questObject.data.id && questObject.status == QuestStatus.Completed)
            {
                questEffectGO.SetActive(false);
                questRewardGO.SetActive(true);
            }
        }
        #endregion Methods
    }
}


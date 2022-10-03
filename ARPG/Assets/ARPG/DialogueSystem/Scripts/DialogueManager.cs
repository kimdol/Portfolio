using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ARPG.DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        #region Variables
        private static DialogueManager instance;

        public TMP_Text nameText;
        public TMP_Text dialogueText;

        public Animator animator = null;

        public Transform blockInputPannel;

        private Queue<string> sentences;

        public event Action OnStartDialogue;
        public event Action OnEndDialogue;
        #endregion Variables

        #region Properties
        public static DialogueManager Instance => instance;
        #endregion Properties

        #region Unity Methods
        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            sentences = new Queue<string>();
        }

        #endregion Unity Methods

        #region Methods

        public void StartDialogue(Dialogue dialogue)
        {
            OnStartDialogue?.Invoke();
            blockInputPannel.gameObject.SetActive(true);

            animator?.SetBool("IsOpen", true);

            nameText.text = dialogue.name;

            sentences.Clear();

            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

            DisplayNextSentence();
        }

        public void DisplayNextSentence()
        {
            if (sentences.Count == 0)
            {
                EndDialogue();
                return;
            }

            string sentence = sentences.Dequeue();
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence));
        }

        IEnumerator TypeSentence(string sentence)
        {
            dialogueText.text = string.Empty;

            yield return new WaitForSeconds(0.25f);

            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return null;
            }
        }

        void EndDialogue()
        {
            animator?.SetBool("IsOpen", false);

            blockInputPannel.gameObject.SetActive(false);
            OnEndDialogue?.Invoke();
        }

        #endregion Methods
    }
}

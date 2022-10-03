using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.QuestSystem
{
    [CreateAssetMenu(fileName = "New Quest Database", menuName = "Quest System/Quests/Database")]
    public class QuestDatabaseObject : ScriptableObject
    {
        #region Variables
        public QuestObject[] questObjects;

        private event Action<QuestObject> OnCompletedQuest;

        #endregion Variables

        #region Unity Methods
        public void OnValidate()
        {
            for (int index = 0; index < questObjects.Length; ++index)
            {
                questObjects[index].data.id = index;
            }
        }
        #endregion Unity Methods
    }
}


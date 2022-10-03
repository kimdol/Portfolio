using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.QuestSystem
{
    public class QuestManager : MonoBehaviour
    {
        #region Variables
        private static QuestManager instance;

        public QuestDatabaseObject questDatabase;

        public event Action<QuestObject> OnCompletedQuest;

        #endregion Variables

        #region Properties
        public static QuestManager Instance => instance;
        #endregion Properties

        #region Unity Methods
        private void Awake()
        {
            instance = this;
        }
        #endregion Unity Methods

        #region Methods
        public void ProcessQuest(QuestType type, int targetId)
        {
            foreach (QuestObject questObject in questDatabase.questObjects)
            {
                if (questObject.status == QuestStatus.Accepted && questObject.data.type == type && questObject.data.targetId == targetId)
                {
                    questObject.data.completedCount++;
                    if (questObject.data.completedCount >= questObject.data.count)
                    {
                        questObject.status = QuestStatus.Completed;
                        OnCompletedQuest?.Invoke(questObject);
                    }
                }
            }
        }
        #endregion Methods
    }
}

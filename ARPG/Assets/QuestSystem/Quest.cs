using System;

namespace ARPG.QuestSystem
{
    [Serializable]
    public class Quest
    {
        #region Variables
        public int id;

        public QuestType type;
        public int targetId;
        public int count;
        public int completedCount;

        public int rewardExp;
        public int rewardGold;
        public int rewardItemId;

        public string title;
        public string description;
        #endregion Variables
    }
}

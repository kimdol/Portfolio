using UnityEngine;

namespace ARPG.QuestSystem
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quests/New Quest")]
    public class QuestObject : ScriptableObject
    {
        public Quest data = new Quest();

        public QuestStatus status;
    }
}

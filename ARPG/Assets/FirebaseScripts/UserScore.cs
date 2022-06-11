using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

namespace ARPG.Firebase.Leaderboard
{
    [Serializable]
    public class UserScore
    {
        #region Variables
        public static string userIdPath = "user_id";
        public static string userNamePath = "username";
        public static string scorePath = "score";
        public static string timestampPath = "timestamp";
        public static string otherDataPath = "data";

        public string userId;
        public string userName;
        public long score;
        public long timestamp;
        public Dictionary<string, object> otherData;
        #endregion Variables

        #region Properties
        public string ShortDateString
        {
            get
            {
                var scoreData = new DateTimeOffset(new DateTime(timestamp * TimeSpan.TicksPerSecond, DateTimeKind.Utc)).LocalDateTime;
                return scoreData.ToShortDateString() + " " + scoreData.ToShortTimeString();
            }
        }
        #endregion Properties

        #region Methods
        public UserScore(string userId, string userName, long score, long timestamp, Dictionary<string, object> otherData = null)
        {
            this.userId = userId;
            this.userName = userName;
            this.score = score;
            this.timestamp = timestamp;
            this.otherData = otherData;
        }

        public UserScore(DataSnapshot record)
        {
            userId = record.Child(userIdPath).Value.ToString();
            if (record.Child(userNamePath).Exists)
            {
                userName = record.Child(userNamePath).Value.ToString();
            }

            long score;
            if (Int64.TryParse(record.Child(scorePath).Value.ToString(), out score))
            {
                this.score = score;
            }
            else
            {
                this.score = Int64.MinValue;
            }

            long timestamp;
            if (Int64.TryParse(record.Child(timestampPath).Value.ToString(), out timestamp))
            {
                this.timestamp = timestamp;
            }

            if (record.Child(otherDataPath).Exists && record.Child(otherDataPath).HasChildren)
            {
                this.otherData = new Dictionary<string, object>();
                foreach (var keyValue in record.Child(otherDataPath).Children)
                {
                    otherData[keyValue.Key] = keyValue.Value;
                }
            }
        }

        public static UserScore CreateScoreFromRecord(DataSnapshot record)
        {
            if (record == null)
            {
                Debug.LogWarning("UserScore의 DataSnapshot record가 Null입니다.");
                return null;
            }

            if (record.Child(userIdPath).Exists && record.Child(scorePath).Exists && record.Child(timestampPath).Exists)
            {
                return new UserScore(record);
            }

            Debug.LogWarning("UserScore의 record format이 잘못되었습니다.");
            return null;
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { userIdPath, userId },
                { userNamePath, userName },
                { scorePath, score },
                { timestampPath, timestamp },
                { otherDataPath, otherData }
            };
        }

        public override string ToString()
        {
            return String.Format("UserID: {0}, Score: {1}, Timestamp: {2}", userId, score, ShortDateString);
        }

        public override int GetHashCode()
        {
            // 소수들을 곱하여 단일성을 확보하도록 함
            return userId.GetHashCode() * 17 + score.GetHashCode() * 31 + timestamp.GetHashCode() * 47;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            var other = obj as UserScore;
            if (other == null)
            {
                return false;
            }

            return userId == other.userId && score == other.score && timestamp == other.timestamp;
        }

        #endregion Methods
    }
}

using ARPG.Firebase.Leaderboard;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ARPG.Firebase.Leaderboard
{
    public class LeaderboardUIController : MonoBehaviour
    {
        public LeaderboardHandler leaderboardHandler;
        public TMP_Text outputText;

        public StatsObject playerStats;
        public TMP_Text ScoreBadgeText;

        private enum TopScoreElement
        {
            Username = 1,
            Timestamp = 2,
            Score = 3
        }

        public int MaxRetrievableScores = 100;
        public RectTransform scoreContentContainer;
        public GameObject scorePrefab;
        /// <summary>
        /// ScoreContainer scroll view에서 작성된 상위점수 prefabs 목록입니다.
        /// </summary>
        private List<GameObject> scoreObjects = new List<GameObject>();

        void Start()
        {
            StartCoroutine(CreateTopscorePrefabs());
        }

        private void OnEnable()
        {
            ScoreBadgeText.text = playerStats.Exp.ToString();

            leaderboardHandler.OnAddedScore += OnAddedUserScore;
            leaderboardHandler.OnUpdatedScore += OnUpdatedUserScore;
            leaderboardHandler.OnRetrievedScore += OnRetrievedUserScore;
            leaderboardHandler.OnUpdatedLeaderboard += OnUpdatedLeaderboard;
            leaderboardHandler.OnInitialized += OnInitialized;
        }

        private void OnDisable()
        {
            leaderboardHandler.OnAddedScore -= OnAddedUserScore;
            leaderboardHandler.OnUpdatedScore -= OnUpdatedUserScore;
            leaderboardHandler.OnRetrievedScore -= OnRetrievedUserScore;
            leaderboardHandler.OnUpdatedLeaderboard -= OnUpdatedLeaderboard;
            leaderboardHandler.OnInitialized -= OnInitialized;
        }

        #region Leaderboard events
        private void OnAddedUserScore(object sender, UserScoreArgs args)
        {
            outputText.text = args.message;
        }

        private void OnUpdatedUserScore(object sender, UserScoreArgs args)
        {
            outputText.text = args.message;
        }

        private void OnRetrievedUserScore(object sender, UserScoreArgs args)
        {
            outputText.text = args.message;
        }

        private void OnUpdatedLeaderboard(object sender, LeaderboardArgs args)
        {
            var scores = args.scores;
            for (var i = 0; i < Math.Min(scores.Count, scoreObjects.Count); i++)
            {
                var score = scores[i];

                var scoreObject = scoreObjects[i];
                scoreObject.SetActive(true);

                var textElements = scoreObject.GetComponentsInChildren<TMP_Text>();
                textElements[(int)TopScoreElement.Username].text = String.IsNullOrEmpty(score.userName) ? score.userId : score.userName;
                textElements[(int)TopScoreElement.Timestamp].text = score.ShortDateString;
                textElements[(int)TopScoreElement.Score].text = score.score.ToString();
            }

            for (var i = scores.Count; i < scoreObjects.Count; i++)
            {
                scoreObjects[i].SetActive(false);
            }
        }

        private void OnInitialized(object sender, EventArgs args)
        {
            Debug.LogFormat("OnInitialized");
        }
        #endregion Leaderboard events

        #region Methods
        private IEnumerator CreateTopscorePrefabs()
        {
            var textElements = scorePrefab.GetComponentsInChildren<TMP_Text>();
            var topScoreElementValues = Enum.GetValues(typeof(TopScoreElement));
            var lastTopScoreElementValue = (int)topScoreElementValues.GetValue(topScoreElementValues.Length - 1);
            if (textElements.Length < lastTopScoreElementValue)
            {
                throw new InvalidOperationException(String.Format(
                    "TopScorePrefab에는 {0}개 이상의 text components가 있어야 합니다. {1}개 발견",
                    lastTopScoreElementValue,
                    textElements.Length));
            }

            for (int i = 0; i < MaxRetrievableScores; i++)
            {
                GameObject scoreObject = Instantiate(scorePrefab, scoreContentContainer.transform);
                scoreObject.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();
                scoreObject.name = "Leaderboard 점수 기록 " + i;
                scoreObject.SetActive(false);

                scoreObjects.Add(scoreObject);

                yield return null;
            }
        }
        #endregion Methods
    }
}


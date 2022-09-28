using Firebase;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace ARPG.Firebase.Leaderboard
{
    /// <summary>
    /// ����ڰ� ������ ���� �� 
    /// �����ڿ��� �˸��� ���� LeaderboardHandler�� ���Ǵ� EventArgs class
    /// </summary>
    public class UserScoreArgs : EventArgs
    {
        public UserScore score;
        public string message;

        public UserScoreArgs(UserScore score, string message)
        {
            this.score = score;
            this.message = message;
        }
    }

    public class LeaderboardArgs : EventArgs
    {
        public DateTime startDate;
        public DateTime endDate;
        public List<UserScore> scores;
    }

    public class LeaderboardHandler : MonoBehaviour
    {
        #region Fields
        private bool readyToInitialize = false;
        private bool initialized = false;

        private bool lowestFirst = false;

        private DatabaseReference databaseRef;

        public bool sendInitializedEvent = false;
        public event EventHandler OnInitialized;

        private bool sendAddedScoreEvent = false;
        private UserScoreArgs addedScoreArgs;
        public event EventHandler<UserScoreArgs> OnAddedScore;

        private bool sendUpdatedScoreEvent = false;
        private UserScoreArgs updatedScoreArgs;
        public event EventHandler<UserScoreArgs> OnUpdatedScore;

        private bool getUserScoreCallQueued = false;
        private bool gettingUserScore = false;

        private bool sendRetrievedScoreEvent = false;
        private UserScoreArgs retrievedScoreArgs;
        public event EventHandler<UserScoreArgs> OnRetrievedScore;

        /// <summary>
        /// (Readonly) ���������� ��û�� ������� �ְ� ���� list�� �����մϴ�.
        /// </summary>
        private List<UserScore> topScores = new List<UserScore>();
        public List<UserScore> TopScores => topScores;

        private Dictionary<string, UserScore> userScores = new Dictionary<string, UserScore>();

        private Query currentNewScoreQuery;

        private bool gettingTopScores = false;
        private int scoresToRetrieve = 20;

        private bool sendUpdatedLeaderbardEvent = false;
        public event EventHandler<LeaderboardArgs> OnUpdatedLeaderboard;


        public TMP_InputField userIdInputField;
        public TMP_InputField userNameInputField;
        public TMP_InputField scoreInputField;
        public StatsObject playerStats;
        private int addedScore = 10;


        public UDateTime startDateTime;
        private long StartTimeTicks => startDateTime.dateTime.Ticks / TimeSpan.TicksPerSecond;

        public UDateTime endDateTime;
        private long EndTimeTicks
        {
            get
            {
                long endTimeTicks = endDateTime.dateTime.Ticks / TimeSpan.TicksPerSecond;
                if (endTimeTicks <= 0)
                {
                    endTimeTicks = DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;
                }

                return endTimeTicks;
            }
        }

        /// <summary>
        /// ���� ���� �񵿱� ȣ���� ���� ���, True�� ��ȯ�մϴ�.
        /// </summary>
        public bool TasksProcessing
        {
            get
            {
                return readyToInitialize || gettingTopScores || gettingUserScore || addingUserScore;
            }
        }

        #endregion Fields

        #region Firebase Database Path
        /// <summary>
        /// Firebase �����ͺ��̽��� ��� ������ �����ϴ� ����Դϴ�.
        /// </summary>
        private string internalAllScoreDataPath = "all_scores";

        public string AllScoreDataPath
        {
            get => internalAllScoreDataPath;
        }

        #endregion Firebase Database Path

        #region Get User Score Variables
        private
        #endregion Get User Score Variables

        #region Unity Methods

        void Start()
        {
            FirebaseInitializer.Initialize(dependencyStatus =>
            {
                if (dependencyStatus == DependencyStatus.Available)
                {
                    readyToInitialize = true;
                    InitializeDatabase();
                }
                else
                {
                    Debug.LogError("��� Firebase ���Ӽ��� Ȯ���� �� �����ϴ�: " + dependencyStatus);
                }
            });
        }

        private void Update()
        {
            if (sendAddedScoreEvent)
            {
                sendAddedScoreEvent = false;
                OnAddedScore(this, addedScoreArgs);
            }

            if (sendUpdatedScoreEvent)
            {
                sendUpdatedScoreEvent = false;
                OnUpdatedScore(this, updatedScoreArgs);
            }

            if (sendRetrievedScoreEvent)
            {
                sendRetrievedScoreEvent = false;
                OnRetrievedScore(this, retrievedScoreArgs);
            }

            if (sendUpdatedLeaderbardEvent)
            {
                sendUpdatedLeaderbardEvent = false;

                OnUpdatedLeaderboard(this, new LeaderboardArgs
                {
                    scores = topScores,
                    startDate = startDateTime,
                    endDate = endDateTime
                });
            }
        }


        private void OnDisable()
        {
            if (currentNewScoreQuery != null)
            {
                currentNewScoreQuery.ChildAdded -= OnScoreAdded;
                currentNewScoreQuery.ChildRemoved -= OnScoreRemoved;
            }
        }

        #endregion Unity Methods

        #region Methods
        private void InitializeDatabase()
        {
            if (initialized)
            {
                return;
            }

            FirebaseApp app = FirebaseApp.DefaultInstance;

            databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

            initialized = true;
            readyToInitialize = false;
            OnInitialized(this, null);
        }

        public Task AddScore(string userId, string userName, int score, long timestamp = -1L, Dictionary<string, object> otherData = null)
        {
            if (timestamp <= 0)
            {
                // ���� �ð�(������ ���̽� ����)
                timestamp = DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;
            }

            var userScore = new UserScore(userId, userName, score, timestamp, otherData);
            return AddScore(userScore);
        }

        private bool addingUserScore = false;

        public Task<UserScore> AddScore(UserScore userScore)
        {
            if (addingUserScore)
            {
                Debug.LogError("����� ������ �߰��ϴ� �۾��� �����ϰ� �ֽ��ϴ�!");
                return null;
            }

            var scoreDictinary = userScore.ToDictionary();
            addingUserScore = true;

            return Task.Run(() =>
            {
                //// �� ������ �߰��ϱ� ���� FirebaseLeaderboard�� �ʱ�ȭ�Ǿ����� Ȯ���մϴ�.
                while (!initialized)
                {
                }

                var newEntry = databaseRef.Child(AllScoreDataPath).Push();

                return newEntry.SetValueAsync(scoreDictinary).ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Debug.LogWarning("Exception adding score: " + task.Exception);
                        return null;
                    }

                    if (!task.IsCompleted)
                    {
                        return null;
                    }

                    addingUserScore = false;

                    addedScoreArgs = new UserScoreArgs(userScore, userScore.ToString() + " Added!");
                    sendAddedScoreEvent = true;

                    return userScore;
                }).Result;
            });
        }

        public Task UpdateScore(string userId, string userName, int score, long timestamp = -1L, Dictionary<string, object> otherData = null)
        {
            if (timestamp <= 0)
            {
                timestamp = DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;
            }

            var userScore = new UserScore(userId, userName, score, timestamp, otherData);
            return UpdateScore(userScore);
        }

        private bool updatingUserScore = false;

        public Task<UserScore> UpdateScore(UserScore userScore)
        {
            if (updatingUserScore)
            {
                Debug.LogError("����� ������ �߰��ϴ� �۾��� �����ϰ� �ֽ��ϴ�!");
                return null;
            }

            var scoreDictinary = userScore.ToDictionary();
            updatingUserScore = true;

            return Task.Run(() =>
            {
                //// �� ������ �߰��ϱ� ���� FirebaseLeaderboard�� �ʱ�ȭ�Ǿ����� Ȯ���մϴ�.
                while (!initialized)
                {
                }

                string testKey = "/-M9JbQnc5c0xu_08pa_Y";
                var newEntry = databaseRef.Child(AllScoreDataPath + testKey);

                return newEntry.UpdateChildrenAsync(scoreDictinary).ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Debug.LogWarning("Exception adding score: " + task.Exception);
                        return null;
                    }

                    if (!task.IsCompleted)
                    {
                        return null;
                    }

                    updatingUserScore = false;

                    updatedScoreArgs = new UserScoreArgs(userScore, userScore.ToString() + " Updated!");
                    sendUpdatedScoreEvent = true;

                    return userScore;
                }).Result;
            });
        }

        public void GetUserScore(string userId)
        {
            if (!initialized && !getUserScoreCallQueued)
            {
                Debug.LogWarning("Firebase�� �ʱ�ȭ�Ǳ� ���� GetUserScore�� ȣ��Ǿ����ϴ�. �ʱ�ȭ ��� ��...");
                getUserScoreCallQueued = true;
                StartCoroutine(GetUserScoreWhenInitialized(userId));
                return;
            }

            if (getUserScoreCallQueued)
            {
                Debug.LogWarning("���� �ʱ�ȭ�� ��ٸ��� ��...");
                return;
            }

            gettingUserScore = true;

            // Time frame������ ����� ������ ���� ���� �������� �����Ͽ� ���� ���� ������ ã���ϴ�.
            databaseRef.Child(AllScoreDataPath)
                .OrderByChild(UserScore.userIdPath)
                .StartAt(userId)
                .EndAt(userId)
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        throw task.Exception;
                    }

                    if (!task.IsCompleted)
                    {
                        return;
                    }

                    if (task.Result.ChildrenCount == 0)
                    {
                        retrievedScoreArgs = new UserScoreArgs(null, String.Format("����� {0}�� ���� ������ ����", userId));
                    }
                    else
                    {
                        // Time range������ ������� ������ ã���ϴ�.
                        var scores = ParseValidUserScoreRecords(task.Result, StartTimeTicks, EndTimeTicks).ToList();

                        if (scores.Count == 0)
                        {
                            retrievedScoreArgs = new UserScoreArgs(null, String.Format("�ð� ����({0} - {1})���� ����� {2}�� ���� ������ �����ϴ�.", StartTimeTicks, EndTimeTicks, userId));
                        }
                        else
                        {
                            var orderedScores = scores.OrderBy(score => score.score);
                            var userScore = lowestFirst ? orderedScores.First() : orderedScores.Last();

                            retrievedScoreArgs = new UserScoreArgs(userScore, userScore.ToString() + " Received!");
                        }
                    }

                    gettingUserScore = false;
                    sendRetrievedScoreEvent = true;
                });
        }

        private List<UserScore> ParseValidUserScoreRecords(DataSnapshot snapshot, long startTS, long endTS)
        {
            return snapshot.Children
                .Select(scoreRecord => UserScore.CreateScoreFromRecord(scoreRecord))
                .Where(score => score != null && score.timestamp > startTS && score.timestamp <= endTS)
                .Reverse()
                .ToList();
        }

        private IEnumerator GetUserScoreWhenInitialized(string userId)
        {
            while (!initialized)
            {
                yield return null;
            }

            getUserScoreCallQueued = false;
            GetUserScore(userId);
        }

        private void GetInitialTopScores(long batchEnd)
        {
            gettingTopScores = true;

            var query = databaseRef.Child(AllScoreDataPath).OrderByChild("score");
            query = lowestFirst ? 
                query.StartAt(batchEnd).LimitToFirst(scoresToRetrieve) : 
                query.EndAt(batchEnd).LimitToLast(scoresToRetrieve);
            
            query.GetValueAsync().ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    SetTopScores();
                    return;
                }
                else if (!task.IsCompleted)
                {
                    return;
                }

                if (!task.Result.HasChildren)
                {
                    // �˻��� ������ ���� ���� �ʽ��ϴ�.
                    SetTopScores();
                    return;
                }

                var scores = ParseValidUserScoreRecords(task.Result, StartTimeTicks, EndTimeTicks);
                foreach (var userScore in scores)
                {
                    if (!userScores.ContainsKey(userScore.userId))
                    {
                        userScores[userScore.userId] = userScore;
                    }
                    else
                    {
                        var bestScore = GetBestScore(userScores[userScore.userId], userScore);
                        userScores[userScore.userId] = bestScore;
                    }

                    if (userScores.Count == scoresToRetrieve)
                    {
                        SetTopScores();
                        return;
                    }
                }

                // scoresToRetrieve���� ������ ����� ������ ã�ų� �˻��� ������ �������� ������
                // ���ݱ��� �߰ߵ� ���� ���� ������ ���� ��ġ(���� ��)�� �����Ͽ�
                // ��ϵ� ���� �������� �ٽ� �����ɴϴ�.
                long nextEndAt = lowestFirst ? scores.First().score + 1L : scores.Last().score - 1L;

                try
                {
                    GetInitialTopScores(nextEndAt);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
                finally
                {
                    SetTopScores();
                }
            });
        }

        /// <summary>
        /// ������ ���� �� ���� ���� ������ �����ɴϴ�.
        /// </summary>
        /// <param name="scores"></param>
        /// <returns>������ ���� ���� ������, ���� ���� �������� ���� �ְ� ����</returns>
        private UserScore GetBestScore(params UserScore[] scores)
        {
            if (scores.Length == 0)
            {
                return null;
            }

            UserScore bestScore = null;
            foreach (var score in scores)
            {
                if (bestScore == null)
                {
                    bestScore = score;
                }
                else if (lowestFirst && score.score < bestScore.score)
                {
                    bestScore = score;
                }
                else if (!lowestFirst && score.score > bestScore.score)
                {
                    bestScore = score;
                }
            }

            return bestScore;
        }

        private void SetTopScores()
        {
            topScores.Clear();

            if (currentNewScoreQuery != null)
            {
                currentNewScoreQuery.ChildAdded -= OnScoreAdded;
                currentNewScoreQuery.ChildRemoved -= OnScoreRemoved;
            }

            topScores.AddRange(lowestFirst
                ? userScores.Values.OrderBy(score => score.score)
                : userScores.Values.OrderByDescending(score => score.score));

            currentNewScoreQuery = databaseRef.Child(AllScoreDataPath).OrderByChild("score");
            if (topScores.Count > 0)
            {
                currentNewScoreQuery = lowestFirst ? currentNewScoreQuery.EndAt(topScores.Last().score) : currentNewScoreQuery.StartAt(topScores.Last().score);
            }

            // ���� ��¥�� �����̸� ���� ���� �߰� �̺�Ʈ�� subscribe.
            if (endDateTime.dateTime.Ticks <= 0)
            {
                currentNewScoreQuery.ChildAdded += OnScoreAdded;
            }

            currentNewScoreQuery.ChildRemoved += OnScoreRemoved;

            sendUpdatedLeaderbardEvent = true;
            gettingTopScores = false;
        }

        #endregion Methods

        #region Query Events
        /// <summary>
        /// ���� ����� leaderboard�� �� �ڸ� ����ŭ ���� ������ �߰��Ǹ� �ٽ� �ҷ��ɴϴ�.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnScoreAdded(object sender, ChildChangedEventArgs args)
        {
            if (args.Snapshot == null || !args.Snapshot.Exists)
            {
                return;
            }

            var score = UserScore.CreateScoreFromRecord(args.Snapshot);
            if (score == null)
            {
                return;
            }

            //Debug.Log("OnScoreAdded args :  " + score.userId + ", " + score.userName + ", " + score.score);

            // ������ ����/���� �ð� ���� �ְ�, ���� ���� ������ ���Ե��� �ʾҴ����� Ȯ���մϴ�.
            if (topScores.Contains(score))
            {
                return;
            }

            if (StartTimeTicks > 0 || EndTimeTicks > 0)
            {
                var endTimeTicks = endDateTime.dateTime.Ticks > 0
                    ? EndTimeTicks
                    : (DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond);
                var startTime = startDateTime.dateTime.Ticks > 0
                    ? endTimeTicks - StartTimeTicks
                    : 0;
                if (score.timestamp > endTimeTicks || score.timestamp < startTime)
                {
                    return;
                }
            }

            // ������ ����ڰ� �̹� �� ���� ������ ���� ����� ��, �߰����� ���ʽÿ�.
            // ������� ������ �� ���� ��� �����մϴ�.
            var existingScore = topScores.Find(inScore => inScore.userId == score.userId);
            if (existingScore != null)
            {
                var bestStore = GetBestScore(existingScore, score);
                if (existingScore == bestStore)
                {
                    return;
                }

                topScores.Remove(existingScore);
            }

            if (topScores.Any(inScore => inScore.userId == score.userId))
            {
                return;
            }

            topScores.Add(score);
            topScores = lowestFirst
                ? topScores.OrderBy(inScore => inScore.score).Take(scoresToRetrieve).ToList()
                : topScores.OrderByDescending(inScore => inScore.score).Take(scoresToRetrieve).ToList();

            sendUpdatedLeaderbardEvent = true;
        }

        /// <summary>
        /// Score record�� �����ͺ��̽����� ���ŵǸ� �ٽ� ȣ���մϴ�.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnScoreRemoved(object sender, ChildChangedEventArgs args)
        {
            if (args.Snapshot == null || !args.Snapshot.Exists)
            {
                return;
            }

            var score = UserScore.CreateScoreFromRecord(args.Snapshot);
            if (score == null)
            {
                return;
            }

            if (topScores.Contains(score))
            {
                topScores.Remove(score);
                RefreshScore();
            }
        }
        #endregion Query Events

        #region UI Methods
        public void AddScore()
        {
            var emailAddress = FirebaseAuthManager.Instance.EmailAddress;
            if (String.IsNullOrEmpty(emailAddress))
            {
                Debug.LogError("�α����� �Ͻʽÿ�");
                return;
            }

            const string spliter = "@";
            string[] emailDataList = emailAddress.Split(spliter.ToCharArray());

            AddScore(emailDataList[0], emailDataList[0], int.Parse(scoreInputField.text));
        }

        public void UpdateUserScore()
        {
            var emailAddress = FirebaseAuthManager.Instance.EmailAddress;
            if (String.IsNullOrEmpty(emailAddress))
            {
                Debug.LogError("�α����� �Ͻʽÿ�");
                return;
            }

            const string spliter = "@";
            string[] emailDataList = emailAddress.Split(spliter.ToCharArray());

            UpdateScore(emailDataList[0], emailDataList[0], int.Parse(scoreInputField.text));
        }

        public void GetUerScore()
        {
            var emailAddress = FirebaseAuthManager.Instance.EmailAddress;
            if (String.IsNullOrEmpty(emailAddress))
            {
                Debug.LogError("�α����� �Ͻʽÿ�");
                return;
            }

            const string spliter = "@";
            string[] emailDataList = emailAddress.Split(spliter.ToCharArray());

            GetUserScore(emailDataList[0]);
        }


        public void RefreshScore()
        {
            if (initialized)
            {
                GetInitialTopScores(lowestFirst ? Int64.MinValue : Int64.MaxValue);
            }
        }
        #endregion UI Methods
    }
}


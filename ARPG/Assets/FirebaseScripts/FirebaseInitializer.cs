using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.Firebase
{
    public static class FirebaseInitializer
    {
        #region Variables

        private static List<Action<DependencyStatus>> initializedCallbacks = new List<Action<DependencyStatus>>();
        private static List<Action> activateFetchCallbacks = new List<Action>();
        private static DependencyStatus dependencyStatus;
        private static bool initialized = false;
        private static bool fetching = false;
        private static bool activateFetched = false;

        public static Action<DependencyStatus> testCallback;

        #endregion Variables

        #region Methods
        public static void Initialize(Action<DependencyStatus> callback)
        {
            lock (initializedCallbacks)
            {
                if (initialized)
                {
                    callback(dependencyStatus);
                    return;
                }


                initializedCallbacks.Add(callback);
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    lock (initializedCallbacks)
                    {
                        dependencyStatus = task.Result;
                        initialized = true;
                        CallInitializedCallbacks();
                    }
                });
            }
        }

        private static void CallInitializedCallbacks()
        {
            lock (initializedCallbacks)
            {
                foreach (var callback in initializedCallbacks)
                {
                    callback(dependencyStatus);
                }

                initializedCallbacks.Clear();
            }
        }

        private static void CallActivateFetchedCallbacks()
        {
            lock (activateFetchCallbacks)
            {
                foreach (var callback in activateFetchCallbacks)
                {
                    callback();
                }

                activateFetchCallbacks.Clear();
            }
        }
        #endregion Methods
    }
}

using Firebase;
using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuthManager
{
    #region Fields
    private static FirebaseAuthManager instance = null;

    private FirebaseAuth auth;
    private FirebaseUser user;

    private string displayName;
    private string emailAddress;
    private Uri photoUrl;

    public Action<bool> OnChangedLoginState;
    #endregion Fields

    #region Properties
    public static FirebaseAuthManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FirebaseAuthManager();
            }

            return instance;
        }
    }

    public String UserId => user?.UserId ?? string.Empty;
    public String DisplayName => displayName;
    public String EmailAddress => emailAddress;
    public Uri PhotoURL => photoUrl;
    #endregion Properties

    public void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += OnAuthStateChanged;

        OnAuthStateChanged(this, null);
    }

    public void CreateUser(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync가 취소되었습니다.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync에 오류가 발생했습니다: " + task.Exception);
                return;
            }

            // Firebase user가 생성되었습니다.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user가 성공적으로 생성되었습니다: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }

    public void SignIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync가 취소되었습니다.");
                return;
            }

            if (task.IsFaulted)
            {
                int errorCode = GetFirebaseErrorCode(task.Exception);
                switch (errorCode)
                {
                    case (int)Firebase.Auth.AuthError.EmailAlreadyInUse:
                        Debug.LogError("Email이 이미 사용 중입니다.");
                        break;

                    case (int)AuthError.WrongPassword:
                        Debug.LogError("Password가 틀렸습니다.");
                        break;
                }

                Debug.LogError("SignInWithEmailAndPasswordAsync에 오류가 발생했습니다: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("SignIn 성공: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }

    public void SignOut()
    {
        auth.SignOut();
    }

    private int GetFirebaseErrorCode(AggregateException exception)
    {
        FirebaseException firebaseException = null;
        foreach (Exception e in exception.Flatten().InnerExceptions)
        {
            firebaseException = e as FirebaseException;
            if (firebaseException != null)
            {
                break;
            }
        }

        return firebaseException?.ErrorCode ?? 0;
    }

    /// <summary>
    /// 로그인 및 로그아웃 이벤트에 응답하려면 전역 인증 객체에 이벤트 핸들러를 연결합니다. 
    /// 사용자의 로그인 상태가 변경될 때마다 이 핸들러가 호출됩니다.
    /// 인증 객체가 완전히 초기화되고 네트워크 호출이 완료된 후에만 핸들러가 실행되므로 핸들러는 로그인한 사용자에 대한 정보를 가져오기에 가장 적합한 위치입니다.
    /// FirebaseAuth 객체의 StateChanged 필드를 사용해 이벤트 핸들러를 등록합니다.
    /// 사용자가 로그인되면 이벤트 핸들러에서 사용자에 대한 정보를 가져올 수 있습니다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private void OnAuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = (user != auth.CurrentUser && auth.CurrentUser != null);
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out: " + user.UserId);
                OnChangedLoginState?.Invoke(false);
            }

            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in: " + user.UserId);
                displayName = user.DisplayName ?? "";
                emailAddress = user.Email ?? "";
                photoUrl = user.PhotoUrl ?? null;

                OnChangedLoginState?.Invoke(true);
            }
        }
    }
}

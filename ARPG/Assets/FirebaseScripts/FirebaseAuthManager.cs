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
                Debug.LogError("CreateUserWithEmailAndPasswordAsync�� ��ҵǾ����ϴ�.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync�� ������ �߻��߽��ϴ�: " + task.Exception);
                return;
            }

            // Firebase user�� �����Ǿ����ϴ�.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user�� ���������� �����Ǿ����ϴ�: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }

    public void SignIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync�� ��ҵǾ����ϴ�.");
                return;
            }

            if (task.IsFaulted)
            {
                int errorCode = GetFirebaseErrorCode(task.Exception);
                switch (errorCode)
                {
                    case (int)Firebase.Auth.AuthError.EmailAlreadyInUse:
                        Debug.LogError("Email�� �̹� ��� ���Դϴ�.");
                        break;

                    case (int)AuthError.WrongPassword:
                        Debug.LogError("Password�� Ʋ�Ƚ��ϴ�.");
                        break;
                }

                Debug.LogError("SignInWithEmailAndPasswordAsync�� ������ �߻��߽��ϴ�: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("SignIn ����: {0} ({1})", newUser.DisplayName, newUser.UserId);
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
    /// �α��� �� �α׾ƿ� �̺�Ʈ�� �����Ϸ��� ���� ���� ��ü�� �̺�Ʈ �ڵ鷯�� �����մϴ�. 
    /// ������� �α��� ���°� ����� ������ �� �ڵ鷯�� ȣ��˴ϴ�.
    /// ���� ��ü�� ������ �ʱ�ȭ�ǰ� ��Ʈ��ũ ȣ���� �Ϸ�� �Ŀ��� �ڵ鷯�� ����ǹǷ� �ڵ鷯�� �α����� ����ڿ� ���� ������ �������⿡ ���� ������ ��ġ�Դϴ�.
    /// FirebaseAuth ��ü�� StateChanged �ʵ带 ����� �̺�Ʈ �ڵ鷯�� ����մϴ�.
    /// ����ڰ� �α��εǸ� �̺�Ʈ �ڵ鷯���� ����ڿ� ���� ������ ������ �� �ֽ��ϴ�.
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

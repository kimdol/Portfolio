using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AuthHandler : MonoBehaviour
{
    public TMP_InputField eMailText;
    public TMP_InputField passwordText;

    public TMP_Text outputText;

    private void Start()
    {
        FirebaseAuthManager.Instance.OnChangedLoginState += OnChangedLoginState;
        FirebaseAuthManager.Instance.InitializeFirebase();
    }

    public void CreateUser()
    {
        string email = eMailText.text;
        string password = passwordText.text;

        FirebaseAuthManager.Instance.CreateUser(email, password);
    }

    public void SignIn()
    {
        string email = eMailText.text;
        string password = passwordText.text;

        FirebaseAuthManager.Instance.SignIn(email, password);
    }

    public void SignOut()
    {
        FirebaseAuthManager.Instance.SignOut();
    }

    private void OnChangedLoginState(bool loggined)
    {
        if (loggined)
        {
            outputText.text = "Signed in: " + FirebaseAuthManager.Instance.UserId;
        }
        else
        {
            outputText.text = "Signed out: " + FirebaseAuthManager.Instance.UserId;
        }
    }
}

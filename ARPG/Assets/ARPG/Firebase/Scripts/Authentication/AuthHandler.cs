using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AuthHandler : MonoBehaviour
{
    public TMP_InputField eMailText;
    public TMP_InputField passwordText;

    public TMP_InputField creationEmailText;
    public TMP_InputField creationPasswordText;

    public TMP_Text outputText;

    private bool updatedError = false;

    private void Start()
    {
        FirebaseAuthManager.Instance.OnChangedLoginState += OnChangedLoginState;
        FirebaseAuthManager.Instance.OnUpdateError += OnUpdateError;
        FirebaseAuthManager.Instance.InitializeFirebase();
    }

    private void Update()
    {
        if (updatedError)
        {
            updatedError = false;
            OnErrorState();
        }
        
    }

    private void OnDisable()
    {
        FirebaseAuthManager.Instance.OnChangedLoginState -= OnChangedLoginState;
        FirebaseAuthManager.Instance.OnUpdateError -= OnUpdateError;
        FirebaseAuthManager.Instance.CancelSubscription();
    }

    public void CreateUser()
    {
        string email = creationEmailText.text;
        string password = creationPasswordText.text;

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
            outputText.text = "Signed out " + FirebaseAuthManager.Instance.UserId;
        }
    }

    private void OnErrorState()
    {
        outputText.text = FirebaseAuthManager.Instance.Error;
    }

    private void OnUpdateError()
    {
        updatedError = true;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.VersionControl;

public enum LSPanelType : int
{
    Popup_Signup = 0,
    Popup_Login = 1,
    MessageError = 2,
    Default,
}

public class LoginSignupPanelControl : MonoBehaviour
{
    [Header("Default staticPanels: Popup_Signup = 0, Popup_Login = 1, MessageError = 2")]
    public GameObject[] staticPanels = null;

    private void Start()
    {
        var userId = FirebaseAuthManager.Instance.UserId;

        if (userId == string.Empty)
        {
            staticPanels[(int)LSPanelType.Popup_Signup].SetActive(false);
            staticPanels[(int)LSPanelType.Popup_Login].SetActive(true);
            staticPanels[(int)LSPanelType.MessageError].SetActive(true);
        }
        else
        {
            staticPanels[(int)LSPanelType.Popup_Signup].SetActive(false);
            staticPanels[(int)LSPanelType.Popup_Login].SetActive(false);
            staticPanels[(int)LSPanelType.MessageError].SetActive(false);
        }

        FirebaseAuthManager.Instance.OnChangedLoginState += OnChangedLoginState;
    }

    private void OnDisable()
    {
        FirebaseAuthManager.Instance.OnChangedLoginState -= OnChangedLoginState;
    }

    public void OnClickLogin()
    {
        staticPanels[(int)LSPanelType.Popup_Signup].SetActive(false);
        staticPanels[(int)LSPanelType.Popup_Login].SetActive(true);
    }

    public void OnClickSignup()
    {
        staticPanels[(int)LSPanelType.Popup_Signup].SetActive(true);
        staticPanels[(int)LSPanelType.Popup_Login].SetActive(false);
    }

    private void OnChangedLoginState(bool loggined)
    {
        if (loggined)
        {
            staticPanels[(int)LSPanelType.Popup_Signup].SetActive(false);
            staticPanels[(int)LSPanelType.Popup_Login].SetActive(false);
            staticPanels[(int)LSPanelType.MessageError].SetActive(false);
        }
        else
        {
            staticPanels[(int)LSPanelType.Popup_Signup].SetActive(false);
            staticPanels[(int)LSPanelType.Popup_Login].SetActive(true);
            staticPanels[(int)LSPanelType.MessageError].SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    Button _loginButton;
    Button _seondButton;

    void Start()
    {
        _loginButton = transform.Find("Connect").GetComponent<Button>();
        _loginButton.onClick.AddListener(Connect);

        _seondButton = transform.Find("Send").GetComponent<Button>();
        _seondButton.onClick.AddListener(Send);
    }

    void Connect()
    {
        NetworkClient.Instance.Connect();
    }

    void Send()
    {
        NetworkClient.Instance.SendServer("Hello there!");
    }
}

using NetworkShared.Packets.ClientServer;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] int _maxUsernameLength = 10;
    [SerializeField] int _maxPasswordLength = 10;

    Transform _loginButton;
    TextMeshProUGUI _loginText;
    TMP_InputField _usernameInput;
    TMP_InputField _passwordInput;
    Transform _usernameError;
    Transform _passwordError;
    Transform _loadingUI;

    bool _isConnected;

    string _username = string.Empty;
    string _password = string.Empty;

    void Start()
    {
        _loginButton = transform.Find("LoginBtn");
        _loginButton.GetComponent<Button>().onClick.AddListener(Login);
        _loginText = _loginButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        _usernameInput = transform.Find("UsernameInput").GetComponent<TMP_InputField>();
        _usernameInput.onValueChanged.AddListener(UpdateUsername);
        _usernameError = _usernameInput.transform.Find("Error");

        _passwordInput = transform.Find("PasswordInput").GetComponent<TMP_InputField>();
        _passwordInput.onValueChanged.AddListener(UpdatePassword);
        _passwordError = _passwordInput.transform.Find("Error");

        _loadingUI = transform.Find("Loading");

        NetworkClient.Instance.OnServerConnected += SetIsConnected;
    }

    void SetIsConnected()
    {
        _isConnected = true;
    }

    private void OnDestroy()
    {
        NetworkClient.Instance.OnServerConnected -= SetIsConnected;
    }

    void UpdateUsername(string value)
    {
        _username = value;
        ValidateAndUpdateUI();
    }

    void UpdatePassword(string value)
    {
        _password = value;
        ValidateAndUpdateUI();
    }

    void ValidateAndUpdateUI()
    {
        Debug.Log(_username + " " + _password);

        var usernameRegex = Regex.Match(_username, "^[a-zA-Z0-9]+$");
        
        var interactable = 
            (!string.IsNullOrWhiteSpace(_username) && 
            !string.IsNullOrWhiteSpace(_password)) && 
            (_username.Length <= _maxUsernameLength && _password.Length <= _maxPasswordLength) && 
            usernameRegex.Success;

        EnabledLoginButton(interactable);

        if(_username != null)
        {
            var usernameTooLong = _username.Length > _maxUsernameLength || !usernameRegex.Success;
            _usernameError.gameObject.SetActive(usernameTooLong);
        }

        if(_password != null)
        {
            var passwordTooLong = _password.Length > _maxPasswordLength;
            _passwordError.gameObject.SetActive(passwordTooLong);
        }
    }

    void EnabledLoginButton(bool interactable)
    {
        Debug.Log(interactable);
        _loginButton.GetComponent<Button>().interactable = interactable;
        var color = _loginButton.GetComponent<Button>().interactable ? Color.white : Color.gray;
        _loginText.color = color;
    }

    void Login()
    {
        StopCoroutine(LoginRoutine());
        StartCoroutine(LoginRoutine());
    }

    IEnumerator LoginRoutine()
    {
        EnabledLoginButton(false);
        _loadingUI.gameObject.SetActive(true);

        // 서버에 연결
        NetworkClient.Instance.Connect();

        while(!_isConnected)
        {
            Debug.Log("대기중");
            yield return null;
        }

        Debug.Log("서버에 연결되었음!");

        var authRequest = new Net_AuthRequest
        {
            Username = _username,
            Password = _password,
        };

        NetworkClient.Instance.SendServer(authRequest);
    }
}

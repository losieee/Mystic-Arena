using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public Button registerButton;
    public Button loginButton;
    public Button logoutButton;
    public Button getDataButton;

    public TMP_Text statusText;

    private AuthManager authManager;

    // Start is called before the first frame update
    void Start()
    {
        authManager = GetComponent<AuthManager>();
        registerButton.onClick.AddListener(OnRegisterClick);
        //loginButton.onClick.AddListener(OnLoginClick);
        //logoutButton.onClick.AddListener(OnLogoutClick);
        //getDataButton.onClick.AddListener(OnGetDatalick);
    }

    private void OnLoginClick()
    {
        StartCoroutine(LoginCorouitine());
    }

    private IEnumerator LoginCorouitine()
    {
        statusText.text = "로그인 중...";
        yield return StartCoroutine(authManager.Login(usernameInput.text, passwordInput.text));
        statusText.text = "로그인 성공";
    }

    private void OnRegisterClick()
    {
        StartCoroutine(RegisterCorouitine());
    }

    private IEnumerator RegisterCorouitine()
    {
        statusText.text = "회원가입 중...";
        yield return StartCoroutine(authManager.Register(usernameInput.text, passwordInput.text));
        statusText.text = "회원가입 성공. 로그인 해주세요";
    }

    private void OnLogoutClick()
    {
        StartCoroutine(LogoutCorouitine());
    }

    private IEnumerator LogoutCorouitine()
    {
        statusText.text = "로그아웃 중...";
        yield return StartCoroutine(authManager.Logout());
        statusText.text = "로그아웃 성공";
    }

    private void OnGetDatalick()
    {
        StartCoroutine(GetDataCorouitine());
    }

    private IEnumerator GetDataCorouitine()
    {
        statusText.text = "데이터 요청 중...";
        yield return StartCoroutine(authManager.GetProtectedData());
        statusText.text = "데이터 요청 완료";
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using System;
using System.Data;
using UnityEngine.UI;
public class AuthManager : MonoBehaviour
{
    // 서버 URL 및 PlayerPrefs 키 상수 정의
    private const string SERVER_URL = "http://localhost:4000";
    private const string ACCESS_TOKEN_PREFS_KEY = "AccessToken";
    private const string REFRESH_TOKEN_PREFS_KEY = "RefreshToken";
    private const string TOKEN_EXPIRY_PREFS_KEY = "ToeknExpiry";

    // 토큰 및 만료 시간 저장 변수
    private string accessToken;
    private string refreshToken;
    private DateTime tokenExpiryTime;

    public Text statusText;

    private AuthManager authManager;

    void Start()
    {
        LoadTokenFromPrets();
    }

    //private void OnRegisterClick()
    //{
    //    StartCoroutine(RegisteerCorouitine());
    //}

    //private IEnumerator RegisteerCorouitine()
    //{
    //    statusText.text = "회원가입 중...";
    //    //yield return StartCoroutine(authManager.Re)
    //}

    // PlayerPrefs에서 토큰 정보 로드
    private void LoadTokenFromPrets()
    {
        accessToken = PlayerPrefs.GetString(ACCESS_TOKEN_PREFS_KEY, "");
        refreshToken = PlayerPrefs.GetString(REFRESH_TOKEN_PREFS_KEY, "");
        long expiryTicks = Convert.ToInt64(PlayerPrefs.GetString(TOKEN_EXPIRY_PREFS_KEY, "0"));
        tokenExpiryTime = new DateTime(expiryTicks);
    }

    // PlayerPres에 토근 정보 저장
    private void SaveTokenToPrefs(string accessToken, string refreshToken, DateTime expiryTime)
    {
        PlayerPrefs.SetString(ACCESS_TOKEN_PREFS_KEY, accessToken);
        PlayerPrefs.SetString(REFRESH_TOKEN_PREFS_KEY, refreshToken);
        PlayerPrefs.SetString(TOKEN_EXPIRY_PREFS_KEY, expiryTime.Ticks.ToString());
        PlayerPrefs.Save();

        this.accessToken = accessToken;
        this.refreshToken = refreshToken;
        this.tokenExpiryTime = expiryTime;
    }

    // 로그인 응답 데이터 구조
    [System.Serializable]
    public class LoginResponse
    {
        public string accessToken;
        public string refreshToken;
    }

    // 토큰 갱신 응답 데이터 구조
    [System.Serializable]
    public class RefreshTokenResponse
    {
        public string accessToekn;
    }
}

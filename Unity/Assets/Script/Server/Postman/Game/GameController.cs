using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameView gameView;
    private PlayerModel playerModel;
    private GameAPI gameAPI;

    // Start is called before the first frame update
    void Start()
    {
        gameAPI = gameObject.AddComponent<GameAPI>();
        gameView.SetRegisterButtonListener(OnRegisterButtonClicked);
        gameView.SetLoginButtonListeener(OnLoginButtonClicked);
        gameView.SetCollectbuttonListener(OnCollectButtonClicked);
    }

    public void OnRegisterButtonClicked()
    {
        string playerName = gameView.playerNameInput.text;
        StartCoroutine(gameAPI.RegisterPlayer(playerName, "1234"));             // 플레이어 등록 요청 보내기
    }

    public void OnLoginButtonClicked()
    {
        string playerName = gameView.playerNameText.text;
        StartCoroutine(LoginPlayerCoroutine(playerName, "1234"));             // 플레이어 로그인 요청 보내기
    }

    public void OnCollectButtonClicked()
    {
        if (playerModel == null)
        {
            Debug.LogWarning("플레이어가 로그인되지 않았습니다.");
            return;
        }

        Debug.Log($"Collecting resources for : {playerModel.playerName}");
        StartCoroutine(CollectCoroutine(playerModel.playerName));               // PlayerModel.name 사용
    }

    private IEnumerator CollectCoroutine(string playerName)
    {
        yield return gameAPI.CollectResources(playerName, player =>
        {
            if (player != null)
            {
                playerModel.player_count = player.player_count;
                UpdateResourcesDisplay();
            }
            else
            {
                Debug.LogError("리소스 수집 실패: 플레이어 데이터가 null입니다.");
            }
        });
    }

    private IEnumerator LoginPlayerCoroutine(string playerName, string password)
    {
        yield return gameAPI.LoginPlayer(playerName, password, player =>
        {
            if (player != null)
            {
                playerModel = player;       // 로그인 성공 시 PlayerModel 객체 생성
                UpdateResourcesDisplay();   // 로그인 성공 후 UI 업데이트
            }
            else
            {
                Debug.LogError("로그인 실패: 플레이어 데이터가 null입니다.");
            }
        });
    }

    private void UpdateResourcesDisplay()
    {
        if(playerModel != null) // playermodel이 null이 아닐 때만 UI 업데이트
        {
            gameView.UpdateResource(playerModel.player_count);
        }
    }
}



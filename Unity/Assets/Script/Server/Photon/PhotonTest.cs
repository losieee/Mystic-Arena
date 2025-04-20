using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;

public class PhotonTest : MonoBehaviourPunCallbacks
{
    public TMP_InputField m_InputField;                             // 닉네임 입력창(추후 삭제)
    public TextMeshProUGUI[] nameTexts = new TextMeshProUGUI[4];    // 플레이어 배열

    void Start()
    {
        Screen.SetResolution(960, 600, false);                      // 해상도
        StartCoroutine(InitializeUI());
    }

    IEnumerator InitializeUI()
    {
        yield return new WaitForEndOfFrame();                       // 렌더링이 끝날때까지 기다린후 UI 초기화

        GameObject inputObj = GameObject.Find("Canvas/InputField");
        if (inputObj != null)                                       // inputObj 가 존재한다면
        {
            m_InputField = inputObj.GetComponent<TMP_InputField>();
        }
        else
        {
            Debug.LogError("[UI] InputField 오브젝트를 찾을 수 없습니다.");
        }

        for (int i = 0; i < 4; i++)                                 // Text1 ~ Text4 초기화
        {
            string objectName = $"Canvas/Text{i + 1}";
            GameObject go = GameObject.Find(objectName);
            if (go != null)
            {
                nameTexts[i] = go.GetComponent<TextMeshProUGUI>();
                nameTexts[i].text = "";
            }
            else
            {
                Debug.LogError($"[UI] {objectName} 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    public void Connect()                                           // 버튼 연결
    {
        if (m_InputField == null)
        {
            Debug.LogError("[Connect] m_InputField가 null입니다.");
            return;
        }

        if (string.IsNullOrEmpty(m_InputField.text))                // m_InputField에 Text가 비어 있으면
        {
            // 사용자가 닉네임을 정하지 않으면 Player1000~9999까지 자동으로 이름을 정해준다.
            m_InputField.text = "Player" + Random.Range(1000, 9999);
            Debug.Log($"player : {m_InputField.text}");
        }

        PhotonNetwork.ConnectUsingSettings();                       // 서버 연결
    }

    public override void OnConnectedToMaster()                      // 마스터 서버 연결 후 호출
    {
        if (m_InputField == null)
        {
            Debug.LogError("[Photon] OnConnectedToMaster에서 m_InputField가 null입니다.");
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = m_InputField.text;

        Debug.Log("[Photon] 마스터 서버 연결 완료 → 랜덤 방 참가 시도");
        PhotonNetwork.JoinRandomRoom();                             // 빈 방이 있으면 참가, 없으면 실패 콜백 호출
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"[Room] 랜덤 방 참가 실패: {message} → 새로운 방 생성 시도");

        // 랜덤한 방 이름을 만들어서 새로 생성
        string newRoomName = "Room" + Random.Range(1000, 9999);
        RoomOptions options = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(newRoomName, options, null);
    }

    public override void OnJoinedRoom()                              // 방 참가 성공 시
    {
        Debug.Log($"[Room] 방 참가 성공: {PhotonNetwork.CurrentRoom.Name}");

        // 바로 호출하지 말고 살짝 지연해서 UI 초기화 타이밍 맞추기
        StartCoroutine(DelayedUpdatePlayerListUI());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)       // 새로운 플레이어 입장 시
    {
        UpdatePlayerListUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)        // 플레이어 나감 시
    {
        UpdatePlayerListUI();
    }

    IEnumerator DelayedUpdatePlayerListUI()
    {
        yield return new WaitForSeconds(0.1f);                        // 아주 짧은 지연
        UpdatePlayerListUI();
    }

    void UpdatePlayerListUI()
    {
        if (nameTexts == null || nameTexts.Length == 0)
        {
            Debug.LogWarning("[UI] nameTexts 배열이 초기화되지 않았습니다.");
            return;
        }

        // 모든 플레이어 정렬 (UID 기준, 일관성 있게)
        Player[] sortedPlayers = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToArray();

        // 모든 텍스트 초기화
        for (int i = 0; i < nameTexts.Length; i++)
        {
            if (nameTexts[i] != null)
                nameTexts[i].text = "";
        }

        for (int i = 0; i < sortedPlayers.Length; i++)
        {
            string nickname = sortedPlayers[i].NickName;

            if (sortedPlayers[i] == PhotonNetwork.LocalPlayer)
            {
                // 본인은 항상 Text1
                nameTexts[0].text = nickname;
            }
            else
            {
                // 다른 유저는 Text2~Text4
                int otherIndex = GetOtherPlayerIndex(sortedPlayers[i]);
                if (otherIndex >= 1 && otherIndex < nameTexts.Length)
                {
                    nameTexts[otherIndex].text = nickname;
                }
            }
        }
    }

    int GetOtherPlayerIndex(Player player)
    {
        // 정렬된 순서에서 현재 플레이어를 기준으로 다른 유저의 순번을 구함
        Player[] sortedPlayers = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToArray();
        int index = System.Array.IndexOf(sortedPlayers, player);
        int myIndex = System.Array.IndexOf(sortedPlayers, PhotonNetwork.LocalPlayer);

        // 나보다 먼저 등장하면 그대로, 나보다 뒤면 -1 해서 빈자리로 맞춤
        if (index < myIndex)
            return index + 1;
        else if (index > myIndex)
            return index;

        return 0; // 본인
    }
}

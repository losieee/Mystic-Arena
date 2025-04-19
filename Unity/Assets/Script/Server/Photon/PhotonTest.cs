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
    public TMP_InputField m_InputField;
    public TextMeshProUGUI[] nameTexts = new TextMeshProUGUI[4];

    void Start()
    {
        Screen.SetResolution(960, 600, false);
        StartCoroutine(InitializeUI());
    }

    IEnumerator InitializeUI()
    {
        yield return new WaitForEndOfFrame();

        GameObject inputObj = GameObject.Find("Canvas/InputField");
        if (inputObj != null)
        {
            m_InputField = inputObj.GetComponent<TMP_InputField>();
        }
        else
        {
            Debug.LogError("[UI] InputField 오브젝트를 찾을 수 없습니다.");
        }

        for (int i = 0; i < 4; i++)
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

    public void Connect()
    {
        if (m_InputField == null)
        {
            Debug.LogError("[Connect] m_InputField가 null입니다.");
            return;
        }

        if (string.IsNullOrEmpty(m_InputField.text))
        {
            m_InputField.text = "Player" + Random.Range(1000, 9999);
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        if (m_InputField == null)
        {
            Debug.LogError("[Photon] OnConnectedToMaster에서 m_InputField가 null입니다.");
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = m_InputField.text;

        RoomOptions options = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom("Room1", options, null);
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayerListUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerListUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerListUI();
    }

    void UpdatePlayerListUI()
    {
        // 모든 플레이어 정렬 (UID 기준, 일관성 있게)
        Player[] sortedPlayers = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToArray();

        for (int i = 0; i < nameTexts.Length; i++)
        {
            if (i < sortedPlayers.Length)
            {
                string nickname = sortedPlayers[i].NickName;

                // 본인은 항상 Text1
                if (sortedPlayers[i] == PhotonNetwork.LocalPlayer)
                {
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
            else
            {
                // 빈 슬롯은 지우기
                nameTexts[i].text = "";
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

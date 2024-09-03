using UnityEngine;
using EnumStruct;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager um_instance;

    public UIState uiState;

    public GameObject uiTitle;
    public GameObject uiLobby;
    public GameObject uiRoom;
    public GameObject uiPlay;
    public GameObject roomList;
    public TextMeshProUGUI nickNameText;
    public TextMeshProUGUI roomNameText;

    // Start is called before the first frame update
    void Start()
    {
        //uiState = UIState.Title;
        if (um_instance == null)
        {
            um_instance = this;
        }
        else if (um_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeUiState(UIState state)
    {
        switch (state)
        {
            case UIState.Title:
                uiTitle.SetActive(true);
                uiLobby.SetActive(false);
                uiRoom.SetActive(false);
                uiPlay.SetActive(false);
                break; 
            case UIState.Lobby:
                uiTitle.SetActive(false);
                uiLobby.SetActive(true);
                uiRoom.SetActive(false);
                uiPlay.SetActive(false);
                break; 
            case UIState.Room:
                uiTitle.SetActive(false);
                uiLobby.SetActive(false);
                uiRoom.SetActive(true);
                uiPlay.SetActive(false);
                break; 
            case UIState.Play:
                uiTitle.SetActive(false);
                uiLobby.SetActive(false);
                uiRoom.SetActive(false);
                uiPlay.SetActive(true);
                break;
        }

        this.uiState = state;
    }

    public void CreatRoomButton()
    {
        //Debug.Log("생성 버튼 닉네임 : " + (nickNameText.text == "") + " / 방 이름 : " + (roomNameText.text == ""));
        //Debug.Log("닉네임 텍스트: '" + nickNameText.text.Trim().Length + "'");

        string nickName = nickNameText.text.Trim().Length >= 2 ? nickNameText.text.Trim() : "";
        string roomName = roomNameText.text.Trim().Length >= 2 ? roomNameText.text.Trim() : "";

        //Debug.Log("닉네임 : " + nickName + " / 방 이름 : " + roomName);
        //Debug.Log("일치 ? 닉네임 : " + (nickName == "") + " / 방 이름 : " + (roomName == ""));

        PhotonManager.pm_instance.RoomSetting(nickName, roomName);
        PhotonManager.pm_instance.CreateRoom();
    }    

    public void JoinRoomButton()
    {
        string nickName = nickNameText.text.Trim().Length >= 2 ? nickNameText.text : "";
        string roomName = "";
        //PhotonManager.pm_instance.RoomSetting(nickName, roomName);
        PhotonManager.pm_instance.JoinRoom();
    }

    public void GameStartButton()
    {
        PhotonManager.pm_instance.GameStart();
    }
}

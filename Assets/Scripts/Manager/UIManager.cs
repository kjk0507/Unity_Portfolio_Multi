using UnityEngine;
using EnumStruct;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;

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

    public bool isPlayScene = false;
    public GameObject checkUnitPosition;
    public Button readyButton;
    public Button resetButton;

    // Start is called before the first frame update
    void Start()
    {
        //uiState = UIState.Title;
        if (um_instance == null)
        {
            um_instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (um_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckReadyButton();
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
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlayScene")
        {
            isPlayScene = true;

            checkUnitPosition = GameObject.Find("CheckUnitPosition");
            readyButton = GameObject.Find("GetReady").GetComponent<Button>();
            resetButton = GameObject.Find("GetWaiting").GetComponent<Button>();

            if (readyButton != null)
            {
                readyButton.onClick.AddListener(GetReady);
            }

            if (resetButton != null)
            {
                resetButton.onClick.AddListener(GetWaiting);
            }
        }
    }

    public void CheckReadyButton()
    {
        if(!isPlayScene || PlayManager.pm_instance.CheckCurPhase() != PlayPhase.UnitPlacement)
        {
            return;
        }

        //Button readyButton = GameObject.Find("GetReady").GetComponent<Button>();
        Image buttonImage = readyButton.GetComponent<Image>();

        if (UnitManager.um_instance.CheckAllUnitPosition())
        {
            buttonImage.color = new Color(1f, 1f, 1f);
        }
        else
        {
            buttonImage.color = new Color(0.5f, 0.5f, 0.5f);
        }

    }

    public void GetReady()
    {
        if (!UnitManager.um_instance.CheckAllUnitPosition())
        {
            return;
        }

        if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Master)
        {
            PlayManager.pm_instance.blueReady = true;            
        } else if(PlayManager.pm_instance.CheckCurRole() == PlayerRole.Participant)
        {
            PlayManager.pm_instance.redReady = true;
        }

        PhotonManager.pm_instance.GetReady();
    }

    public void GetWaiting()
    {
        if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Master)
        {
            PlayManager.pm_instance.blueReady = false;
        }
        else if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Participant)
        {
            PlayManager.pm_instance.redReady = false;
        }

        UnitManager.um_instance.ResetUnitPositionAndCreate();
        PhotonManager.pm_instance.GetWaiting();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;
using Photon.Pun;
using Photon.Realtime;

public class UIManager : MonoBehaviour
{
    public static UIManager um_instance;

    public UIState uiState;

    public GameObject uiTitle;
    public GameObject uiLobby;
    public GameObject uiRoom;
    public GameObject uiPlay;
    public GameObject roomList;

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
        PhotonManager.pm_instance.CreatRoom();
    }    

    public void JoinRoomButton()
    {
        PhotonManager.pm_instance.JoinRoom();
    }

    public void GameStartButton()
    {
        PhotonManager.pm_instance.GameStart();
    }
}

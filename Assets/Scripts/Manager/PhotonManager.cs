using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using EnumStruct;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager pm_instance;

    private readonly string gameVersion = "v1.0";
    private string userId = "tester1";

    public GameObject roomList;
    public GameObject roomPrefab;

    public List<RoomInfo> roomInfo = new List<RoomInfo>();

    private void Awake()
    {
        // 방장이 씬 로딩시 나머지 사람들이 자동으로 싱크
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;

        // 포톤 엔진 접속 -> 로비에 자동 입장
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        if (pm_instance == null)
        {
            pm_instance = this;
        }
        else if (pm_instance != this)
        {
            Destroy(gameObject);
        }

        Debug.Log("포톤 매니저 시작");
        PhotonNetwork.NickName = userId;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 포톤 서버에 접속
    public override void OnConnectedToMaster()
    {
        Debug.Log("00.포톤 서버 접속");

        Debug.Log("01.랜덤 룸 접속 시도");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("02.랜덤 룸 접속 실패");

        //Debug.Log("03.랜덤 룸 생성");
        //RoomOptions ro = new RoomOptions();
        //ro.IsOpen = true;
        //ro.IsVisible = true;
        //ro.MaxPlayers = 10;

        //// 룸 생성 및 자동 입장
        //PhotonNetwork.CreateRoom("room_1", ro);        
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("04.방 생성 완료");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("05.방 입장 완료");
        UIManager.um_instance.ChangeUiState(UIState.Room);        
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Error.방 생성 실패: {message}");
    }

    public void CreatRoom()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 10;

        PhotonNetwork.CreateRoom("room_1", ro);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom("room_1");
    }

    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("PlayScene");
            //PlayManager.pm_instance.ChangeConnectState();
        }
    }
}

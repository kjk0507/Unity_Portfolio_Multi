
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using EnumStruct;
using ExitGames.Client.Photon;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager pm_instance;

    private readonly string gameVersion = "v1.0";
    public string userId = "";
    public string roomName = "";

    public GameObject roomPosition;
    public GameObject roomPrefab;

    public List<RoomInfo> roomInfo = new List<RoomInfo>();
    public Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();

    public string clickedRoomName;

    private void Awake()
    {
        // 방장이 씬 로딩시 나머지 사람들이 자동으로 싱크
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;

        // 포톤 엔진 접속 -> 로비에 자동 입장
        PhotonNetwork.ConnectUsingSettings();

        //photonView = GetComponent<PhotonView>();
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
        Debug.Log("01.포톤 로비 접속");
        PhotonNetwork.JoinLobby();

        //Debug.Log("01.랜덤 룸 접속 시도");
        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Error.랜덤 룸 접속 실패");

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

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        Debug.Log("PlayerCount : " + playerCount);

        if (playerCount == 2)
        {
            PlayManager.pm_instance.ChangeRole(PlayerRole.Participant);
        }
        else if (playerCount == 3)
        {
            PlayManager.pm_instance.ChangeRole(PlayerRole.Spectoator);
        }

        UIManager.um_instance.ChangeUiState(UIState.Room);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Error.방 생성 실패: {message}");
    }

    public void CreatRoom()
    {
        //Debug.Log("방 생성시 아이디 : " + (userId == "") + " 방 이름 : " + (roomName == ""));

        if (userId == "" || userId == null)
        {
            userId = "User_" + Random.Range(0, 100);
        }

        if(roomName == "" || roomName == null)
        {
            roomName = "Room_" + Random.Range(0, 100);
        }

        Hashtable customProperties = new Hashtable
        {
            { "MasterClientId", userId }
        };

        //Debug.Log("방 생성 아이디 : " + userId + " 방 이름 : " + roomName);

        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 10;
        ro.CustomRoomProperties = customProperties;
        ro.CustomRoomPropertiesForLobby = new string[] { "MasterClientId" };

        PhotonNetwork.NickName = userId;

        PhotonNetwork.CreateRoom(roomName, ro);
    }

    public override void OnRoomListUpdate(List<Photon.Realtime.RoomInfo> roomList)
    {
        //Debug.Log("생성된 방의 수 : " + roomList.Count);

        GameObject tempRoom = null;
        foreach (var room in roomList)
        {
            if (room.RemovedFromList == true)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    string masterClientId = room.CustomProperties["MasterClientId"] as string;

                    GameObject originalPrefab = Resources.Load<GameObject>("Prefab/UI/UI_Room");
                    GameObject instance = Instantiate(originalPrefab);
                    instance.transform.SetParent(roomPosition.transform, false);
                    instance.GetComponent<RoomInfo>().CreatRoomInfo(roomDict.Count, room.Name, masterClientId);
                    //Debug.Log("생성 닉네임 : " + masterClientId + " / 방 이름 : " + room.Name);
                    roomDict.Add(room.Name, instance);
                }
                else
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                }
            }
        }
    }

    public void JoinRoom()
    {
        if(clickedRoomName == null || clickedRoomName == "")
        {
            return;
        }

        if (userId == "")
        {
            userId = "User_" + Random.Range(0, 100);
        }

        PhotonNetwork.NickName = userId;
        PhotonNetwork.JoinRoom(clickedRoomName);
    }

    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PlayManager.pm_instance.ChangeRole(PlayerRole.Master);
            PhotonNetwork.LoadLevel("PlayScene");
        }
    }

    public void RoomSetting(string nickName, string room)
    {
        Debug.Log("방 세팅 실행");
        userId = nickName;
        roomName = room;
    }

    public void ClickRoom(string roomname)
    {
        if(clickedRoomName == roomname)
        {
            if (userId == "")
            {
                userId = "User_" + Random.Range(0, 100);
            }

            PhotonNetwork.NickName = userId;
            PhotonNetwork.JoinRoom(roomname);
        }

        clickedRoomName = roomname;
    }
}

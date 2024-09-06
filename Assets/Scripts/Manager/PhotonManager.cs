using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using EnumStruct;
using ExitGames.Client.Photon;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnitStruct;
using TMPro;

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

    public GameObject roomInfoObject;
    public GameObject roomGameStartButton;

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

        // 커스텀 타입 사용을 등록
        RegisterCustomTypes();
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

        UIManager.um_instance.ChangeUiState(UIState.Lobby);

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

        if (!PhotonNetwork.IsMasterClient)
        {
            roomInfoObject.GetComponent<RoomInfo>().CheckParticipant(userId);
            roomInfoObject.GetComponent<RoomInfo>().CheckRefresh();
            roomGameStartButton.SetActive(false);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Error.방 생성 실패: {message}");
    }

    public void CreateRoom()
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

        roomInfoObject.GetComponent<RoomInfo>().ChangeRoomInfo(roomName, userId);
        roomInfoObject.GetComponent<RoomInfo>().CheckRefresh();
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
                    instance.GetComponent<CreateRoomInfo>().CreatRoomInfo(roomDict.Count, room.Name, masterClientId);
                    roomInfoObject.GetComponent<RoomInfo>().ChangeRoomInfo(room.Name, masterClientId);
                    roomInfoObject.GetComponent<RoomInfo>().CheckRefresh();
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("새로운 플레이어가 방에 들어왔습니다: " + newPlayer.NickName);
        roomInfoObject.GetComponent<RoomInfo>().CheckParticipant(newPlayer.NickName);
        roomInfoObject.GetComponent<RoomInfo>().CheckRefresh();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("플레이어가 방을 떠났습니다: " + otherPlayer.NickName);
        roomInfoObject.GetComponent<RoomInfo>().CheckParticipant("");
        roomInfoObject.GetComponent<RoomInfo>().CheckRefresh();
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

        roomInfoObject.GetComponent<RoomInfo>().CheckParticipant(userId);
        roomInfoObject.GetComponent<RoomInfo>().CheckRefresh();
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
    public void GetReady()
    {
        photonView.RPC("OnGetReady", RpcTarget.Others);
    }

    [PunRPC]
    public void OnGetReady()
    {
        if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Master)
        {
            PlayManager.pm_instance.redReady = true;
        }
        else if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Participant)
        {
            PlayManager.pm_instance.blueReady = true;
        }
    }

    public void GetWaiting()
    {
        photonView.RPC("OnGetWaiting", RpcTarget.Others);
    }

    [PunRPC]
    public void OnGetWaiting()
    {
        if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Master)
        {
            PlayManager.pm_instance.redReady = false;
        }
        else if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Participant)
        {
            PlayManager.pm_instance.blueReady = false;
        }
    }

    private void RegisterCustomTypes()
    {
        PhotonPeer.RegisterType(typeof(Position), (byte)0, SerializePosition, DeserializePosition);
    }

    private static byte[] SerializePosition(object customObject)
    {
        Position pos = (Position)customObject;
        using (MemoryStream memStream = new MemoryStream())
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memStream, pos);
            return memStream.ToArray();
        }
    }

    private static object DeserializePosition(byte[] data)
    {
        using (MemoryStream memStream = new MemoryStream(data))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            return (Position)binaryFormatter.Deserialize(memStream);
        }
    }

    // ----------------------------------  RPC  ---------------------------------- //
    public Vector3 ChangePositionToVector(Position postion)
    {
        return new Vector3(postion.x, 0, postion.y);
    }

    public Position ChangeVectorToPosition(Vector3 vector)
    {
        return new Position((int)vector.x, (int)vector.z);
    }

    public void GetGameStart()
    {
        Position[] bluePositions = UnitManager.um_instance.GetBlueUnitPositionList().ToArray();
        Position[] redPositions = UnitManager.um_instance.GetRedUnitPositionList().ToArray();
        photonView.RPC("OnGetGameStart", RpcTarget.Others, bluePositions, redPositions);
    }

    [PunRPC]
    public void OnGetGameStart(Position[] blueUnitPositionArray, Position[] redUnitPositionArray)
    {
        List<Position> blueUnitPositionList = new List<Position>(blueUnitPositionArray);
        List<Position> redUnitPositionList = new List<Position>(redUnitPositionArray);

        if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Master)
        {
            UnitManager.um_instance.SetRedUnitList(redUnitPositionList);
        }
        else if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Participant)
        {
            UnitManager.um_instance.SetBlueUnitList(blueUnitPositionList);
        }
    }

    public void MoveUnit(Position curPosition, Position targetPosition)
    {
        Vector3 curTemp = ChangePositionToVector(curPosition);
        Vector3 TarTemp = ChangePositionToVector(targetPosition);
        photonView.RPC("OnMoveUnit", RpcTarget.Others, curTemp, TarTemp);
    }

    [PunRPC]
    public void OnMoveUnit(Vector3 curTemp, Vector3 tarTemp)
    {
        Position curPosition = ChangeVectorToPosition(curTemp);
        Position tarPosition = ChangeVectorToPosition(tarTemp);
        UnitManager.um_instance.MoveUnit(curPosition, tarPosition);
    }

    public void AttackUnit(Position curPosition, Position targetPosition)
    {
        Vector3 curTemp = ChangePositionToVector(curPosition);
        Vector3 tarTemp = ChangePositionToVector(targetPosition);
        photonView.RPC("OnAttackUnit", RpcTarget.Others, curTemp, tarTemp);
    }

    [PunRPC]
    public void OnAttackUnit(Vector3 curTemp, Vector3 tarTemp)
    {
        Position curPosition = ChangeVectorToPosition(curTemp);
        Position tarPosition = ChangeVectorToPosition(tarTemp);
        UnitManager.um_instance.AttackUnit(curPosition, tarPosition);
    }

    //public void ChangeTrun()
    //{
    //    photonView.RPC("OnChangeTrun", RpcTarget.Others);
    //}

    //[PunRPC]
    //public void OnChangeTrun()
    //{
    //    PlayManager.pm_instance.ChangeTurn();
    //}
}

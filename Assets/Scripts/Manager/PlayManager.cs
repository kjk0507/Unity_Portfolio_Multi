using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.GlobalIllumination;
using UnitStruct;

public class PlayManager : MonoBehaviour
{
    public static PlayManager pm_instance;
    private bool isConnect = true;
    public Transform[] spawnPoints;
    public bool isMyturn = false;

    public PlayPhase curPhase;
    public PlayerRole curRole;

    public GameObject cameraObject;
    public GameObject lightObject;

    public GameObject checkedObject;
    public bool isMakePlatform = false;

    public Position targetPosition;

    public bool blueReady = false;
    public bool redReady = false;

    private void Awake()
    {
        if (pm_instance == null)
        {
            pm_instance = this;
            //DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (pm_instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Debug.Log("Play Start");
        //CreatePlayer();
    }

    void Update()
    {
        CheckClickUnit();
        //CheckUnitGo();
        CheckBothReady();
    }

    public void ChangeConnectState()
    {
        isConnect = true;
    }

    //IEnumerator CreatePlayer()
    //{
    //    yield return new WaitUntil(delegate
    //    {
    //        return isConnect;
    //    });

    //    string prefabPath = "Prefab/Player";
    //    GameObject prefab = Resources.Load<GameObject>(prefabPath);
    //    GameObject instance = Instantiate(prefab);
    //    // playerTemp = PhotonNetwork.Instantiate("Prefab/Player", Vector3.one, Quaternion.identity, 0);
    //}

    //public void CreatePlayer()
    //{
    //    Debug.Log("플레이어 생성");
    //    //GameObject playerTemp = PhotonNetwork.Instantiate("Prefab/Player", Vector3.one, Quaternion.identity, 0);

    //    spawnPoints = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

    //    Vector3 pos = spawnPoints[PhotonNetwork.CurrentRoom.PlayerCount].position;
    //    Quaternion rot = spawnPoints[PhotonNetwork.CurrentRoom.PlayerCount].rotation;

    //    GameObject playerTemp = PhotonNetwork.Instantiate("Prefab/Player", pos, rot, 0);
    //}

    public void ChangePhase(PlayPhase phase)
    {
        curPhase = phase;
    }

    public void ChangeRole(PlayerRole role)
    {
        curRole = role;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlayScene")
        {
            cameraObject = GameObject.FindWithTag("MainCamera");
            lightObject = GameObject.FindWithTag("DirectionalLight");
            ChangePhase(PlayPhase.UnitPlacement);

            Debug.Log("this is playscend");
            ChangeView();
        }
    }

    public void ChangeView()
    {        
        if (curRole == PlayerRole.Master)
        {

        }
        else if (curRole == PlayerRole.Participant)
        {
            cameraObject.transform.position = new Vector3(0f, 13f, 17f);

            Quaternion currentRotation = cameraObject.transform.rotation;
            Vector3 eulerRotation = currentRotation.eulerAngles;
            eulerRotation.x = 140f;
            eulerRotation.z = 180f;
            cameraObject.transform.rotation = Quaternion.Euler(eulerRotation);

            Quaternion currentLightRotation = lightObject.transform.rotation;
            Vector3 eulerLightRotation = currentLightRotation.eulerAngles;
            eulerLightRotation.y = 150f;
            lightObject.transform.rotation = Quaternion.Euler(eulerLightRotation);
        }
    }

    public PlayerRole CheckCurRole()
    {
        return curRole;
    }

    public PlayPhase CheckCurPhase()
    {
        return curPhase;
    }

    public void CheckClickUnit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 나중에 파란색 이동 패널 클릭도 감지해야함
            GameObject clickedObject;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {                
                clickedObject = hit.collider.gameObject;

                Debug.Log("click : " +  clickedObject.name);

                int playerUnit = 0;
                int blueUnit = LayerMask.NameToLayer("BlueUnit");
                int redUnit = LayerMask.NameToLayer("RedUnit");
                int moveUnit = LayerMask.NameToLayer("MoveUnit");

                if (curRole == PlayerRole.Master)
                {
                    playerUnit = blueUnit;
                }
                else if (curRole == PlayerRole.Participant)
                {
                    playerUnit = redUnit;
                }

                if (clickedObject.layer == playerUnit)
                {
                    checkedObject = clickedObject;
                    Debug.Log("checkedObject : " + checkedObject.name);                    
                }
                else if(checkedObject != null && clickedObject.layer == moveUnit)
                {
                    // 오브젝트가 등록된 상태 -> moveUnit 클릭한 경우
                    //string unitName = checkedObject.GetComponent<UnitState>().status.GetName();
                    Position targetPosition = hit.collider.gameObject.GetComponent<TileState>().status.GetPosition();
                    CheckMoveUnit(targetPosition);
                    checkedObject = null;
                }
                else
                {
                    checkedObject = null;
                }
            }
            else
            {
                checkedObject = null;
            }

            CheckUnitGo();
        }
    }

    public void CheckUnitGo()
    {
        if (isMyturn && checkedObject != null)
        {
            Position curPosition = checkedObject.GetComponent<UnitState>().status.GetPosition();
            //isMakePlatform = true;
            if(curPosition.x != -5 && curPosition.y != -5)
            {
                UnitManager.um_instance.PositionCheck(curPosition);                
            }
        }

        if(checkedObject == null)
        {
            //isMakePlatform = false;
            UnitManager.um_instance.ClearPositionLapping();
        }
    }

    // 페이즈 별 상황 -> 각 페이즈가 끝난 후엔 포톤으로 통신해서 변경 사항을 알려 줘야됨
    // UnitPlacement 유닛 배치 -> 처음 한번만 실행
    // TurnStart     턴 시작
    // UsingItem     아이템 사용 페이즈
    // MoveUnit      유닛 이동
    // TurnEnd       턴 종료

    // UnitPlaceMent -> UI에서 준비완료를 누른 후 두개 다 true 인 경우 종료
    public void CheckBothReady()
    {
        if(blueReady && redReady)
        {
            ChangePhase(PlayPhase.TurnStart);

            blueReady = false;
            redReady = false;

            PhotonManager.pm_instance.GetGameStart();
            UIManager.um_instance.HiddingReadyButton();

            if (curRole == PlayerRole.Master)
            {
                isMyturn = true;
            }
            else
            {
                isMyturn= false;
            }
        }
    }

    // TurnStart     턴 시작

    public void CheckMoveUnit(Position targetPosition)
    {
        // 타겟 포지션에 뭔가 있는지 확인
        GameObject targetObj = UnitManager.um_instance.CheckObjByPosition(targetPosition);
        
        if(targetObj != null)
        {
            // 아이템이 있다면 습득


            // 적이 있다면 공격

            // 도착지라면 게임 끝
        }
        else
        {
            // 해당 클라이언트 움직임
            checkedObject.GetComponent<UnitState>().MovePosition(targetPosition);
            string name = checkedObject.GetComponent<UnitState>().status.GetName();

            // 상대방에게 이동 전달
            PhotonManager.pm_instance.MoveUnit(name, targetPosition, null);
        }
    }
}

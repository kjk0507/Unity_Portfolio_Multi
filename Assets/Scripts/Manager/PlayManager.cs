using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.GlobalIllumination;
using UnitStruct;
using Unity.VisualScripting;

public class PlayManager : MonoBehaviour
{
    public static PlayManager pm_instance;
    private bool isConnect = true;
    public Transform[] spawnPoints;
    public bool isMyturn = false;
    public bool isAction = false; // 이동 / 공격을 했다면 true 아니면 false(클릭 가능)

    public PlayPhase curPhase;
    public PlayerRole curRole;

    public GameObject cameraObject;
    public GameObject lightObject;

    public GameObject checkedObject;
    public GameObject checkedEnemyObject;
    public bool isMakePlatform = false;

    public Position targetPosition;

    public bool blueReady = false;
    public bool redReady = false;

    // 아이템 생성 관련
    public int itemTurnCount = 0;
    public List<GameObject> itemList = new List<GameObject>();

    //public List<GameObject> playerInventory = new List<GameObject>();
    public bool isUseItem = false; // false일때만 아이템 사용가능, 턴이 돌아오면 false로 변경
    public bool isUseChangePosition = false;
    public bool isUseMoveEnemy = false;
    public bool isUseDoubleMove = false;
    public bool isUseUncover = false;

    public int item01;
    public int item02;
    public int item03;
    public int item04;

    // 위치변경
    public GameObject changeUnit01;
    public GameObject changeUnit02;
    public GameObject fakeUnit01;
    public GameObject fakeUnit02;

    // 숨김 확인 대상
    public GameObject uncoverTargetObj;

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
        //item01 = 1;
        //item02 = 1;
        //item03 = 1;
        //item04 = 1;
    }

    void Update()
    {
        CheckClickUnit();
        //CheckUnitGo();
        CheckBothReady();
        CreateItem();
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
                int enemyUnit = 0;
                int blueUnit = LayerMask.NameToLayer("BlueUnit");
                int redUnit = LayerMask.NameToLayer("RedUnit");
                int moveUnit = LayerMask.NameToLayer("MoveUnit");

                if (curRole == PlayerRole.Master)
                {
                    playerUnit = blueUnit;
                    enemyUnit = redUnit;
                }
                else if (curRole == PlayerRole.Participant)
                {
                    playerUnit = redUnit;
                    enemyUnit = blueUnit;
                }

                if (clickedObject.layer == playerUnit && !isUseChangePosition)
                {
                    checkedObject = clickedObject;
                    //Debug.Log("checkedObject : " + checkedObject.name);                    
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

                // 위치 변경 아이템을 사용한 경우
                if(isUseChangePosition)
                {                
                    if (clickedObject.layer == playerUnit && changeUnit01 == null)
                    {
                        changeUnit01 = clickedObject;
                        string unitName = clickedObject.GetComponent<UnitState>().status.GetName();
                        if(unitName == "Knight_Red1" || unitName == "Knight_Red2" || unitName == "Knight_Red3" || unitName == "Knight_Red4")
                        {
                            unitName = "Knight_Red";
                        } 
                        else if (unitName == "Knight_Blue1" || unitName == "Knight_Blue2" || unitName == "Knight_Blue3" || unitName == "Knight_Blue4")
                        {
                            unitName = "Knight_Blue";
                        }

                        GameObject unit = Resources.Load<GameObject>("Prefab/Unit/Fake/" + unitName);
                        Quaternion rotation = Quaternion.Euler(40, 0, 0);
                        Vector3 position = new Vector3(-2.2f, 6.5f, -18f);
                        fakeUnit01 = Instantiate(unit, position, rotation);
                    } 
                    else if (clickedObject.layer == playerUnit && changeUnit01 != null)
                    {
                        changeUnit02 = clickedObject;
                        string unitName = clickedObject.GetComponent<UnitState>().status.GetName();
                        if (unitName == "Knight_Red1" || unitName == "Knight_Red2" || unitName == "Knight_Red3" || unitName == "Knight_Red4")
                        {
                            unitName = "Knight_Red";
                        }
                        else if (unitName == "Knight_Blue1" || unitName == "Knight_Blue2" || unitName == "Knight_Blue3" || unitName == "Knight_Blue4")
                        {
                            unitName = "Knight_Blue";
                        }

                        GameObject unit = Resources.Load<GameObject>("Prefab/Unit/Fake/" + unitName);
                        Quaternion rotation = Quaternion.Euler(40, 0, 0);
                        Vector3 position = new Vector3(0.4f, 6.5f, -18f);
                        fakeUnit02 = Instantiate(unit, position, rotation);
                    }
                    else
                    {
                        //
                    }
                }

                // 적 유닛 클릭 상황인 경우
                if (clickedObject.layer == enemyUnit && !isUseUncover)
                {
                    checkedEnemyObject = clickedObject;
                } else if(clickedObject.layer == enemyUnit && isUseUncover)
                {
                    uncoverTargetObj = clickedObject;
                    GameObject unit = Resources.Load<GameObject>("Prefab/Unit/Fake/Ghost");
                    Quaternion rotation = Quaternion.Euler(-40, -180, 0);
                    Vector3 position = new Vector3(-2.2f, 6.5f, -18f);
                    fakeUnit01 = Instantiate(unit, position, rotation);
                }

                // 
                if(isUseMoveEnemy && checkedEnemyObject != null && clickedObject.layer == moveUnit)
                {
                    Position targetPosition = hit.collider.gameObject.GetComponent<TileState>().status.GetPosition();
                    CheckMoveUnit(targetPosition);
                    checkedEnemyObject = null;
                }

            }
            else
            {
                checkedObject = null;
                checkedEnemyObject = null;
            }

            CheckUnitGo();
        }
    }

    public void ClearChangeUnit()
    {
        isUseChangePosition = false;
        isUseUncover = false;
        changeUnit01 = null;
        changeUnit02 = null;
        Destroy(fakeUnit01);
        Destroy(fakeUnit02);

        uncoverTargetObj = null;
    }

    public void ChangeUnitPosition()
    {
        if(changeUnit01 == null || changeUnit02 == null)
        {
            return;
        }

        Position tempPosition01 = changeUnit01.GetComponent<UnitState>().status.GetPosition();
        Position tempPosition02 = changeUnit02.GetComponent<UnitState>().status.GetPosition();
        Vector3 tempVector01 = changeUnit01.transform.position;
        Vector3 tempVector02 = changeUnit02.transform.position;
        Quaternion tempRotation01 = changeUnit01.transform.rotation;
        Quaternion tempRotation02 = changeUnit02.transform.rotation;

        changeUnit01.GetComponent<UnitState>().status.SetPosition(tempPosition02.x, tempPosition02.y);
        changeUnit02.GetComponent<UnitState>().status.SetPosition(tempPosition01.x, tempPosition01.y);
        changeUnit01.transform.position = tempVector02;
        changeUnit02.transform.position = tempVector01;
        changeUnit01.transform.rotation = tempRotation02;
        changeUnit02.transform.rotation = tempRotation01;

        PhotonManager.pm_instance.ChangeIsUseChangeUnit(tempPosition01, tempPosition02);
        UIManager.um_instance.ButtonItemUseCancel();
    }

    public void ChangeUnitPositionPhoton(Position tempA, Position tempB)
    {
        GameObject unit01 = UnitManager.um_instance.CheckObjByPosition(tempA);
        GameObject unit02 = UnitManager.um_instance.CheckObjByPosition(tempB);

        Position tempPosition01 = unit01.GetComponent<UnitState>().status.GetPosition();
        Position tempPosition02 = unit02.GetComponent<UnitState>().status.GetPosition();
        Vector3 tempVector01 = unit01.transform.position;
        Vector3 tempVector02 = unit02.transform.position;
        Quaternion tempRotation01 = unit01.transform.rotation;
        Quaternion tempRotation02 = unit02.transform.rotation;

        unit01.GetComponent<UnitState>().status.SetPosition(tempPosition02.x, tempPosition02.y);
        unit02.GetComponent<UnitState>().status.SetPosition(tempPosition01.x, tempPosition01.y);
        unit01.transform.position = tempVector02;
        unit02.transform.position = tempVector01;
        unit01.transform.rotation = tempRotation02;
        unit02.transform.rotation = tempRotation01;
    }

    public void UncoverUnitHide()
    {
        Debug.Log(uncoverTargetObj.ToString());

        if (uncoverTargetObj == null)
        {
            return;
        }

        Debug.Log(uncoverTargetObj.GetComponent<UnitState>().status.GetName());

        uncoverTargetObj.GetComponent<UnitState>().isHide = false;
        UIManager.um_instance.ButtonItemUseCancel();
    }

    public void CheckUnitGo()
    {
        if (isAction || !isMyturn)
        {
            UnitManager.um_instance.ClearPositionLapping();
            return;
        }

        if (isMyturn && checkedObject != null && !isUseMoveEnemy)
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

        // 상대말 이동 아이템 사용시
        if(isMyturn && isUseMoveEnemy && checkedEnemyObject != null)
        {
            Position curPosition = checkedEnemyObject.GetComponent<UnitState>().status.GetPosition();
            UnitManager.um_instance.PositionCheck(curPosition);
        } else if(isMyturn && isUseMoveEnemy && checkedEnemyObject == null)
        {
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
            UIManager.um_instance.UncoverItemUI();

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

    public void ChangeTurn()
    {
        // 더블 아이템 사용시 / 상대 말 위치 이동 아이템 사용시 턴카운트가 넘어가지 않음
        if (isUseMoveEnemy)
        {
            isUseMoveEnemy = false;
            isAction = false;
            return;
        }

        if (isUseDoubleMove)
        {
            isUseDoubleMove = false;
            isAction = false;
            return;
        }

        if (itemList.Count < 4)
        {
            itemTurnCount++;
        }
        else
        {
            itemTurnCount = 0;
        }

        UnitManager.um_instance.CheckEnding();

        if (isMyturn)
        {
            isMyturn = false;
            isAction = true;
        }
        else
        {
            isMyturn = true;
            isAction = false;
            isUseItem = false;
        }
    }

    public void CreateItem()
    {
        if(curRole != PlayerRole.Master)
        {
            return;
        }

        if(itemList.Count > 4 || itemTurnCount < 3)
        {
            return;
        }

        itemTurnCount = 0;

        Position temp;

        do
        {
            int randomX = Random.Range(-3, 4);
            int randomY = Random.Range(-3, 4);

            temp = new Position(randomX, randomY);

        } 
        while (!UnitManager.um_instance.CheckEmptyPosition(temp));

        CreateItemPosition(temp);
        PhotonManager.pm_instance.CreatItem(temp);
    }

    public void CreateItemPosition(Position point)
    {
        Vector3 itemPosition = new Vector3(point.x, 0.2f, point.y * 1.5f);
        GameObject itemPrefab = Resources.Load<GameObject>("Prefab/Env/Item");
        GameObject item = Instantiate(itemPrefab, itemPosition, Quaternion.identity);
        itemList.Add(item);
    }

    public void IsActionTrue()
    {
        isAction = true;
    }

    public void GameEnding()
    {
        isMyturn = false;
        isAction = false;
    }

    public void AddItem()
    {
        //GameObject item = null;
        //playerInventory.Add(item);

        int num = Random.Range(1, 5);
        ItemNameNum temp = (ItemNameNum)num;

        switch (temp)
        {
            case ItemNameNum.ChangePosition:
                item01++;
                UIManager.um_instance.ChangeItemNum(ItemNameNum.ChangePosition);
                break;
            case ItemNameNum.MoveEnemy:
                item02++;
                UIManager.um_instance.ChangeItemNum(ItemNameNum.MoveEnemy);
                break;
            case ItemNameNum.DoubleMove:
                item03++;
                UIManager.um_instance.ChangeItemNum(ItemNameNum.DoubleMove);
                break;
            case ItemNameNum.Uncover:
                item04++;
                UIManager.um_instance.ChangeItemNum(ItemNameNum.Uncover);
                break;
            //case ItemNameNum.Bomb:
            //    item05++;
            //    UIManager.um_instance.ChangeItemNum(ItemNameNum.Bomb);
            //    break;
        }
    }

    public int GetItemNum(ItemNameNum num)
    {
        switch(num)
        {
            case ItemNameNum.ChangePosition:
                return item01;
            case ItemNameNum.MoveEnemy: 
                return item02;
            case ItemNameNum.DoubleMove: 
                return item03;
            case ItemNameNum.Uncover: 
                return item04;
        }

        return 0;
    }

    // TurnStart     턴 시작 : 아이템 사용 -> 공격 -> 이동 -> 턴 종료 순서로 작동

    public void CheckMoveUnit(Position targetPosition)
    {
        // 아이템 사용 여부 확인
        if(isUseMoveEnemy)
        {
            checkedObject = checkedEnemyObject;
        }

        // 타겟 포지션에 뭔가 있는지 확인
        GameObject targetObj = UnitManager.um_instance.CheckObjByPosition(targetPosition);
        
        if(targetObj != null)
        {
            PlayerDefine define = targetObj.GetComponent<UnitState>().status.GetDefine();

            if(define == PlayerDefine.Item)
            {
                // 아이템이 있다면 습득

            }
            else if(define == PlayerDefine.Blue || define == PlayerDefine.Red)
            {
                // 적이 있다면 공격
                checkedObject.GetComponent<UnitState>().AttackUnit(targetPosition);
                Position curPosition = checkedObject.GetComponent<UnitState>().status.GetPosition();

                // 상대방에게 전달
                PhotonManager.pm_instance.AttackUnit(curPosition, targetPosition);

            }
            // 끝인지 확인하는건 다른 곳에서 작동
            //else if(define == PlayerDefine.Goal)
            //{
            //    // 도착지라면 게임 끝

            //}
        }
        else
        {
            // 해당 클라이언트 움직임
            checkedObject.GetComponent<UnitState>().MoveUnit(targetPosition);
            Position curPosition = checkedObject.GetComponent<UnitState>().status.GetPosition();

            // 상대방에게 이동 전달
            PhotonManager.pm_instance.MoveUnit(curPosition, targetPosition);
        }
    }
}

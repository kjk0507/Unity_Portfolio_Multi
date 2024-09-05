using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitStruct;
using UnityEngine.SceneManagement;
using EnumStruct;

public class UnitManager : MonoBehaviour
{
    public static UnitManager um_instance;

    public List<GameObject> blueList = new List<GameObject>();

    public List<GameObject> tileList = new List<GameObject>();
    public List<GameObject> bluePlaceLocation = new List<GameObject>();
    public List<GameObject> redPlaceLocation = new List<GameObject>();

    public List<GameObject> itemList = new List<GameObject>();
    public List<GameObject> blueUnitList = new List<GameObject>();
    public List<GameObject> redUnitList = new List<GameObject>();

    // 저 경우 카메라를 2개로 나눠서 정 방향이 자신, 반대 방향이 상대방으로 지정

    void Start()
    {
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

    void Update()
    {
        
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlayScene")
        {
            CreateUnitBlue();
            CreateUnit();
            CheckTileObject();
        }
    }

    public void CheckTileObject()
    {
        //GameObject[] tileObjects = GameObject.FindGameObjectsWithTag("Tile");
        //tileList = new List<GameObject>(tileObjects);
        GameObject tile = Resources.Load<GameObject>("Prefab/Env/CanGo");

    }

    public Vector3 GetPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / 1f);
        int z = Mathf.RoundToInt(position.z / 1.5f);

        return new Vector3(x * 1f, 0, z * 1.5f);
    }

    public bool IsRightPosition(Vector3 position)
    {
        bool result = false;

        PlayerRole curRole = PlayManager.pm_instance.CheckCurRole();

        if(curRole == PlayerRole.Master)
        {
            // 위치가 범위안에 들어왔는지
            if(position.x >= -2f && position.x <= 2f && position.z >= -4.5f && position.z <= -3f)
            { 
                // 해당 칸에 다른 오브젝트가 있는지
                foreach(var obj in blueUnitList)
                {
                    Position temp = obj.GetComponent<UnitState>().status.GetPosition();

                    if(temp.x == position.x && temp.y == position.z / 1.5 )
                    {
                        return false;
                    }
                }
                result = true;
            }
        } else if(curRole == PlayerRole.Participant)
        {
            if (position.x >= -2f && position.x <= 2f && position.z <= 4.5f && position.z >= 3f)
            {
                foreach (var obj in redUnitList)
                {
                    Position temp = obj.GetComponent<UnitState>().status.GetPosition();
                    if (temp.x == position.x && temp.y == position.z / 1.5)
                    {
                        return false;
                    }
                }
                result = true;
            }
        }

        return result;
    }

    public void CreateUnitBlue() 
    {
        GameObject bluePrefab = Resources.Load<GameObject>("Prefab/Env/CanGo");
        for(int i = 0; i < 4; i++)
        {
            GameObject instance = Instantiate(bluePrefab);
            instance.GetComponent<TileState>().status = new Status(-5, -5);
            instance.GetComponent<TileState>().status.SetName("MoveUnit");
            instance.SetActive(false);
            blueList.Add(instance);
        }
    }

    public void PositionCheck(Position position)
    {
        int x = position.x;
        int y = position.y;

        Position positionRight = new Position(x + 1 , y); // 오른쪽
        Position positionLeft = new Position(x - 1, y); // 왼쪽
        Position positionUp = new Position(x, y + 1); // 위
        Position positionDown = new Position(x, y - 1); // 아래

        //GameObject bluePrefab = Resources.Load<GameObject>("Prefab/Env/CanGo");
        //GameObject instance = Instantiate(bluePrefab);
        Vector3 instantiatePosition = new Vector3();

        if(blueList.Count <= 0)
        {
            CreateUnitBlue();
        }

        GameObject instanceRight = blueList[0];
        GameObject instanceLeft = blueList[1];
        GameObject instanceUp = blueList[2];
        GameObject instanceDown = blueList[3];

        // 상단에 있어야 클릭이 쉬우므로 좌표 수정, 높이를 3으로 변경
        // 좌표상 y는 길이가 1.5배이므로 변경 계산
        if (!CheckLapping(positionRight))
        {
            if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Master)
            {
                instantiatePosition = new Vector3(positionRight.x, 3f, -3.5f + positionRight.y * 1.5f);
            }
            else if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Participant)
            {
                instantiatePosition = new Vector3(positionRight.x, 3f, 3.5f + positionRight.y * 1.5f);
            }
            instanceRight.SetActive(true);
            instanceRight.GetComponent<TileState>().status.SetPosition(positionRight.x, positionRight.y);
            instanceRight.transform.position = instantiatePosition;
            //GameObject instance = Instantiate(bluePrefab, instantiatePosition, Quaternion.identity);
            //blueList.Add(instance);
        }
        else
        {
            instanceRight.SetActive(false);
        }

        if (!CheckLapping(positionLeft))
        {
            if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Master)
            {
                instantiatePosition = new Vector3(positionLeft.x, 3f, -3.5f + positionLeft.y * 1.5f);
            }
            else if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Participant)
            {
                instantiatePosition = new Vector3(positionLeft.x, 3f, 3.5f + positionLeft.y * 1.5f);
            }

            instanceLeft.SetActive(true);
            instanceLeft.GetComponent<TileState>().status.SetPosition(positionLeft.x, positionLeft.y);
            instanceLeft.transform.position = instantiatePosition;
            //GameObject instance = Instantiate(bluePrefab, instantiatePosition, Quaternion.identity);
            //blueList.Add(instance);
        }
        else
        {
            instanceLeft.SetActive(false);
        }

        if (!CheckLapping(positionUp))
        {            
            if(PlayManager.pm_instance.CheckCurRole() == PlayerRole.Master)
            {
                instantiatePosition = new Vector3(positionUp.x, 3f, -3.5f + positionUp.y * 1.5f);
            } else if(PlayManager.pm_instance.CheckCurRole() == PlayerRole.Participant)
            {
                instantiatePosition = new Vector3(positionUp.x, 3f, 3.5f + positionUp.y * 1.5f);
            }
            
            instanceUp.SetActive(true);
            instanceUp.GetComponent<TileState>().status.SetPosition(positionUp.x, positionUp.y);
            instanceUp.transform.position = instantiatePosition;
            //GameObject instance = Instantiate(bluePrefab, instantiatePosition, Quaternion.identity);
            //blueList.Add(instance);
        }
        else
        {
            instanceUp.SetActive(false);
        }

        if (!CheckLapping(positionDown))
        {
            if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Master)
            {
                instantiatePosition = new Vector3(positionDown.x, 3f, -3.5f + positionDown.y * 1.5f);
            }
            else if (PlayManager.pm_instance.CheckCurRole() == PlayerRole.Participant)
            {
                instantiatePosition = new Vector3(positionDown.x, 3f, 3.5f + positionDown.y * 1.5f);
            }
            instanceDown.SetActive(true);
            instanceDown.GetComponent<TileState>().status.SetPosition(positionDown.x, positionDown.y);
            instanceDown.transform.position = instantiatePosition;
            //GameObject instance = Instantiate(bluePrefab, instantiatePosition, Quaternion.identity);
            //blueList.Add(instance);
        }
        else
        {
            instanceDown.SetActive(false);
        }
    }

    public bool CheckLapping(Position position)
    {
        bool result = false;

        PlayerRole curRole = PlayManager.pm_instance.CheckCurRole();

        if(curRole == PlayerRole.Master)
        {
            foreach (var otherUnit in blueUnitList)
            {
                Status otherStatue = otherUnit.GetComponent<UnitState>().status;
                if ((position.x == otherStatue.positionX && position.y == otherStatue.positionY) || (position.x < -3 || position.x > 3 || position.y < -3 || position.y > 3))
                {
                    return true;
                }
            }
        } else if(curRole == PlayerRole.Participant){
            foreach (var otherUnit in redUnitList)
            {
                Status otherStatue = otherUnit.GetComponent<UnitState>().status;
                if ((position.x == otherStatue.positionX && position.y == otherStatue.positionY) || (position.x < -3 || position.x > 3 || position.y < -3 || position.y > 3))
                {
                    return true;
                }
            }
        }        

        return result;
    }

    public void ClearPositionLapping()
    {
        foreach(var obj in blueList)
        {
            obj.SetActive(false);
        }

    }

    public void CreateUnit()
    {
        // 게임씬에서 유닛 생성 및 위치하는 것 vs 방 들어가기 전에 설정
        GameObject blueRoyalty1 = Resources.Load<GameObject>("Prefab/Unit/Prince_Blue");
        GameObject blueRoyalty2 = Resources.Load<GameObject>("Prefab/Unit/King_Blue");
        GameObject blueRoyalty3 = Resources.Load<GameObject>("Prefab/Unit/Princess_Blue");
        GameObject blueRoyalty4 = Resources.Load<GameObject>("Prefab/Unit/Queen_Blue");
        GameObject blueKnight1 = Resources.Load<GameObject>("Prefab/Unit/Knight_Blue");
        GameObject blueKnight2 = Resources.Load<GameObject>("Prefab/Unit/Knight_Blue");
        GameObject blueKnight3 = Resources.Load<GameObject>("Prefab/Unit/Knight_Blue");
        GameObject blueKnight4 = Resources.Load<GameObject>("Prefab/Unit/Knight_Blue");

        List<GameObject> bluelist = new List<GameObject> { blueRoyalty1, blueRoyalty2, blueRoyalty3, blueRoyalty4, blueKnight1, blueKnight2, blueKnight3, blueKnight4 };

        List<Vector3> bluePositions = new List<Vector3>
        {
            new Vector3(2, 0, -7f),
            new Vector3(2, 0, -8.5f),
            new Vector3(1, 0, -7f),
            new Vector3(1, 0, -8.5f),
            new Vector3(-2, 0, -7f),
            new Vector3(-2, 0, -8.5f),
            new Vector3(-1, 0, -7f),
            new Vector3(-1, 0, -8.5f),
        };

        List<string> blueUnitNames = new List<string>
        {
            "Prince_Blue", "King_Blue", "Princess_Blue", "Queen_Blue", "Knight_Blue1", "Knight_Blue2", "Knight_Blue3", "Knight_Blue4"
        };

        for (int i = 0; i < bluelist.Count; i++)
        {
            // 포지션은 놓아진 뒤에 정해짐
            //bluelist[i].GetComponent<UnitState>().status.SetPosition();
            GameObject unit = Instantiate(bluelist[i], bluePositions[i], Quaternion.identity);
            if(i < 4)
            {
                unit.GetComponent<UnitState>().status = new Status(blueUnitNames[i], PlayerDefine.Blue, UnitType.Royalty);
            }
            else
            {
                unit.GetComponent<UnitState>().status = new Status(blueUnitNames[i], PlayerDefine.Blue, UnitType.Knight);
            }

            blueUnitList.Add(unit);
        }

        GameObject redRoyalty1 = Resources.Load<GameObject>("Prefab/Unit/Prince_Red");
        GameObject redRoyalty2 = Resources.Load<GameObject>("Prefab/Unit/King_Red");
        GameObject redRoyalty3 = Resources.Load<GameObject>("Prefab/Unit/Princess_Red");
        GameObject redRoyalty4 = Resources.Load<GameObject>("Prefab/Unit/Queen_Red");
        GameObject redKnight1 = Resources.Load<GameObject>("Prefab/Unit/Knight_Red");
        GameObject redKnight2 = Resources.Load<GameObject>("Prefab/Unit/Knight_Red");
        GameObject redKnight3 = Resources.Load<GameObject>("Prefab/Unit/Knight_Red");
        GameObject redKnight4 = Resources.Load<GameObject>("Prefab/Unit/Knight_Red");
        //redRoyalty1.GetComponent<UnitState>().status = new Status(PlayerDefine.Red, UnitType.Royalty);
        //redRoyalty2.GetComponent<UnitState>().status = new Status(PlayerDefine.Red, UnitType.Royalty);
        //redRoyalty3.GetComponent<UnitState>().status = new Status(PlayerDefine.Red, UnitType.Royalty);
        //redRoyalty4.GetComponent<UnitState>().status = new Status(PlayerDefine.Red, UnitType.Royalty);
        //redKnight1.GetComponent<UnitState>().status = new Status(PlayerDefine.Red, UnitType.Knight);
        //redKnight2.GetComponent<UnitState>().status = new Status(PlayerDefine.Red, UnitType.Knight);
        //redKnight3.GetComponent<UnitState>().status = new Status(PlayerDefine.Red, UnitType.Knight);
        //redKnight4.GetComponent<UnitState>().status = new Status(PlayerDefine.Red, UnitType.Knight);

        List<GameObject> redlist = new List<GameObject> { redRoyalty1, redRoyalty2, redRoyalty3, redRoyalty4, redKnight1, redKnight2, redKnight3, redKnight4 };

        List<Vector3> redPositions = new List<Vector3>
        {
            new Vector3(-2, 0, 7f),
            new Vector3(-2, 0, 8.5f),
            new Vector3(-1, 0, 7f),
            new Vector3(-1, 0, 8.5f),
            new Vector3(2, 0, 7f),
            new Vector3(2, 0, 8.5f),
            new Vector3(1, 0, 7f),
            new Vector3(1, 0, 8.5f),
        };

        List<string> redUnitNames = new List<string>
        {
            "Prince_Red", "King_Red", "Princess_Red", "Queen_Red", "Knight_Red1", "Knight_Red2", "Knight_Red3", "Knight_Red4"
        };

        for (int i = 0; i < redlist.Count; i++)
        {
            // 포지션은 놓아진 뒤에 정해짐
            //bluelist[i].GetComponent<UnitState>().status.SetPosition();
            Quaternion rotation = Quaternion.Euler(0, 180, 0);
            GameObject unit = Instantiate(redlist[i], redPositions[i], rotation);
            if (i < 4)
            {
                unit.GetComponent<UnitState>().status = new Status(redUnitNames[i], PlayerDefine.Red, UnitType.Royalty);
            }
            else
            {
                unit.GetComponent<UnitState>().status = new Status(redUnitNames[i], PlayerDefine.Red, UnitType.Knight);
            }
            redUnitList.Add(unit);
        }
    }

    public void ResetUnitPositionAndCreate()
    {
        foreach(var obj in blueUnitList)
        {
            Destroy(obj);
        }

        foreach (var obj in redUnitList)
        {
            Destroy(obj);
        }

        blueUnitList = new List<GameObject>();
        redUnitList = new List<GameObject>();

        CreateUnit();
    }

    public bool CheckAllUnitPosition()
    {
        bool result = true;

        PlayerRole curRole = PlayManager.pm_instance.CheckCurRole();

        if(curRole == PlayerRole.Master)
        {
            foreach(var obj in blueUnitList)
            {
                Position temp = obj.GetComponent<UnitState>().status.GetPosition();
                if(temp.x == -5 || temp.y == -5)
                {
                    return false;
                }
            }
        }
        else if(curRole == PlayerRole.Participant)
        {
            foreach (var obj in redUnitList)
            {
                Position temp = obj.GetComponent<UnitState>().status.GetPosition();
                if (temp.x == -5 || temp.y == -5)
                {
                    return false;
                }
            }
        }

        return result;
    }

    public List<Position> GetBlueUnitPositionList()
    {
        List<Position> result = new List<Position>();
        foreach (var obj in blueUnitList)
        {
            Position tempPosition = obj.GetComponent<UnitState>().status.GetPosition();
            result.Add(tempPosition);
        }

        return result;
    }

    public void SetBlueUnitList(List<Position> list)
    {
        for(int i = 0; i < blueUnitList.Count; i++)
        {
            GameObject temp = blueUnitList[i];
            Position listPosition = list[i];

            temp.transform.position = new Vector3(listPosition.x, 0, listPosition.y * 1.5f);
            temp.GetComponent<UnitState>().status.SetPosition(listPosition.x, listPosition.y);
        }

        foreach (var obj in blueUnitList)
        {
            Vector3 currentRotation = obj.transform.eulerAngles;
            currentRotation.y += 180;
            obj.transform.eulerAngles = currentRotation;
        }

        foreach (var obj in redUnitList)
        {
            Vector3 currentRotation = obj.transform.eulerAngles;
            currentRotation.y += 180;
            obj.transform.eulerAngles = currentRotation;
        }
    }

    public List <Position> GetRedUnitPositionList()
    {
        List<Position> result = new List<Position>();
        foreach (var obj in redUnitList)
        {
            Position tempPosition = obj.GetComponent<UnitState>().status.GetPosition();
            result.Add(tempPosition);
        }

        return result;
    }

    public void SetRedUnitList(List<Position> list)
    {
        for (int i = 0; i < redUnitList.Count; i++)
        {
            GameObject temp = redUnitList[i];
            Position listPosition = list[i];

            temp.transform.position = new Vector3(listPosition.x, 0, listPosition.y * 1.5f);
            temp.GetComponent<UnitState>().status.SetPosition(listPosition.x, listPosition.y);
        }

        foreach (var obj in blueUnitList)
        {
            Vector3 currentRotation = obj.transform.eulerAngles;
            currentRotation.y += 180;
            obj.transform.eulerAngles = currentRotation;
        }

        foreach (var obj in redUnitList)
        {
            Vector3 currentRotation = obj.transform.eulerAngles;
            currentRotation.y += 180;
            obj.transform.eulerAngles = currentRotation;
        }
    }

    public GameObject CheckObjByPosition(Position position)
    {
        // 아이템 확인
        foreach (var obj in itemList)
        {
            
        }

        // 블루 유닛 확인
        foreach (var obj in blueUnitList)
        {
            Position temp = obj.GetComponent<UnitState>().status.GetPosition();
            if(temp.x == position.x && temp.y == position.y)
            {
                return obj;
            }
        }

        // 레드 유닛 확인
        foreach (var obj in redUnitList)
        {
            Position temp = obj.GetComponent<UnitState>().status.GetPosition();
            if (temp.x == position.x && temp.y == position.y)
            {
                return obj;
            }
        }

        return null;
    }

    public GameObject CheckObjByName(string name)
    {
        // 아이템 확인
        foreach (var obj in itemList)
        {

        }

        // 블루 유닛 확인
        foreach (var obj in blueUnitList)
        {
            string temp = obj.GetComponent<UnitState>().status.GetName();
            if (temp == name)
            {
                return obj;
            }
        }

        // 레드 유닛 확인
        foreach (var obj in redUnitList)
        {
            string temp = obj.GetComponent<UnitState>().status.GetName();
            if (temp == name)
            {
                return obj;
            }
        }

        return null;
    }

    public void MoveUnit(string name, Position targetPosition, string targetName)
    {
        if(targetName == null)
        {
            // 이름이 없다는건 빈 칸이라는 의미
            // 움직일 대상 유닛
            GameObject obj = CheckObjByName(name);
            obj.GetComponent<UnitState>().MovePosition(targetPosition);
        }
    }
}

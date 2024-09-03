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
                    if (temp.x == position.x && temp.y == position.z)
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

        // 좌표상 y는 길이가 1.5배이므로 변경 계산
        if (!CheckLapping(positionRight))
        {
            instantiatePosition = new Vector3(positionRight.x, 0, positionRight.y * 1.5f);
            instanceRight.SetActive(true);
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
            instantiatePosition = new Vector3(positionLeft.x, 0, positionLeft.y * 1.5f);
            instanceLeft.SetActive(true);
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
            instantiatePosition = new Vector3(positionUp.x, 0, positionUp.y * 1.5f);
            instanceUp.SetActive(true);
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
            instantiatePosition = new Vector3(positionDown.x, 0, positionDown.y * 1.5f);
            instanceDown.SetActive(true);
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
        GameObject blueKnight1 = Resources.Load<GameObject>("Prefab/Unit/Solider_Blue");
        GameObject blueKnight2 = Resources.Load<GameObject>("Prefab/Unit/Solider_Blue");
        GameObject blueKnight3 = Resources.Load<GameObject>("Prefab/Unit/Solider_Blue");
        GameObject blueKnight4 = Resources.Load<GameObject>("Prefab/Unit/Solider_Blue");

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

        for (int i = 0; i < bluelist.Count; i++)
        {
            // 포지션은 놓아진 뒤에 정해짐
            //bluelist[i].GetComponent<UnitState>().status.SetPosition();
            GameObject unit = Instantiate(bluelist[i], bluePositions[i], Quaternion.identity);
            if(i < 4)
            {
                unit.GetComponent<UnitState>().status = new Status(PlayerDefine.Blue, UnitType.Royalty);
            }
            else
            {
                unit.GetComponent<UnitState>().status = new Status(PlayerDefine.Blue, UnitType.Knight);
            }

            blueUnitList.Add(unit);
        }

        GameObject redRoyalty1 = Resources.Load<GameObject>("Prefab/Unit/Prince_Red");
        GameObject redRoyalty2 = Resources.Load<GameObject>("Prefab/Unit/King_Red");
        GameObject redRoyalty3 = Resources.Load<GameObject>("Prefab/Unit/Princess_Red");
        GameObject redRoyalty4 = Resources.Load<GameObject>("Prefab/Unit/Queen_Red");
        GameObject redKnight1 = Resources.Load<GameObject>("Prefab/Unit/Solider_Red");
        GameObject redKnight2 = Resources.Load<GameObject>("Prefab/Unit/Solider_Red");
        GameObject redKnight3 = Resources.Load<GameObject>("Prefab/Unit/Solider_Red");
        GameObject redKnight4 = Resources.Load<GameObject>("Prefab/Unit/Solider_Red");
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

        for (int i = 0; i < redlist.Count; i++)
        {
            // 포지션은 놓아진 뒤에 정해짐
            //bluelist[i].GetComponent<UnitState>().status.SetPosition();
            Quaternion rotation = Quaternion.Euler(0, 180, 0);
            GameObject unit = Instantiate(redlist[i], redPositions[i], rotation);
            if (i < 4)
            {
                unit.GetComponent<UnitState>().status = new Status(PlayerDefine.Red, UnitType.Royalty);
            }
            else
            {
                unit.GetComponent<UnitState>().status = new Status(PlayerDefine.Red, UnitType.Knight);
            }
            redUnitList.Add(unit);
        }
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;
using UnityEngine.SceneManagement;

public class PlayManager : MonoBehaviour
{
    public static PlayManager pm_instance;
    private bool isConnect = true;
    public Transform[] spawnPoints;
    public bool isMyturn = false;
    public PlayPhase curPhase;
    public PlayerRole curRole;
    public GameObject cameraObject;

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

    public void CreatePlayer()
    {
        Debug.Log("플레이어 생성");
        //GameObject playerTemp = PhotonNetwork.Instantiate("Prefab/Player", Vector3.one, Quaternion.identity, 0);

        spawnPoints = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        Vector3 pos = spawnPoints[PhotonNetwork.CurrentRoom.PlayerCount].position;
        Quaternion rot = spawnPoints[PhotonNetwork.CurrentRoom.PlayerCount].rotation;

        GameObject playerTemp = PhotonNetwork.Instantiate("Prefab/Player", pos, rot, 0);
    }

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
            cameraObject.transform.position = new Vector3(0f, 13f, 2.5f);

            Quaternion currentRotation = cameraObject.transform.rotation;
            Vector3 eulerRotation = currentRotation.eulerAngles;
            eulerRotation.z = 180f;
            cameraObject.transform.rotation = Quaternion.Euler(eulerRotation);
        }
    }


}

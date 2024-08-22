using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm_instance;

    private void Awake()
    {
        // 씬 전환시 GameManager 유지
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Destroy(gameObject); // 중복된 인스턴스는 파괴
        }
        else
        {
            DontDestroyOnLoad(gameObject); // 그렇지 않으면 유지
        }
    }

    void Start()
    {
        if (gm_instance == null)
        {
            gm_instance = this;
        }
        else if (gm_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }
}

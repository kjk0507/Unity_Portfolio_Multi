using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCameraSetting : MonoBehaviour
{
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        float widthScale = Screen.width / 1080.0f; // 기준 해상도에 따른 비율
        float heightScale = Screen.height / 1920.0f;
        float scale = Mathf.Min(widthScale, heightScale);
        transform.localScale = initialScale * scale;
    }
}

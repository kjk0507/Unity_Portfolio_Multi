using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public TextMeshProUGUI ui_roomNumText;
    public TextMeshProUGUI ui_masterNickNameText;
    public TextMeshProUGUI ui_roomNameText;

    public string roomNum;
    public string roomName;
    public string masterNickName;
    public string participantNickName;

    private void Update()
    {
        if(roomNum != null)
        {
            ui_roomNumText.text = roomNum;
        }

        if(roomName != null)
        {
            ui_roomNameText.text = roomName;
        }

        if(masterNickName != null)
        {
            ui_masterNickNameText.text= masterNickName;
        }
    }

    public void CreatRoomInfo(int num, string room, string masterNick)
    {
        roomNum = "No.0" + num.ToString();
        roomName = "방 이름 : " + room;
        masterNickName = "방장 : " + masterNick;
    }
}

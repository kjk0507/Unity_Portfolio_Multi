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
            ui_roomNumText.text = "No.0" + roomNum;
        }

        if(roomName != null)
        {
            ui_roomNameText.text = "방 이름 : " + roomName;
        }

        if(masterNickName != null)
        {
            ui_masterNickNameText.text= "방장 : " + masterNickName;
        }
    }

    public void CreatRoomInfo(int num, string room, string masterNick)
    {
        roomNum = num.ToString();
        roomName = room;
        masterNickName = masterNick;
    }

    public void ClickRoom()
    {
        PhotonManager.pm_instance.ClickRoom(roomName);
    }
}

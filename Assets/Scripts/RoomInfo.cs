using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public TextMeshProUGUI ui_roomNameText;
    public TextMeshProUGUI ui_masterNickNameText;
    public TextMeshProUGUI ui_participantNickNameText;

    public string roomName;
    public string masterNickName;
    public string participantNickName;

    void Update()
    {
        
    }

    public void ChangeRoomInfo(string room, string masterNick)
    {        
        roomName = room;
        masterNickName = masterNick;
    }

    public void CheckParticipant(string name)
    {
        participantNickName = name;
    }

    public void CheckRefresh()
    {
        if (roomName != null)
        {
            ui_roomNameText.text = "방 이름 : " + roomName;
        }

        if (masterNickName != null)
        {
            ui_masterNickNameText.text = "방장 : " + masterNickName;
        }

        if (participantNickName != null)
        {
            ui_participantNickNameText.text = "참가자 : " + participantNickName;
        }
        else
        {
            ui_participantNickNameText.text = "참가자 : 없음";
        }
    }
}

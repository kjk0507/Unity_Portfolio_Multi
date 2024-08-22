using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnumStruct
{
    public enum UIState
    {
        Title,
        Lobby,
        Room,
        Play,
    }

    public enum PlayerRole
    {
        Master, // 방장
        Participant, // 참가자
        Spectoator, // 관람자
    }
}

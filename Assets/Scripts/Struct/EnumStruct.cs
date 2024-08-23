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
        Default, // 기본
        Master, // 방장
        Participant, // 참가자
        Spectoator, // 관람자
    }

    public enum PlayPhase
    {
        UnitPlacement, // 유닛 배치 -> 처음 한번만 실행
        TurnStart, // 턴 시작
        UsingItem, // 아이템 사용 페이즈
        MoveUnit,  // 유닛 이동
        TurnEnd,   // 턴 종료
    }
}

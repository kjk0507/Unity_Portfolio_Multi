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
        None,
        UnitPlacement, // 유닛 배치 -> 처음 한번만 실행
        TurnStart, // 턴 시작 -> 아이템 사용 및 유닛 이동 가능
        TurnEnd,   // 턴 종료
    }

    public enum UnitType
    {
        Royalty,
        Knight,
    }

    public enum PlayerDefine
    {
        Blue,
        Red,
        Item,
        Goal,
    }

    public enum UnitAnimation
    {
        Idle, // 기본
        Walk, // 이동
        Attack,  // 공격
        Death,   // 사망
    }

    public enum ItemNameNum
    {
        None,
        ChangePosition,
        MoveEnemy,
        DoubleMove,
        Uncover,
        Bomb,
    }
}

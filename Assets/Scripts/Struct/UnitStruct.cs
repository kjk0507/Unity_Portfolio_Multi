using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;
using UnityEngine.UIElements;

namespace UnitStruct
{
    public class Status
    {
        public PlayerDefine define; // 아군 / 적군
        public UnitType type; // 왕족 / 기사
        public bool isHide;  // true : 정체 안 밝혀짐 / false : 정체 밝혀짐
        public int positionX;
        public int positionY;

        public Status(PlayerDefine playerDefine, UnitType unitType)
        {
            define = playerDefine;
            type = unitType;
            isHide = true;
            positionX = -5;
            positionY = -5;
        }

        public Position GetPosition()
        {
            return new Position(positionX, positionY);
        }

        public void SetPosition(int x, int y)
        {
            this.positionX = x;
            this.positionY = y;
        }

        public void ChangeHide()
        {
            isHide = true;
        }

        public PlayerDefine GetDefine()
        {
            return define;
        }

        public void SetDefine(PlayerDefine def)
        {
            this.define = def;
        }

        public UnitType GetUnitType()
        {
            return type;
        }

        public void SetUnitType(UnitType ut)
        {
            this.type = ut;
        }
    }

    public class Position
    {
        public int x;
        public int y;
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}

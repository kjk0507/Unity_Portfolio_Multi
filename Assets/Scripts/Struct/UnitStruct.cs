using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;
using UnityEngine.UIElements;

namespace UnitStruct
{
    public class Status
    {
        public string name;
        public PlayerDefine define; // 아군 / 적군
        public UnitType type; // 왕족 / 기사
        public bool isHide;  // true : 정체 안 밝혀짐 / false : 정체 밝혀짐
        public bool isReflection;
        public int positionX;
        public int positionY;

        public Status(string name, PlayerDefine playerDefine, UnitType unitType)
        {
            this.name = name;
            define = playerDefine;
            type = unitType;
            isHide = true;
            positionX = -5;
            positionY = -5;
        }

        public Status(int x, int y)
        {
            positionX = x;
            positionY = y;
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

        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
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

        public bool GetIsReflection()
        {
            return isReflection;
        }

        public void SetIsReflection(bool type)
        {
            isReflection = type;
        }
    }

    [System.Serializable]
    public class Position
    {
        public int x;
        public int y;
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Position a, Position b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;

            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a == b);
        }
    }
}

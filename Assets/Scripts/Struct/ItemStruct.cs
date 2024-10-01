using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemStruct
{
    public class ItemInfo
    {
        public string name;
        public string itemInfo;

        public string GetName()
        {
            return name;
        }

        public void SetName(string str)
        {
            name = str;
        }

        public string GetItemInfo()
        {
            return itemInfo;
        }

        public void SetItemInfo(string str)
        {
            itemInfo = str;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumStruct;

public class ItemCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject unit = other.gameObject;

        if(unit.GetComponent<UnitState>() != null )
        {
            PlayerDefine define = unit.GetComponent<UnitState>().status.GetDefine();

            if(define == PlayerDefine.Blue || define == PlayerDefine.Red)
            {
                if (PlayManager.pm_instance.isMyturn)
                {
                    PlayManager.pm_instance.AddItem();
                }
                
                Destroy(gameObject);
            }

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 拾取武器、子弹

public class PickUpItems : MonoBehaviour
{
    [SerializeField]
    public ContainerItem itemInfo;

    // Player碰到物品
    void OnTriggerEnter(Collider other) {
        Debug.LogFormat("other.tag:{0}",other.tag);
        if (other.tag != "Player")
        {
            return;
        }    
        PickUp(other);
    }

    void PickUp(Collider collider){
        OnPickUp(collider);
    }

    public virtual void OnPickUp(Collider collider){

    }
    
}
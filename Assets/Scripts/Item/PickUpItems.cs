using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 拾取武器、子弹

public class PickUpItems : MonoBehaviour
{
    public ContainerItem itemInfo;

    // Player碰到物品
    void OnTriggerEnter(Collider other) {
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
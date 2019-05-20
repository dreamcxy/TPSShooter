using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp:PickUpItems{
    public override void OnPickUp(Collider collider){
        base.OnPickUp(collider);
        Container container = collider.gameObject.GetComponentInChildren<Container>();
        container.Add(itemInfo);       
        Destroy(gameObject);
    }
}
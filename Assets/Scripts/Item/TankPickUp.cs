using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPickUp : PickUpItems
{

    public override void OnPickUp(Collider collider){
        base.OnPickUp(collider);
        Debug.Log("按键从坦克中出来");

        collider.transform.Find("soldier").gameObject.SetActive(false);
        // GameObject.Find("CameraRig").GetComponent<CamRig>().enabled = false;

        collider.GetComponent<CharacterStates>().isInTank = true;
        this.GetComponent<TankController>().enabled = true;
        // Camera.main.enabled = false;
        

    }





}

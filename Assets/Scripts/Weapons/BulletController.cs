using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class BulletController : MonoBehaviour
{
    public float bulletDamage = 10f;
    public float bulletSpeed = 2f;
    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("on shoot....");
        if (other.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
            other.gameObject.GetComponent<CharacterStates>().health -= bulletDamage;
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        this.transform.Translate(Vector3.right * bulletSpeed * Time.deltaTime);
    }
}
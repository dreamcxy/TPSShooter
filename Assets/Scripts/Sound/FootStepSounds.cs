using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextureType
{
    public string groundTag;
    public AudioClip[] footStepSound;
}


public class FootStepSounds : MonoBehaviour
{
    public TextureType[] terrianTypes;
    public AudioSource audioSource;
    SoundController sc;

    private void Start()
    {
        sc = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        audioSource = this.gameObject.GetComponent<AudioSource>(); 
    }

    public void PlayFootStepSound()
    {
        RaycastHit hit;
        Vector3 start = transform.position + transform.up;
        Vector3 dir = Vector3.down;
        if (Physics.Raycast(start, dir, out hit, 2f))
        {
            Debug.Log(hit.collider);
            if (hit.collider)
            {
                
                PlaySurfaceSound(hit.collider.gameObject);
            }
        }
    }

    public void PlaySurfaceSound(GameObject groundSurface)
    {
        if (audioSource == null)
        {
            Debug.LogError("no audio source...");
            return;
        }
        if (sc == null)
        {
            Debug.LogError("no sound Controller...");
            return;
        }
        if (terrianTypes.Length > 0)
        {
            foreach (TextureType type in terrianTypes)
            {
                if (type.footStepSound.Length == 0)
                {
                    return;
                }
                if (groundSurface.tag == type.groundTag)
                {
                    sc.InstantiateClip(transform.position + Vector3.down, type.footStepSound[Random.Range(0, type.footStepSound.Length)]);
                }
            }
        }
    }
}
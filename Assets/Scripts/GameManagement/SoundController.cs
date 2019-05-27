using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundController : MonoBehaviour
{
    public void PlaySound(AudioSource audioSource, AudioClip audioClip, bool randomizePith = false, float randomPitchMin = 1, float randomPitchMax = 1)
    {
        audioSource.clip = audioClip;
        if (randomizePith == true)
        {
            audioSource.pitch = Random.Range(randomPitchMin, randomPitchMax);
        }
        audioSource.Play();
    }


    public void InstantiateClip(Vector3 pos, AudioClip clip, float time = 2f, bool randomizePith = false, float randomPitchMin = 1, float randomPitchMax = 1)
    {
        GameObject clone = new GameObject("one shot audio");
        clone.transform.position = pos;
        AudioSource audio = clone.AddComponent<AudioSource>();
        audio.maxDistance = 10f;
        audio.spatialBlend = 1;
        audio.clip = clip;
        audio.Play();
        Destroy(clone, time);
    }
}
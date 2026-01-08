using UnityEngine;

public class CountdownAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip fight;


    public void PlayFightSound()
    {
        audioSource.PlayOneShot(fight);
    }
}
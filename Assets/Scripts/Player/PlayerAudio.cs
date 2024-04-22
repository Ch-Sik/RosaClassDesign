using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip footstep;
    [SerializeField] private AudioClip climb;
    [SerializeField] private AudioClip whoosh;

    // Start is called before the first frame update
    void Start()
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            Debug.LogError("AudioSource를 찾을 수 없음!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayFootstepSound()
    {
        audioSource.PlayOneShot(footstep);
    }

    public void PlayClimbSound()
    {
        audioSource.PlayOneShot(climb);
    }

    public void PlayWhooshSound()
    {
        audioSource.PlayOneShot(whoosh);
    }
}

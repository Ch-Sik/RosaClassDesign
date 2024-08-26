using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [InfoBox("enabled 될 때 자동으로 효과음하나를 재생하는 컴포넌트.\nAudioManager의 볼륨에 영향받음.")]

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clip;

    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.Instance;
        audioManager.OnAudioVolumeChanged += OnVolumeChanged;
        
        if(audioSource != null )
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void OnVolumeChanged(AudioType type, float value)
    {
        if (type != AudioType.SFX) return;
        audioSource.volume = value;
    }

    private void OnEnable()
    {
        audioSource.PlayOneShot(clip);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{
    [InfoBox("BGM 재생용 컴포넌트.\nAudioManager의 볼륨에 영향받음.")]

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clip;

    [SerializeField] bool playAutomatically;

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

        if(playAutomatically)
        {
            PlayBGM();
        }
    }

    void OnVolumeChanged(AudioType type, float value)
    {
        if (type != AudioType.SFX) return;
        audioSource.volume = value;
    }

    public void PlayBGM()
    {
        // TODO: 맵 데이터에 BGM 저장하여 그거 읽어오도록 구현하면 좋을 것 같은데
        // TODO: BGM 교체시에 fade 효과 구현

        audioSource.clip = clip;
        audioSource.Play();
    }
}

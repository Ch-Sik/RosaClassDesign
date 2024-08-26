using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class AudioManager : MonoBehaviour
{
    [InfoBox("SFXplayer, BGMplayer 등을 통해 전반적인 사운드 볼륨을 조정하는 컴포넌트." +
        "\n씬에 딱 하나면 있으면 됨. GameManager에다가 부착하면 될 듯")]
    [SerializeField, ReadOnly, Range(0, 1f)] float[] _volumes;
    public static AudioManager Instance;

    public delegate void AudioVolumeChangeEvent(AudioType type, float value);
    public AudioVolumeChangeEvent OnAudioVolumeChanged;

    private void Awake()
    {
        Instance = this;
        _volumes = new float[Enum.GetNames(typeof(AudioType)).Length];
    }

    private void Start()
    {
        // TODO: 저장된 설정값 읽어와서 볼륨 바꾸도록 수정
        for(int i=0; i< _volumes.Length; i++)
        {
            // 처음에 일괄 볼륨 설정하도록 하여
            // OnAudioVolumeChanged를 참조하는 모든 오디오 관련 컴포넌트들의 볼륨 일괄 조정
            SetSoundVolume((AudioType)i, 1);
        }
    }

    [Button("BGM 볼륨 수정 테스트(Runtime Only)")]
    void SetSoundVolume(AudioType audioType, float value)
    {
        _volumes[(int)audioType] = value;
        OnAudioVolumeChanged(audioType, value);
    }
}

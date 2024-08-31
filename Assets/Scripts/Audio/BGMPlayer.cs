using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{
    [InfoBox("BGM 재생용 컴포넌트.\nAudioManager의 볼륨에 영향받음.")]

    // fade 효과를 위해 오디오 소스를 2개 사용
    [SerializeField] AudioSource audioSourceA;
    [SerializeField] AudioSource audioSourceB;

    [SerializeField] AudioClip startBGMclip;
    [SerializeField] bool playAutomatically;
    [SerializeField] float fadeDuration = 0.3f;

    AudioManager audioManager;
    Sequence fadeSeq = null;
    bool readyToFade = true;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.Instance;
        audioManager.OnAudioVolumeChanged += OnVolumeChanged;

        Debug.Assert(audioSourceA != null);
        Debug.Assert(audioSourceB != null);

        if(playAutomatically)
        {
            PlayBGM(startBGMclip);
        }
    }

    void OnVolumeChanged(AudioType type, float value)
    {
        if (type != AudioType.SFX) return;
        audioSourceA.volume = value;
    }

    public void PlayBGM(AudioClip newClip)
    {
        // TODO: 맵 데이터에 BGM 저장하여 그거 읽어오도록 구현하면 좋을 것 같은데
        if (audioSourceA.isPlaying)
        {
            if (audioSourceA.clip != newClip)
                SwitchBGM(newClip);
            else
                Debug.LogWarning("이미 재생중인 BGM임!");
        }
        else
        {
            audioSourceA.clip = newClip;
            audioSourceA.Play();
        }
    }

    [Button("브금 전환 테스트")]
    private void SwitchBGM(AudioClip newClip)
    {
        Debug.Log($"브금 전환: {audioSourceA.clip.name} → {newClip}");

        if(!readyToFade)
        {
            Debug.LogError("이미 BGM 전환 수행중임! 기다렸다가 다시 시도하세요");
            return;
        }
        readyToFade = false;

        audioSourceB.clip = newClip;
        audioSourceB.loop = true;

        fadeSeq = DOTween.Sequence()
            .Append(audioSourceA.DOFade(0, fadeDuration))
            .AppendCallback(() =>
            {
                audioSourceA.Stop();
                audioSourceB.Play();
            })
            .Append(audioSourceB.DOFade(1, fadeDuration))
            .OnComplete(() => {
                readyToFade = true;
                // 두 AudioSource의 참조를 교환
                AudioSource temp = audioSourceA;
                audioSourceA = audioSourceB;
                audioSourceB = temp;
            });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Com.LuisPedroFonseca.ProCamera2D;
using Sirenix.OdinInspector;

public class G_ImprisonDoor : GimmickSignalReceiver
{
    [SerializeField] private BoxCollider2D col;
    [SerializeField] private Transform doorSprite;
    [SerializeField] private float moveTime;
    [SerializeField] private float moveDelay = 1.5f;                                      //Procam2dCinem은 기본 1초의 Easing 타임을 ㅏㄱ짐.
    GameObject cam;
    ProCamera2DCinematics cinematics;
    public List<cinematicsSetting> cinematicsSettings = new List<cinematicsSetting>();

    bool isOpen = true;
    bool isClear = false;

    private void Start()
    {
        cam = Camera.main.gameObject;
        cinematics = cam.GetComponent<ProCamera2DCinematics>();
        col.enabled = false;
    }

    public void Init(bool activated)
    {
        if (activated)
        {
            col.enabled = false;
            doorSprite.localPosition = new Vector3(0, 2.5f, 0);
        }
    }

    [Button]
    public void Close()
    {
        if (!isOpen || isClear)
            return;

        Cinematic();
        Sequence sq = DOTween.Sequence()
            .AppendInterval(moveDelay)
            .Append(doorSprite.DOMoveY(-1.5f, moveTime * 0.6f).SetRelative(true))
            .AppendCallback(() =>
            {
                col.enabled = true;
                isOpen = false;
            })
            .Append(doorSprite.DOMoveY(-1f, moveTime * 0.4f).SetRelative(true));
    }

    [Button]
    public void Open()
    {
        if (isOpen)
            return;

        isClear = true;

        Cinematic();
        Sequence sq = DOTween.Sequence()
            .AppendInterval(moveDelay)
            .Append(doorSprite.DOMoveY(1.5f, moveTime * 0.6f).SetRelative(true))
            .AppendCallback(() =>
            {
                col.enabled = false;
                isOpen = true;
            })
            .Append(doorSprite.DOMoveY(1f, moveTime * 0.4f).SetRelative(true));
    }

    public void Cinematic()
    {
        if (cinematics != null)
        {
            while (cinematics.CinematicTargets.Count > 0)
            {
                cinematics.CinematicTargets.RemoveAt(0);
            }

            for (int i = 0; i < cinematicsSettings.Count; i++)
            {
                cinematics.AddCinematicTarget(transform, cinematicsSettings[i].easeInDur, cinematicsSettings[i].holdDur, cinematicsSettings[i].zoomAmount);
            }

            cinematics.Play();
            //추후에 이벤트 취급으로 하여 플레이어의 조작을 막을 필요가 있음
        }
    }

    public override void OnAct()
    {
        Debug.Log("신호 옴");
        Open();
    }

    public override void OffAct()
    {
    }

    public override void ImmediateOnAct()
    {
        if (isOpen)
            return;

        isClear = true;

        doorSprite.DOMoveY(2.5f, 0f).SetRelative(true);
        col.enabled = false;
        isOpen = true;
    }

    public override void ImmediateOffAct()
    {
    }
}

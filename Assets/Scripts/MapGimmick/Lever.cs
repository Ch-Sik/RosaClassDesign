using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct cinematicsSetting
{
    public float easeInDur;
    public float holdDur;
    public float zoomAmount;
}
public class Lever : MonoBehaviour
{
    [SerializeField] bool isUseMulti = false;
    [SerializeField] bool leverActivated = false;
    [SerializeField] bool isUseValue = false;
    [SerializeField] List<LeverGear> leverGears = new List<LeverGear>();
    [SerializeField] List<int> gearsValue = new List<int>();
    [SerializeField] Transform leverHandle;
    [SerializeField] GameObject cam;
    [SerializeField] ProCamera2DCinematics cinematics;

    public bool isCinematics = false;
    public List<cinematicsSetting> cinematicsSettings = new List<cinematicsSetting>();
    private void Start()
    {
        cam = Camera.main.gameObject;
        cinematics = cam.GetComponent<ProCamera2DCinematics>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit");
        if (leverActivated) return;        // 루틴 중복 실행 방지
        if(!isUseMulti) leverActivated = true;

        if(leverGears == null)
        {
            Debug.LogWarning($"레버로 인해 작동할 장치가 지정되어있지 않음!");
            return;
        }

        if(cinematics != null && isCinematics)
        {
            while(cinematics.CinematicTargets.Count > 0)
            {
                cinematics.CinematicTargets.RemoveAt(0);
            }
            cinematics.AddCinematicTarget(gameObject.transform, cinematicsSettings[0].easeInDur, cinematicsSettings[0].holdDur, cinematicsSettings[0].zoomAmount);
            for(int i = 1; i < cinematicsSettings.Count; i++)
            {
                cinematics.AddCinematicTarget(leverGears[0].gameObject.transform, cinematicsSettings[i].easeInDur, cinematicsSettings[i].holdDur, cinematicsSettings[i].zoomAmount);
            }
            

            cinematics.Play();
            //추후에 이벤트 취급으로 하여 플레이어의 조작을 막을 필요가 있음
        }


        StartCoroutine(ActivateLever());
    }

    private IEnumerator ActivateLever()
    {
        DOTween.Sequence()
            .Append(leverHandle.DORotate(new Vector3(0, 0, -45), 0.4f, RotateMode.FastBeyond360))
            .AppendCallback(() => 
            {
                for(int i = 0; i< leverGears.Count; i++)
                {
                    if (isUseValue) leverGears[i].Activate(gearsValue[i]);
                    else leverGears[i].Activate();
                }
            });
        yield return 0;
    }
}

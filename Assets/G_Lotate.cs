using Com.LuisPedroFonseca.ProCamera2D;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Lotate : GimmickSignalReceiver
{
    enum LotateType
    {
        OnOff,
        Once,
        Reverse
    }

    [FoldoutGroup("선택"), SerializeField, Tooltip("Cinematic은 OnOff와 Reverse에서만 동작함")]
    [ShowIf("ShowCinematicOption")] bool useAutoCinematic = true;
    [FoldoutGroup("선택"), SerializeField] LotateType type;

    public bool isLotate = true;
    public float lotationTime = 4f;
    public float lotationAmount = 360f;
    public int lotationDir = 1; // 정방향 1, 역방향 -1
    public bool isReverse = false;
    private bool canMove = false;
    public bool isArrive = false;

    GameObject cam;
    ProCamera2DCinematics cinematics;
    public List<cinematicsSetting> cinematicsSettings = new List<cinematicsSetting>();

    // Update is called once per frame
    void Update()
    {
        /*
        if(isLotate)
        {
            
        }
        */
        
    }

    public override void OffAct()
    {
        switch (type)
        {
            case LotateType.OnOff:
                canMove = false;
                break;
            case LotateType.Once:
                canMove = true;
                break;
            case LotateType.Reverse:
                Reverse();
                break;
        }
    }

    public override void OnAct()
    {
        switch (type)
        {
            case LotateType.OnOff:
                canMove = true;
                if (useAutoCinematic)
                    Cinematic();
                break;
            case LotateType.Once:
                canMove = true;
                break;
            case LotateType.Reverse:
                Reverse();
                if (useAutoCinematic)
                    Cinematic();
                break;
        }
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
    public void Reverse()
    {
        lotationDir *= -1;

    }

    private void Start()
    {
        cam = Camera.main.gameObject;
        cinematics = cam.GetComponent<ProCamera2DCinematics>();

        if (type == LotateType.OnOff ||
            type == LotateType.Once )
            canMove = false;
        else
        {
            canMove = true;
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;
        if (type == LotateType.Once && isArrive) return;
        if (canMove)
        {
            float rotationSpeed = 360f / lotationTime;

            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
}

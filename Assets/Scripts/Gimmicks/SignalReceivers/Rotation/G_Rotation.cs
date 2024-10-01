using Com.LuisPedroFonseca.ProCamera2D;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Rotation : GimmickSignalReceiver
{
    enum RotateType
    {
        OnOff,
        Once,
        Reverse
    }

    [FoldoutGroup("선택"), SerializeField, Tooltip("Cinematic은 OnOff와 Reverse에서만 동작함")]
    [ShowIf("ShowCinematicOption")] bool useAutoCinematic = true;
    [FoldoutGroup("선택"), SerializeField] RotateType type;

    public float rotationTime = 4f; // 1회 회전하는 데 걸리는 시간
    public float rotationAmount = 360f; // 0 ~ 360 도 단위
    public int rotationDir = 1; // 반시계방향 1, 시계방향 -1
    private bool canRotate = false;
    public bool isArrive = false;

    GameObject cam;
    ProCamera2DCinematics cinematics;
    public List<cinematicsSetting> cinematicsSettings = new List<cinematicsSetting>();

    bool ShowCinematicOption()
    {
        switch (type)
        {
            case RotateType.OnOff:
            case RotateType.Reverse:
                return true;
            case RotateType.Once:
                return false;
        }

        return false;
    }

    public override void OffAct()
    {
        switch (type)
        {
            case RotateType.OnOff:
                canRotate = false;
                break;
            case RotateType.Once:
                canRotate = true;
                break;
            case RotateType.Reverse:
                Reverse();
                break;
        }
    }

    public override void OnAct()
    {
        switch (type)
        {
            case RotateType.OnOff:
                canRotate = true;
                if (useAutoCinematic)
                    Cinematic();
                break;
            case RotateType.Once:
                canRotate = true;
                StartCoroutine(Stop());
                break;
            case RotateType.Reverse:
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
        rotationDir *= -1;

    }

    private void Start()
    {
        cam = Camera.main.gameObject;
        cinematics = cam.GetComponent<ProCamera2DCinematics>();

        if (type == RotateType.OnOff ||
            type == RotateType.Once)
            canRotate = false;
        else
        {
            canRotate = true;
        }
    }

    private void FixedUpdate()
    {
        if (!canRotate) return;
        if (type == RotateType.Once && isArrive) return;
        if (canRotate)
        {
            float rotationSpeed = rotationAmount / rotationTime;

            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime * rotationDir);
        }
    }

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(rotationTime);
        isArrive = true;
    }
}

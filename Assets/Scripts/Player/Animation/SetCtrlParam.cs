using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnyPortrait;

public class SetCtrlParam : MonoBehaviour
{
    public apPortrait portrait;

    Rect leftHalfScreen = new Rect(0, 0, 0.5f, 1f);
    Rect wholeScreen = new Rect(0, 0, 1f, 1f);

    private Vector2 headDirection = new Vector2(0, 0);
    private Vector2 eyeDirection = new Vector2(0, 0);
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 viewportPosition = Camera.main.ScreenToViewportPoint(mousePosition);

        // Viewport 좌표를 (-1, -1)에서 (1, 1) 범위로 매핑
        headDirection = MapToRange(viewportPosition, leftHalfScreen.min, leftHalfScreen.max, new Vector2(-1, -1), new Vector2(1, 1));
        eyeDirection = MapToRange(viewportPosition, wholeScreen.min, wholeScreen.max, new Vector2(-1, -1), new Vector2(1, 1));

        portrait.SetControlParamVector2("Head Direction", headDirection);
        portrait.SetControlParamVector2("Eye Direction", eyeDirection);
    }

    Vector2 MapToRange(Vector3 value, Vector3 fromMin, Vector3 fromMax, Vector2 toMin, Vector2 toMax)
    {
        Vector2 result;
        result.x = Mathf.Lerp(toMin.x, toMax.x, Mathf.InverseLerp(fromMin.x, fromMax.x, value.x));
        result.y = Mathf.Lerp(toMin.y, toMax.y, Mathf.InverseLerp(fromMin.y, fromMax.y, value.y));
        return result;
    }
}

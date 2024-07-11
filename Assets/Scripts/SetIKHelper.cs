using AnyPortrait;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetIKHelper : MonoBehaviour
{
    public apPortrait girl;
    public SpriteRenderer aimUI;
    public Animator girlAnimator;
    public float minAngle;
    public float maxAngle;

    private Vector3 _aimPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos_Screen = Input.mousePosition;
        if (Camera.main != null)
        {
            _aimPosition = Camera.main.ScreenToWorldPoint(mousePos_Screen);
            _aimPosition.z = 0;
            aimUI.transform.position = _aimPosition;
            aimUI.enabled = true;
            //Set Aim Helper Bone Position
            if (girl != null)
            {
                // Calculate the angle between the mouse position and girl position
                Vector2 girlToMouse = (Vector2)_aimPosition - (Vector2)girl.transform.position;
                float angle = Vector2.SignedAngle(Vector2.right, girlToMouse);
                Debug.Log(angle);

                // Check if the angle is within the desired range (minAngle to maxAngle)
                if (angle >= minAngle && angle <= maxAngle)
                {
                    // Set Aim Helper Bone Position only if the angle is within the desired range
                    girl.SetBonePosition("IKHelper", _aimPosition, Space.World);
                }
            }
            else
            {
                Debug.LogError("girl object is not initialized!");
            }
        }
        else
        {
            Debug.LogError("Camera.main is null!");
        }
    }
}

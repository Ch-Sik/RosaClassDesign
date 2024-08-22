using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Lotate : MonoBehaviour
{
    public bool isLotate = true;
    public float lotationTime = 4f;

    // Update is called once per frame
    void Update()
    {
        if(isLotate)
        {
            float rotationSpeed = 360f / lotationTime;

            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        
    }
}

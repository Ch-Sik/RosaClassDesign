using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Imp_Interaction : MonoBehaviour
{
    // 싱글톤
    private static Imp_Interaction instance;
    //public static InputManager Instance { get { return _instance; } }

    public static Imp_Interaction Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Imp_Interaction>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<Imp_Interaction>();
                }
            }
            return instance;
        }
    }

    public UnityEvent onInteraction;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(onInteraction != null)
                onInteraction.Invoke();
        }
    }
}

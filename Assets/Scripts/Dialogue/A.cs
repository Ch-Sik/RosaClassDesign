using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A : MonoBehaviour
{
    public Transform communicationStartTransform;
    public bool isGo = false;

    private void Start()
    {
        communicationStartTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isGo)
            return;

        if (Mathf.Abs(PlayerRef.Instance.transform.position.x - communicationStartTransform.position.x) > 0.3f)
        {
            int direction = PlayerRef.Instance.transform.position.x - communicationStartTransform.position.x < 0 ? 1 : -1;
            PlayerRef.Instance.movement.Walk(Vector2.one * direction);
            PlayerRef.Instance.animation.anim.SetBool("isWalking", true);
        }
        else
        {
            PlayerRef.Instance.animation.anim.SetBool("isWalking", false);
            PlayerRef.Instance.movement.Walk(Vector2.zero);
            PlayerRef.Instance.movement.LookAt2D(transform.position);
            isGo = false;
        }
    }

    [Button]
    public void StartCommunication()
    {
        isGo = true;

        InputManager.Instance.SetMoveInputState(PlayerMoveState.NO_MOVE);
        InputManager.Instance.SetUiInputState(UiState.DIALOG);
    }

    public void ChooseCommunication()
    {
        
    }
}

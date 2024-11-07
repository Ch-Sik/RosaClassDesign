using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCommunicationInteraction : MonoBehaviour
{
    public float delay = 1f;
    public CommunicationDecision decision;

    [SerializeField] private Transform communicationStartTransform;
    private bool isGo = false;
    [HideInInspector] public int ID;

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

            Invoke("ChooseCommunication", delay);
        }
    }

    [Button]
    public void StartCommunication()
    {
        //ID가 없는 경우 리턴
        if (!CommunicationManager.Instance.HaveCommunicationID(ID))
            return;

        isGo = true;
        ID = decision.GetID();

        InputManager.Instance.SetMoveInputState(PlayerMoveState.NO_MOVE);
        InputManager.Instance.SetUiInputState(UiState.DIALOG);
    }

    public void ChooseCommunication()
    {
        //ID에 따라 수행
        CommunicationManager.Instance.StartCommunication(ID);
    }
}

[Serializable]
public class CommunicationDecision
{
    //첫 대화인지 파악
    [HideInInspector] public bool isFirst = true;
    public int initialID;
    public int iterativeID;
    public List<CommunicationDecisionNode> flagedID = new List<CommunicationDecisionNode>();

    public int GetID()
    {
        //첫 대화
        if (isFirst)
        {
            isFirst = false;
            return initialID;
        }

        //반복 대화
        return iterativeID;
    }
}

[Serializable]
public class CommunicationDecisionNode
{
    public bool IsAvailable()
    {
        for (int i = 0; i < flagBooleans.Count; i++)
        {
            //
            //만약 조건이 하나라도 거짓이라면 false
        }

        return true;
    }

    public List<FlagBoolean> flagBooleans = new List<FlagBoolean>();
    public int ID;
}

[Serializable]
public class FlagBoolean
{
    string flag;
    bool value;
}
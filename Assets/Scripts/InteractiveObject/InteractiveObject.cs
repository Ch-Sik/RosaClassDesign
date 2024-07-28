using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour
{
    public bool showGizmos = false;

    public bool canUse = true;

    Collider2D col;
    public UnityEvent function;
    public SpriteRenderer interactiveKeySprite;

    private void Start()
    {
        interactiveKeySprite.enabled = false;

        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public void ActivateInteraction()
    {
        this.col.enabled = true;
    }

    public void InactiveInteraction()
    {
        this.col.enabled = false;
    }

    public void RemoveEvent()
    {
        PlayerRef.Instance.Controller.interactFunction.RemoveAllListeners();
    }

    public void SetEvent()
    {
        if (!canUse)
            return;

        PlayerRef.Instance.Controller.interactFunction.RemoveAllListeners();
        PlayerRef.Instance.Controller.interactFunction.AddListener(() => function.Invoke());
    }

    private void OnActive() 
    {
        interactiveKeySprite.enabled = true;
    }

    private void OnInactive()
    {
        interactiveKeySprite.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canUse)
            return;

        if (collision.tag != "Player")
            return;

        Debug.Log("이벤트 할당");

        OnActive();
        SetEvent();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!canUse)
            return;

        if (collision.tag != "Player")
            return;

        Debug.Log("이벤트 수거");

        OnInactive();
        RemoveEvent();
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;
        Gizmos.color = Color.green;
    }
}

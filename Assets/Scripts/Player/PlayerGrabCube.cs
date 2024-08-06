using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabCube : MonoBehaviour
{
    [ShowInInspector] private bool onGrab = false;          //그랩을 하는 중인가?
    [ShowInInspector] private bool canGrab = true;          //그랩을 할 수 있는가?

    [SerializeField] private GameObject grabDisplay;
    [SerializeField] private GameObject unGrabDisplay;

    [SerializeField] private Transform curCube;
    [SerializeField] private BoxCollider2D virtualCol;

    public void Grab()
    {
        if (!canGrab)
            return;
        canGrab = false;

        curCube.GetComponent<G_Cube>().Grab(transform);

        onGrab = true;
        ActiveUnGrabDisplay();

        curCube.SetParent(transform);

        virtualCol.gameObject.SetActive(true);

        PlayerRef.Instance.Move.isGrabCube = true;

        SetUnGrabEvent();
    }

    public void UnGrab()
    {
        if (!onGrab)
            return;

        curCube.GetComponent<G_Cube>().UnGrab();

        canGrab = true;
        onGrab = false;

        PlayerRef.Instance.Move.isGrabCube = false;
        
        InactiveAllDisplay();
        curCube.SetParent(null);

        virtualCol.gameObject.SetActive(false);

        PlayerRef.Instance.Controller.ResetInteraction();
    }

    public bool CanGrab()
    {
        PlayerMovement con1 = PlayerRef.Instance.Move;
        PlayerController con2 = PlayerRef.Instance.Controller;

        if (con2.currentActionState != PlayerActionState.DEFAULT ||
            con2.currentMoveState != PlayerMoveState.DEFAULT)
            return false;

        if (con1.isGrabCube ||
            !con1.isGrounded ||
            con1.isDoingSuperDash ||
            con1.isJumpingUp ||
            con1.isDoingAttack ||
            con1.isWallClimbing)
            return false;

        return true;
    }

    /*
     *             
            con1.isDoingSuperDash ||
            con1.isJumpingUp ||
            con1.isDoingAttack ||
            con1.isWallClimbing)
    */
    #region Trigger Events
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Cube"))
            return;
        if (onGrab)
            return;
        if (!canGrab)
            return;
        if (!CanGrab())
            return;

        canGrab = true;
        ActiveGrabDisplay();

        SetGrabEvent();

        curCube = collision.transform;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Cube"))
            return;

        if (onGrab)
            return;

        PlayerRef.Instance.Controller.ResetInteraction(true);
        InactiveAllDisplay();

        curCube = null;
    }
    #endregion

    #region Interaction
    public void SetGrabEvent()
    {
        PlayerRef.Instance.Controller.ResetInteraction(true);
        PlayerRef.Instance.Controller.SetInteraction(Grab, true);
    }

    public void SetUnGrabEvent()
    {
        PlayerRef.Instance.Controller.ResetInteraction(true);
        PlayerRef.Instance.Controller.SetInteraction(UnGrab, true);
    }
    #endregion

    #region Display Funcs
    public void InactiveAllDisplay()
    {
        grabDisplay.SetActive(false);
        unGrabDisplay.SetActive(false);
    }

    public void ActiveGrabDisplay()
    {
        grabDisplay.SetActive(true);
        unGrabDisplay.SetActive(false);
    }

    public void ActiveUnGrabDisplay()
    {
        grabDisplay.SetActive(false);
        unGrabDisplay.SetActive(true);
    }
    #endregion
}

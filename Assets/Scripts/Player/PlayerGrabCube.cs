using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabCube : MonoBehaviour
{
    [ShowInInspector] private bool GrabbingCube = false;          //그랩을 하는 중인가?
    [ShowInInspector] private bool canGrab = true;          //그랩을 할 수 있는가?

    [SerializeField] private GameObject grabDisplay;
    [SerializeField] private GameObject unGrabDisplay;

    [SerializeField] private Transform curCube;
    [SerializeField] private BoxCollider2D virtualCol;

    public bool isHoldingCube { get { return GrabbingCube; } }

    public void Grab()
    {
        if (!canGrab)
            return;
        canGrab = false;

        curCube.GetComponent<G_Cube>().Grab(transform);

        GrabbingCube = true;
        ActiveUnGrabDisplay();

        curCube.SetParent(transform);

        virtualCol.gameObject.SetActive(true);

        PlayerRef.Instance.movement.isGrabCube = true;

        SetUnGrabEvent();
    }

    public void UnGrab()
    {
        if (!GrabbingCube)
            return;

        curCube.GetComponent<G_Cube>().UnGrab();

        canGrab = true;
        GrabbingCube = false;

        PlayerRef.Instance.movement.isGrabCube = false;

        InactiveAllDisplay();
        curCube.SetParent(null);

        virtualCol.gameObject.SetActive(false);

        PlayerRef.Instance.controller.ResetInteraction();
    }

    public void UnGrab(bool isRespawn = false)
    {
        if (!GrabbingCube)
            return;

        curCube.GetComponent<G_Cube>().UnGrab(isRespawn);

        canGrab = true;
        GrabbingCube = false;

        PlayerRef.Instance.movement.isGrabCube = false;
        
        InactiveAllDisplay();
        curCube.SetParent(null);

        virtualCol.gameObject.SetActive(false);

        PlayerRef.Instance.controller.ResetInteraction();
    }

    public bool CanGrab()
    {
        PlayerMovement con1 = PlayerRef.Instance.movement;
        PlayerController con2 = PlayerRef.Instance.controller;

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
        if (GrabbingCube)
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

        if (GrabbingCube)
            return;

        PlayerRef.Instance.controller.ResetInteraction(true);
        InactiveAllDisplay();

        curCube = null;
    }
    #endregion

    #region Interaction
    public void SetGrabEvent()
    {
        PlayerRef.Instance.controller.ResetInteraction(true);
        PlayerRef.Instance.controller.SetInteraction(Grab, true);
    }

    public void SetUnGrabEvent()
    {
        PlayerRef.Instance.controller.ResetInteraction(true);
        PlayerRef.Instance.controller.SetInteraction(UnGrab, true);
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

using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class G_FallingObject : MonoBehaviour
{
    public float fallingLength = 3.0f;
    public float fallingTime = 3.0f;
    public float beforeFallingDelay = 1.0f;
    public float afterFallingDelay = 1.0f;
    [OnValueChanged("ChangeObjectSize")] public Vector2 sizeOfFallingObject = Vector2.one;

    [Space]
    public GameObject platform;
    public Transform respawnPoint;
    public EdgeCollider2D top1;
    public EdgeCollider2D top2;
    public Transform damageObject;
    public Transform body;

    private float fixedFallingLength;

    public void ChangeObjectSize()
    {
        Vector2[] newPointsTop1 = new Vector2[2];
        newPointsTop1[0] = new Vector2(-1 * sizeOfFallingObject.x / 2, sizeOfFallingObject.y / 2);
        newPointsTop1[1] = new Vector2(1 * sizeOfFallingObject.x / 2, sizeOfFallingObject.y / 2);
        top1.points = newPointsTop1;
        Vector2[] newPointsTop2 = new Vector2[2];
        newPointsTop2[0] = new Vector2(-1 * sizeOfFallingObject.x / 2, sizeOfFallingObject.y / 2);
        newPointsTop2[1] = new Vector2(1 * sizeOfFallingObject.x / 2, sizeOfFallingObject.y / 2);
        top2.points = newPointsTop2;
        body.localScale = sizeOfFallingObject;
        damageObject.localScale = new Vector3(sizeOfFallingObject.x - sizeOfFallingObject.x / 10, sizeOfFallingObject.y / 2);
        damageObject.localPosition = new Vector3(0, -1 * sizeOfFallingObject.y / 4);
    }

    private void Start()
    {
        fixedFallingLength = fallingLength;
        damageObject.GetComponent<G_FallingObjectDamage>().respawnPoint = respawnPoint.position;
        Fall();
    }

    public void Fall()
    {
        Sequence falling = DOTween.Sequence()
        .AppendInterval(beforeFallingDelay)
        .Append(platform.transform.DOLocalMoveY(-1 * fixedFallingLength, fallingTime).SetRelative(true))
        .AppendInterval(afterFallingDelay)
        .Append(platform.transform.DOLocalMoveY(fixedFallingLength, fallingTime).SetRelative(true))
        .SetLoops(-1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x,
                                                        transform.position.y - fallingLength));
        Gizmos.DrawWireCube(new Vector3(transform.position.x,
                                        transform.position.y - fallingLength),
                            sizeOfFallingObject);
    }
}

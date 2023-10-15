using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColliderSetter : MonoBehaviour
{
    [Header("자동 콜라이더 세팅용 옵션")]
    [SerializeField] bool up;
    [SerializeField] bool down;
    [SerializeField] bool left;
    [SerializeField] bool right;
    float upY, downY, leftX, rightX;

    void CalculateCoordinates()
    {
        // 스프라이트를 기반으로 콜라이더 위치 계산
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        upY = transform.lossyScale.y * sprite.size.y * 0.5f;
        downY = -upY;
        rightX = transform.lossyScale.x * sprite.size.x * 0.5f;
        leftX = -rightX;
    }


    [ContextMenu("This/Add collider Up")]
    public void AddColliderUp()
    {
        CalculateCoordinates();
        SetColliderUp();
    }

    [ContextMenu("This/Add collider Down")]
    public void AddColliderDown()
    {
        CalculateCoordinates();
        SetColliderDown();
    }

    [ContextMenu("This/Add collider Left")]
    public void AddColliderLeft()
    {
        CalculateCoordinates();
        SetColliderLeft();
    }

    [ContextMenu("This/Add collider Right")]
    public void AddColliderRight()
    {
        CalculateCoordinates();
        SetColliderRight();
    }

    [ContextMenu("This/Add collider 4way")]
    public void AddCollider4()
    {
        CalculateCoordinates();
        SetColliderUp();
        SetColliderDown();
        SetColliderLeft();
        SetColliderRight();
    }

    [ContextMenu("This/Add collider auto")]
    public void AddColliderAsSet()
    {
        CalculateCoordinates();
        if (up) SetColliderUp();
        if (down) SetColliderDown();
        if(left) SetColliderLeft();
        if(right) SetColliderRight();
    }

    [ContextMenu("Every Object with ColliderSetter/Add collider 4way")]
    public void AllAddCollider4()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] all = scene.GetRootGameObjects();
        foreach (GameObject go in all)
        {
            ColliderSetter cs = go.GetComponentInChildren<ColliderSetter>();
            if (cs != null)
            {
                cs.AddCollider4();
            }
        }
    }

    [ContextMenu("Every Object with ColliderSetter/Add collider auto")]
    public void AllAddColliderAsSet()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] all = scene.GetRootGameObjects();
        foreach (GameObject go in all)
        {
            ColliderSetter[] colliderSetters = go.GetComponentsInChildren<ColliderSetter>();
            if (colliderSetters.Length != 0)
            {
                foreach(var cs in colliderSetters)
                    cs.AddColliderAsSet();
            }
        }
    }

    public void SetColliderUp()
    {
        // 위쪽
        GameObject go_up = new GameObject("up");
        go_up.transform.parent = transform;
        go_up.transform.localPosition = Vector3.zero;
        go_up.layer = LayerMask.NameToLayer("Ground");
        EdgeCollider2D col_up = go_up.AddComponent<EdgeCollider2D>();
        Vector2[] points = col_up.points;
        points[0] = new Vector2(leftX, upY);
        points[1] = new Vector2(rightX, upY);
        col_up.points = points;
    }

    public void SetColliderDown()
    {
        // 아래쪽
        GameObject go_down = new GameObject("down");
        go_down.transform.parent = transform;
        go_down.transform.localPosition = Vector3.zero;
        go_down.layer = LayerMask.NameToLayer("Ceil");
        EdgeCollider2D col_down = go_down.AddComponent<EdgeCollider2D>();
        Vector2[] points = col_down.points;
        points[0] = new Vector2(leftX, downY);
        points[1] = new Vector2(rightX, downY);
        col_down.points = points;
    }

    public void SetColliderLeft()
    {
        // 왼쪽
        GameObject go_left = new GameObject("left");
        go_left.transform.parent = transform;
        go_left.transform.localPosition = Vector3.zero;
        go_left.layer = LayerMask.NameToLayer("Wall_left");
        EdgeCollider2D col_left = go_left.AddComponent<EdgeCollider2D>();
        Vector2[] points = col_left.points;
        points[0] = new Vector2(leftX, upY);
        points[1] = new Vector2(leftX, downY);
        col_left.points = points;
    }

    public void SetColliderRight()
    {
        // 오른쪽
        GameObject go_right = new GameObject("right");
        go_right.transform.parent = transform;
        go_right.transform.localPosition = Vector3.zero;
        go_right.layer = LayerMask.NameToLayer("Wall_right");
        EdgeCollider2D col_right = go_right.AddComponent<EdgeCollider2D>();
        Vector2[] points = col_right.points;
        points[0] = new Vector2(rightX, upY);
        points[1] = new Vector2(rightX, downY);
        col_right.points = points;
    }

    // 콜라이더 세팅 후 정리용
    [ContextMenu("Remove Component/in this gameObject")]
    public void removeThis()
    {
        DestroyImmediate(this);
    }

    [ContextMenu("Remove Component/in all gameObject in scene")]
    public void removeAll()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] all = scene.GetRootGameObjects();
        foreach (GameObject go in all)
        {
            ColliderSetter[] colliderSetters = go.GetComponentsInChildren<ColliderSetter>();
            if (colliderSetters.Length != 0)
            {
                foreach (var cs in colliderSetters)
                    cs.removeThis();
            }
        }
    }
}

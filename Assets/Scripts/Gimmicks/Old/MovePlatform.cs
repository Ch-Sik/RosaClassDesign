using UnityEngine;
using System.Collections.Generic;

public class MovePlatform : MonoBehaviour
{
    public List<Transform> points;
    public float speed = 2f;
    private int currentIndex = 0; 

    void Update()
    {
        if (points.Count == 0) return;

        Transform targetPoint = points[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentIndex = (currentIndex + 1) % points.Count;
        }
    }

    public void HandleChildTriggerEnter(Transform other)
    {
        other.SetParent(transform);
    }

    public void HandleChildTriggerExit(Transform other)
    {
        if(Application.isPlaying)
        {
            if (other.parent == transform)
            {
                other.SetParent(null);
            }
        }
        
    }
}
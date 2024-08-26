using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int minCount;

    List<GameObject> pool = new List<GameObject>();
    int curIndex;

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<minCount; i++)
        {
            GameObject instance = Instantiate(prefab);
            instance.transform.SetParent(transform);
            instance.SetActive(false);
            pool.Add(instance);
        }
    }
    
    public GameObject GetNextFromPool()
    {
        int checkedObjectCount = 0;
        while(checkedObjectCount <= pool.Count)
        {
            curIndex = (curIndex + 1) % pool.Count;
            checkedObjectCount++;

            if (pool[curIndex].activeSelf)
                continue;
            else
                return pool[curIndex];
        }
        GameObject newOne = Instantiate(prefab);
        pool.Add(newOne);
        return newOne;
    }

}

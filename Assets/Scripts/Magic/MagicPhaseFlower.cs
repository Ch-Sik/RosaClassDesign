using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MagicPhaseFlower : MonoBehaviour
{
    [SerializeField]
    float radius = 3f;

    List<PhaseBlock> triggeringBlocks = new List<PhaseBlock>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("PhaseBlock"))
        {
            PhaseBlock block = collision.GetComponent<PhaseBlock>();
            block.PhaseFlowerCount++;
            triggeringBlocks.Add(block);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("PhaseBlock"))
        {
            PhaseBlock block = collision.GetComponent<PhaseBlock>();
            block.PhaseFlowerCount--;
            triggeringBlocks.Remove(block);
        }
    }

    // TODO: Destroy/OnDestroy 대신 별도의 함수를 사용하여 '사라지는 연출' 구현
    private void OnDestroy()
    {
        foreach(PhaseBlock block in triggeringBlocks)
        {
            if(block.gameObject != null)    // Scene unload 시에 nullReferenceError 방지
                block.PhaseFlowerCount--;
        }
    }
}

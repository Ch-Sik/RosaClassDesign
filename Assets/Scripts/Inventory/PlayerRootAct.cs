using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 획득 클래스
/// </summary>

public class PlayerRootAct : MonoBehaviour
{
    public bool showGizmo = false;            //기즈모 표기 유무
    public float rootRadius = 1;              //획득 범위
    public LayerMask dropitemLayer;           //드랍 아이템 인식 레이어

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))                //추후 Input변경 요망
            ItemRoot();
    }

    public void ItemRoot()  //아이템 획득 함수
    {
        //범위 내 아이템을 모두 인식하여 배열로 받아옴.
        Collider2D[] items =
        Physics2D.OverlapCircleAll(transform.position, rootRadius, dropitemLayer);

        //범위 내 아이템이 없다면, 리턴
        if (items.Length == 0)
            return;

        //있다면, 아이템들의 거리를 비교하여 최단 거리에 있는 아이템의 인덱스를 추출
        float min = 100;
        int index = 0;

        for (int i = 0; i < items.Length; i++)
        {
            float distance = Vector2.Distance(items[i].transform.position, transform.position);
            if (distance < min)
            {
                min = distance;
                index = i;
            }
        }

        //추출된 아이템의 데이터를 받아옴
        DropItem dropItem = items[index].GetComponent<DropItem>();

        //추출된 아이템 코드의 아이템을 개수만큼 추가함.
        InventoryController.Instance.AddItem(dropItem.code, dropItem.quantity);
        //이벤트 발동
        OnItemRoot(dropItem.code, dropItem.quantity);
        //해당 아이템의 삭제
        Destroy(items[index].gameObject);
    }

    public void OnItemRoot(ItemCode code, int quantity)
    { 
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmo == false)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rootRadius);
    }
}

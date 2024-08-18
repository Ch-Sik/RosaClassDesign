using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMyosotis : MonoBehaviour
{
    private void Start()
    {
        Init();
        WarpPlayer();
    }

    private void Init()
    {
        // 일단 구색 갖추기용으로 Init을 선언해두기는 했는데 뭐 딱히 초기화할 게 없는 것 같은데... 지울까

    }

    private void WarpPlayer()
    {
        //Sequence doWarp = DOTween.Sequence()
        //    .Append(PlayerRef.Instance.transform.DOMove(transform.position, 0.05f));
        PlayerRef.Instance.transform.position = this.transform.position;
        PlayerRef.Instance.movement.moveVector = Vector2.zero;
    }
}

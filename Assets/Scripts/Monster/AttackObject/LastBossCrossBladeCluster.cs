using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBossCrossBladeCluster : MonoBehaviour
{
    [SerializeField] MonsterProjectile projRight;
    [SerializeField] MonsterProjectile projUp;
    [SerializeField] MonsterProjectile projLeft;
    [SerializeField] MonsterProjectile projDown;

    public void LaunchProjectiles(float projSpeed)
    {
        // 클러스터 본체는 자탄 발사 후 사라져야 하니 자탄들은 자식에서 떼어냄
        projRight.transform.parent = null;
        projUp.transform.parent = null;
        projLeft.transform.parent = null;
        projDown.transform.parent = null;

        // 자탄 발사
        projRight.InitProjectile(Vector2.right * projSpeed);
        projUp.InitProjectile(Vector2.up * projSpeed);
        projLeft.InitProjectile(Vector2.left * projSpeed);
        projDown.InitProjectile(Vector2.down * projSpeed);

        // 클러스터 본체는 소멸
        DoDestroy();
    }

    void DoDestroy()
    {
        // TODO: 사라지는 연출 추가
        Destroy(gameObject, 1f);
    }
}

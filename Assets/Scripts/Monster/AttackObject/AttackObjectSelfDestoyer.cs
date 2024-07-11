using UnityEngine;
using Sirenix.OdinInspector;

public class AttackObjectSelfDestoyer : MonoBehaviour
{
    [InfoBox("오브젝트의 최대 수명을 설정.\n" +
        "별도의 삭제 조건을 구현하기 어렵거나,\n" +
        "만일의 경우를 대비해 메모리 누수를 막을 때 사용")]
    [SerializeField]
    private float maxLifetime = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, maxLifetime);
    }
}

using UnityEngine;
using Panda;

// Melee와 똑같은 기능인데 함수명만 다른 클래스를 구성하기 위한 간단한 상속.
public class Task_A_Boss1QuickMelee : Task_A_Melee
{
    [Task]
    private void Melee_Quick()
    {
        ExecuteAttack();
    }
}

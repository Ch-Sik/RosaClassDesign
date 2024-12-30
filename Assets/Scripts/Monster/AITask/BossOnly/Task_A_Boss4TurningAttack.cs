using Panda;

public class Task_A_Boss4TurningAttack : Task_A_Melee
{
    [Task]
    private void TurningAttack()
    {
        ExecuteAttack();
    }

    protected override void OnStartupBegin()
    {
        // 적 바라보는 코드 무효화
        // GameObject enemy;
        // blackboard.TryGet(BBK.Enemy, out enemy);
        // Debug.Assert(enemy != null, "근접공격 패턴: 적을 찾을 수 없음!");

        // LookAt2D(enemy.transform.position);

        attackInstance.Init();
    }

    protected override void OnRecoveryBegin()
    {
        base.OnRecoveryBegin();
        Flip();
    }
}

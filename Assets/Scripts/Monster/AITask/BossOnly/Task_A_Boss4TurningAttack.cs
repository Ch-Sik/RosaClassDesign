using Panda;

public class Task_A_Boss4TurningAttack : Task_A_Melee
{
    [Task]
    private void TurningAttack()
    {
        ExecuteAttack();
    }

    protected override void OnRecoveryBegin()
    {
        base.OnRecoveryBegin();
        Flip();
    }
}

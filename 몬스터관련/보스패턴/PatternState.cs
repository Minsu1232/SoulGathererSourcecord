using UnityEngine;
using static IMonsterState;
/// <summary>
/// ������ ���� ���¸� �����ϴ� Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 3�� '���� �ý��� ���� �� Ȯ�� ���' > 1�� '���� ���� �ý���'
/// 
/// �ֿ� ���:
/// - ���� ���� ���� �� ����
/// - ���� ���� ��ȯ ����
/// - ���� �Ϸ� �� ������ ���� ��ȯ
/// - ���� ���� �� �ٸ� ���·��� ��ȯ ����
/// </remarks>
public class PatternState : MonsterBaseState
{
    private readonly BossPattern pattern;
    private readonly BossData bossData;

    public PatternState(BossAI owner, BossPattern pattern) : base(owner)
    {
        this.pattern = pattern;
        this.bossData = (owner.GetStatus().GetMonsterClass() as BossMonster)?.GetBossData();
    }

    public override void Enter()
    {
        Debug.Log($"Entering Pattern State: {pattern.GetType().Name}");
        pattern.Attack(transform, player, monsterClass);
    }

    public override void Execute()
    {
        //// ������ ���� ������ üũ
        //if (!pattern.IsAttacking)
        //{
           
        //    owner.ChangeState(MonsterStateType.Idle);
        //}
    }

    public override void Exit()
    {
        if (owner.GetAttackStrategy() is BasePhysicalAttackStrategy baseAttack)
        {
            baseAttack.UpdateLastAttackTime();  // ���� �ʵ� ���� ��� �޼��� ���
            Debug.Log("���� ����");
        }
        pattern.StopAttack();
        pattern.Cleanup();
    }

    public override bool CanTransition()
    {
        // ���� ���� �߿��� �ٸ� ���·� ��ȯ ���ϰ�
        Debug.Log("curretn :" + pattern.IsAttacking);
        return !pattern.IsAttacking;
    }
}
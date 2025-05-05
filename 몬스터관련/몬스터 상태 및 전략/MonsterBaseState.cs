using UnityEngine;
/// <summary>
/// ��� ���� ������ ���� ����� ������ �߻� Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 2�� '���� ������ ����' > '���� ����(FSM)'
/// 
/// �ֿ� ���:
/// - ���� ���°� �������� ����ϴ� ���� �� ��ƿ��Ƽ �޼��� ����
/// - CreatureAI�� ���� ��ü ���� ���� ���� ����
/// - �÷��̾���� �Ÿ� ��� �� ���� ���� ����
/// </remarks>

public abstract class MonsterBaseState : IMonsterState
{
    protected CreatureAI owner;
    protected ICreatureStatus status;       // MonsterStatus ��� �������̽� ���
    protected IMonsterClass monsterClass;   // MonsterClass ��� �������̽� ���
    protected Transform transform;       // ������ Transform
    protected Transform player;          // �÷��̾� Transform

    public MonsterBaseState(CreatureAI owner)  // �����ڵ� CreatureAI�� ����
    {
        this.owner = owner;
        this.status = owner.GetStatus();
        this.monsterClass = status.GetMonsterClass();
        this.transform = owner.transform;
        this.player = GameInitializer.Instance.GetPlayerClass().playerTransform;
    }

    public virtual void Enter()
    {
        // ���� ���� �� �⺻ ����
    }

    public virtual void Execute()
    {
        // ���� ������Ʈ �� �⺻ ����
    }

    public virtual void Exit()
    {
        // ���� ���� �� �⺻ ����
    }

    public virtual bool CanTransition()
    {
        // �⺻������ ���� ��ȯ ����
        return true;
    }

    // ��ƿ��Ƽ �޼����
    protected float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }

    protected bool IsInRange(float range)
    {
        return GetDistanceToPlayer() <= range;
    }

    protected Vector3 GetDirectionToPlayer()
    {
        return (player.position - transform.position).normalized;
    }
}
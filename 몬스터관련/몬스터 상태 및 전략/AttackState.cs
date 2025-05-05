using static IMonsterState;
using UnityEngine;
/// <summary>
/// ������ ���� ���¸� �����ϴ� Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 2�� '���� ������ ����' > '���� ����(FSM)'
/// 
/// �ֿ� ���:
/// - �پ��� ���� ����(Strategy) ����ü�� ����
/// - �Ϲ� ���Ϳ� ���� ���� �� ������ ��Ƽ ���� ���� ����
/// - ���� �ִϸ��̼� Ʈ���� ���� �� ���� ���� ó��
public class AttackState : MonsterBaseState
{
    private readonly IAttackStrategy attackStrategy;
    private Animator animator;
    private bool attackStarted = false;
    private string currentAnimTrigger;  // ���� ��� ���� �ִϸ��̼� Ʈ���� ����
    private bool isTransitioning = false;
    public AttackState(CreatureAI owner, IAttackStrategy strategy) : base(owner)
    {
        attackStrategy = strategy;
        animator = owner.GetComponent<Animator>();

        // BossMultiAttackStrategy �̺�Ʈ ����
        if (strategy is BossMultiAttackStrategy multiStrategy) // �������� or ��Ƽ���������̳ʿ�
        {
            multiStrategy.OnStrategyStateChanged += () =>
            {
                string triggerName = attackStrategy.GetAnimationTriggerName();
                animator.SetTrigger(triggerName);
                Debug.Log("����" + triggerName) ;
                // ���� �غ� ���� �Ķ���� ����             
            };
        }
        else if(strategy is ChargeAttackStrategy chargeStrategy) // �Ϲݸ��Ϳ�
        {
            chargeStrategy.OnChargeStateChanged += () =>
            {

                string triggerName = attackStrategy.GetAnimationTriggerName();
                animator.SetTrigger(triggerName);
            };
        }
    }

    public override void Enter()
    {
        if (!attackStarted && !isTransitioning)
        {
            attackStrategy.StartAttack();
            attackStarted = true;
            attackStrategy.Attack(transform, player, monsterClass);
            currentAnimTrigger = attackStrategy.GetAnimationTriggerName();
            animator.SetTrigger(currentAnimTrigger);

            Debug.Log(attackStrategy.ToString());
        }
  
    }
    public override void Execute()
    {
        // ����� �α� �߰�
       
        if (attackStrategy is ChargeAttackStrategy chargeStrategy) // �Ϲݸ��Ϳ�
        {
            chargeStrategy.UpdateCharge(transform);
        }
        // BossMultiAttackStrategy�� ���� ������Ʈ
   else if (attackStrategy is BossMultiAttackStrategy multiStrategy) // ������
        {
            multiStrategy.UpdateStrategy(transform);
        }
        // ������ �Ϸ�Ǿ��ų� Ÿ�Ӿƿ��� ���
        if (!attackStrategy.IsAttacking)
        {
            owner.ChangeState(MonsterStateType.Idle);
            return;
        }


        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // �ּ� �Ÿ� ���� (�� ���� ���� ũ�⿡ ���� ����)
        float minDistance = 3.0f;

        // �ʹ� ������ ������ ȸ������ ����
        if (distanceToPlayer < minDistance)
            return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (directionToPlayer != Vector3.zero)
        {
            // Y�ุ ȸ���ϵ��� ����
            Vector3 flatDirection = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
            if (flatDirection.magnitude > 0.01f) // ���� ���Ͱ� �ʹ� ���� ������ Ȯ��
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }


    public override void Exit()
    {
        
        isTransitioning = true;
        attackStrategy.StopAttack();
        animator.ResetTrigger(currentAnimTrigger);
        attackStarted = false;
        isTransitioning = false;
    }

    public override bool CanTransition()
    {
        return !attackStrategy.IsAttacking && !isTransitioning;
    }
}
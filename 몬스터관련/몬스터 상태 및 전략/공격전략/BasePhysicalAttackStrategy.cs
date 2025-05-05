
using System;
using UnityEngine;
/// <summary>
/// ���� ���� �������� ���� ����� ���� �⺻ Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 2�� '���� ������ ����' > '���� ����(Strategy)'
/// 
/// �ֿ� ���:
/// - ���� ���� ���� �� ��ٿ� ó��
/// - ������ ��� �� ����
/// - ��ǥ���� ���� ȸ�� ó��
/// - ���� ���� ���� �Ǵ� ����
/// </remarks>
public abstract class BasePhysicalAttackStrategy : IAttackStrategy
{
    protected bool isAttacking;
    protected float lastAttackTime;
    protected bool isAttackAnimation;


    public virtual bool IsAttacking => isAttacking;
    public float GetLastAttackTime => lastAttackTime;
    public abstract PhysicalAttackType AttackType { get; }

    public virtual string GetAnimationTriggerName() => $"Attack_{AttackType}";
    public virtual float GetAttackPowerMultiplier() => 1.0f;

    public virtual void StartAttack()
    {
        isAttackAnimation = true;
        isAttacking = true;

    }

    public virtual void StopAttack()
    {
        isAttacking = false;

    }

    public virtual void OnAttackAnimationEnd()
    {
        isAttackAnimation = false;
        isAttacking = false;
    }

    public virtual bool CanAttack(float distanceToTarget, IMonsterClass monsterData)
    {
        if (isAttacking) return false; //  ���� ���̸� ���� �Ұ�


        Debug.Log($"���� �� ��?? : {distanceToTarget <= monsterData.CurrentAttackRange} ���ݼӵ���? {Time.time >= lastAttackTime + monsterData.CurrentAttackSpeed} "
               );

        return distanceToTarget <= monsterData.CurrentAttackRange &&
               Time.time >= lastAttackTime + monsterData.CurrentAttackSpeed;
    }

    public virtual void ApplyDamage(IDamageable target, IMonsterClass monsterData)
    {
        int baseDamage = monsterData.CurrentAttackPower;
        float multiplier = GetAttackPowerMultiplier();

        if (target is PlayerClass playerTarget)
        {         
            float damageReceiveRate = playerTarget.GetStats().DamageReceiveRate;
            int finalDamage = Mathf.RoundToInt(baseDamage * multiplier * damageReceiveRate);
            target.TakeDamage(finalDamage);
        }
        else
        {
            int finalDamage = Mathf.RoundToInt(baseDamage * multiplier);
            target.TakeDamage(finalDamage);
        }
    }

    protected void FaceTarget(Transform transform, Transform target)
    {
        //if (!isAttackAnimation)
        //{
         
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (directionToTarget != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(directionToTarget);
            }
        //}
    }

    public abstract void Attack(Transform transform, Transform target, IMonsterClass monsterData);

    internal void UpdateLastAttackTime()
    {
        lastAttackTime = Time.time;
    }

    public void ResetAttackTime()
    {
        lastAttackTime = Time.time;
    }
}

using System;
using UnityEngine;
/// <summary>
/// 물리 공격 전략들의 공통 기능을 담은 기본 클래스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 2절 '적용 디자인 패턴' > '전략 패턴(Strategy)'
/// 
/// 주요 기능:
/// - 공격 상태 관리 및 쿨다운 처리
/// - 데미지 계산 및 적용
/// - 목표물을 향한 회전 처리
/// - 공격 가능 여부 판단 로직
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
        if (isAttacking) return false; //  공격 중이면 공격 불가


        Debug.Log($"공격 할 수?? : {distanceToTarget <= monsterData.CurrentAttackRange} 공격속도가? {Time.time >= lastAttackTime + monsterData.CurrentAttackSpeed} "
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
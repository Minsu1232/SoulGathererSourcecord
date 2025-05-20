using UnityEngine;
/// <summary>
/// 모든 몬스터 상태의 공통 기능을 구현한 추상 클래스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 2절 '적용 디자인 패턴' > '상태 패턴(FSM)'
/// 
/// 주요 기능:
/// - 몬스터 상태가 공통으로 사용하는 참조 및 유틸리티 메서드 제공
/// - CreatureAI와 상태 객체 간의 참조 관계 설정
/// - 플레이어와의 거리 계산 및 방향 벡터 제공
/// </remarks>

public abstract class MonsterBaseState : IMonsterState
{
    protected CreatureAI owner;
    protected ICreatureStatus status;       // MonsterStatus 대신 인터페이스 사용
    protected IMonsterClass monsterClass;   // MonsterClass 대신 인터페이스 사용
    protected Transform transform;       // 몬스터의 Transform
    protected Transform player;          // 플레이어 Transform

    public MonsterBaseState(CreatureAI owner)  // 생성자도 CreatureAI로 변경
    {
        this.owner = owner;
        this.status = owner.GetStatus();
        this.monsterClass = status.GetMonsterClass();
        this.transform = owner.transform;
        this.player = GameInitializer.Instance.GetPlayerClass().playerTransform;
    }

    public virtual void Enter()
    {
        // 상태 진입 시 기본 동작
    }

    public virtual void Execute()
    {
        // 상태 업데이트 시 기본 동작
    }

    public virtual void Exit()
    {
        // 상태 종료 시 기본 동작
    }

    public virtual bool CanTransition()
    {
        // 기본적으로 상태 전환 가능
        return true;
    }

    // 유틸리티 메서드들
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
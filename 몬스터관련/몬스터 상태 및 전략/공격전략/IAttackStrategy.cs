// 물리 공격 타입 정의
using UnityEngine;
/// <summary>
/// 물리 공격 타입 정의
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조'
/// 
/// 주요 타입:
/// - Basic: 기본 공격 (몸통 박치기 등)
/// - Jump: 점프 공격
/// - Charge: 돌진 공격
/// - Combo: 연속 공격
/// - Spin: 회전 공격
public enum PhysicalAttackType
{
    Basic,      // 기본 공격 (애니메이션 움직임)
    Jump,       // 점프 공격
    Charge,     // 돌진 공격    
    Spin,       // 회전 공격
    // 추가 가능
}
/// <summary>
/// 확장된 공격 전략 인터페이스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 2절 '적용 디자인 패턴' > '전략 패턴(Strategy)'
/// 
/// 주요 기능:
/// - 다양한 물리 공격 타입 지원
/// - 공격 상태 관리
/// - 애니메이션 트리거 제공
/// - 공격력 배율 제어
/// - 데미지 적용 로직
/// </remarks>
// 확장된 공격 전략 인터페이스
public interface IAttackStrategy
{
    void Attack(Transform transform, Transform target, IMonsterClass monsterData);
    bool CanAttack(float distanceToTarget, IMonsterClass monsterData);
    void StartAttack();
    void StopAttack();
    void ApplyDamage(IDamageable target, IMonsterClass monsterData);
    bool IsAttacking { get; }
    float GetLastAttackTime { get; }
    void OnAttackAnimationEnd();
    void ResetAttackTime();
    // 새로 추가되는 속성들
    PhysicalAttackType AttackType { get; }
    string GetAnimationTriggerName();
    float GetAttackPowerMultiplier(); // 공격력 배율
}
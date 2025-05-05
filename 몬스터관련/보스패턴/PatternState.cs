using UnityEngine;
using static IMonsterState;
/// <summary>
/// 보스의 패턴 상태를 관리하는 클래스
/// </summary>
/// <remarks>
/// 기술문서 참조: 3장 '보스 시스템 구조 및 확장 방식' > 1절 '보스 고유 시스템'
/// 
/// 주요 기능:
/// - 보스 패턴 실행 및 관리
/// - 패턴 상태 전환 제어
/// - 패턴 완료 후 적절한 상태 전환
/// - 패턴 실행 중 다른 상태로의 전환 방지
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
        //// 패턴이 실행 중인지 체크
        //if (!pattern.IsAttacking)
        //{
           
        //    owner.ChangeState(MonsterStateType.Idle);
        //}
    }

    public override void Exit()
    {
        if (owner.GetAttackStrategy() is BasePhysicalAttackStrategy baseAttack)
        {
            baseAttack.UpdateLastAttackTime();  // 직접 필드 접근 대신 메서드 사용
            Debug.Log("지금 나감");
        }
        pattern.StopAttack();
        pattern.Cleanup();
    }

    public override bool CanTransition()
    {
        // 패턴 실행 중에는 다른 상태로 전환 못하게
        Debug.Log("curretn :" + pattern.IsAttacking);
        return !pattern.IsAttacking;
    }
}
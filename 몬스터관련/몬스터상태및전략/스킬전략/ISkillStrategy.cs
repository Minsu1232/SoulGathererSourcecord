using UnityEngine;
/// <summary>
/// 몬스터 스킬 전략을 정의하는 인터페이스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 2절 '적용 디자인 패턴' > '전략 패턴(Strategy)'
/// 
/// 주요 기능:
/// - 스킬 사용 가능 여부 판단
/// - 스킬 효과와의 연동
/// - 스킬 시작 및 업데이트 로직
/// - 스킬 쿨다운 및 범위 관리
/// </remarks>
public interface ISkillStrategy
{
    void Initialize(ISkillEffect skillEffect);
    void StartSkill(Transform transform, Transform target, IMonsterClass monsterData);
    void UpdateSkill(Transform transform, Transform target, IMonsterClass monsterData);
    
    bool IsSkillComplete { get; }
    bool CanUseSkill(float distanceToTarget, IMonsterClass monsterData);
    bool IsUsingSkill { get; }
    float GetLastSkillTime { get; }

    float SkillRange { get; set; }
}
// ICreatureAI.cs

using static IMonsterState;
/// <summary>
/// 모든 생물체 AI의 공통 인터페이스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조'
/// 
/// 주요 기능:
/// - 몬스터 상태 전환 기능 정의
/// - 현재 상태 조회 기능
/// - 데미지 처리 로직 정의
/// - 상태 정보 조회 인터페이스
public interface ICreatureAI
{
    //상태변화
    void ChangeState(MonsterStateType newState);
    //현재상태 Get
    IMonsterState GetCurrentState();
    //피격
    void OnDamaged(int damage);
    //Status로 몬스터 타입 판별
    ICreatureStatus GetStatus();
}
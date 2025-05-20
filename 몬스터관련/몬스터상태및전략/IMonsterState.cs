/// <summary>
/// 몬스터 상태 인터페이스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 2절 '적용 디자인 패턴' > '상태 패턴(FSM)'
/// 
/// 주요 기능:
/// - 몬스터의 다양한 상태 타입 정의
/// - 상태 진입, 실행, 종료 및 전환 가능 여부를 결정하는 메서드 정의
/// </remarks>
public interface IMonsterState
{
    public enum MonsterStateType
    {
        Spawn,
        Idle,
        Move,
        Attack,
        Skill,
        Hit,
        Groggy,
        PhaseTransition,
        Gimmick,
        Pattern,
        Die
    }
    void Enter();
    void Execute();
    void Exit();
    bool CanTransition(); // 현재 상태에서 전환 가능한지
}
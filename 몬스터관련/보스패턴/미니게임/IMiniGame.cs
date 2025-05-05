/// <summary>
/// 미니게임 결과와 타입 정의
/// </summary>
public enum MiniGameResult
{
    Perfect,
    Good,
    Miss,
    Cancel
}

public enum MiniGameType
{
    None,
    Dodge,
    Parry,
    QuickTime
}
/// <summary>
/// 미니게임 기본 인터페이스
/// </summary>
/// <remarks>
/// 기술문서 참조: 3장 '보스 시스템 구조 및 확장 방식' > 1절 '보스 고유 시스템' > '패턴'
/// 
/// 주요 기능:
/// - 미니게임 초기화 및 실행
/// - 난이도 설정
/// - 미니게임 결과 반환
/// - 다양한 미니게임 구현체 지원
/// </remarks>
// 기본 인터페이스
public interface IMiniGame
{
    MiniGameType Type { get; }
    bool IsComplete { get; }
    void Initialize(float difficulty);
    void Start();
    void Update();
    void Cancel();
    MiniGameResult GetResult();
}
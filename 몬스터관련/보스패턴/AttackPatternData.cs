using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 보스의 공격 패턴 데이터를 정의하는 클래스
/// </summary>
/// <remarks>
/// 기술문서 참조: 3장 '보스 시스템 구조 및 확장 방식' > 1절 '보스 고유 시스템'
/// 
/// 주요 기능:
/// - 패턴 구성 단계 및 실행 조건 정의
/// - 패턴별 쿨다운 및 가중치 설정
/// - 미니게임 성공 요구사항 관리
/// - 패턴 난이도 설정 및 조정
/// </remarks>
[System.Serializable]

[System.Serializable]
public class AttackPatternData
{
    [Header("Pattern Settings")]
    public string patternName;
    public int patternIndex;
    public BossPatternType patternType;  // 추가
    public List<AttackStepData> steps = new List<AttackStepData>();
    public float patternWeight = 1.0f;
    public int phaseNumber;

    [Header("Timing")]
    public float patternCooldown;
    public float warningDuration;

    [Header("Requirements")]
    public float healthThresholdMin;  // 이 체력% 이상일 때만 사용 가능
    public float healthThresholdMax;  // 이 체력% 이하일 때만 사용 가능

    [Header("Effects")]
    public GameObject patternStartEffect;
    public GameObject patternEndEffect;
    public string warningMessage;

    [Header("Mini Game Success Requirements")]  // 새로 추가한 부분
    public bool isDisabled = false;            // 패턴 비활성화 여부
    public int requiredSuccessCount = 3;       // 필요한 성공 횟수
    public int currentSuccessCount = 0;        // 현재 성공 횟수

    [Header("Difficulty Settings")]
    public float baseDifficulty = 1f;          // 기본 난이도
    public float maxDifficulty = 3f;           // 최대 난이도
    public float difficultyIncreaseStep = 0.5f;  // 성공당 증가량

    // 런타임에서 관리될 현재 난이도 (BossMonster에서 관리)
    public float currentDifficulty;

}

public enum BossPatternType
{
    None,
    BasicToJump,
    JumpToBasic,
    ChargeToJump,
    SpinToJump,
    // 필요한 패턴 타입들 추가
}
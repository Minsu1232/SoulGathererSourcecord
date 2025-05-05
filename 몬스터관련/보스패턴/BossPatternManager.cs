using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 보스 패턴의 성공 횟수와 난이도를 관리하는 클래스
/// </summary>
/// <remarks>
/// 기술문서 참조: 3장 '보스 시스템 구조 및 확장 방식' > 1절 '보스 고유 시스템'
/// 
/// 주요 기능:
/// - 패턴 성공 횟수 추적 및 관리
/// - 패턴별 난이도 조정
/// - 미니게임 성공 시 보스 그로기 상태 진입 결정
/// - 패턴 활성화/비활성화 관리
/// </remarks>
public class BossPatternManager
{
   public Dictionary<AttackPatternData, int> patternSuccessCounts;
    private HashSet<AttackPatternData> disabledPatterns;
    public Dictionary<AttackPatternData, float> patternDifficulties;
    private BossStatus bossStatus;

    public BossPatternManager(BossStatus status)
    {
        bossStatus = status;
        patternSuccessCounts = new Dictionary<AttackPatternData, int>();
        disabledPatterns = new HashSet<AttackPatternData>();
        patternDifficulties = new Dictionary<AttackPatternData, float>();
        InitializePatternStates();
    }
    public int GetPatternSuccessCount(AttackPatternData pattern)
    {
        return patternSuccessCounts.ContainsKey(pattern) ? patternSuccessCounts[pattern] : 0;
    }
    private void InitializePatternStates()
    {
        var bossMonster = bossStatus.GetBossMonster() as BossMonster;
        if (bossMonster == null) return;

        var bossData = bossMonster.GetBossData();
        foreach (var phaseData in bossData.phaseData)
        {
            foreach (var pattern in phaseData.availablePatterns)
            {
                patternSuccessCounts[pattern] = 0;
                patternDifficulties[pattern] = 1f; // 기본 난이도
            }
        }
    }

    public float GetPatternDifficulty(AttackPatternData pattern)
    {
        if (!patternDifficulties.ContainsKey(pattern))
        {
            patternDifficulties[pattern] = 1f;
        }
        return patternDifficulties[pattern];
    }

    private void IncreaseDifficulty(AttackPatternData pattern)
    {
        float currentDifficulty = GetPatternDifficulty(pattern);
        float newDifficulty = Mathf.Min(currentDifficulty + 0.5f, 3f);
        patternDifficulties[pattern] = newDifficulty;
        Debug.Log($"Pattern {pattern.patternName} difficulty increased to: {newDifficulty}");
    }

    private void ResetPattern(AttackPatternData pattern)
    {
        patternDifficulties[pattern] = 1f;
        patternSuccessCounts[pattern] = 0;
        Debug.Log($"Pattern {pattern.patternName} reset: Difficulty and success count back to initial values");
    }
    // 미니게임 결과에 따라 패턴 성공 처리를 하고, 그로기 상태 진입 여부를 결정합니다.
    public bool HandleMiniGameSuccess(MiniGameResult result, AttackPatternData currentPattern)
    {
        if (result == MiniGameResult.Miss)
        {           
            // UI 업데이트
            bossStatus.GetBossUIManager()?.UpdatePatternSuccess(currentPattern, 0);
            return false;
        }

        bool wasSuccess = HandlePatternSuccess(currentPattern);
        if (result == MiniGameResult.Perfect || result == MiniGameResult.Good)
        {
            IncreaseDifficulty(currentPattern);
        }

        // UI 업데이트
        bossStatus.GetBossUIManager()?.UpdatePatternSuccess(currentPattern, GetPatternSuccessCount(currentPattern));

        Debug.Log($"Pattern: {currentPattern.patternName}, Current Difficulty: {GetPatternDifficulty(currentPattern)}");

        if (wasSuccess)
        {
            ResetPattern(currentPattern);
            // 성공 후 리셋되었으므로 UI도 0으로 업데이트
            bossStatus.GetBossUIManager()?.UpdatePatternSuccess(currentPattern, 0);
        }

        return wasSuccess;
    }
   
    public bool HandlePatternSuccess(AttackPatternData pattern)
    {
        if (disabledPatterns.Contains(pattern)) return false;
        
        patternSuccessCounts[pattern]++;
        return patternSuccessCounts[pattern] >= pattern.requiredSuccessCount;
    }
}
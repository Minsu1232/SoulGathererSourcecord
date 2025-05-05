using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// ���� ������ ���� Ƚ���� ���̵��� �����ϴ� Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 3�� '���� �ý��� ���� �� Ȯ�� ���' > 1�� '���� ���� �ý���'
/// 
/// �ֿ� ���:
/// - ���� ���� Ƚ�� ���� �� ����
/// - ���Ϻ� ���̵� ����
/// - �̴ϰ��� ���� �� ���� �׷α� ���� ���� ����
/// - ���� Ȱ��ȭ/��Ȱ��ȭ ����
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
                patternDifficulties[pattern] = 1f; // �⺻ ���̵�
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
    // �̴ϰ��� ����� ���� ���� ���� ó���� �ϰ�, �׷α� ���� ���� ���θ� �����մϴ�.
    public bool HandleMiniGameSuccess(MiniGameResult result, AttackPatternData currentPattern)
    {
        if (result == MiniGameResult.Miss)
        {           
            // UI ������Ʈ
            bossStatus.GetBossUIManager()?.UpdatePatternSuccess(currentPattern, 0);
            return false;
        }

        bool wasSuccess = HandlePatternSuccess(currentPattern);
        if (result == MiniGameResult.Perfect || result == MiniGameResult.Good)
        {
            IncreaseDifficulty(currentPattern);
        }

        // UI ������Ʈ
        bossStatus.GetBossUIManager()?.UpdatePatternSuccess(currentPattern, GetPatternSuccessCount(currentPattern));

        Debug.Log($"Pattern: {currentPattern.patternName}, Current Difficulty: {GetPatternDifficulty(currentPattern)}");

        if (wasSuccess)
        {
            ResetPattern(currentPattern);
            // ���� �� ���µǾ����Ƿ� UI�� 0���� ������Ʈ
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
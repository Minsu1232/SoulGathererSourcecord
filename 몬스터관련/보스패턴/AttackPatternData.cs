using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// ������ ���� ���� �����͸� �����ϴ� Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 3�� '���� �ý��� ���� �� Ȯ�� ���' > 1�� '���� ���� �ý���'
/// 
/// �ֿ� ���:
/// - ���� ���� �ܰ� �� ���� ���� ����
/// - ���Ϻ� ��ٿ� �� ����ġ ����
/// - �̴ϰ��� ���� �䱸���� ����
/// - ���� ���̵� ���� �� ����
/// </remarks>
[System.Serializable]

[System.Serializable]
public class AttackPatternData
{
    [Header("Pattern Settings")]
    public string patternName;
    public int patternIndex;
    public BossPatternType patternType;  // �߰�
    public List<AttackStepData> steps = new List<AttackStepData>();
    public float patternWeight = 1.0f;
    public int phaseNumber;

    [Header("Timing")]
    public float patternCooldown;
    public float warningDuration;

    [Header("Requirements")]
    public float healthThresholdMin;  // �� ü��% �̻��� ���� ��� ����
    public float healthThresholdMax;  // �� ü��% ������ ���� ��� ����

    [Header("Effects")]
    public GameObject patternStartEffect;
    public GameObject patternEndEffect;
    public string warningMessage;

    [Header("Mini Game Success Requirements")]  // ���� �߰��� �κ�
    public bool isDisabled = false;            // ���� ��Ȱ��ȭ ����
    public int requiredSuccessCount = 3;       // �ʿ��� ���� Ƚ��
    public int currentSuccessCount = 0;        // ���� ���� Ƚ��

    [Header("Difficulty Settings")]
    public float baseDifficulty = 1f;          // �⺻ ���̵�
    public float maxDifficulty = 3f;           // �ִ� ���̵�
    public float difficultyIncreaseStep = 0.5f;  // ������ ������

    // ��Ÿ�ӿ��� ������ ���� ���̵� (BossMonster���� ����)
    public float currentDifficulty;

}

public enum BossPatternType
{
    None,
    BasicToJump,
    JumpToBasic,
    ChargeToJump,
    SpinToJump,
    // �ʿ��� ���� Ÿ�Ե� �߰�
}
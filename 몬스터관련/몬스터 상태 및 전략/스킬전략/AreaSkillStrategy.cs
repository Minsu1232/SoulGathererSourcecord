using UnityEngine;
using System;

/// <summary>
/// ���Ӽ� ���� ȿ���� ���� ��ų���� �����ϴ� ���� Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 3�� '�ý��� �ٽ� Ư¡' > '���� ��ų ����'
/// 
/// �ֿ� ���:
/// - ���� �ð� ��� ��ų ȿ�� ����
/// - ����Ʈ �ʱ�ȭ �� ���� Ÿ�̹� ����
/// - ��ų ��ٿ� �� ���� Ȯ��
/// - ��ų ȿ�� �Ϸ� ó��
/// </remarks>
public class AreaSkillStrategy : ISkillStrategy
{
    private ISkillEffect skillEffect;
    private ICreatureStatus monsterStatus;
    private bool isUsingSkill = false;
    private bool skillComplete = false;
    private float lastSkillTime;
    private float skillCoolTime;
    public Transform Target { get; set; } // Ÿ�� �Ӽ� ����
    // ��ų ���� ������ ������
    private float skillExecutionTimer = 0f;    // ��ų�� ����� �� ��� �ð�
    private bool skillExecuted = false;        // ��ų ���� ����
    private bool effectApplied = false;        // ȿ�� ���� ����

    // Ÿ�̹� ���� ������
    private float totalDuration;               // ��ų �� ���� �ð�
    private float effectDuration;              // ȿ�� ���� �ð� (howlDuration�� ���� ��)

    public float SkillRange { get; set; }
    public bool IsSkillComplete => skillComplete;
    public bool IsUsingSkill => isUsingSkill;
    public float GetLastSkillTime => lastSkillTime;

    protected CreatureAI owner;

    public AreaSkillStrategy(CreatureAI owner)
    {
        this.owner = owner;
        monsterStatus = owner.GetStatus();
        Target = owner.transform;
    }

    public void Initialize(ISkillEffect effect)
    {
        this.skillEffect = effect;

        // ��� ��ų ����Ʈ�� ���� ������ ������� �̺�Ʈ ����
        effect.OnEffectCompleted += () => {
            Debug.Log("[AreaSkillStrategy] ȿ�� �Ϸ� �ݹ� ����");
            // ȿ���� �Ϸ�Ǹ� ��ų�� �Ϸ� ó��
            CompleteSkill();
        };

        Debug.Log($"[AreaSkillStrategy] �ʱ�ȭ��: {effect.GetType().Name}");
    }

    public void StartSkill(Transform transform, Transform target, IMonsterClass monsterData)
    {
        try
        {
            Debug.Log($"[AreaSkillStrategy] ��ų ����: ����={monsterData.MONSTERNAME}");

            // ��Ÿ�� ����
            skillCoolTime = monsterData.CurrentSkillCooldown;

            // ȿ�� ���� �ð� ���� (HowlSkillEffect��� howlDuration ���)
            if (skillEffect is HowlSkillEffect)
            {
                // BossData���� howlDuration ���� ������ (BossData�� ĳ���� �ʿ�)
                ICreatureData bossData = monsterData.GetMonsterData();
                if (bossData != null)
                {
                    effectDuration = bossData.howlDuration;
                    Debug.Log($"[AreaSkillStrategy] Howl ���� �ð�: {effectDuration}��");
                }
                else
                {
                    effectDuration = 2.0f; // �⺻��
                    Debug.Log("[AreaSkillStrategy] ���: BossData ĳ���� ����, �⺻ ���� �ð� 2�� ���");
                }
            }
            else
            {
                // �ٸ� ���� ȿ�� ��ų�� skillDuration �Ǵ� �ٸ� �� ���
                effectDuration = monsterData.CurrentSKillDuration;
            }
            Debug.Log("!@#!@#!@#" + effectDuration);
            // �� ���� �ð��� ȿ�� ���� �ð� + �ణ�� ���� �ð�
            totalDuration = effectDuration + 1.0f;

            // ��ų ����Ʈ �ʱ�ȭ
            skillEffect.Initialize(monsterStatus, target);

            // ��ų ���� �ʱ�ȭ
            isUsingSkill = true;
            skillComplete = false;
            skillExecuted = false;
            effectApplied = false;
            skillExecutionTimer = 0f;
            lastSkillTime = Time.time;

            Debug.Log($"[AreaSkillStrategy] ��ų �ʱ�ȭ �Ϸ�. ȿ�� ���� �ð�: {effectDuration}��, �� ���� �ð�: {totalDuration}��");
        }
        catch (Exception e)
        {
            Debug.LogError($"[AreaSkillStrategy] StartSkill ����: {e.Message}\n{e.StackTrace}");
            CompleteSkill(); // ���� �߻� �� ��ų �Ϸ� ó��
        }
    }

    public void UpdateSkill(Transform transform, Transform target, IMonsterClass monsterData)
    {
        if (!isUsingSkill) return;

        try
        {
            // ��ų ���� �ð� ������Ʈ
            skillExecutionTimer += Time.deltaTime;

            // ��ų ȿ�� ���� (ó�� �� ����)
            if (!skillExecuted)
            {
                Debug.Log("[AreaSkillStrategy] ��ų ȿ�� ����");
                skillEffect.Execute();
                skillExecuted = true;

                // ���⼭�� ��ų ȿ���� �����ϰ�, ���� ȿ�� ������ 
                // ȿ�� ���ο��� ���� ó���� (HowlSkillEffect�� ��� DOTween����)
            }

            // �� ���� �ð��� ������ ��ų �Ϸ�
            if (skillExecutionTimer >= totalDuration)
            {
                Debug.Log($"[AreaSkillStrategy] ��ų ���� �ð� ����: {skillExecutionTimer:F2}�� ���");
                CompleteSkill();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[AreaSkillStrategy] UpdateSkill ����: {e.Message}\n{e.StackTrace}");
            CompleteSkill(); // ���� �߻� �� ��ų �Ϸ� ó��
        }
    }

    protected void CompleteSkill()
    {
        Debug.Log("[AreaSkillStrategy] ��ų �Ϸ�");
        isUsingSkill = false;
        skillComplete = true;
        skillExecuted = false;
        effectApplied = false;
        skillExecutionTimer = 0f;
        lastSkillTime = Time.time;
        // ��ų ����Ʈ�� OnComplete ȣ��
        if (skillEffect != null)
        {
            skillEffect.OnComplete();
        }
    }

    public bool CanUseSkill(float distanceToTarget, IMonsterClass monsterData)
    {
        return !isUsingSkill &&
               Time.time > lastSkillTime + skillCoolTime &&
               distanceToTarget <= SkillRange;
    }
}
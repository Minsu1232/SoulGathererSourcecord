using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ������ ���� ��ų ������ ����� �� �ְ� ���ִ� �����̳� Ŭ����
/// ����ġ ������� ��ų�� �����ϰ� ��Ÿ���� �����մϴ�.
/// </summary>
/// <remarks>
/// ������� ����: 3�� '���� �ý��� ���� �� Ȯ�� ���' > 2�� '����� Ȯ�� ���'
/// 
/// �ֿ� ���:
/// - �پ��� ��ų ������ ���� ����
/// - ����ġ ����� ��ų ���� ����
/// - ���� ��ٿ� Ÿ�̸� ����
/// - ���� ID ����� ��ų ���� �ڵ�ȭ
/// - ������ ��ȯ �� ���� ���� �� �ʱ�ȭ
/// </remarks>
public class BossMultiSkillStrategy : ISkillStrategy
{
    private List<ISkillStrategy> strategies = new List<ISkillStrategy>();
    private Dictionary<ISkillStrategy, float> weights = new Dictionary<ISkillStrategy, float>();
    public ISkillStrategy currentStrategy;
    private readonly BasicSkillStrategy defaultStrategy;

    // ���� Ÿ�̸� - BossMultiSkillStrategy ��ü ��Ÿ�� ������
    private float unifiedLastSkillTime;

    // ���� ���� �̺�Ʈ �߰�
    public event System.Action OnSkillStateChanged;

    public bool IsSkillComplete => currentStrategy?.IsSkillComplete ?? true;
    public bool IsUsingSkill => currentStrategy?.IsUsingSkill ?? false;
    public float GetLastSkillTime => unifiedLastSkillTime;
    public float SkillRange { get; set; } = 10f; // �⺻��

    public BossMultiSkillStrategy(CreatureAI owner)
    {
        defaultStrategy = new BasicSkillStrategy(owner);
        unifiedLastSkillTime = 0f;
    }

    /// <summary>
    /// ��ų ������ ����ġ�� �Բ� �߰��մϴ�.
    /// </summary>
    /// <param name="strategy">�߰��� ��ų ����</param>
    /// <param name="weight">���� ����ġ</param>
    public void AddStrategy(ISkillStrategy strategy, float weight)
    {
        if (!strategies.Contains(strategy))
        {
            strategies.Add(strategy);
            weights[strategy] = weight;
            // ������ ��ų ���� ����
            strategy.SkillRange = this.SkillRange;
            Debug.Log($"��ų ���� �߰���: {strategy.GetType().Name}, ����ġ: {weight}");
        }
    }
    /// <summary>
    /// ���� ID�� ������� �̸� ������ ��ų ������ �߰��մϴ�.
    /// </summary>
    /// <param name="configId">���� ID</param>
    /// <param name="weight">���� ����ġ</param>
    /// <param name="owner">AI ������</param>
    /// <param name="data">���� ������</param>
    public void AddSkillStrategyFromConfig(
        int configId,
        float weight,
        CreatureAI owner,
        ICreatureData data)
    {
        try
        {
            Debug.Log($"[AddSkillStrategyFromConfig] ���� ID: {configId}, ����ġ: {weight}");

            if (owner == null)
            {
                Debug.LogError("[AddSkillStrategyFromConfig] owner�� null�� �� �����ϴ�.");
                return;
            }

            if (data == null)
            {
                Debug.LogError("[AddSkillStrategyFromConfig] data�� null�� �� �����ϴ�.");
                return;
            }

            // SkillStrategyFactory�� ���� ��ų ���� ����
            ISkillStrategy skillStrategy = SkillStrategyFactory.CreateFromConfig(configId, owner, data);

            if (skillStrategy != null)
            {
                // �̹� ������ ������ �ִ��� Ȯ��
                if (strategies.Contains(skillStrategy))
                {
                    Debug.LogWarning($"[AddSkillStrategyFromConfig] �ش� ��ų ������ �̹� �����մϴ�: {skillStrategy.GetType().Name}");
                    return;
                }

                AddStrategy(skillStrategy, weight);
                Debug.Log($"[AddSkillStrategyFromConfig] ��ų ���� �߰� �Ϸ�: {skillStrategy.GetType().Name}, ����ġ: {weight}");
            }
            else
            {
                Debug.LogError($"[AddSkillStrategyFromConfig] ��ų ���� ���� ����: ID {configId}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[AddSkillStrategyFromConfig] ���� �߻�: {e.Message}\n{e.StackTrace}");
        }
    }


    // ���� Ÿ�� ���ڿ��� BuffType �迭�� ��ȯ
    private BuffType[] ParseBuffTypes(string buffTypesString)
    {
        if (string.IsNullOrEmpty(buffTypesString))
            return new BuffType[0];

        string[] types = buffTypesString.Split('|');
        BuffType[] result = new BuffType[types.Length];

        for (int i = 0; i < types.Length; i++)
        {
            if (System.Enum.TryParse(types[i], out BuffType buffType))
                result[i] = buffType;
            else
                result[i] = BuffType.None;
        }

        return result;
    }

    // �����ڷ� �������� float �� ���ڿ��� float �迭�� ��ȯ
    private float[] ParseFloatArray(string floatString)
    {
        if (string.IsNullOrEmpty(floatString))
            return new float[0];

        string[] values = floatString.Split('|');
        float[] result = new float[values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            if (float.TryParse(values[i], out float value))
                result[i] = value;
        }

        return result;
    }

    public void Initialize(ISkillEffect skillEffect)
    {
        // ��� ������ ����Ʈ �ʱ�ȭ
        foreach (var strategy in strategies)
        {
            strategy.Initialize(skillEffect);
        }

        // �⺻ �������� �ʱ�ȭ
        defaultStrategy.Initialize(skillEffect);
    }

    public void StartSkill(Transform transform, Transform target, IMonsterClass monsterData)
    {
        if (currentStrategy == null || !currentStrategy.IsUsingSkill)
        {
            currentStrategy = SelectStrategy(Vector3.Distance(transform.position, target.position), monsterData);
            Debug.Log($"���õ� ��ų ����: {currentStrategy.GetType().Name}");
        }

        if (currentStrategy != null)
        {
            Debug.Log(currentStrategy.ToString());
            
            currentStrategy.StartSkill(transform, target, monsterData);
        }
    }

    public void UpdateSkill(Transform transform, Transform target, IMonsterClass monsterData)
    {
        if (currentStrategy != null && currentStrategy.IsUsingSkill)
        {
            currentStrategy.UpdateSkill(transform, target, monsterData);

            // ��ų�� �Ϸ�Ǹ� ���� Ÿ�̸� ������Ʈ �� ���� ���� �ʱ�ȭ
            if (currentStrategy.IsSkillComplete)
            {
                unifiedLastSkillTime = Time.time;  // ��ų ��� �� ���� ��Ÿ�� ������Ʈ
                currentStrategy = null;
                OnSkillStateChanged?.Invoke();
            }
        }
    }

    public bool CanUseSkill(float distanceToTarget, IMonsterClass monsterData)
    {
      

        float cooldownTime = monsterData.CurrentSkillCooldown;
        float timeSinceLastSkill = Time.time - unifiedLastSkillTime;
        bool cooldownReady = Time.time >= unifiedLastSkillTime + cooldownTime;

       

        // ���յ� ��Ÿ�� üũ
        if (!cooldownReady)
            return false;

        // ���� ��ų ��� ���̸� �Ұ���
        if (IsUsingSkill)
            return false;

        // �켱���� ������� ��� ������ ù ��° ��ų ã��
        bool anyStrategyReady = false;
        foreach (var strategy in strategies)
        {
            bool canUse = strategy.CanUseSkill(distanceToTarget, monsterData);
            
            if (canUse)
            {
                anyStrategyReady = true;
                break;
            }
        }

        return anyStrategyReady;
    }

    private ISkillStrategy SelectStrategy(float distanceToTarget, IMonsterClass monsterData)
    {
        // ����ġ ������� ��ų ����
        var availableStrategies = strategies
            .Where(s => s.CanUseSkill(distanceToTarget, monsterData))
            .ToList();

        if (availableStrategies.Count == 0)
            return defaultStrategy;

        float totalWeight = availableStrategies.Sum(s => weights[s]);
        float random = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var strategy in availableStrategies)
        {
            cumulative += weights[strategy];
            if (random <= cumulative)
                return strategy;
        }

        // ����ġ ��� ������ ������ ��� ù ��° ���� ��ȯ
        return availableStrategies[0];
    }

    /// <summary>
    /// �ʿ��� ��� ��ų �ߴ� �޼���
    /// </summary>
    public void StopSkill()
    {
        if (currentStrategy != null && currentStrategy.IsUsingSkill)
        {
            // �����Ǿ� �ִٸ� ȣ��
            if (currentStrategy is SkillState skillState)
            {
                skillState.Exit(); // ���� - ���� ������ �°� ���� �ʿ�
            }
            currentStrategy = null;
        }
    }

    /// <summary>
    /// ������ ��ȯ �� �� ������ ���� ��ų�� �ٷ� ������� �ʵ��� unifiedLastSkillTime�� ���� �ð����� �缳��
    /// </summary>
    public void ResetTimer(float bufferTime = 1f)
    {
        unifiedLastSkillTime = Time.time + bufferTime;
    }

    /// <summary>
    /// ���� ���� ����Ʈ�� ����, Ÿ�̸ӿ� ���۸� ��� �ʱ�ȭ�մϴ�.
    /// </summary>
    public void ResetAll()
    {
        // ���� ���� ����Ʈ�� ����ġ ������ ��� ���ϴ�.
        strategies.Clear();
        weights.Clear();
        currentStrategy = null;

        // Ÿ�̸Ӹ� ���� �ð����� �ʱ�ȭ�մϴ�.
        unifiedLastSkillTime = Time.time;

        Debug.Log("ResetAll: ��ų ���� ��� �ʱ�ȭ �� Ÿ�̸� �缳�� �Ϸ�");
    }
}
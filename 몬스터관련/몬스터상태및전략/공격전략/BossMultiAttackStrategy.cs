using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// ���� ���� ������ ����ġ�� �ο��� ���� �� �����ϴ� ���� ���� ���� ����
/// </summary>
/// <remarks>
/// ������� ����: 3�� '���� �ý��� ���� �� Ȯ�� ���'
/// 
/// �ֿ� ���:
/// - �پ��� ���� ������ ���� ����
/// - ����ġ ����� ���� ���� ����
/// - ���� ��ٿ� Ÿ�̸� ����
/// - ������ ��ȯ �� ���� ���� �� �ʱ�ȭ
/// </remarks>
public class BossMultiAttackStrategy : IAttackStrategy
{
    private List<IAttackStrategy> strategies = new List<IAttackStrategy>();
    private Dictionary<IAttackStrategy, float> weights = new Dictionary<IAttackStrategy, float>();
    private IAttackStrategy currentStrategy;
    private readonly BasicAttackStrategy defaultStrategy;

    // ���� Ÿ�̸�: ������ ��ȯ �� �ʱ�ȭ�� �� �ֵ��� public �޼��� ����
    private float unifiedLastAttackTime;

    public event System.Action OnStrategyStateChanged;  // ���� ���� �̺�Ʈ �߰�
    public BossMultiAttackStrategy()
    {
        defaultStrategy = new BasicAttackStrategy();
        // �⺻ ���� �� �ٷ� ���� �����ϵ��� (���� �߿��� �̷���, ������ ��ȯ �ÿ��� ResetTimer()�� �缳��)
       
    }

    public bool IsAttacking => currentStrategy?.IsAttacking ?? false;
    public float GetLastAttackTime => unifiedLastAttackTime;
    public PhysicalAttackType AttackType => currentStrategy?.AttackType ?? defaultStrategy.AttackType;

    public void AddStrategy(IAttackStrategy strategy, float weight)
    {
        if (!strategies.Contains(strategy))
        {
            strategies.Add(strategy);
            weights[strategy] = weight;
            Debug.Log(strategies.ToString() + "�߰�");
        }
    }

    public void Attack(Transform transform, Transform target, IMonsterClass monsterData)
    {

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (Time.time < unifiedLastAttackTime + monsterData.CurrentAttackSpeed)
            return;

        if (currentStrategy == null ||
            (!currentStrategy.IsAttacking && !currentStrategy.CanAttack(distanceToTarget, monsterData)))
        {
            if (currentStrategy?.IsAttacking ?? false)
                return;

            currentStrategy = SelectStrategy(distanceToTarget, monsterData);
            
            // ���ο� ������ ChargeAttackStrategy�� ��� �̺�Ʈ ����
            if (currentStrategy is ChargeAttackStrategy chargeStrategy)
            {
                chargeStrategy.OnChargeStateChanged += () => 
                {
                    OnStrategyStateChanged?.Invoke();
                };
            }
        }

        if (currentStrategy != null && currentStrategy.CanAttack(distanceToTarget, monsterData))
        {
            currentStrategy.Attack(transform, target, monsterData);
        }
    } 

    public bool CanAttack(float distanceToTarget, IMonsterClass monsterData)
    {
        if (Time.time < unifiedLastAttackTime + monsterData.CurrentAttackSpeed)
            return false;

        if (currentStrategy != null && currentStrategy.CanAttack(distanceToTarget, monsterData))
            return true;

        return defaultStrategy.CanAttack(distanceToTarget, monsterData);
    }

    private IAttackStrategy SelectStrategy(float distanceToTarget, IMonsterClass monsterData)
    {
        var availableStrategies = strategies.Where(s => s.CanAttack(distanceToTarget, monsterData)).ToList();
        Debug.Log($"��� ������ ���� ��: {availableStrategies.Count}");
        foreach (var strat in availableStrategies)
        {
            Debug.Log($"������ ����: {strat.GetType().Name}");
        }
        if (availableStrategies.Count == 0)
        {
            Debug.Log("��������");
            return defaultStrategy;
        }

        float totalWeight = availableStrategies.Sum(s => weights[s]);
        float random = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var strat in availableStrategies)
        {
            cumulative += weights[strat];
            if (random <= cumulative)
                return strat;
        }

        // foreach �������� ���õ��� �ʾ��� ����� �⺻��
        return availableStrategies[0];
    }

    public void StopAttack()
    {
        currentStrategy?.StopAttack();
        currentStrategy = null;
        UpdateLastAttackTime();
    }

    public string GetAnimationTriggerName() => currentStrategy?.GetAnimationTriggerName() ?? defaultStrategy.GetAnimationTriggerName();
    public float GetAttackPowerMultiplier() => currentStrategy?.GetAttackPowerMultiplier() ?? defaultStrategy.GetAttackPowerMultiplier();
    public void StartAttack() => currentStrategy?.StartAttack();
    public void OnAttackAnimationEnd() => currentStrategy?.OnAttackAnimationEnd();
    public void ApplyDamage(IDamageable target, IMonsterClass monsterData) => currentStrategy?.ApplyDamage(target, monsterData);

    public void UpdateLastAttackTime()
    {
        
        unifiedLastAttackTime = Time.time;
       
        if (currentStrategy is BasePhysicalAttackStrategy baseStrategy)
        {
            baseStrategy.UpdateLastAttackTime();
        }
    }
    // Execute ���� �߰�
    public void UpdateStrategy(Transform transform)
    {
        if (currentStrategy is ChargeAttackStrategy chargeStrategy)
        {
            chargeStrategy.UpdateCharge(transform);
        }
    }
    /// <summary>
    /// ������ ��ȯ �� �� ������ ���� ������ �ٷ� ������� �ʵ��� unifiedLastAttackTime�� ���� �ð����� �缳��
    /// </summary>
    public void ResetTimer(float bufferTime = 1f)
    {
        unifiedLastAttackTime = Time.time + bufferTime;
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
        unifiedLastAttackTime = Time.time;

        Debug.Log("ResetAll: Strategies cleared and timer reset to " + unifiedLastAttackTime);
    }

    public void ResetAttackTime()
    {
        throw new System.NotImplementedException();
    }
}

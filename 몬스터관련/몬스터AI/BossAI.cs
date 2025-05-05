using static IMonsterState;
using System.Collections.Generic;
using System;
using UnityEngine;
/// <summary>
/// ���� ���� ���� AI �ý���
/// </summary>
/// <remarks>
/// ������� ����: 3�� '���� �ý��� ���� �� Ȯ�� ���'
/// 
/// �ֿ� ���:
/// - ������ ��ȯ ����: ü�¿� ���� ���� ��ȯ ���� = > ��Ʈ���� ���� ����
/// - ��ͻ���: ü�¿� ���� ��� �ߵ� ��Ŀ���� = > ��Ʈ���� ���� ����
/// - ����: �̴ϰ��� �� ���� ���� ������ ���� ���� �ý��� = > ���� ��Ʈ��  +�߰�
/// - ������ ������: é�� ���� ������ ������ ����
/// - ���� ���� ���� �߰�: PhaseTransitionState, GimmickState, PatternState 
/// - ���� ���� BTreeNode �߰� : ExecutePhaseTransitionNode, ExecuteGimmickNode ��
/// - BossMultiAttackStrategy�� ���� �پ��� ���� ���� ����

public class BossAI : CreatureAI


{  // ���� ������ BossMultiAttackStrategy �ν��Ͻ��� ����
    private BossMultiAttackStrategy globalMultiAttackStrategy;    
  
    // ���� ���� �̺�Ʈ �߰�
    public event Action<BossPattern> OnPatternChanged;

    private MiniGameManager miniGameManager;
    private BossUIManager bossUIManager;

    private BossMonster bossMonster;
    
    //���� ���� �߰�
    private IPhaseTransitionStrategy currentPhaseStrategy;
    private IGimmickStrategy gimmickStrategy;
    private ISuccessUI successUI;
    //���� ������ ���� ���� �������� ���� �̿�
    private BossPattern bossPatternStartaegy;
    protected override void Start()
    {
        miniGameManager = FindObjectOfType<MiniGameManager>();
        bossUIManager = GetComponent<BossUIManager>();
        globalMultiAttackStrategy = new BossMultiAttackStrategy();
        successUI = GetComponentInChildren<SuccessUI>();
        base.Start();
     
        InitializeBehaviorTree();    
    }
    // ���� �ʱ�ȭ �ż���
    protected override void InitializeStates()
    {
        IMonsterClass monsterClass = creatureStatus.GetMonsterClass();
        ICreatureData data = monsterClass.GetMonsterData();

        // 1. states ��ųʸ� �ʱ�ȭ
        states = new Dictionary<IMonsterState.MonsterStateType, IMonsterState>();

        // 2. ���� ���� ���� �ʱ�ȭ
        bossMonster = creatureStatus.GetMonsterClass() as BossMonster;
        BossData bossData = bossMonster.GetBossData();

        states[MonsterStateType.Spawn] = new SpawnState(this, spawnStrategy ?? new BasicSpawnStrategy());
        states[MonsterStateType.Idle] = new IdleState(this, idleStrategy ?? new BasicIdleStrategy());
        states[MonsterStateType.Move] = new MoveState(this, moveStrategy ?? new BasicMovementStrategy());
        // ���� ������ �������� �����ϴ� globalMultiAttackStrategy�� ����մϴ�.
        states[MonsterStateType.Attack] = new AttackState(this, globalMultiAttackStrategy);
        states[MonsterStateType.Skill] = new SkillState(this, skillStrategy ?? new BasicSkillStrategy(this));
        states[MonsterStateType.Hit] = new HitState(this, hitStrategy ?? new BasicHitStrategy());
        states[MonsterStateType.Groggy] = new GroggyState(this, groggyStrategy ?? new BasicGroggyStrategy(3f), bossUIManager);
        states[MonsterStateType.Die] = new DieState(this, dieStrategy ?? new BasicDieStrategy());
        //states[MonsterStateType.PhaseTransition] = new PhaseTransitionState(this, currentPhaseStrategy,bossMonster ?? new BossPhaseTransitionStrategy(bossMonster, this,bossUIManager));
        states[MonsterStateType.Gimmick] = new GimmickState(this, gimmickStrategy ?? new HazardGimmickStrategy(),bossUIManager);
        // 3. ��� ���� �ʱ�ȭ
        InitializeStrategies(data);

        // 4. states�� ���� �Ҵ�
        states[MonsterStateType.Spawn] = new SpawnState(this, spawnStrategy);
        states[MonsterStateType.Idle] = new IdleState(this, idleStrategy);
        states[MonsterStateType.Move] = new MoveState(this, moveStrategy);
        // ���� ������ �������� �����ϴ� globalMultiAttackStrategy�� ����մϴ�.
        states[MonsterStateType.Attack] = new AttackState(this, globalMultiAttackStrategy);
        states[MonsterStateType.Skill] = new SkillState(this, skillStrategy);
        states[MonsterStateType.Hit] = new HitState(this, hitStrategy);
        states[MonsterStateType.Groggy] = new GroggyState(this, groggyStrategy, bossUIManager);
        states[MonsterStateType.Die] = new DieState(this, dieStrategy);
        states[MonsterStateType.PhaseTransition] = new PhaseTransitionState(this, currentPhaseStrategy, bossMonster);
        states[MonsterStateType.Gimmick] = new GimmickState(this, gimmickStrategy, bossUIManager);
        if (bossMonster.CurrentPhaseData.availablePatterns.Count > 0) // ������ �ѹ��� Ŀ��Ʈ ����� ���� ������ ����
        {
            var phasePattern = bossMonster.CurrentPhaseData.availablePatterns.Find(p => p.phaseNumber == bossMonster.CurrentPhase);
            if (phasePattern != null)
            {
                bossPatternStartaegy = BossStrategyFactory.CreatePatternStrategy(
                    phasePattern,
                    this,
                    miniGameManager,
                    bossData
                );
                states[MonsterStateType.Pattern] = new PatternState(this, bossPatternStartaegy);
            }
            else
            {
                Debug.LogWarning($"No pattern found for initial phase {bossMonster.CurrentPhase}");
            }
        }
        // 5. �ʱ� ���� ����
        ChangeState(MonsterStateType.Spawn);

        Debug.Log(bossMonster.CurrentPhaseData.phaseName);
    }
    public void InitializeBehaviorTree()
    {
        behaviorTree = new Selector(this,
        //������ ��ȯ üũ �� ����
        new BTSequence(this,
            new CheckPhaseTransitionNode(this),
            new ExecutePhaseTransitionNode(this)
        ),
        // ��� üũ
        new BTSequence(this,
        new CheckGimmickNode(this),
        new ExecuteGimmickNode(this)
        ),
        // ���� ����
        new BTSequence(this,
            new CheckHealthCondition(this),
            new ChangeStateAction(this, MonsterStateType.Move, MovementStrategyType.Retreat)
        ),
        new TimeDelayDecorator(this,
        new BTSequence(this,
            new CheckPlayerInAttackRange(this),
            new CheckPatternDistance(this),
            new CombatDecisionNode(this)
        ), 0.5f),
        new BTSequence(this,
            new CheckPlayerInRange(this),
            new ChangeStateAction(this, MonsterStateType.Move, MovementStrategyType.Basic)
        )
    );
    }
    private void InitializeStrategies(ICreatureData data)
    {
        // 1. ���� ������ �ʱ�ȭ
        spawnStrategy = StrategyFactory.CreateSpawnStrategy(data.spawnStrategy);
        moveStrategy = StrategyFactory.CreateMovementStrategy(data.moveStrategy);
        idleStrategy = StrategyFactory.CreatIdleStrategy(data.idleStrategy);
        dieStrategy = StrategyFactory.CreatDieStrategy(data.dieStrategy);
        hitStrategy = StrategyFactory.CreatHitStrategy(data.hitStrategy);
        groggyStrategy = StrategyFactory.CreateGroggyStrategy(data.groggyStrategy, data);

        // 2. ���� ���� ���� �ʱ�ȭ
        if (data is BossData bossData)
        {
            // ���� ���� �ʱ�ȭ (globalMultiAttackStrategy ���)
            foreach (var strategyData in bossMonster.CurrentPhaseData.phaseAttackStrategies)
            {
                var strategy = StrategyFactory.CreateAttackStrategy(strategyData.type, bossData, this);
                if (strategy != null)
                {
                    globalMultiAttackStrategy.AddStrategy(strategy, strategyData.weight);
                }
            }
            attackStrategy = globalMultiAttackStrategy;

            // ��Ƽ ��ų ���� �ʱ�ȭ
            BossMultiSkillStrategy multiSkillStrategy = new BossMultiSkillStrategy(this);

            // ���� �������� ��ų ���� �߰�
            PhaseData currentPhaseData = bossMonster.CurrentPhaseData;

            for (int i = 0; i < currentPhaseData.skillConfigIds.Count; i++)
            {
                int configId = currentPhaseData.skillConfigIds[i];
                float weight = i < currentPhaseData.skillConfigWeights.Count ?
                              currentPhaseData.skillConfigWeights[i] : 1.0f;

                multiSkillStrategy.AddSkillStrategyFromConfig(configId, weight, this, bossData);
            }

            // ���� ����
            multiSkillStrategy.SkillRange = data.skillRange;

            // ���� ����
            skillStrategy = multiSkillStrategy;

            // ������ �� ��� ���� �ʱ�ȭ
            currentPhaseStrategy = BossStrategyFactory.CreatePhaseTransitionStrategy(
                bossMonster.CurrentPhaseData.phaseTransitionType,
                bossMonster,
                this,
                bossUIManager
            );

            gimmickStrategy = BossStrategyFactory.CreateGimmickStrategy(
                bossMonster.CurrentPhaseGimmickData.type,
                this,
                bossMonster.CurrentPhaseGimmickData,
                bossMonster.CurrentPhaseGimmickData.hazardPrefab,
                bossData,
                successUI
            );
        }
        else
        {
            // �Ϲ� ������ ��� �⺻ ��ų ���� ���
            skillStrategy = StrategyFactory.CreateSkillStrategy(data.skillStrategy, this);
        }

        // ������ ��ų ����Ʈ �ʱ�ȭ
        //InitializeSkillEffect(data);
    }

    /// <summary>
    /// ������ ���� ����� ���� ��� ����(����, ��ų, ����)�� ������Ʈ�մϴ�.
    /// </summary>
    /// <remarks>
    /// ������ ��ȯ �� ȣ��Ǿ� ������ �ൿ ������ ���ο� ����� �°� �����մϴ�.
    /// </remarks>
    public void UpdatePhaseStrategies()
    {
        if (bossMonster == null) return;
        IMonsterClass monsterClass = creatureStatus.GetMonsterClass();
        ICreatureData data = monsterClass.GetMonsterData();
        var currentPhase = bossMonster.CurrentPhaseData;

        // ���� BossMultiAttackStrategy �ν��Ͻ��� ���� ����(���� ����Ʈ, Ÿ�̸� ��)�� ������ �ʱ�ȭ�մϴ�.
        globalMultiAttackStrategy.ResetAll();

        // ���� �������� ���� ���� �����͸� globalMultiAttackStrategy�� �߰��մϴ�.
        foreach (var strategyData in currentPhase.phaseAttackStrategies)
        {
            var strategy = StrategyFactory.CreateAttackStrategy(strategyData.type, bossMonster.GetBossData(), this);
            if (strategy != null)
            {
                globalMultiAttackStrategy.AddStrategy(strategy, strategyData.weight);
            }
        }

        // ��ų ���� ������Ʈ
        if (skillStrategy is BossMultiSkillStrategy multiSkillStrategy)
        {
            // ���� ��ų ���� ����
            multiSkillStrategy.ResetAll();

            // ���� �������� ��ų ���� �߰�
            for (int i = 0; i < currentPhase.skillConfigIds.Count; i++)
            {
                int configId = currentPhase.skillConfigIds[i];
                float weight = i < currentPhase.skillConfigWeights.Count ?
                              currentPhase.skillConfigWeights[i] : 1.0f;

                multiSkillStrategy.AddSkillStrategyFromConfig(configId, weight, this, bossMonster.GetBossData());
            }           
            // ��ų ��� ������ ���� Ÿ�̸� ���� (������ ��ȯ �� �ٷ� ��ų ��� ����)
            multiSkillStrategy.ResetTimer(1.5f);
        }

        // ���� ������Ʈ: ���� ����� �ش��ϴ� ������ �ִٸ� ���� ������ ���� �����Ͽ� ���¿� �Ҵ��մϴ�.
        if (currentPhase.availablePatterns.Count > 0)
        {
            var phasePattern = currentPhase.availablePatterns.Find(p => p.phaseNumber == bossMonster.CurrentPhase);

            if (phasePattern != null)
            {
                bossPatternStartaegy.CleanAll();
                bossPatternStartaegy = null;

                bossPatternStartaegy = BossStrategyFactory.CreatePatternStrategy(
                    phasePattern,
                    this,
                    miniGameManager,
                    bossMonster.GetBossData()
                );
                

                states[MonsterStateType.Pattern] = new PatternState(this, bossPatternStartaegy);
                OnPatternChanged?.Invoke(bossPatternStartaegy);
                Debug.Log(bossPatternStartaegy.ToString());
            }
            else
            {
                Debug.LogWarning($"No pattern found for phase {bossMonster.CurrentPhase}");
            }
        }

    }



    protected override void Update()
    {
        base.Update();

        behaviorTree?.Execute();     

    }



    #region Strategy Implementation
    public override IAttackStrategy GetAttackStrategy() => attackStrategy;
    public override ISkillStrategy GetSkillStrategy() => skillStrategy;
    public override ISpawnStrategy GetSpawnStrategy() => spawnStrategy;
    public override IMovementStrategy GetMovementStrategy() => moveStrategy;
    public override IIdleStrategy GetIdleStrategy() => idleStrategy;
    public override IDieStrategy GetDieStrategy() => dieStrategy;
    public override IHitStrategy GetHitStrategy() => hitStrategy;
    public override IGroggyStrategy GetGroggyStrategy() => groggyStrategy;
    public override IGimmickStrategy GetGimmickStrategy() => gimmickStrategy;
    public override IPhaseTransitionStrategy GetPhaseTransitionStrategy() => currentPhaseStrategy;
    public override BossPattern GetBossPatternStartegy() => bossPatternStartaegy;


    public BossMonster GetBossMonster()
    {
        return bossMonster;
    }
    public override void SetAttackStrategy(IAttackStrategy newStrategy)
    {
        Debug.Log($"SetAttackStrategy called with {newStrategy.GetType().Name} from:\n{System.Environment.StackTrace}");
        attackStrategy = newStrategy;

        if (states == null)
        {
            Debug.Log("�����");
        }
        if (states.ContainsKey(MonsterStateType.Attack))
        {
            Debug.Log("���������� �ٲ�");
            states[MonsterStateType.Attack] = new AttackState(this, attackStrategy);
        }
    }

    public override void SetMovementStrategy(IMovementStrategy newStrategy)
    {
        moveStrategy = newStrategy;
        if (states.ContainsKey(MonsterStateType.Move))
        {
            states[MonsterStateType.Move] = new MoveState(this, moveStrategy);
        }
    }

    public override void SetSkillStrategy(ISkillStrategy newStrategy)
    {

        skillStrategy = newStrategy;
        if (states.ContainsKey(MonsterStateType.Skill))
        {
            states[MonsterStateType.Skill] = new SkillState(this, skillStrategy);
        }
    }

    public override void SetIdleStrategy(IIdleStrategy newStrategy)
    {
        idleStrategy = newStrategy;
        if (states.ContainsKey(MonsterStateType.Idle))
        {
            states[MonsterStateType.Idle] = new IdleState(this, idleStrategy);
        }
    }



    #endregion
}
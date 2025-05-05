using static IMonsterState;
using System.Collections.Generic;
using System;
using UnityEngine;
/// <summary>
/// 보스 몬스터 전용 AI 시스템
/// </summary>
/// <remarks>
/// 기술문서 참조: 3장 '보스 시스템 구조 및 확장 방식'
/// 
/// 주요 기능:
/// - 페이즈 변환 상태: 체력에 따른 전략 전환 구현 = > 비트리를 통해 추적
/// - 기믹상태: 체력에 따른 기믹 발동 메커니즘 = > 비트리를 통해 추적
/// - 패턴: 미니게임 및 기존 공격 전략의 연속 공격 시스템 = > 기존 비트리  +추가
/// - 에센스 게이지: 챕터 보스 고유의 게이지 관리
/// - 보스 전용 상태 추가: PhaseTransitionState, GimmickState, PatternState 
/// - 보스 전용 BTreeNode 추가 : ExecutePhaseTransitionNode, ExecuteGimmickNode 등
/// - BossMultiAttackStrategy를 통한 다양한 공격 패턴 관리

public class BossAI : CreatureAI


{  // 전역 변수로 BossMultiAttackStrategy 인스턴스를 선언
    private BossMultiAttackStrategy globalMultiAttackStrategy;    
  
    // 패턴 변경 이벤트 추가
    public event Action<BossPattern> OnPatternChanged;

    private MiniGameManager miniGameManager;
    private BossUIManager bossUIManager;

    private BossMonster bossMonster;
    
    //보스 전용 추가
    private IPhaseTransitionStrategy currentPhaseStrategy;
    private IGimmickStrategy gimmickStrategy;
    private ISuccessUI successUI;
    //보스 패턴은 기존 공격 전략들을 합쳐 이용
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
    // 상태 초기화 매서드
    protected override void InitializeStates()
    {
        IMonsterClass monsterClass = creatureStatus.GetMonsterClass();
        ICreatureData data = monsterClass.GetMonsterData();

        // 1. states 딕셔너리 초기화
        states = new Dictionary<IMonsterState.MonsterStateType, IMonsterState>();

        // 2. 보스 몬스터 참조 초기화
        bossMonster = creatureStatus.GetMonsterClass() as BossMonster;
        BossData bossData = bossMonster.GetBossData();

        states[MonsterStateType.Spawn] = new SpawnState(this, spawnStrategy ?? new BasicSpawnStrategy());
        states[MonsterStateType.Idle] = new IdleState(this, idleStrategy ?? new BasicIdleStrategy());
        states[MonsterStateType.Move] = new MoveState(this, moveStrategy ?? new BasicMovementStrategy());
        // 공격 전략은 전역으로 관리하는 globalMultiAttackStrategy를 사용합니다.
        states[MonsterStateType.Attack] = new AttackState(this, globalMultiAttackStrategy);
        states[MonsterStateType.Skill] = new SkillState(this, skillStrategy ?? new BasicSkillStrategy(this));
        states[MonsterStateType.Hit] = new HitState(this, hitStrategy ?? new BasicHitStrategy());
        states[MonsterStateType.Groggy] = new GroggyState(this, groggyStrategy ?? new BasicGroggyStrategy(3f), bossUIManager);
        states[MonsterStateType.Die] = new DieState(this, dieStrategy ?? new BasicDieStrategy());
        //states[MonsterStateType.PhaseTransition] = new PhaseTransitionState(this, currentPhaseStrategy,bossMonster ?? new BossPhaseTransitionStrategy(bossMonster, this,bossUIManager));
        states[MonsterStateType.Gimmick] = new GimmickState(this, gimmickStrategy ?? new HazardGimmickStrategy(),bossUIManager);
        // 3. 모든 전략 초기화
        InitializeStrategies(data);

        // 4. states에 전략 할당
        states[MonsterStateType.Spawn] = new SpawnState(this, spawnStrategy);
        states[MonsterStateType.Idle] = new IdleState(this, idleStrategy);
        states[MonsterStateType.Move] = new MoveState(this, moveStrategy);
        // 공격 전략은 전역으로 관리하는 globalMultiAttackStrategy를 사용합니다.
        states[MonsterStateType.Attack] = new AttackState(this, globalMultiAttackStrategy);
        states[MonsterStateType.Skill] = new SkillState(this, skillStrategy);
        states[MonsterStateType.Hit] = new HitState(this, hitStrategy);
        states[MonsterStateType.Groggy] = new GroggyState(this, groggyStrategy, bossUIManager);
        states[MonsterStateType.Die] = new DieState(this, dieStrategy);
        states[MonsterStateType.PhaseTransition] = new PhaseTransitionState(this, currentPhaseStrategy, bossMonster);
        states[MonsterStateType.Gimmick] = new GimmickState(this, gimmickStrategy, bossUIManager);
        if (bossMonster.CurrentPhaseData.availablePatterns.Count > 0) // 페이즈 넘버와 커레트 페이즈가 같은 패턴을 주입
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
        // 5. 초기 상태 설정
        ChangeState(MonsterStateType.Spawn);

        Debug.Log(bossMonster.CurrentPhaseData.phaseName);
    }
    public void InitializeBehaviorTree()
    {
        behaviorTree = new Selector(this,
        //페이즈 전환 체크 및 실행
        new BTSequence(this,
            new CheckPhaseTransitionNode(this),
            new ExecutePhaseTransitionNode(this)
        ),
        // 기믹 체크
        new BTSequence(this,
        new CheckGimmickNode(this),
        new ExecuteGimmickNode(this)
        ),
        // 기존 로직
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
        // 1. 기존 전략들 초기화
        spawnStrategy = StrategyFactory.CreateSpawnStrategy(data.spawnStrategy);
        moveStrategy = StrategyFactory.CreateMovementStrategy(data.moveStrategy);
        idleStrategy = StrategyFactory.CreatIdleStrategy(data.idleStrategy);
        dieStrategy = StrategyFactory.CreatDieStrategy(data.dieStrategy);
        hitStrategy = StrategyFactory.CreatHitStrategy(data.hitStrategy);
        groggyStrategy = StrategyFactory.CreateGroggyStrategy(data.groggyStrategy, data);

        // 2. 보스 전용 전략 초기화
        if (data is BossData bossData)
        {
            // 공격 전략 초기화 (globalMultiAttackStrategy 사용)
            foreach (var strategyData in bossMonster.CurrentPhaseData.phaseAttackStrategies)
            {
                var strategy = StrategyFactory.CreateAttackStrategy(strategyData.type, bossData, this);
                if (strategy != null)
                {
                    globalMultiAttackStrategy.AddStrategy(strategy, strategyData.weight);
                }
            }
            attackStrategy = globalMultiAttackStrategy;

            // 멀티 스킬 전략 초기화
            BossMultiSkillStrategy multiSkillStrategy = new BossMultiSkillStrategy(this);

            // 현재 페이즈의 스킬 구성 추가
            PhaseData currentPhaseData = bossMonster.CurrentPhaseData;

            for (int i = 0; i < currentPhaseData.skillConfigIds.Count; i++)
            {
                int configId = currentPhaseData.skillConfigIds[i];
                float weight = i < currentPhaseData.skillConfigWeights.Count ?
                              currentPhaseData.skillConfigWeights[i] : 1.0f;

                multiSkillStrategy.AddSkillStrategyFromConfig(configId, weight, this, bossData);
            }

            // 범위 설정
            multiSkillStrategy.SkillRange = data.skillRange;

            // 전략 설정
            skillStrategy = multiSkillStrategy;

            // 페이즈 및 기믹 전략 초기화
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
            // 일반 몬스터의 경우 기본 스킬 전략 사용
            skillStrategy = StrategyFactory.CreateSkillStrategy(data.skillStrategy, this);
        }

        // 기존의 스킬 이펙트 초기화
        //InitializeSkillEffect(data);
    }

    /// <summary>
    /// 보스의 현재 페이즈에 따라 모든 전략(공격, 스킬, 패턴)을 업데이트합니다.
    /// </summary>
    /// <remarks>
    /// 페이즈 전환 시 호출되어 보스의 행동 패턴을 새로운 페이즈에 맞게 조정합니다.
    /// </remarks>
    public void UpdatePhaseStrategies()
    {
        if (bossMonster == null) return;
        IMonsterClass monsterClass = creatureStatus.GetMonsterClass();
        ICreatureData data = monsterClass.GetMonsterData();
        var currentPhase = bossMonster.CurrentPhaseData;

        // 전역 BossMultiAttackStrategy 인스턴스의 내부 상태(전략 리스트, 타이머 등)를 완전히 초기화합니다.
        globalMultiAttackStrategy.ResetAll();

        // 현재 페이즈의 공격 전략 데이터를 globalMultiAttackStrategy에 추가합니다.
        foreach (var strategyData in currentPhase.phaseAttackStrategies)
        {
            var strategy = StrategyFactory.CreateAttackStrategy(strategyData.type, bossMonster.GetBossData(), this);
            if (strategy != null)
            {
                globalMultiAttackStrategy.AddStrategy(strategy, strategyData.weight);
            }
        }

        // 스킬 전략 업데이트
        if (skillStrategy is BossMultiSkillStrategy multiSkillStrategy)
        {
            // 기존 스킬 전략 리셋
            multiSkillStrategy.ResetAll();

            // 현재 페이즈의 스킬 구성 추가
            for (int i = 0; i < currentPhase.skillConfigIds.Count; i++)
            {
                int configId = currentPhase.skillConfigIds[i];
                float weight = i < currentPhase.skillConfigWeights.Count ?
                              currentPhase.skillConfigWeights[i] : 1.0f;

                multiSkillStrategy.AddSkillStrategyFromConfig(configId, weight, this, bossMonster.GetBossData());
            }           
            // 스킬 사용 방지를 위한 타이머 리셋 (페이즈 전환 후 바로 스킬 사용 방지)
            multiSkillStrategy.ResetTimer(1.5f);
        }

        // 패턴 업데이트: 현재 페이즈에 해당하는 패턴이 있다면 패턴 전략을 새로 생성하여 상태에 할당합니다.
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
            Debug.Log("비었다");
        }
        if (states.ContainsKey(MonsterStateType.Attack))
        {
            Debug.Log("새전략으로 바꿈");
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
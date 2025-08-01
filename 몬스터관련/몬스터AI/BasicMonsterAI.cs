using DG.Tweening;
using static IMonsterState;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 기본 몬스터 AI 구현 클래스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조'
/// 
/// 주요 기능:
/// - CreatureAI 추상 클래스를 구체화한 일반 몬스터용 AI 구현
/// - 몬스터 상태별 전략 초기화 및 관리
/// - 행동 트리를 통한 몬스터 의사 결정 로직 구현
/// - 모듈식 스킬 구성 - 이펙트/이동/충돌 컴포넌트 분리
/// - StrategyFactory를 통한 전략 객체 생성 및 의존성 주입

public class BasicCreatureAI : CreatureAI
{
    // 전략들을 전역 변수로 관리


    private void Awake()
    {
        DOTween.Init();
        DOTween.SetTweensCapacity(500, 50);
    }
    protected override void Start()
    {
        base.Start();

       
    }

   
   
    protected override void InitializeStates()
    {
        IMonsterClass monsterClass = creatureStatus.GetMonsterClass();
        ICreatureData data = monsterClass.GetMonsterData();

        // 데이터의 전략 타입에 따라 전략 생성
        InitializeStrategies(data);
        InitializeSkillEffect(data);
        InitializeBehaviorTree();

        // 생성된 전략으로 상태 초기화
        states = new Dictionary<IMonsterState.MonsterStateType, IMonsterState>
        {
            { MonsterStateType.Spawn, new SpawnState(this, spawnStrategy) },
            { MonsterStateType.Idle, new IdleState(this, idleStrategy) },
            { MonsterStateType.Move, new MoveState(this, moveStrategy) },
            { MonsterStateType.Attack, new AttackState(this, attackStrategy) },
            { MonsterStateType.Skill, new SkillState(this, skillStrategy) },
            { MonsterStateType.Hit, new HitState(this, hitStrategy) },
            { MonsterStateType.Groggy, new GroggyState(this, groggyStrategy) },
            { MonsterStateType.Die, new DieState(this, dieStrategy) }
        };

        // 초기 상태 설정
        ChangeState(MonsterStateType.Spawn);
    }

    private void InitializeStrategies(ICreatureData data)
    {
        spawnStrategy = StrategyFactory.CreateSpawnStrategy(data.spawnStrategy);
        moveStrategy = StrategyFactory.CreateMovementStrategy(data.moveStrategy);
        attackStrategy = StrategyFactory.CreateAttackStrategy(data.attackStrategy, data,this);
        idleStrategy = StrategyFactory.CreatIdleStrategy(data.idleStrategy);
        skillStrategy = StrategyFactory.CreateSkillStrategy(data.skillStrategy, this);
        dieStrategy = StrategyFactory.CreatDieStrategy(data.dieStrategy);
        hitStrategy = StrategyFactory.CreatHitStrategy(data.hitStrategy);
        groggyStrategy = StrategyFactory.CreateGroggyStrategy(data.groggyStrategy, data);
    }

    private void InitializeSkillEffect(ICreatureData data)
    {
        ISkillEffect skillEffect = StrategyFactory.CreateSkillEffect(
            data.skillEffectType,
            data,
                this
        );

        if (skillEffect != null)
        {
            skillStrategy.Initialize(skillEffect);
            skillStrategy.SkillRange = data.skillRange;
        }
        else
        {
            Debug.LogError($"Failed to create skill effect for monster: {data.MonsterName}");
        }
    }

    public void InitializeBehaviorTree()
    {
        behaviorTree = new Selector(this,
        

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
        ), 0.3f),

     new TimeDelayDecorator(this,
        new BTSequence(this,
        new CheckPlayerInRange(this),
            new ChangeStateAction(this, MonsterStateType.Move, MovementStrategyType.Basic)
        ),
        0.2f
    )
    );
    }

    protected override void Update()
    {
        base.Update();
        behaviorTree?.Execute();
        
    }

    #region Strategy Getters
    public override IAttackStrategy GetAttackStrategy() => attackStrategy;
    public override ISkillStrategy GetSkillStrategy() => skillStrategy;
    public override ISpawnStrategy GetSpawnStrategy() => spawnStrategy;
    public override IMovementStrategy GetMovementStrategy() => moveStrategy;
    public override IIdleStrategy GetIdleStrategy() => idleStrategy;
    public override IDieStrategy GetDieStrategy() => dieStrategy;
    public override IHitStrategy GetHitStrategy() => hitStrategy;
    public override IGroggyStrategy GetGroggyStrategy() => groggyStrategy;
    #endregion

    #region Strategy Setters
    public override void SetMovementStrategy(IMovementStrategy newStrategy)
    {
        moveStrategy = newStrategy;
        if (states.ContainsKey(MonsterStateType.Move))
        {
            states[MonsterStateType.Move] = new MoveState(this, moveStrategy);
        }
    }

    public override void SetAttackStrategy(IAttackStrategy newStrategy)
    {
        attackStrategy = newStrategy;
        if (states.ContainsKey(MonsterStateType.Attack))
        {
            states[MonsterStateType.Attack] = new AttackState(this, attackStrategy);
        }
    }
    #endregion

    public void OnAttackAnimationEnd()
    {
        attackStrategy.OnAttackAnimationEnd();
    }

    public override void SetSkillStrategy(ISkillStrategy newStrategy)
    {
        throw new System.NotImplementedException();
    }

    public override void SetIdleStrategy(IIdleStrategy newStrategy)
    {
        throw new System.NotImplementedException();
    }

    public override IPhaseTransitionStrategy GetPhaseTransitionStrategy()
    {
        throw new System.NotImplementedException();
    }

    public override IGimmickStrategy GetGimmickStrategy()
    {
        throw new System.NotImplementedException();
    }
}
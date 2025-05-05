using DG.Tweening;
using static IMonsterState;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �⺻ ���� AI ���� Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����'
/// 
/// �ֿ� ���:
/// - CreatureAI �߻� Ŭ������ ��üȭ�� �Ϲ� ���Ϳ� AI ����
/// - ���� ���º� ���� �ʱ�ȭ �� ����
/// - �ൿ Ʈ���� ���� ���� �ǻ� ���� ���� ����
/// - ���� ��ų ���� - ����Ʈ/�̵�/�浹 ������Ʈ �и�
/// - StrategyFactory�� ���� ���� ��ü ���� �� ������ ����

public class BasicCreatureAI : CreatureAI
{
    // �������� ���� ������ ����


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

        // �������� ���� Ÿ�Կ� ���� ���� ����
        InitializeStrategies(data);
        InitializeSkillEffect(data);
        InitializeBehaviorTree();

        // ������ �������� ���� �ʱ�ȭ
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

        // �ʱ� ���� ����
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
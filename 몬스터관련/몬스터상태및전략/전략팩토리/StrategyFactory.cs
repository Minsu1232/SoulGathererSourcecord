using UnityEngine;
/// <summary>
/// �پ��� ���� ���� ��ü�� �����ϴ� ���丮 Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 2�� '���� ������ ����' > '���丮 ����(Factory)'
/// 
/// �ֿ� ���:
/// - ������ ��� ���� ��ü ���� �� �ʱ�ȭ
/// - ���º� ���� ��ü(����, �̵�, ��ų ��) ����
/// - ������Ÿ�� �̵� �� �浹 ȿ�� ����
/// - ������ ������ ���� ��ü �� ���յ� ����
/// </remarks>
public static class StrategyFactory
{
    public static ISpawnStrategy CreateSpawnStrategy(SpawnStrategyType type)
    {
        return type switch
        {         
            _ => new BasicSpawnStrategy()
        };
    }

    public static IMovementStrategy CreateMovementStrategy(MovementStrategyType type)
    {
        return type switch
        {    
            MovementStrategyType.Retreat => new RetreatMovementStrategy(),
            _ => new BasicMovementStrategy()
        };
    }

    public static IAttackStrategy CreateAttackStrategy(AttackStrategyType type, ICreatureData data, CreatureAI creatureAI)
    {
        return type switch
        {
            AttackStrategyType.Jump => new JumpAttackStrategy(data.shorckEffectPrefab,data.shockwaveRadius, creatureAI),
            AttackStrategyType.Combo => new ComboAttackStrategy(),
            AttackStrategyType.Charge => new ChargeAttackStrategy(creatureAI,data,creatureAI.animator),
            _ => new BasicAttackStrategy()
        };;
    }

    public static IIdleStrategy CreatIdleStrategy(IdleStrategyType type)
    {
        return type switch
        {           
            _ => new BasicIdleStrategy()
        };
    }

    public static ISkillStrategy CreateSkillStrategy(SkillStrategyType type, CreatureAI owner)
    {
        return type switch
        {
            SkillStrategyType.Buff => new BuffSkillStrategy(owner),     
            SkillStrategyType.MultiShot => new MultiShotSkillStrategy(owner),
            SkillStrategyType.Area => new AreaSkillStrategy(owner),
            _ => new BasicSkillStrategy(owner)
        };
    }

    public static IDieStrategy CreatDieStrategy(DieStrategyType type)
    {
        return type switch
        {       
            _ => new BasicDieStrategy()
        };
    }

    public static IHitStrategy CreatHitStrategy(HitStrategyType type)
    {
        return type switch
        {      
            _ => new BasicHitStrategy()
        };
    }

    public static IProjectileMovement CreateProjectileMovement(ProjectileMovementType type, ICreatureData data)
    {
        return type switch
        {
            ProjectileMovementType.Homing => new HomingMovement(),
            ProjectileMovementType.Parabolic => new ParabolicMovement(data.heightFactor),
            ProjectileMovementType.Straight => new StraightMovement(),
            ProjectileMovementType.StraightRotation => new StraightRotationMovement(
                data.projectileRotationAxis,  // ICreatureData ������ ȸ����
                data.projectileRotationSpeed  // ICreatureData ������ ȸ�� �ӵ�
            ),
            ProjectileMovementType.None => null
        

        };
    }

    public static IProjectileImpact CreateProjectileImpact(ProjectileImpactType type, ICreatureData data)
    {
        return type switch
        {
            ProjectileImpactType.Poison => new AreaImpact(
                data.areaEffectPrefab,
                data.areaDuration,
                data.areaRadius
            ),
            ProjectileImpactType.DelayedExplosion => new DelayedExplosionImpact(
            data.safeZoneRadius,             // ���� ���� �ݰ�
            data.dangerRadiusMultiplier,     // ���� ���� ���
            data.skillDuration,              // ���� ���� �ð�
            true,                            // �⺻������ ����(������) ����
            data.ExplosionEffect                   // ���� ����Ʈ
        ),
            
            _ => null
        };
    }
    public static IGroggyStrategy CreateGroggyStrategy(GroggyStrategyType type, ICreatureData data)
    {
        return type switch
        {         
            _ => new BasicGroggyStrategy(data.groggyTime)
        };
    }
    public static ISkillEffect CreateSkillEffect(SkillEffectType effectType, ICreatureData data, CreatureAI owner)
    {
        switch (effectType)
        {
            case SkillEffectType.Projectile:
                if (data.projectilePrefab == null)
                {
                    Debug.LogError($"Projectile prefab is missing for monster: {data.MonsterName}");
                    return null;
                }
                var moveStrategy = CreateProjectileMovement(data.projectileType,data);
                var impactEffect = CreateProjectileImpact(data.projectileImpactType, data);
                return new ProjectileSkillEffect(
                    data.projectilePrefab,
                    data.projectileSpeed,
                    moveStrategy,
                    impactEffect,
                    data.hitEffect,
                    data.heightFactor
                );

            case SkillEffectType.AreaEffect:
                if (data.areaEffectPrefab == null)
                {
                    Debug.LogError($"Area effect prefab is missing for monster: {data.MonsterName}");
                    return null;
                }
                return new AreaSkillEffect(
                    data.areaEffectPrefab,
                    data.areaRadius,
                    data.skillDamage
                );
            case SkillEffectType.Howl:
                return new HowlSkillEffect(
                    data.howlEffectPrefab,
                    data.areaEffectPrefab,
                    data.howlSound,
                    data.howlRadius,
                    data.EssenceAmount,
                    data.howlDuration,
                    data.skillDamage,
                    owner.transform,
                    "@@@@@@@@@������",
                    data.circleIndicatorPrefab
                );
            case SkillEffectType.CircularProjectile:
                if (data.projectilePrefab == null)
                {
                    Debug.LogError($"Projectile prefab is missing for monster: {data.MonsterName}");
                    return null;
                }
                var moveStrategy_ = CreateProjectileMovement(data.projectileType, data);
                var impactEffect_ = CreateProjectileImpact(data.projectileImpactType, data);
                return new CircularProjectileSkillEffect(
                    data.projectilePrefab,
                    data.circleIndicatorPrefab,
                    data.projectileSpeed,
                    moveStrategy_,
                    impactEffect_,
                    data.hitEffect,
                    data.heightFactor
                );

            case SkillEffectType.Buff:
                return new BuffSkillEffect(
                   data.buffData.buffTypes,    // ���� ���� Ÿ�� �迭
        data.buffData.durations,    // �� ������ ���ӽð� �迭
        data.buffData.values,        // �� ������ ��ġ�� �迭
        data.buffEffectPrefab,
        owner.transform
                );

            case SkillEffectType.Summon:
                if (data.summonPrefab == null)
                {
                    Debug.LogError($"Summon prefab is missing for monster: {data.MonsterName}");
                    return null;
                }
                return new SummonSkillEffect(
                    data.summonPrefab,
                    data.summonCount,
                    data.summonRadius  // MonsterData�� �߰� �ʿ�
                );

            default:
                Debug.LogError($"Unknown skill effect type: {effectType} for monster: {data.MonsterName}");
                return null;
        }
    }

}

using UnityEngine;
/// <summary>
/// 다양한 몬스터 전략 객체를 생성하는 팩토리 클래스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 2절 '적용 디자인 패턴' > '팩토리 패턴(Factory)'
/// 
/// 주요 기능:
/// - 데이터 기반 전략 객체 생성 및 초기화
/// - 상태별 전략 객체(공격, 이동, 스킬 등) 생성
/// - 프로젝타일 이동 및 충돌 효과 생성
/// - 의존성 주입을 통한 객체 간 결합도 감소
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
                data.projectileRotationAxis,  // ICreatureData 설정한 회전축
                data.projectileRotationSpeed  // ICreatureData 설정한 회전 속도
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
            data.safeZoneRadius,             // 안전 영역 반경
            data.dangerRadiusMultiplier,     // 위험 영역 배수
            data.skillDuration,              // 폭발 지연 시간
            true,                            // 기본값으로 링형(빨간색) 설정
            data.ExplosionEffect                   // 폭발 이펙트
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
                    "@@@@@@@@@여기임",
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
                   data.buffData.buffTypes,    // 여러 버프 타입 배열
        data.buffData.durations,    // 각 버프의 지속시간 배열
        data.buffData.values,        // 각 버프의 수치값 배열
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
                    data.summonRadius  // MonsterData에 추가 필요
                );

            default:
                Debug.LogError($"Unknown skill effect type: {effectType} for monster: {data.MonsterName}");
                return null;
        }
    }

}

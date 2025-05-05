
using UnityEngine;
/// <summary>
/// 일반화된 영역 생성 효과
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 3절 '시스템 핵심 특징' > '모듈식 스킬 구성'
/// 
/// 주요 기능:
/// - 충돌 지점에 영역 이펙트 생성
/// - 영역 지속 시간 및 범위 설정
/// - 데미지 영역과 연동
/// </remarks>
public class AreaImpact : IProjectileImpact
{
    private GameObject areaEffectPrefab;
    private float duration;
    private float radius;

    public AreaImpact(GameObject areaEffectPrefab, float duration, float radius)
    {
        this.areaEffectPrefab = areaEffectPrefab;
        this.duration = duration;
        this.radius = radius;
    }

    public void OnImpact(Vector3 impactPosition, float damage)
    {
        if (areaEffectPrefab == null) return;

        GameObject areaEffect = Object.Instantiate(areaEffectPrefab, impactPosition, Quaternion.identity);
        if (areaEffect.TryGetComponent<IDamageArea>(out var damageArea))
        {            
            damageArea.Initialize(damage, duration, radius);
        }
    }
}
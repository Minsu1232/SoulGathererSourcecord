using System;
using UnityEngine;
/// <summary>
/// 범위 공격 효과를 구현하는 스킬 효과 클래스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 3절 '시스템 핵심 특징' > '모듈식 스킬 구성'
/// 
/// 주요 기능:
/// - 지정된 위치에 영역 이펙트 생성
/// - 범위 내 플레이어에게 데미지 적용
/// - 이펙트 프리팹과 데미지 값 통합 관리
/// </remarks>
public class AreaSkillEffect : ISkillEffect
{
    private GameObject areaEffectPrefab;
    private float radius;
    private ICreatureStatus monsterStatus;
    private Transform target;
    private float damage;

    public event Action OnEffectCompleted;

    public Transform transform { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public AreaSkillEffect(GameObject prefab, float radius, float damage)
    {
        this.areaEffectPrefab = prefab;
        this.radius = radius;
        this.damage = damage;
    }

    public void Initialize(ICreatureStatus status, Transform target)
    {
        this.monsterStatus = status;
        this.target = target;
    }

    public void Execute()
    {
        GameObject effect = GameObject.Instantiate(areaEffectPrefab,
            target.position,
            Quaternion.identity);
        // 범위 공격 로직 구현
    }

    public void OnComplete()
    {
        // 범위 공격 완료 처리
    }
}
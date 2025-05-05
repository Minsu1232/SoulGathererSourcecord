
using UnityEngine;
/// <summary>
/// 프로젝타일이 가질 수 있는 효과들의 인터페이스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 3절 '시스템 핵심 특징' > '모듈식 스킬 구성'
/// 
/// 주요 기능:
/// - 발사체 충돌 시 효과 처리
/// - 다양한 충돌 효과 구현체 지원
/// </remarks>
public interface IProjectileImpact
{
    void OnImpact(Vector3 impactPosition, float damage);
}
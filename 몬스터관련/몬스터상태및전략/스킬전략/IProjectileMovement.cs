
using UnityEngine;
/// <summary>
/// 발사체 이동 전략 인터페이스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 3절 '시스템 핵심 특징' > '모듈식 스킬 구성'
/// 
/// 주요 기능:
/// - 다양한 발사체 이동 패턴 정의
/// - 이동 로직과 시각 효과 분리
/// </remarks>
public interface IProjectileMovement
{
    void Move(Transform projectileTransform, Transform target, float speed);
}
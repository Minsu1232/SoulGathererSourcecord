using System;
using UnityEngine;
/// <summary>
/// 몬스터 스킬의 시각 효과 및 데미지 적용을 담당하는 인터페이스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 3절 '시스템 핵심 특징' > '모듈식 스킬 구성'
/// 
/// 주요 기능:
/// - 스킬 효과 초기화 및 실행
/// - 효과 완료 시 이벤트 발생
/// - 다양한 시각 효과와 데미지 로직 분리
/// </remarks>
public interface ISkillEffect
{
    void Initialize(ICreatureStatus status, Transform target);
    void Execute();
    void OnComplete();

  
    event Action OnEffectCompleted;


}
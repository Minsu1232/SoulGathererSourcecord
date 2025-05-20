using UnityEngine;
/// <summary>
/// 포물선 궤적으로 발사체를 이동시키는 전략 구현
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 3절 '시스템 핵심 특징' > '모듈식 스킬 구성'
/// 
/// 주요 기능:
/// - 시간에 따른 포물선 궤적 계산
/// - 목표 거리에 따른 높이 자동 조정
/// - 발사체 방향 자연스럽게 회전
/// </remarks>
public class ParabolicMovement : IProjectileMovement
{
   // 포물선 높이 계수 (높을수록 더 높게 솟아오름)
    private float heightFactor = 0.7f; //기본값

  public ParabolicMovement(float heightFactor) 
    {
        this.heightFactor = heightFactor;

    }

    public void Move(Transform projectile, Transform target, float speed)
    {
        BaseProjectile baseProjectile = projectile.GetComponent<BaseProjectile>();
        if (!baseProjectile.isInitialized)
        {
            baseProjectile.isInitialized = true;
        }

        baseProjectile.elapsedTime += Time.deltaTime;
        
        // 속도를 고려한 총 비행 시간 계산
        float distance = Vector3.Distance(baseProjectile.startPos, baseProjectile.targetPosition);
        float totalFlightTime = distance / speed;

        // 진행도 계산 (0~1 사이 값)
        float t = Mathf.Clamp01(baseProjectile.elapsedTime / totalFlightTime);
        
        // 위치 계산 (선형 보간 + 포물선 높이)
        Vector3 linearPosition = Vector3.Lerp(baseProjectile.startPos, baseProjectile.targetPosition, t);
        
        // 포물선 아치 높이 계산 (속도에 따라 적절히 조정)
        float arcHeight = Mathf.Max(2f, distance * heightFactor * (speed / 20f));
        
        // 포물선 높이 계수 (0에서 최대 높이까지 올라갔다가 다시 0으로)
        float heightProgress = 4f * t * (1f - t);
        
        // 최종 위치 계산
        Vector3 position = linearPosition + new Vector3(0, arcHeight * heightProgress, 0);

        // 방향 계산 (현재 위치 → 다음 계산될 위치)
        float nextT = Mathf.Clamp01(t + 0.01f);
        Vector3 nextPosition = Vector3.Lerp(baseProjectile.startPos, baseProjectile.targetPosition, nextT) +
                             new Vector3(0, arcHeight * 4f * nextT * (1f - nextT), 0);

        Vector3 direction = (nextPosition - position).normalized;
        
        // 위치 및 방향 설정
        projectile.position = position;
        if (direction != Vector3.zero)
        {
            projectile.forward = direction;
        }
        
        // 목적지 도착 시 처리
        if (t >= 1.0f)
        {
            projectile.position = baseProjectile.targetPosition;
        }
    }
}
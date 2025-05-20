using UnityEngine;
/// <summary>
/// ������ �������� �߻�ü�� �̵���Ű�� ���� ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 3�� '�ý��� �ٽ� Ư¡' > '���� ��ų ����'
/// 
/// �ֿ� ���:
/// - �ð��� ���� ������ ���� ���
/// - ��ǥ �Ÿ��� ���� ���� �ڵ� ����
/// - �߻�ü ���� �ڿ������� ȸ��
/// </remarks>
public class ParabolicMovement : IProjectileMovement
{
   // ������ ���� ��� (�������� �� ���� �ھƿ���)
    private float heightFactor = 0.7f; //�⺻��

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
        
        // �ӵ��� ����� �� ���� �ð� ���
        float distance = Vector3.Distance(baseProjectile.startPos, baseProjectile.targetPosition);
        float totalFlightTime = distance / speed;

        // ���൵ ��� (0~1 ���� ��)
        float t = Mathf.Clamp01(baseProjectile.elapsedTime / totalFlightTime);
        
        // ��ġ ��� (���� ���� + ������ ����)
        Vector3 linearPosition = Vector3.Lerp(baseProjectile.startPos, baseProjectile.targetPosition, t);
        
        // ������ ��ġ ���� ��� (�ӵ��� ���� ������ ����)
        float arcHeight = Mathf.Max(2f, distance * heightFactor * (speed / 20f));
        
        // ������ ���� ��� (0���� �ִ� ���̱��� �ö󰬴ٰ� �ٽ� 0����)
        float heightProgress = 4f * t * (1f - t);
        
        // ���� ��ġ ���
        Vector3 position = linearPosition + new Vector3(0, arcHeight * heightProgress, 0);

        // ���� ��� (���� ��ġ �� ���� ���� ��ġ)
        float nextT = Mathf.Clamp01(t + 0.01f);
        Vector3 nextPosition = Vector3.Lerp(baseProjectile.startPos, baseProjectile.targetPosition, nextT) +
                             new Vector3(0, arcHeight * 4f * nextT * (1f - nextT), 0);

        Vector3 direction = (nextPosition - position).normalized;
        
        // ��ġ �� ���� ����
        projectile.position = position;
        if (direction != Vector3.zero)
        {
            projectile.forward = direction;
        }
        
        // ������ ���� �� ó��
        if (t >= 1.0f)
        {
            projectile.position = baseProjectile.targetPosition;
        }
    }
}

using UnityEngine;
/// <summary>
/// �߻�ü �̵� ���� �������̽�
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 3�� '�ý��� �ٽ� Ư¡' > '���� ��ų ����'
/// 
/// �ֿ� ���:
/// - �پ��� �߻�ü �̵� ���� ����
/// - �̵� ������ �ð� ȿ�� �и�
/// </remarks>
public interface IProjectileMovement
{
    void Move(Transform projectileTransform, Transform target, float speed);
}
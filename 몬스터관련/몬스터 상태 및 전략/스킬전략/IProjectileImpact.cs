
using UnityEngine;
/// <summary>
/// ������Ÿ���� ���� �� �ִ� ȿ������ �������̽�
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 3�� '�ý��� �ٽ� Ư¡' > '���� ��ų ����'
/// 
/// �ֿ� ���:
/// - �߻�ü �浹 �� ȿ�� ó��
/// - �پ��� �浹 ȿ�� ����ü ����
/// </remarks>
public interface IProjectileImpact
{
    void OnImpact(Vector3 impactPosition, float damage);
}
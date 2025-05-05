using System;
using UnityEngine;
/// <summary>
/// ���� ��ų�� �ð� ȿ�� �� ������ ������ ����ϴ� �������̽�
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 3�� '�ý��� �ٽ� Ư¡' > '���� ��ų ����'
/// 
/// �ֿ� ���:
/// - ��ų ȿ�� �ʱ�ȭ �� ����
/// - ȿ�� �Ϸ� �� �̺�Ʈ �߻�
/// - �پ��� �ð� ȿ���� ������ ���� �и�
/// </remarks>
public interface ISkillEffect
{
    void Initialize(ICreatureStatus status, Transform target);
    void Execute();
    void OnComplete();

  
    event Action OnEffectCompleted;


}
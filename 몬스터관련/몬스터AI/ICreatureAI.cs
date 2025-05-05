// ICreatureAI.cs

using static IMonsterState;
/// <summary>
/// ��� ����ü AI�� ���� �������̽�
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����'
/// 
/// �ֿ� ���:
/// - ���� ���� ��ȯ ��� ����
/// - ���� ���� ��ȸ ���
/// - ������ ó�� ���� ����
/// - ���� ���� ��ȸ �������̽�
public interface ICreatureAI
{
    //���º�ȭ
    void ChangeState(MonsterStateType newState);
    //������� Get
    IMonsterState GetCurrentState();
    //�ǰ�
    void OnDamaged(int damage);
    //Status�� ���� Ÿ�� �Ǻ�
    ICreatureStatus GetStatus();
}
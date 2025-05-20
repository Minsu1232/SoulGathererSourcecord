/// <summary>
/// ���� ���� �������̽�
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 2�� '���� ������ ����' > '���� ����(FSM)'
/// 
/// �ֿ� ���:
/// - ������ �پ��� ���� Ÿ�� ����
/// - ���� ����, ����, ���� �� ��ȯ ���� ���θ� �����ϴ� �޼��� ����
/// </remarks>
public interface IMonsterState
{
    public enum MonsterStateType
    {
        Spawn,
        Idle,
        Move,
        Attack,
        Skill,
        Hit,
        Groggy,
        PhaseTransition,
        Gimmick,
        Pattern,
        Die
    }
    void Enter();
    void Execute();
    void Exit();
    bool CanTransition(); // ���� ���¿��� ��ȯ ��������
}
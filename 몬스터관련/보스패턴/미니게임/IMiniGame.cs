/// <summary>
/// �̴ϰ��� ����� Ÿ�� ����
/// </summary>
public enum MiniGameResult
{
    Perfect,
    Good,
    Miss,
    Cancel
}

public enum MiniGameType
{
    None,
    Dodge,
    Parry,
    QuickTime
}
/// <summary>
/// �̴ϰ��� �⺻ �������̽�
/// </summary>
/// <remarks>
/// ������� ����: 3�� '���� �ý��� ���� �� Ȯ�� ���' > 1�� '���� ���� �ý���' > '����'
/// 
/// �ֿ� ���:
/// - �̴ϰ��� �ʱ�ȭ �� ����
/// - ���̵� ����
/// - �̴ϰ��� ��� ��ȯ
/// - �پ��� �̴ϰ��� ����ü ����
/// </remarks>
// �⺻ �������̽�
public interface IMiniGame
{
    MiniGameType Type { get; }
    bool IsComplete { get; }
    void Initialize(float difficulty);
    void Start();
    void Update();
    void Cancel();
    MiniGameResult GetResult();
}
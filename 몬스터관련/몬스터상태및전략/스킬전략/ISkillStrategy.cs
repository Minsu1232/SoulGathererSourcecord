using UnityEngine;
/// <summary>
/// ���� ��ų ������ �����ϴ� �������̽�
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 2�� '���� ������ ����' > '���� ����(Strategy)'
/// 
/// �ֿ� ���:
/// - ��ų ��� ���� ���� �Ǵ�
/// - ��ų ȿ������ ����
/// - ��ų ���� �� ������Ʈ ����
/// - ��ų ��ٿ� �� ���� ����
/// </remarks>
public interface ISkillStrategy
{
    void Initialize(ISkillEffect skillEffect);
    void StartSkill(Transform transform, Transform target, IMonsterClass monsterData);
    void UpdateSkill(Transform transform, Transform target, IMonsterClass monsterData);
    
    bool IsSkillComplete { get; }
    bool CanUseSkill(float distanceToTarget, IMonsterClass monsterData);
    bool IsUsingSkill { get; }
    float GetLastSkillTime { get; }

    float SkillRange { get; set; }
}
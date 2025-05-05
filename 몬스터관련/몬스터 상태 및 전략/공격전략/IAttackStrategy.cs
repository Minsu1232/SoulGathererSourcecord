// ���� ���� Ÿ�� ����
using UnityEngine;
/// <summary>
/// ���� ���� Ÿ�� ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����'
/// 
/// �ֿ� Ÿ��:
/// - Basic: �⺻ ���� (���� ��ġ�� ��)
/// - Jump: ���� ����
/// - Charge: ���� ����
/// - Combo: ���� ����
/// - Spin: ȸ�� ����
public enum PhysicalAttackType
{
    Basic,      // �⺻ ���� (�ִϸ��̼� ������)
    Jump,       // ���� ����
    Charge,     // ���� ����    
    Spin,       // ȸ�� ����
    // �߰� ����
}
/// <summary>
/// Ȯ��� ���� ���� �������̽�
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 2�� '���� ������ ����' > '���� ����(Strategy)'
/// 
/// �ֿ� ���:
/// - �پ��� ���� ���� Ÿ�� ����
/// - ���� ���� ����
/// - �ִϸ��̼� Ʈ���� ����
/// - ���ݷ� ���� ����
/// - ������ ���� ����
/// </remarks>
// Ȯ��� ���� ���� �������̽�
public interface IAttackStrategy
{
    void Attack(Transform transform, Transform target, IMonsterClass monsterData);
    bool CanAttack(float distanceToTarget, IMonsterClass monsterData);
    void StartAttack();
    void StopAttack();
    void ApplyDamage(IDamageable target, IMonsterClass monsterData);
    bool IsAttacking { get; }
    float GetLastAttackTime { get; }
    void OnAttackAnimationEnd();
    void ResetAttackTime();
    // ���� �߰��Ǵ� �Ӽ���
    PhysicalAttackType AttackType { get; }
    string GetAnimationTriggerName();
    float GetAttackPowerMultiplier(); // ���ݷ� ����
}
using System;
using UnityEngine;
/// <summary>
/// ���� ���� ȿ���� �����ϴ� ��ų ȿ�� Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 3�� '�ý��� �ٽ� Ư¡' > '���� ��ų ����'
/// 
/// �ֿ� ���:
/// - ������ ��ġ�� ���� ����Ʈ ����
/// - ���� �� �÷��̾�� ������ ����
/// - ����Ʈ �����հ� ������ �� ���� ����
/// </remarks>
public class AreaSkillEffect : ISkillEffect
{
    private GameObject areaEffectPrefab;
    private float radius;
    private ICreatureStatus monsterStatus;
    private Transform target;
    private float damage;

    public event Action OnEffectCompleted;

    public Transform transform { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public AreaSkillEffect(GameObject prefab, float radius, float damage)
    {
        this.areaEffectPrefab = prefab;
        this.radius = radius;
        this.damage = damage;
    }

    public void Initialize(ICreatureStatus status, Transform target)
    {
        this.monsterStatus = status;
        this.target = target;
    }

    public void Execute()
    {
        GameObject effect = GameObject.Instantiate(areaEffectPrefab,
            target.position,
            Quaternion.identity);
        // ���� ���� ���� ����
    }

    public void OnComplete()
    {
        // ���� ���� �Ϸ� ó��
    }
}
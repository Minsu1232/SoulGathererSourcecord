// DashImpactComponent.cs - ��� �浹 �� ������ ��� ������Ʈ
using UnityEngine;
using System.Collections.Generic;

public class DashImpactComponent : MonoBehaviour
{
    private float damageMultiplier = 0f;   // �÷��̾� ���ݷ� ��� ������ ����
    private PlayerClass playerClass;
    private PlayerDashComponent dashComponent;

    [SerializeField] private LayerMask monsterLayer; // ���� ���̾� ����ũ
    [SerializeField] private float impactRadius = 1.5f; // �浹 ���� �ݰ�

    private void Awake()
    {
        playerClass = GameInitializer.Instance.GetPlayerClass();
        dashComponent = GetComponent<PlayerDashComponent>();

        if (playerClass == null || dashComponent == null)
        {
            Debug.LogError("DashImpactComponent: �ʿ��� ������Ʈ�� �����ϴ�.");
            enabled = false;
            return;
        }

        // ���� ���̾� ����
        if (monsterLayer == 0)
        {
            monsterLayer = LayerMask.GetMask("Monster");
        }

        // ��� �̺�Ʈ ����
        dashComponent.OnDashStart.AddListener(OnDashStart);
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        if (dashComponent != null)
        {
            dashComponent.OnDashStart.RemoveListener(OnDashStart);
        }
    }

    // ��� ���� �� �浹 üũ
    private void OnDashStart()
    {
        if (damageMultiplier > 0f)
        {
            Debug.Log("#@@");
            CheckImpact();
        }
    }

    // �浹 üũ �� ������ ����
    private void CheckImpact()
    {
        // �ֺ� ���� Ž��
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, impactRadius, monsterLayer);

        // �̹� ó���� ���� ��ü ������
        HashSet<ICreatureStatus> processedCreatures = new HashSet<ICreatureStatus>();

        foreach (var hitCollider in hitColliders)
        {
            // ������ ICreatureStatus ������Ʈ ã��
            ICreatureStatus creature = hitCollider.GetComponentInParent<ICreatureStatus>();

            // ��ȿ�� �����̰� ���� ó������ ���� ��쿡�� ������ ����
            if (creature != null && !processedCreatures.Contains(creature))
            {
                // ó���� ���ͷ� ���
                processedCreatures.Add(creature);

                // �÷��̾� ���ݷ� ���� ������ ���
                int damage = Mathf.RoundToInt(playerClass.PlayerStats.AttackPower * damageMultiplier);

                // ������ ����
                creature.TakeDamage(damage);

                Debug.Log($"��� ��� ������ ����: {damage} ������ (���ݷ� {playerClass.PlayerStats.AttackPower}�� {damageMultiplier * 100}%)");
            }
        }
    }

    // �浹 ������ ���� (���ݷ� ��� ����)
    public void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = Mathf.Max(0f, multiplier);
        Debug.Log($"��� ��� ������ ����: ���ݷ��� {damageMultiplier * 100}%");
    }

    // ���� �浹 ������ ���� ��ȯ
    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }

    // �浹 �ݰ� ����
    public void SetImpactRadius(float radius)
    {
        impactRadius = Mathf.Max(0.5f, radius);
    }

    // ����׿� �ð�ȭ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
// FlameAreaEffect.cs - �Ҳ� ������ ���� ȿ��
using System.Collections.Generic;
using UnityEngine;

public class FlameAreaEffect : BaseAreaEffect
{
    [SerializeField] private LayerMask monsterLayer;

    private void Awake()
    {
        // ���� ���̾� ����
        if (monsterLayer == 0)
        {
            monsterLayer = LayerMask.GetMask("Monster");
        }
    }

    protected override void ApplyAreaDamage()
    {
       
        // ���� �� ���� Ž��
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, monsterLayer);

        // �̹� ó���� ���� ��ü ����
        HashSet<ICreatureStatus> processedCreatures = new HashSet<ICreatureStatus>();

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider.gameObject.name); 
            // ������ ICreatureStatus ������Ʈ ã��
            ICreatureStatus creature = hitCollider.GetComponentInParent<ICreatureStatus>();

            // ��ȿ�� �����̰� ���� ó������ ���� ��쿡�� ������ ����
            if (creature != null && !processedCreatures.Contains(creature))
            {
                // ó���� ���ͷ� ���
                processedCreatures.Add(creature);

                // ������ ���� (tick �� ������)
                int tickDamage = Mathf.RoundToInt(damage * tickRate);
                creature.TakeDamage(tickDamage);

                Debug.Log($"�Ҳ� ������ ����: {tickDamage} ������");
            }
        }
    }

    // ����׿� �ð�ȭ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
// LightningJudgmentComponent.cs
using UnityEngine;
using System.Collections.Generic;
public class LightningJudgmentComponent : MonoBehaviour
{
    private float damageMultiplier = 0f; // �÷��̾� ���ݷ� ��� ���� ������ ����
    private PlayerClass playerClass;

    [SerializeField] private GameObject lightningEffectPrefab; // ���� VFX ������
    [SerializeField] private AudioClip thunderSound; // ���� ����
    [SerializeField] private float hitDelay = 0.1f; // ������ �������� ����
    

    private void Awake()
    {
        playerClass = GameInitializer.Instance.GetPlayerClass();
        if (playerClass == null)
        {
            Debug.LogError("LightningJudgmentComponent: PlayerClass�� ã�� �� �����ϴ�.");
            Destroy(this);
            return;
        }

        // �⺻ ������ �ε� (���� ���)
        if (lightningEffectPrefab == null)
        {
            // ��巹������ �ε��ϰų� Resources���� �ε�
            lightningEffectPrefab = GameInitializer.Instance.lightningEffect;
        }
  
    }

    // Ư�� ��ų ��� �� ȣ��� �޼���
    public void ExecuteLightningJudgment()
    {
        if (damageMultiplier <= 0f)
            return;

        StartCoroutine(StrikeLightningOnAllMonsters());
    }

    // ��� ���Ϳ��� ���� ����ġ��
    private System.Collections.IEnumerator StrikeLightningOnAllMonsters()
    {
        // ���� �Ŵ������� Ȱ��ȭ�� ���� ��� ��������
        List<ICreatureStatus> activeMonsters = DungeonManager.Instance.GetActiveMonsters();

        // �� ���Ϳ��� ���� ����ġ��
        foreach (var monster in activeMonsters)
        {
            // null üũ �� ���� ���� Ȯ��
            if (monster == null || monster.GetMonsterClass() == null || !monster.GetMonsterClass().IsAlive)
                continue;

            // ���� ��ġ ��������
            Vector3 monsterPosition;
            try
            {
                monsterPosition = monster.GetMonsterTransform().position;
            }
            catch (System.Exception)
            {
                // Transform�� �������µ� ������ ������ �ǳʶٱ�
                continue;
            }

            // ���� ����Ʈ ����
            GameObject lightningEffect = Instantiate(
                lightningEffectPrefab,
                new Vector3(monsterPosition.x, monsterPosition.y + 2f, monsterPosition.z),
                Quaternion.identity
            );

            // ���� ���� ���
            if (thunderSound != null)
            {
                AudioSource.PlayClipAtPoint(thunderSound, monsterPosition);
            }

            // ������ ��� �� ����
            int damage = CalculateLightningDamage();
            monster.TakeDamage(damage);

            Debug.Log($"���� ���� �ߵ�: {monster.GetType().Name}���� {damage} ������!");

            // ���� ����Ʈ�� ���� �ð� �� �ڵ� ����
            Destroy(lightningEffect, 2f);

            // ���� �������� �ణ�� ������
            yield return new WaitForSeconds(hitDelay);
        }
    }

    // ���� ������ ���
    private int CalculateLightningDamage()
    {
        int baseDamage = playerClass.PlayerStats.AttackPower;
        return Mathf.RoundToInt(baseDamage * damageMultiplier);
    }

    // ������ ���� ����
    public void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = Mathf.Max(0f, multiplier);
        Debug.Log($"���� ���� ������ ���� ����: ���ݷ��� {damageMultiplier * 100}%");
    }

    // ���� ������ ���� ��ȯ
    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }
}
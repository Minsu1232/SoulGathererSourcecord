// �������� Ŭ���� �� ü�� ȸ�� ������Ʈ
using UnityEngine;

public class StageHealComponent : MonoBehaviour
{
    private float healPercent = 0f;
    private PlayerClass playerClass;

    private void Awake()
    {
        playerClass = GameInitializer.Instance.GetPlayerClass();
        if (playerClass == null)
        {
            Debug.LogError("StageHealComponent: �÷��̾� Ŭ������ ã�� �� �����ϴ�.");
            Destroy(this);
            return;
        }

        // �������� Ŭ���� �̺�Ʈ ����
        DungeonManager.OnStageCleared += HandleStageCleared;
        Debug.Log("�������� ȸ�� ������Ʈ: �̺�Ʈ ���� �Ϸ�");
    }

    // �������� Ŭ���� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯
    private void HandleStageCleared()
    {
        if (healPercent > 0f && playerClass != null)
        {
            // �ִ� ü���� ������ŭ ȸ��
            int maxHealth = playerClass.PlayerStats.MaxHealth;
            int healthToRestore = Mathf.RoundToInt(maxHealth * healPercent);

            // �ּ� ȸ���� ���� (1 �̻�)
            healthToRestore = Mathf.Max(1, healthToRestore);

            // �÷��̾� ü�� ȸ��
            playerClass.ModifyPower(healthToRestore);
            Debug.Log($"�������� Ŭ���� ȸ�� �ߵ�: �ִ� ü�� {maxHealth}�� {healPercent * 100f}%�� {healthToRestore} ȸ��");

            // �ʿ��ϴٸ� ���⿡ ȸ�� ����Ʈ �߰�
            // PlayHealEffect();
        }
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        DungeonManager.OnStageCleared -= HandleStageCleared;
        Debug.Log("�������� ȸ�� ������Ʈ: �̺�Ʈ ���� ����");
    }

    // ȸ�� �ۼ�Ʈ �߰�
    public void AddHealPercent(float percent)
    {
        healPercent += percent;
        Debug.Log($"�������� Ŭ���� ȸ�� �ɷ� �߰�: ���� {healPercent * 100f}%");
    }

    // ȸ�� �ۼ�Ʈ ����
    public void RemoveHealPercent(float percent)
    {
        healPercent -= percent;
        healPercent = Mathf.Max(0f, healPercent);
        Debug.Log($"�������� Ŭ���� ȸ�� �ɷ� ����: ���� {healPercent * 100f}%");
    }

    // ���� ȸ�� �ۼ�Ʈ ��ȯ
    public float GetHealPercent()
    {
        return healPercent;
    }
}
// MovementAbility.cs - �̵� ���� �ɷ� Ŭ���� (DungeonAbility Ȯ��)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class MovementAbility : DungeonAbility
{
    public enum MovementAbilityType
    {
        DashForce,       // ��� �� ����
        DashInvincible,  // ��� �� ª�� ���� �ð�
        DashImpact,      // ��� �浹 �� ������ ������
        DashFlame        // ��� �� �Ҳ� �����Ͽ� ������
    }

    public MovementAbilityType movementType;


    // �����ڷ� �ʱ�ȭ
    public void Initialize(MovementAbilityType type, float value, string abilityName, string abilityDescription, Rarity abilityRarity)
    {
        movementType = type;
        effectValue = value;
        originalValue = value;

        id = $"movement_{type.ToString().ToLower()}";
        name = abilityName;
        description = abilityDescription;
        rarity = abilityRarity;
    }

    // CSV �����ͷ� �ʱ�ȭ�ϴ� �޼��� �߰�
    public static MovementAbility FromCSVData(Dictionary<string, string> csvData)
    {
        MovementAbility ability = new MovementAbility();

        // �ʼ� �� �Ľ�
        string id = csvData["ID"];
        MovementAbilityType type = (MovementAbilityType)System.Enum.Parse(typeof(MovementAbilityType), csvData["Type"]);
        string name = csvData["Name"];
        string description = csvData["Description"];
        Rarity rarity = (Rarity)int.Parse(csvData["Rarity"]);
        float baseValue = float.Parse(csvData["BaseValue"]);

        // ������ ���� �Ľ� �߰�
        if (csvData.ContainsKey("LevelMultiplier") && !string.IsNullOrEmpty(csvData["LevelMultiplier"]))
        {
            ability.levelMultiplier = float.Parse(csvData["LevelMultiplier"]);
        }

        // �ɷ� �ʱ�ȭ
        ability.Initialize(type, baseValue, name, description, rarity);
        ability.id = id; // ID �� ����� (����ũ�� ID ���)

        // ������ ��ΰ� �ִٸ� ��巹����� �ε�
        if (csvData.ContainsKey("IconPath") && !string.IsNullOrEmpty(csvData["IconPath"]))
        {
            string iconAddress = csvData["IconPath"];
            Debug.Log(iconAddress);
            // ��巹���� �񵿱� �ε�
            Addressables.LoadAssetAsync<Sprite>(iconAddress).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    ability.icon = handle.Result;
                    Debug.Log($"������ �ε� ����: {iconAddress}");
                }
                else
                {
                    Debug.LogWarning($"�������� �ε��� �� �����ϴ�: {iconAddress}");
                }
            };
        }
        else
        {
            Debug.LogWarning($"�������� �ε��� �� �����ϴ�");
        }

        return ability;
    }

    public override void OnAcquire(PlayerClass player)
    {
        // �̵� �ɷ� ȿ�� ����
        ApplyEffect(player, effectValue);

        // ����� �α�
        Debug.Log($"ȹ���� �̵� �ɷ�: {name} (Lv.{level}) - {effectValue}");
    }

    public override void OnLevelUp(PlayerClass player)
    {
        // ������ �÷��� ����
        isLevelingUp = true;

        // ���� ȿ�� ����
        OnReset(player);

        // ������ �� ȿ�� ����
        level++;
        effectValue = originalValue * (1 + (level - 1) * levelMultiplier);

        // �� ȿ�� ����
        OnAcquire(player);

        // ������ �÷��� ����
        isLevelingUp = false;

        // ����� �α�
        Debug.Log($"�������� �̵� �ɷ�: {name} (Lv.{level}) - {effectValue}");
    }

    public override void OnReset(PlayerClass player)
    {
        // �̵� �ɷ� ȿ�� ����
        RemoveEffect(player, effectValue);
    }

    // �̵� �ɷ� ȿ�� ����
    protected override void ApplyEffect(PlayerClass player, float value)
    {
        switch (movementType)
        {
            case MovementAbilityType.DashForce:
                ApplyDashForce(player, value);
                break;
            case MovementAbilityType.DashInvincible:
                // �ּ� ó��: ���� �������� ����
                ApplyDashInvincible(player, value);
                Debug.Log($"��� ���� ȿ���� ���� �������� �ʾҽ��ϴ�: {value}");
                break;
            case MovementAbilityType.DashImpact:
                // �ּ� ó��: ���� �������� ����
                ApplyDashImpact(player, value);
                Debug.Log($"��� ��� ȿ���� ���� �������� �ʾҽ��ϴ�: {value}");
                break;
            case MovementAbilityType.DashFlame:
                // �ּ� ó��: ���� �������� ����
                ApplyDashFlame(player, value);
                Debug.Log($"��� �Ҳ� ȿ���� ���� �������� �ʾҽ��ϴ�: {value}");
                break;
        }
    }

    // �̵� �ɷ� ȿ�� ����
    protected override void RemoveEffect(PlayerClass player, float value)
    {
        switch (movementType)
        {
            case MovementAbilityType.DashForce:
                RemoveDashForce(player, value);
                break;
            case MovementAbilityType.DashInvincible:

                RemoveDashInvincible(player, value);
                break;
            case MovementAbilityType.DashImpact:

                RemoveDashImpact(player, value);
                break;
            case MovementAbilityType.DashFlame:

                RemoveDashFlame(player, value);
                break;
        }
    }

    // ��� �� ȿ�� ����
    private void ApplyDashForce(PlayerClass player, float forceAmount)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        PlayerDashComponent dashComp = playerObj.GetComponent<PlayerDashComponent>();
        if (dashComp == null)
        {
            dashComp = playerObj.AddComponent<PlayerDashComponent>();
        }

        dashComp.IncreaseDashForce(forceAmount);
        Debug.Log($"��� �� ȿ�� ����: +{forceAmount}, ����: {dashComp.GetDashForce()}");
    }

    // ��� �� ȿ�� ����
    private void RemoveDashForce(PlayerClass player, float forceAmount)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        PlayerDashComponent dashComp = playerObj.GetComponent<PlayerDashComponent>();
        if (dashComp != null)
        {
            dashComp.IncreaseDashForce(-forceAmount); // ���������� ����
        }
    }

    // ��� ���� ȿ�� ����
    private void ApplyDashInvincible(PlayerClass player, float duration)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        PlayerDashComponent dashComp = playerObj.GetComponent<PlayerDashComponent>();
        if (dashComp == null)
        {
            dashComp = playerObj.AddComponent<PlayerDashComponent>();
        }

        // ��� ���� ������Ʈ ����
        DashInvincibleComponent invincibleComp = playerObj.GetComponent<DashInvincibleComponent>();
        if (invincibleComp == null)
        {
            invincibleComp = playerObj.AddComponent<DashInvincibleComponent>();
        }

        invincibleComp.SetInvincibleDuration(duration);
        Debug.Log($"��� ���� ȿ�� ����: {duration}��");
    }

    // ��� ���� ȿ�� ����
    private void RemoveDashInvincible(PlayerClass player, float duration)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        DashInvincibleComponent invincibleComp = playerObj.GetComponent<DashInvincibleComponent>();
        if (invincibleComp != null)
        {
            invincibleComp.SetInvincibleDuration(invincibleComp.GetInvincibleDuration() - duration);

            // ȿ���� 0�̸� ������Ʈ ���� (������ ���� �ƴ� ����)
            if (invincibleComp.GetInvincibleDuration() <= 0f && !isLevelingUp)
            {
                GameObject.Destroy(invincibleComp);
            }
        }
    }

    // ��� ��� ������ ȿ�� ����
    private void ApplyDashImpact(PlayerClass player, float multiplierAmount)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        PlayerDashComponent dashComp = playerObj.GetComponent<PlayerDashComponent>();
        if (dashComp == null)
        {
            dashComp = playerObj.AddComponent<PlayerDashComponent>();
        }

        // ��� �浹 ������ ������Ʈ ����  
        DashImpactComponent impactComp = playerObj.GetComponent<DashImpactComponent>();
        if (impactComp == null)
        {
            impactComp = playerObj.AddComponent<DashImpactComponent>();
        }

        // ���� ������ ������� ��ȯ (��: 20% -> 0.2)
        float multiplier = multiplierAmount / 100f;
        impactComp.SetDamageMultiplier(impactComp.GetDamageMultiplier() + multiplier);
        Debug.Log($"��� ��� ������ ȿ�� ����: ���ݷ��� {multiplier * 100}%");
    }

    // ��� ��� ������ ȿ�� ����
    private void RemoveDashImpact(PlayerClass player, float multiplierAmount)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        DashImpactComponent impactComp = playerObj.GetComponent<DashImpactComponent>();
        if (impactComp != null)
        {
            // ���� ������ ������� ��ȯ (��: 20% -> 0.2)
            float multiplier = multiplierAmount / 100f;
            impactComp.SetDamageMultiplier(impactComp.GetDamageMultiplier() - multiplier);

            // ȿ���� 0�̸� ������Ʈ ���� (������ ���� �ƴ� ����)
            if (impactComp.GetDamageMultiplier() <= 0f && !isLevelingUp)
            {
                GameObject.Destroy(impactComp);
            }
        }
    }

    private void ApplyDashFlame(PlayerClass player, float damageAmount)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        DashFlameComponent flameComp = playerObj.GetComponent<DashFlameComponent>();

        if (flameComp == null)
        {
            flameComp = playerObj.AddComponent<DashFlameComponent>();
        }

        // ������Ʈ Ȱ��ȭ
        flameComp.enabled = true;

        // ���� ���� ����
        float multiplier = damageAmount / 100f;
        flameComp.SetDamageMultiplier(flameComp.GetDamageMultiplier() + multiplier);

        // ��͵� ����
        flameComp.SetFlameRarity(this.rarity);

        Debug.Log($"��� �Ҳ� ȿ�� ����: ���ݷ��� {multiplier * 100}%, ��͵�: {this.rarity}");
    }

    // ��� �Ҳ� ȿ�� ����
    private void RemoveDashFlame(PlayerClass player, float damageAmount)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        DashFlameComponent flameComp = playerObj.GetComponent<DashFlameComponent>();
        if (flameComp != null)
        {
            // ���� ���� ����
            float multiplier = damageAmount / 100f;
            flameComp.SetDamageMultiplier(flameComp.GetDamageMultiplier() - multiplier);

            // ȿ���� 0�̸� ������Ʈ ��Ȱ��ȭ (������ ���� �ƴ� ����)
            if (flameComp.GetDamageMultiplier() <= 0f && !isLevelingUp)
            {
                flameComp.enabled = false;
            }
        }
    }
}
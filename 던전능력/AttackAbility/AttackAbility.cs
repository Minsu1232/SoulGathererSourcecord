// AttackAbility.cs - ���� �ɷ� Ŭ���� (DungeonAbility ���)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class AttackAbility : DungeonAbility
{
    public enum AttackAbilityType
    {
        ComboEnhancement,  // �޺� ��ȭ (����Ÿ�� ������ ����)
        WeakPrey,          // ���� ���� (ü���� ���� ������ �߰� ����)
        ChainStrike,       // ���� Ÿ�� (�߰� Ÿ�� Ȯ��)
        GaugeBoost,        // ������ ��ȭ (���� ������ ������ ����)
        ArmorCrush         // ���� �ı� (�� ���� ����)
    }

    public AttackAbilityType attackType;
  

    // �����ڷ� �ʱ�ȭ
    public void Initialize(AttackAbilityType type, float value, string abilityName, string abilityDescription, Rarity abilityRarity)
    {
        attackType = type;
        effectValue = value;
        originalValue = value;

        id = $"attack_{type.ToString().ToLower()}";
        name = abilityName;
        description = abilityDescription;
        rarity = abilityRarity;
    }

    // CSV �����ͷ� �ʱ�ȭ�ϴ� �޼��� �߰�
    public static AttackAbility FromCSVData(Dictionary<string, string> csvData)
    {
        AttackAbility ability = new AttackAbility();

        // �ʼ� �� �Ľ�
        string id = csvData["ID"];
        AttackAbilityType type = (AttackAbilityType)System.Enum.Parse(typeof(AttackAbilityType), csvData["Type"]);
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

        if (csvData.ContainsKey("IconPath") && !string.IsNullOrEmpty(csvData["IconPath"]))
        {
            string iconAddress = csvData["IconPath"];
            Debug.Log(iconAddress);
            // �ֵ巹���� �񵿱� �ε�
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
        // ���� �ɷ� ȿ�� ����
        ApplyEffect(player, effectValue);

        // ����� �α�
        Debug.Log($"ȹ���� ���� �ɷ�: {name} (Lv.{level}) - {effectValue}");
    }

    // OnLevelUp �޼��� ����
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
        Debug.Log($"�������� ���� �ɷ�: {name} (Lv.{level}) - {effectValue}");
    }

    public override void OnReset(PlayerClass player)
    {
        // ���� �ɷ� ȿ�� ����
        RemoveEffect(player, effectValue);
    }

    // ���� �ɷ� ȿ�� ����
    protected override void ApplyEffect(PlayerClass player, float value)
    {
        switch (attackType)
        {
            case AttackAbilityType.ComboEnhancement:
                ApplyComboEnhancement(player, value);
                break;
            case AttackAbilityType.WeakPrey:
                ApplyWeakPrey(player, value);
                break;
            case AttackAbilityType.ChainStrike:
                ApplyChainStrike(player, value);
                break;
            case AttackAbilityType.GaugeBoost:
                ApplyGaugeBoost(player, value);
                break;
            case AttackAbilityType.ArmorCrush:
                ApplyArmorCrush(player, value);
                break;
        }
    }

    // ���� �ɷ� ȿ�� ����
    protected override void RemoveEffect(PlayerClass player, float value)
    {
        switch (attackType)
        {
            case AttackAbilityType.ComboEnhancement:
                RemoveComboEnhancement(player, value);
                break;
            case AttackAbilityType.WeakPrey:
                RemoveWeakPrey(player, value);
                break;
            case AttackAbilityType.ChainStrike:
                RemoveChainStrike(player, value);
                break;
            case AttackAbilityType.GaugeBoost:
                RemoveGaugeBoost(player, value);
                break;
            case AttackAbilityType.ArmorCrush:
                RemoveArmorCrush(player, value);
                break;
        }
    }

    // �޺� ��ȭ ȿ�� ����
    private void ApplyComboEnhancement(PlayerClass player, float enhancementPercent)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        // ������Ʈ �߰� �Ǵ� ����
        ComboEnhancementComponent comboComp = playerObj.GetComponent<ComboEnhancementComponent>();
        if (comboComp == null)
        {
            comboComp = playerObj.AddComponent<ComboEnhancementComponent>();
        }

        comboComp.AddEnhancement(enhancementPercent / 100f);
        Debug.Log($"�޺� ��ȭ ȿ�� ����: {enhancementPercent}%, ���� �޺� ������: {comboComp.GetEnhancementAmount() * 100f}%");
    }

    // �޺� ��ȭ ȿ�� ����
    private void RemoveComboEnhancement(PlayerClass player, float enhancementPercent)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        ComboEnhancementComponent comboComp = playerObj.GetComponent<ComboEnhancementComponent>();
        if (comboComp != null)
        {
            comboComp.RemoveEnhancement(enhancementPercent / 100f);

            // ȿ���� 0�̸� ������Ʈ ���� (������ ���� �ƴ� ����)
            if (Mathf.Approximately(comboComp.GetEnhancementAmount(), 0f) && !isLevelingUp)
            {
                GameObject.Destroy(comboComp);
            }
        }
    }

    //���� ���� ȿ�� ����
    private void ApplyWeakPrey(PlayerClass player, float damageBonus)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        WeakPreyComponent weakPreyComp = playerObj.GetComponent<WeakPreyComponent>();
        if (weakPreyComp == null)
        {
            weakPreyComp = playerObj.AddComponent<WeakPreyComponent>();
        }

        weakPreyComp.AddDamageBonus(damageBonus / 100f);
        Debug.Log($"���� ���� ȿ�� ����: {damageBonus}%, ���� �߰� ����: {weakPreyComp.GetDamageBonus() * 100f}%");
    }

    // ���� ���� ȿ�� ����
    private void RemoveWeakPrey(PlayerClass player, float damageBonus)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        WeakPreyComponent weakPreyComp = playerObj.GetComponent<WeakPreyComponent>();
        if (weakPreyComp != null)
        {
            weakPreyComp.RemoveDamageBonus(damageBonus / 100f);

            // ȿ���� 0�̸� ������Ʈ ���� (������ ���� �ƴ� ����)
            if (Mathf.Approximately(weakPreyComp.GetDamageBonus(), 0f) && !isLevelingUp)
            {
                GameObject.Destroy(weakPreyComp);
            }
        }
    }

    // ���� Ÿ�� ȿ�� ����
    private void ApplyChainStrike(PlayerClass player, float chancePercent)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        ChainStrikeComponent chainStrikeComp = playerObj.GetComponent<ChainStrikeComponent>();
        if (chainStrikeComp == null)
        {
            chainStrikeComp = playerObj.AddComponent<ChainStrikeComponent>();
        }

        chainStrikeComp.AddChainChance(chancePercent / 100f);
        Debug.Log($"���� Ÿ�� ȿ�� ����: {chancePercent}%, ���� Ȯ��: {chainStrikeComp.GetChainChance() * 100f}%");
    }

    // ���� Ÿ�� ȿ�� ����
    private void RemoveChainStrike(PlayerClass player, float chancePercent)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        ChainStrikeComponent chainStrikeComp = playerObj.GetComponent<ChainStrikeComponent>();
        if (chainStrikeComp != null)
        {
            chainStrikeComp.RemoveChainChance(chancePercent / 100f);

            // ȿ���� 0�̸� ������Ʈ ���� (������ ���� �ƴ� ����)
            if (Mathf.Approximately(chainStrikeComp.GetChainChance(), 0f) && !isLevelingUp)
            {
                GameObject.Destroy(chainStrikeComp);
            }
        }
    }

    // ������ ��ȭ ȿ�� ����
    private void ApplyGaugeBoost(PlayerClass player, float boostPercent)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        GaugeBoostComponent gaugeBoostComp = playerObj.GetComponent<GaugeBoostComponent>();
        if (gaugeBoostComp == null)
        {
            gaugeBoostComp = playerObj.AddComponent<GaugeBoostComponent>();
        }

        gaugeBoostComp.AddGaugeBoost(boostPercent / 100f);
        Debug.Log($"������ ��ȭ ȿ�� ����: {boostPercent}%, ���� ������: {gaugeBoostComp.GetGaugeBoost() * 100f}%");
    }

    // ������ ��ȭ ȿ�� ����
    private void RemoveGaugeBoost(PlayerClass player, float boostPercent)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        GaugeBoostComponent gaugeBoostComp = playerObj.GetComponent<GaugeBoostComponent>();
        if (gaugeBoostComp != null)
        {
            gaugeBoostComp.RemoveGaugeBoost(boostPercent / 100f);

            // ȿ���� 0�̸� ������Ʈ ���� (������ ���� �ƴ� ����)
            if (Mathf.Approximately(gaugeBoostComp.GetGaugeBoost(), 0f) && !isLevelingUp)
            {
                GameObject.Destroy(gaugeBoostComp);
            }
        }
    }

    // ���� �ı� ȿ�� ����
    private void ApplyArmorCrush(PlayerClass player, float crushPercent)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        ArmorCrushComponent armorCrushComp = playerObj.GetComponent<ArmorCrushComponent>();
        if (armorCrushComp == null)
        {
            armorCrushComp = playerObj.AddComponent<ArmorCrushComponent>();
        }

        armorCrushComp.AddCrushAmount(crushPercent / 100f);
        Debug.Log($"���� �ı� ȿ�� ����: {crushPercent}%, ���� ������: {armorCrushComp.GetCrushAmount() * 100f}%");
    }

    // ���� �ı� ȿ�� ����
    private void RemoveArmorCrush(PlayerClass player, float crushPercent)
    {
        GameObject playerObj = GameInitializer.Instance.gameObject;
        ArmorCrushComponent armorCrushComp = playerObj.GetComponent<ArmorCrushComponent>();
        if (armorCrushComp != null)
        {
            armorCrushComp.RemoveCrushAmount(crushPercent / 100f);

            // ȿ���� 0�̸� ������Ʈ ���� (������ ���� �ƴ� ����)
            if (Mathf.Approximately(armorCrushComp.GetCrushAmount(), 0f) && !isLevelingUp)
            {
                GameObject.Destroy(armorCrushComp);
            }
        }
    }
}
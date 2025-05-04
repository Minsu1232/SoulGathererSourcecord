using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

[System.Serializable]
public abstract class DungeonAbility
{
    public string id;             // ���� �ĺ���
    public string name;           // �̸�
    public string description;    // ����
    public Sprite icon;           // ������
    public string iconAddress;    // ������ �ּ�
    public Rarity rarity;         // ��͵�
    public float effectValue;     // ȿ��
    public int level = 1;         // �ɷ� ����
    public float levelMultiplier = 0.3f; // ������ ���� ��� (�⺻�� 30%)
    protected bool isLevelingUp = false;
    protected float originalValue; // ���� ȿ���� (������ �� ���)

    // CSV �����Ϳ��� �����Ƽ ������ ���� ���׸� �޼���
    // T: ������ �����Ƽ Ÿ��, TEnum: �ش� �����Ƽ�� Ÿ�� ������
    public static T FromCSVData<T, TEnum>(Dictionary<string, string> csvData, string typeFieldName) where T : DungeonAbility, new()
    {
        T ability = new T();

        // �ʼ� �� �Ľ�
        string id = csvData["ID"];
        // Ÿ�� �Ľ��� �ڽ� Ŭ�������� ó���ؾ� ��
        string name = csvData["Name"];
        string description = csvData["Description"];
        Rarity rarity = (Rarity)int.Parse(csvData["Rarity"]);
        float baseValue = float.Parse(csvData["BaseValue"]);

        // ������ ���� �Ľ� �߰�
        if (csvData.ContainsKey("LevelMultiplier") && !string.IsNullOrEmpty(csvData["LevelMultiplier"]))
        {
            ability.levelMultiplier = float.Parse(csvData["LevelMultiplier"]);
        }

        // ������ �ּ� ����
        if (csvData.ContainsKey("IconPath") && !string.IsNullOrEmpty(csvData["IconPath"]))
        {
            ability.iconAddress = csvData["IconPath"];

            // ������ ��ΰ� �ִٸ� ��巹����� �ε�
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

        // �⺻ �Ӽ� ����
        ability.id = id;
        ability.name = name;
        ability.description = description;
        ability.rarity = rarity;
        ability.effectValue = baseValue;
        ability.originalValue = baseValue;

        return ability;
    }

    // �Ʒ��� ������ �޼����
    public virtual void OnAcquire(PlayerClass player)
    {
        // ȿ�� ���� (�ڽ� Ŭ�������� ����)
        ApplyEffect(player, effectValue);

        // ����� �α�
        Debug.Log($"ȹ���� �ɷ�: {name} (Lv.{level}) - {effectValue}");
    }

    public virtual void OnLevelUp(PlayerClass player)
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
        Debug.Log($"�������� �ɷ�: {name} (Lv.{level}) - {effectValue}");
    }

    public virtual void OnReset(PlayerClass player)
    {
        // ȿ�� ���� (�ڽ� Ŭ�������� ����)
        RemoveEffect(player, effectValue);
    }

    // �ڽ� Ŭ�������� �����ؾ� �ϴ� �޼���
    protected abstract void ApplyEffect(PlayerClass player, float value);
    protected abstract void RemoveEffect(PlayerClass player, float value);

    // ������Ʈ�� �����ϰ� �����ϴ� ���� �޼���
    protected void SafeDestroy<TComponent>(GameObject obj) where TComponent : Component
    {
        TComponent component = obj.GetComponent<TComponent>();
        if (component != null && !isLevelingUp)
        {
            GameObject.Destroy(component);
        }
    }
}

// ��͵� enum
public enum Rarity
{
    Common,     // �Ϲ�
    Uncommon,   // ���
    Rare,       // ���
    Epic,       // ����
    Legendary   // ����
}
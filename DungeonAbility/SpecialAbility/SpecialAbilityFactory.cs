// SpecialAbilityFactory.cs - Ư�� �ɷ� ���丮 (CSV ���)
using System.Collections.Generic;
using UnityEngine;

public static class SpecialAbilityFactory
{
    // ��� Ư�� �ɷ� ����
    public static List<SpecialAbility> CreateAllSpecialAbilities()
    {
        // SpecialAbilityLoader�� �ʱ�ȭ�ƴ��� Ȯ��
        if (SpecialAbilityLoader.Instance == null)
        {
            Debug.LogError("SpecialAbilityLoader�� �ʱ�ȭ���� �ʾҽ��ϴ�.");

            // ����: �ϵ��ڵ��� �⺻ �ɷ� ��ȯ
            return CreateHardcodedSpecialAbilities();
        }

        // CSV���� ��� �ɷ� ����
        return SpecialAbilityLoader.Instance.CreateAllSpecialAbilities();
    }

    // Ư�� ID�� Ư�� �ɷ� ����
    public static SpecialAbility CreateSpecialAbilityById(string id)
    {
        // SpecialAbilityLoader�� �ʱ�ȭ�ƴ��� Ȯ��
        if (SpecialAbilityLoader.Instance == null)
        {
            Debug.LogError("SpecialAbilityLoader�� �ʱ�ȭ���� �ʾҽ��ϴ�.");

            // ����: �ϵ��ڵ��� Ư�� ID�� �ɷ� ã��
            return FindHardcodedSpecialAbility(id);
        }

        // CSV���� Ư�� ID�� �ɷ� ����
        return SpecialAbilityLoader.Instance.CreateSpecialAbility(id);
    }

    // ����: �ϵ��ڵ��� �⺻ �ɷ� ���� (CSV �ε� ���� ��)
    private static List<SpecialAbility> CreateHardcodedSpecialAbilities()
    {
        List<SpecialAbility> abilities = new List<SpecialAbility>();

        // �� Ÿ�Ժ��� ���� ��͵��� �ɷ� ����

        // ������ ���� �ɷ� (���� ��͵�)
        abilities.Add(CreateResourceRetentionAbility(Rarity.Common, 10f, "�⺻ ����", "Ư�� ��ų ��� �� �������� 10% �����˴ϴ�. (������ +3%)"));
        abilities.Add(CreateResourceRetentionAbility(Rarity.Uncommon, 20f, "������ ����", "Ư�� ��ų ��� �� �������� 20% �����˴ϴ�. (������ +6%)"));
        abilities.Add(CreateResourceRetentionAbility(Rarity.Rare, 30f, "ȿ���� ���", "Ư�� ��ų ��� �� �������� 30% �����˴ϴ�. (������ +9%)"));
        abilities.Add(CreateResourceRetentionAbility(Rarity.Epic, 40f, "������ ��ȯ", "Ư�� ��ų ��� �� �������� 40% �����˴ϴ�. (������ +12%)"));
        abilities.Add(CreateResourceRetentionAbility(Rarity.Legendary, 50f, "���� ������", "Ư�� ��ų ��� �� �������� 50% �����˴ϴ�. (������ +15%)"));

        return abilities;
    }

    // ������ ���� �ɷ� ���� ����� �޼���
    private static SpecialAbility CreateResourceRetentionAbility(Rarity rarity, float value, string name, string description)
    {
        SpecialAbility ability = new SpecialAbility();
        ability.Initialize(
            SpecialAbility.SpecialAbilityType.ResourceRetention,
            value,
            name,
            description,
            rarity
        );
        // ID�� ��͵��� �����Ͽ� ����ũ�ϰ� ����
        ability.id = $"resource_retention_{rarity.ToString().ToLower()}";
        return ability;
    }

    // ����: �ϵ��ڵ��� Ư�� ID�� �ɷ� ã��
    private static SpecialAbility FindHardcodedSpecialAbility(string id)
    {
        List<SpecialAbility> abilities = CreateHardcodedSpecialAbilities();
        return abilities.Find(a => a.id == id);
    }
}
// �нú� �ɷ� ���丮 (CSV ���)
using System.Collections.Generic;
using UnityEngine;

public static class PassiveAbilityFactory
{
    // ��� �нú� �ɷ� ����
    public static List<PassiveAbility> CreateAllPassiveAbilities()
    {
        // PassiveAbilityLoader�� �ʱ�ȭ�ƴ��� Ȯ��
        if (PassiveAbilityLoader.Instance == null)
        {
            Debug.LogError("PassiveAbilityLoader�� �ʱ�ȭ���� �ʾҽ��ϴ�.");

            // ����: �ϵ��ڵ��� �⺻ �ɷ� ��ȯ
            return CreateHardcodedPassiveAbilities();
        }

        // CSV���� ��� �ɷ� ����
        return PassiveAbilityLoader.Instance.CreateAllPassiveAbilities();
    }

    // Ư�� ID�� �нú� �ɷ� ����
    public static PassiveAbility CreatePassiveAbilityById(string id)
    {
        // PassiveAbilityLoader�� �ʱ�ȭ�ƴ��� Ȯ��
        if (PassiveAbilityLoader.Instance == null)
        {
            Debug.LogError("PassiveAbilityLoader�� �ʱ�ȭ���� �ʾҽ��ϴ�.");

            // ����: �ϵ��ڵ��� �⺻ �ɷ� ã��
            return FindHardcodedPassiveAbility(id);
        }

        // CSV���� Ư�� ID�� �ɷ� ����
        return PassiveAbilityLoader.Instance.CreatePassiveAbility(id);
    }

    // ����: �ϵ��ڵ��� �⺻ �ɷ� ���� (CSV �ε� ���� ��)
    private static List<PassiveAbility> CreateHardcodedPassiveAbilities()
    {
        List<PassiveAbility> abilities = new List<PassiveAbility>();

        // �� Ÿ�Ժ��� ���� ��͵��� �ɷ� ����

        // ���� ���� �ɷ� (���� ��͵�)
        abilities.Add(CreateDamageReductionAbility(Rarity.Common, 5f, "�⺻ ���", "�޴� ���ذ� 5% �����մϴ�. (������ +1.5%)"));
        abilities.Add(CreateDamageReductionAbility(Rarity.Uncommon, 10f, "������ ���", "�޴� ���ذ� 10% �����մϴ�. (������ +3%)"));
        abilities.Add(CreateDamageReductionAbility(Rarity.Rare, 15f, "ö�� ���", "�޴� ���ذ� 15% �����մϴ�. (������ +4.5%)"));
        abilities.Add(CreateDamageReductionAbility(Rarity.Epic, 20f, "�Ұ�ħ ���", "�޴� ���ذ� 20% �����մϴ�. (������ +6%)"));
        abilities.Add(CreateDamageReductionAbility(Rarity.Legendary, 25f, "���� ����", "�޴� ���ذ� 25% �����մϴ�. (������ +7.5%)"));

        // ���� �ɷ� (���� ��͵�)
        abilities.Add(CreateLifeStealAbility(Rarity.Common, 3f, "���� ����", "���� �� �������� 3%��ŭ ü���� ȸ���մϴ�. (������ +0.9%)"));
        abilities.Add(CreateLifeStealAbility(Rarity.Uncommon, 5f, "����� ���", "���� �� �������� 5%��ŭ ü���� ȸ���մϴ�. (������ +1.5%)"));
        abilities.Add(CreateLifeStealAbility(Rarity.Rare, 8f, "���� ���", "���� �� �������� 8%��ŭ ü���� ȸ���մϴ�. (������ +2.4%)"));
        abilities.Add(CreateLifeStealAbility(Rarity.Epic, 12f, "���� �ǽ�", "���� �� �������� 12%��ŭ ü���� ȸ���մϴ�. (������ +3.6%)"));
        abilities.Add(CreateLifeStealAbility(Rarity.Legendary, 15f, "�Ҹ��� ����", "���� �� �������� 15%��ŭ ü���� ȸ���մϴ�. (������ +4.5%)"));

        // �ݰ� �ɷ� (���� ��͵�)
        abilities.Add(CreateCounterattackAbility(Rarity.Common, 10f, "�⺻ �ݰ�", "�ǰ� �� ���� �������� 10%�� �����ڿ��� �ݻ��մϴ�. (������ +3%)"));
        abilities.Add(CreateCounterattackAbility(Rarity.Uncommon, 15f, "���� ����", "�ǰ� �� ���� �������� 15%�� �����ڿ��� �ݻ��մϴ�. (������ +4.5%)"));
        abilities.Add(CreateCounterattackAbility(Rarity.Rare, 20f, "������ ����", "�ǰ� �� ���� �������� 20%�� �����ڿ��� �ݻ��մϴ�. (������ +6%)"));
        abilities.Add(CreateCounterattackAbility(Rarity.Epic, 25f, "������ ����", "�ǰ� �� ���� �������� 25%�� �����ڿ��� �ݻ��մϴ�. (������ +7.5%)"));
        abilities.Add(CreateCounterattackAbility(Rarity.Legendary, 30f, "������ �ݻ�", "�ǰ� �� ���� �������� 30%�� �����ڿ��� �ݻ��մϴ�. (������ +9%)"));

        // ������ ã�� �ɷ� (���� ��͵�)
        abilities.Add(CreateItemFindAbility(Rarity.Common, 10f, "����� ���", "������ ��� Ȯ���� 10% �����մϴ�. (������ +3%)"));
        abilities.Add(CreateItemFindAbility(Rarity.Uncommon, 20f, "����� �ձ�", "������ ��� Ȯ���� 20% �����մϴ�. (������ +6%)"));
        abilities.Add(CreateItemFindAbility(Rarity.Rare, 30f, "���� ��ɲ�", "������ ��� Ȯ���� 30% �����մϴ�. (������ +9%)"));
        abilities.Add(CreateItemFindAbility(Rarity.Epic, 40f, "Ȳ���� ��", "������ ��� Ȯ���� 40% �����մϴ�. (������ +12%)"));
        abilities.Add(CreateItemFindAbility(Rarity.Legendary, 50f, "������ �߰���", "������ ��� Ȯ���� 50% �����մϴ�. (������ +15%)"));


        abilities.Add(CreateStageHealAbility(Rarity.Common, 2f, "ȸ���� ���", "�������� Ŭ���� �� �ִ� ü���� 2%�� ȸ���մϴ�. (������ +0.6%)"));
        abilities.Add(CreateStageHealAbility(Rarity.Uncommon, 3f, "������ ����", "�������� Ŭ���� �� �ִ� ü���� 3%�� ȸ���մϴ�. (������ +0.9%)"));
        abilities.Add(CreateStageHealAbility(Rarity.Rare, 4f, "ġ���� ���", "�������� Ŭ���� �� �ִ� ü���� 4%�� ȸ���մϴ�. (������ +1.2%)"));
        abilities.Add(CreateStageHealAbility(Rarity.Epic, 5f, "������ Ȱ��", "�������� Ŭ���� �� �ִ� ü���� 5%�� ȸ���մϴ�. (������ +1.5%)"));
        abilities.Add(CreateStageHealAbility(Rarity.Legendary, 6f, "�Ҹ��� ȸ��", "�������� Ŭ���� �� �ִ� ü���� 6%�� ȸ���մϴ�. (������ +1.8%)"));
        return abilities;
    }
    private static PassiveAbility CreateStageHealAbility(Rarity rarity, float value, string name, string description)
    {
        PassiveAbility ability = new PassiveAbility();
        ability.Initialize(
            PassiveAbility.PassiveType.StageHeal,
            value,
            name,
            description,
            rarity
        );
        ability.id = $"stage_heal_{rarity.ToString().ToLower()}";
        return ability;
    }
    // ���� ���� �ɷ� ���� ����� �޼���
    private static PassiveAbility CreateDamageReductionAbility(Rarity rarity, float value, string name, string description)
    {
        PassiveAbility ability = new PassiveAbility();
        ability.Initialize(
            PassiveAbility.PassiveType.DamageReduction,
            value,
            name,
            description,
            rarity
        );
        // ID�� ��͵��� �����Ͽ� ����ũ�ϰ� ����
        ability.id = $"damage_reduction_{rarity.ToString().ToLower()}";
        return ability;
    }

    // ���� �ɷ� ���� ����� �޼���
    private static PassiveAbility CreateLifeStealAbility(Rarity rarity, float value, string name, string description)
    {
        PassiveAbility ability = new PassiveAbility();
        ability.Initialize(
            PassiveAbility.PassiveType.LifeSteal,
            value,
            name,
            description,
            rarity
        );
        ability.id = $"life_steal_{rarity.ToString().ToLower()}";
        return ability;
    }

    // �ݰ� �ɷ� ���� ����� �޼���
    private static PassiveAbility CreateCounterattackAbility(Rarity rarity, float value, string name, string description)
    {
        PassiveAbility ability = new PassiveAbility();
        ability.Initialize(
            PassiveAbility.PassiveType.Counterattack,
            value,
            name,
            description,
            rarity
        );
        ability.id = $"counterattack_{rarity.ToString().ToLower()}";
        return ability;
    }

    // ������ ã�� �ɷ� ���� ����� �޼���
    private static PassiveAbility CreateItemFindAbility(Rarity rarity, float value, string name, string description)
    {
        PassiveAbility ability = new PassiveAbility();
        ability.Initialize(
            PassiveAbility.PassiveType.ItemFind,
            value,
            name,
            description,
            rarity
        );
        ability.id = $"item_find_{rarity.ToString().ToLower()}";
        return ability;
    }

    // ����: �ϵ��ڵ��� Ư�� ID�� �ɷ� ã��
    private static PassiveAbility FindHardcodedPassiveAbility(string id)
    {
        List<PassiveAbility> abilities = CreateHardcodedPassiveAbilities();
        return abilities.Find(a => a.id == id);
    }
}
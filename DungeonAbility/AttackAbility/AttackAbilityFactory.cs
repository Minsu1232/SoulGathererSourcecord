// AttackAbilityFactory.cs - ���� �ɷ� ���丮 (CSV ���)
using System.Collections.Generic;
using UnityEngine;

public static class AttackAbilityFactory
{
    // ��� ���� �ɷ� ����
    public static List<AttackAbility> CreateAllAttackAbilities()
    {
        // AttackAbilityLoader�� �ʱ�ȭ�ƴ��� Ȯ��
        if (AttackAbilityLoader.Instance == null)
        {
            Debug.LogError("AttackAbilityLoader�� �ʱ�ȭ���� �ʾҽ��ϴ�.");

            // ����: �ϵ��ڵ��� �⺻ �ɷ� ��ȯ
            return CreateHardcodedAttackAbilities();
        }

        // CSV���� ��� �ɷ� ����
        return AttackAbilityLoader.Instance.CreateAllAttackAbilities();
    }

    // Ư�� ID�� ���� �ɷ� ����
    public static AttackAbility CreateAttackAbilityById(string id)
    {
        // AttackAbilityLoader�� �ʱ�ȭ�ƴ��� Ȯ��
        if (AttackAbilityLoader.Instance == null)
        {
            Debug.LogError("AttackAbilityLoader�� �ʱ�ȭ���� �ʾҽ��ϴ�.");

            // ����: �ϵ��ڵ��� �⺻ �ɷ� ã��
            return FindHardcodedAttackAbility(id);
        }

        // CSV���� Ư�� ID�� �ɷ� ����
        return AttackAbilityLoader.Instance.CreateAttackAbility(id);
    }

    // ����: �ϵ��ڵ��� �⺻ �ɷ� ���� (CSV �ε� ���� ��)
    private static List<AttackAbility> CreateHardcodedAttackAbilities()
    {
        List<AttackAbility> abilities = new List<AttackAbility>();

        // �� Ÿ�Ժ��� ���� ��͵��� �ɷ� ����

        //�޺� ��ȭ �ɷ�(���� ��͵�)
        //abilities.Add(CreateComboEnhancementAbility(Rarity.Common, 15f, "�⺻ �޺�", "���� ���� �� �޺� �������� 15% �����մϴ�. (������ +4.5%)"));
        //abilities.Add(CreateComboEnhancementAbility(Rarity.Uncommon, 25f, "���� �޺�", "���� ���� �� �޺� �������� 25% �����մϴ�. (������ +7.5%)"));
        //abilities.Add(CreateComboEnhancementAbility(Rarity.Rare, 35f, "������ �޺�", "���� ���� �� �޺� �������� 35% �����մϴ�. (������ +10.5%)"));
        //abilities.Add(CreateComboEnhancementAbility(Rarity.Epic, 45f, "�ı��� �޺�", "���� ���� �� �޺� �������� 45% �����մϴ�. (������ +13.5%)"));
        //abilities.Add(CreateComboEnhancementAbility(Rarity.Legendary, 60f, "���� ����Ÿ", "���� ���� �� �޺� �������� 60% �����մϴ�. (������ +18%)"));

        //���� ���� �ɷ�(���� ��͵�)
        //abilities.Add(CreateWeakPreyAbility(Rarity.Common, 15f, "���� Ÿ��", "ü���� 30% ������ ������ 15% �߰� ���ظ� �ݴϴ�. (������ +4.5%)"));
        //abilities.Add(CreateWeakPreyAbility(Rarity.Uncommon, 25f, "ġ���� ����", "ü���� 30% ������ ������ 25% �߰� ���ظ� �ݴϴ�. (������ +7.5%)"));
        //abilities.Add(CreateWeakPreyAbility(Rarity.Rare, 35f, "���� ����", "ü���� 35% ������ ������ 35% �߰� ���ظ� �ݴϴ�. (������ +10.5%)"));
        //abilities.Add(CreateWeakPreyAbility(Rarity.Epic, 45f, "��ɲ��� ��", "ü���� 40% ������ ������ 45% �߰� ���ظ� �ݴϴ�. (������ +13.5%)"));
        //abilities.Add(CreateWeakPreyAbility(Rarity.Legendary, 60f, "���� ���", "ü���� 50% ������ ������ 60% �߰� ���ظ� �ݴϴ�. (������ +18%)"));

        //���� Ÿ�� �ɷ�(���� ��͵�)
        //abilities.Add(CreateChainStrikeAbility(Rarity.Common, 10f, "���� Ÿ��", "���� �� 10% Ȯ���� �߰� Ÿ���� ���մϴ�. (������ +3%)"));
        //abilities.Add(CreateChainStrikeAbility(Rarity.Uncommon, 15f, "���� Ÿ��", "���� �� 15% Ȯ���� �߰� Ÿ���� ���մϴ�. (������ +4.5%)"));
        //abilities.Add(CreateChainStrikeAbility(Rarity.Rare, 20f, "���� ����", "���� �� 20% Ȯ���� �߰� Ÿ���� ���մϴ�. (������ +6%)"));
        //abilities.Add(CreateChainStrikeAbility(Rarity.Epic, 25f, "������ ��ǳ", "���� �� 25% Ȯ���� �߰� Ÿ���� ���մϴ�. (������ +7.5%)"));
        //abilities.Add(CreateChainStrikeAbility(Rarity.Legendary, 30f, "���� ����", "���� �� 30% Ȯ���� �߰� Ÿ���� ���մϴ�. (������ +9%)"));

        //������ ��ȭ �ɷ�(���� ��͵�)
        //abilities.Add(CreateGaugeBoostAbility(Rarity.Common, 15f, "�⺻ ����", "���� ������ �������� 15% �����մϴ�. (������ +4.5%)"));
        //abilities.Add(CreateGaugeBoostAbility(Rarity.Uncommon, 25f, "���� ����", "���� ������ �������� 25% �����մϴ�. (������ +7.5%)"));
        //abilities.Add(CreateGaugeBoostAbility(Rarity.Rare, 35f, "���� ����", "���� ������ �������� 35% �����մϴ�. (������ +10.5%)"));
        //abilities.Add(CreateGaugeBoostAbility(Rarity.Epic, 45f, "�޼� ����", "���� ������ �������� 45% �����մϴ�. (������ +13.5%)"));
        //abilities.Add(CreateGaugeBoostAbility(Rarity.Legendary, 60f, "������", "���� ������ �������� 60% �����մϴ�. (������ +18%)"));

        //���� �ı� �ɷ�(���� ��͵�)
        //abilities.Add(CreateArmorCrushAbility(Rarity.Common, 5f, "���� ����", "���� �� ���� ������ 5% ���ҽ�ŵ�ϴ�. (������ +1.5%)"));
        //abilities.Add(CreateArmorCrushAbility(Rarity.Uncommon, 10f, "�� �ı�", "���� �� ���� ������ 10% ���ҽ�ŵ�ϴ�. (������ +3%)"));
        //abilities.Add(CreateArmorCrushAbility(Rarity.Rare, 15f, "�� �м�", "���� �� ���� ������ 15% ���ҽ�ŵ�ϴ�. (������ +4.5%)"));
        //abilities.Add(CreateArmorCrushAbility(Rarity.Epic, 20f, "���� ����", "���� �� ���� ������ 20% ���ҽ�ŵ�ϴ�. (������ +6%)"));
        //abilities.Add(CreateArmorCrushAbility(Rarity.Legendary, 25f, "���� ����", "���� �� ���� ������ 25% ���ҽ�ŵ�ϴ�. (������ +7.5%)"));

        return abilities;
    }

    // �޺� ��ȭ �ɷ� ���� ����� �޼���
    private static AttackAbility CreateComboEnhancementAbility(Rarity rarity, float value, string name, string description)
    {
        AttackAbility ability = new AttackAbility();
        ability.Initialize(
            AttackAbility.AttackAbilityType.ComboEnhancement,
            value,
            name,
            description,
            rarity
        );
        // ID�� ��͵��� �����Ͽ� ����ũ�ϰ� ����
        ability.id = $"combo_enhancement_{rarity.ToString().ToLower()}";
        return ability;
    }

    // ���� ���� �ɷ� ���� ����� �޼���
    private static AttackAbility CreateWeakPreyAbility(Rarity rarity, float value, string name, string description)
    {
        AttackAbility ability = new AttackAbility();
        ability.Initialize(
            AttackAbility.AttackAbilityType.WeakPrey,
            value,
            name,
            description,
            rarity
        );
        ability.id = $"weak_prey_{rarity.ToString().ToLower()}";
        return ability;
    }

    // ���� Ÿ�� �ɷ� ���� ����� �޼���
    private static AttackAbility CreateChainStrikeAbility(Rarity rarity, float value, string name, string description)
    {
        AttackAbility ability = new AttackAbility();
        ability.Initialize(
            AttackAbility.AttackAbilityType.ChainStrike,
            value,
            name,
            description,
            rarity
        );
        ability.id = $"chain_strike_{rarity.ToString().ToLower()}";
        return ability;
    }

    // ������ ��ȭ �ɷ� ���� ����� �޼���
    private static AttackAbility CreateGaugeBoostAbility(Rarity rarity, float value, string name, string description)
    {
        AttackAbility ability = new AttackAbility();
        ability.Initialize(
            AttackAbility.AttackAbilityType.GaugeBoost,
            value,
            name,
            description,
            rarity
        );
        ability.id = $"gauge_boost_{rarity.ToString().ToLower()}";
        return ability;
    }

    // ���� �ı� �ɷ� ���� ����� �޼���
    private static AttackAbility CreateArmorCrushAbility(Rarity rarity, float value, string name, string description)
    {
        AttackAbility ability = new AttackAbility();
        ability.Initialize(
            AttackAbility.AttackAbilityType.ArmorCrush,
            value,
            name,
            description,
            rarity
        );
        ability.id = $"armor_crush_{rarity.ToString().ToLower()}";
        
        return ability;
    }

    // ����: �ϵ��ڵ��� Ư�� ID�� �ɷ� ã��
    private static AttackAbility FindHardcodedAttackAbility(string id)
    {
        List<AttackAbility> abilities = CreateHardcodedAttackAbilities();
        return abilities.Find(a => a.id == id);
    }
}
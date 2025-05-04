// MovementAbilityFactory.cs - �̵� �ɷ� ���丮 (CSV ���)
using System.Collections.Generic;
using UnityEngine;

public static class MovementAbilityFactory
{
    // ��� �̵� �ɷ� ����
    public static List<MovementAbility> CreateAllMovementAbilities()
    {
        // MovementAbilityLoader�� �ʱ�ȭ�ƴ��� Ȯ��
        if (MovementAbilityLoader.Instance == null)
        {
            Debug.LogError("MovementAbilityLoader�� �ʱ�ȭ���� �ʾҽ��ϴ�.");

            // ����: �ϵ��ڵ��� �⺻ �ɷ� ��ȯ
            return CreateHardcodedMovementAbilities();
        }

        // CSV���� ��� �ɷ� ����
        return MovementAbilityLoader.Instance.CreateAllMovementAbilities();
    }

    // Ư�� ID�� �̵� �ɷ� ����
    public static MovementAbility CreateMovementAbilityById(string id)
    {
        // MovementAbilityLoader�� �ʱ�ȭ�ƴ��� Ȯ��
        if (MovementAbilityLoader.Instance == null)
        {
            Debug.LogError("MovementAbilityLoader�� �ʱ�ȭ���� �ʾҽ��ϴ�.");

            // ����: �ϵ��ڵ��� Ư�� ID�� �ɷ� ã��
            return FindHardcodedMovementAbility(id);
        }

        // CSV���� Ư�� ID�� �ɷ� ����
        return MovementAbilityLoader.Instance.CreateMovementAbility(id);
    }

    // ����: �ϵ��ڵ��� �⺻ �ɷ� ���� (CSV �ε� ���� ��)
    private static List<MovementAbility> CreateHardcodedMovementAbilities()
    {
        Debug.Log("����!!!!!!!");
        List<MovementAbility> abilities = new List<MovementAbility>();

        // �� Ÿ�Ժ��� ���� ��͵��� �ɷ� ����

        // ��� �� ���� �ɷ� (���� ��͵�)
        abilities.Add(CreateDashForceAbility(Rarity.Common, 5f, "��ȭ�� ���", "��� ���� 5 �����մϴ�. (������ +1.5)"));
        abilities.Add(CreateDashForceAbility(Rarity.Uncommon, 10f, "������ ���", "��� ���� 10 �����մϴ�. (������ +3)"));
        abilities.Add(CreateDashForceAbility(Rarity.Rare, 15f, "�ͷ��� ���", "��� ���� 15 �����մϴ�. (������ +4.5)"));
        abilities.Add(CreateDashForceAbility(Rarity.Epic, 20f, "������ ���", "��� ���� 20 �����մϴ�. (������ +6)"));
        abilities.Add(CreateDashForceAbility(Rarity.Legendary, 30f, "������ ����", "��� ���� 30 �����մϴ�. (������ +9)"));

        // ��� ���� �ɷ� (���� ��͵�)
        abilities.Add(CreateDashInvincibleAbility(Rarity.Common, 0.2f, "���� ȸ��", "��� �� 0.2�� ���� ���� ���°� �˴ϴ�. (������ +0.06��)"));
        abilities.Add(CreateDashInvincibleAbility(Rarity.Uncommon, 0.3f, "�ܻ� ���", "��� �� 0.3�� ���� ���� ���°� �˴ϴ�. (������ +0.09��)"));
        abilities.Add(CreateDashInvincibleAbility(Rarity.Rare, 0.4f, "���� ���", "��� �� 0.4�� ���� ���� ���°� �˴ϴ�. (������ +0.12��)"));
        abilities.Add(CreateDashInvincibleAbility(Rarity.Epic, 0.5f, "���� �̵�", "��� �� 0.5�� ���� ���� ���°� �˴ϴ�. (������ +0.15��)"));
        abilities.Add(CreateDashInvincibleAbility(Rarity.Legendary, 0.7f, "�ð� ����", "��� �� 0.7�� ���� ���� ���°� �˴ϴ�. (������ +0.21��)"));

        // ��� ��� �ɷ� (���� ��͵�)
        abilities.Add(CreateDashImpactAbility(Rarity.Common, 10f, "��� ���", "��� �� ���� �浹�ϸ� 10�� ���ظ� �����ϴ�. (������ +3)"));
        abilities.Add(CreateDashImpactAbility(Rarity.Uncommon, 20f, "���� Ÿ��", "��� �� ���� �浹�ϸ� 20�� ���ظ� �����ϴ�. (������ +6)"));
        abilities.Add(CreateDashImpactAbility(Rarity.Rare, 30f, "���� ���", "��� �� ���� �浹�ϸ� 30�� ���ظ� �����ϴ�. (������ +9)"));
        abilities.Add(CreateDashImpactAbility(Rarity.Epic, 40f, "�ı��� ����", "��� �� ���� �浹�ϸ� 40�� ���ظ� �����ϴ�. (������ +12)"));
        abilities.Add(CreateDashImpactAbility(Rarity.Legendary, 60f, "������ �ı���", "��� �� ���� �浹�ϸ� 60�� ���ظ� �����ϴ�. (������ +18)"));

        // ��� �Ҳ� �ɷ� (���� ��͵�)
        abilities.Add(CreateDashFlameAbility(Rarity.Common, 5f, "�Ҳ� ����", "��� �� �Ҳ��� ���� �ʴ� 5�� ���ظ� �����ϴ�. (������ +1.5)"));
        abilities.Add(CreateDashFlameAbility(Rarity.Uncommon, 10f, "ȭ�� ���", "��� �� �Ҳ��� ���� �ʴ� 10�� ���ظ� �����ϴ�. (������ +3)"));
        abilities.Add(CreateDashFlameAbility(Rarity.Rare, 15f, "�ͷ��� �Ҳ�", "��� �� �Ҳ��� ���� �ʴ� 15�� ���ظ� �����ϴ�. (������ +4.5)"));
        abilities.Add(CreateDashFlameAbility(Rarity.Epic, 20f, "������ ����", "��� �� �Ҳ��� ���� �ʴ� 20�� ���ظ� �����ϴ�. (������ +6)"));
        abilities.Add(CreateDashFlameAbility(Rarity.Legendary, 30f, "�һ����� ����", "��� �� �Ҳ��� ���� �ʴ� 30�� ���ظ� �����ϴ�. (������ +9)"));

        return abilities;
    }

    // ��� �� �ɷ� ���� ����� �޼���
    private static MovementAbility CreateDashForceAbility(Rarity rarity, float value, string name, string description)
    {
        MovementAbility ability = new MovementAbility();
        ability.Initialize(
            MovementAbility.MovementAbilityType.DashForce,
            value,
            name,
            description,
            rarity
        );
        // ID�� ��͵��� �����Ͽ� ����ũ�ϰ� ����
        ability.id = $"dash_force_{rarity.ToString().ToLower()}";
        return ability;
    }

    // ��� ���� �ɷ� ���� ����� �޼���
    private static MovementAbility CreateDashInvincibleAbility(Rarity rarity, float value, string name, string description)
    {
        MovementAbility ability = new MovementAbility();
        ability.Initialize(
            MovementAbility.MovementAbilityType.DashInvincible,
            value,
            name,
            description,
            rarity
        );
        ability.id = $"dash_invincible_{rarity.ToString().ToLower()}";
        return ability;
    }

    // ��� ��� �ɷ� ���� ����� �޼���
    private static MovementAbility CreateDashImpactAbility(Rarity rarity, float value, string name, string description)
    {
        MovementAbility ability = new MovementAbility();
        ability.Initialize(
            MovementAbility.MovementAbilityType.DashImpact,
            value,
            name,
            description,
            rarity
        );
        ability.id = $"dash_impact_{rarity.ToString().ToLower()}";
        return ability;
    }

    // ��� �Ҳ� �ɷ� ���� ����� �޼���
    private static MovementAbility CreateDashFlameAbility(Rarity rarity, float value, string name, string description)
    {
        MovementAbility ability = new MovementAbility();
        ability.Initialize(
            MovementAbility.MovementAbilityType.DashFlame,
            value,
            name,
            description,
            rarity
        );
        ability.id = $"dash_flame_{rarity.ToString().ToLower()}";
        return ability;
    }

    // ����: �ϵ��ڵ��� Ư�� ID�� �ɷ� ã��
    private static MovementAbility FindHardcodedMovementAbility(string id)
    {
        List<MovementAbility> abilities = CreateHardcodedMovementAbilities();
        return abilities.Find(a => a.id == id);
    }
}
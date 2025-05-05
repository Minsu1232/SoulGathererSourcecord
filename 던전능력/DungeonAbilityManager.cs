// DungeonAbilityManager.cs - �ܼ�ȭ�� ����
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonAbilityManager : MonoBehaviour
{
    // Singleton ����
    public static DungeonAbilityManager Instance { get; private set; }

    // ���� ���� ���� �ɷµ�
    private List<DungeonAbility> currentAbilities = new List<DungeonAbility>();

    public event System.Action OnAbilityAcquired; // �ɷ� ���� ȣ�� �̺�Ʈ
    public event System.Action OnAbilityLevelUp; // �ɷ� �ߺ� ���� ������ ȣ�� �̺�Ʈ
    private float[] rarityWeights = { 0.5f, 0.3f, 0.15f, 0.04f, 0.01f }; // ��͵��� ����ġ
    private float rareAbilityChanceMultiplier = 1f;
    
    private int abilitiesPerSelection = 3; // �� ���� ������ ������ ��
    public void SetAbilitiesPerSelection(int count)
    {
        abilitiesPerSelection = count;
        Debug.Log($"�ɷ� ���� ���� ����: {count}��");
    }
    private PlayerClass playerClass;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerClass = GameInitializer.Instance.GetPlayerClass();
        if (playerClass == null)
        {
            Debug.LogError("PlayerClass�� ã�� �� �����ϴ�.");
        }

        // �δ� �ʱ�ȭ Ȯ��
        InitializeLoaders();
    }

    // �ʿ��� �δ� �ʱ�ȭ
    private void InitializeLoaders()
    {
        // �нú� �ɷ� �δ� �ʱ�ȭ
        if (PassiveAbilityLoader.Instance == null)
        {
            GameObject loaderObj = new GameObject("PassiveAbilityLoader");
            loaderObj.AddComponent<PassiveAbilityLoader>();
            Debug.Log("PassiveAbilityLoader ������");
        }

        // ���� �ɷ� �δ� �ʱ�ȭ
        if (AttackAbilityLoader.Instance == null)
        {
            GameObject loaderObj = new GameObject("AttackAbilityLoader");
            loaderObj.AddComponent<AttackAbilityLoader>();
            Debug.Log("AttackAbilityLoader ������");
        }
        if (MovementAbilityLoader.Instance == null)
        {
            GameObject loaderObj = new GameObject("MovementAbilityLoader");
            loaderObj.AddComponent<MovementAbilityLoader>();
            Debug.Log("MovementAbilityLoader ������");
        }
        if (SpecialAbilityLoader.Instance == null)
        {
            GameObject loaderObj = new GameObject("SpecialAbilityLoader");
            loaderObj.AddComponent<SpecialAbilityLoader>();
            Debug.Log("SpecialAbilityLoader ������");
        }
        // �ʿ��� �ٸ� �δ� �ʱ�ȭ...
    }

    // ���� ���� �� �ʱ�ȭ
    public void InitializeDungeon()
    {
        ResetAllAbilities();
        currentAbilities.Clear();
    }

    // ���� ���� �� ��� �ɷ� �ʱ�ȭ
    public void ResetAllAbilities()
    {
        foreach (DungeonAbility ability in currentAbilities)
        {
            ability.OnReset(playerClass);
        }
    }

    // �������� Ŭ���� �� ������ ����
    //public List<DungeonAbility> GetAbilitySelection()
    //{
    //    List<DungeonAbility> availableAbilities = FilterAvailableAbilities();
    //    List<DungeonAbility> selection = new List<DungeonAbility>();

    //    // �ʿ��� ������ŭ �����ϰ� ����
    //    for (int i = 0; i < abilitiesPerSelection; i++)
    //    {
    //        if (availableAbilities.Count == 0) break;

    //        // ����ġ ��� ���� ����
    //        DungeonAbility selectedAbility = SelectWeightedRandomAbility(availableAbilities);
    //        selection.Add(selectedAbility);

    //        // �ߺ� ������ ���� �̹� ���õ� �ɷ°� ���� ID�� ���� �ɷ� ����
    //        availableAbilities.RemoveAll(a => a.id == selectedAbility.id);
    //    }

    //    return selection;
    //}

    // ���� ��Ȳ�� �´� �ɷ� ���͸�
    private List<DungeonAbility> FilterAvailableAbilities()
    {
        List<DungeonAbility> filtered = new List<DungeonAbility>();
        List<DungeonAbility> allAbilities = GetAllAbilities();
        Debug.Log("���⼭ ������3");
        foreach (DungeonAbility ability in allAbilities)
        {
            // �̹� ���� ���� �ɷ��� �������� ����� ������ �ɼ����� ����
            Debug.Log(currentAbilities);
            
            DungeonAbility existingAbility = currentAbilities.Find(a => a.id == ability.id);
           
            if (existingAbility != null)
            {
                Debug.Log(existingAbility.id);
                // �ִ� ������ �ƴ� ��쿡�� ������ �ɼ� ����
                if (existingAbility.level < 5) // �ִ� ���� ����
                {
                    // Ÿ�Կ� ���� ������ ����
                    DungeonAbility upgradedAbility = CreateUpgradedAbilityCopy(ability, existingAbility);
                    if (upgradedAbility != null)
                    {
                        filtered.Add(upgradedAbility);
                    }
                }
            }
            else
            {
                // �������� ���� �� �ɷ� �߰�
                filtered.Add(ability);
            }
        }

        return filtered;
    }

    // ��� ������ �ɷ� ��� ��������
    private List<DungeonAbility> GetAllAbilities()
    {
        List<DungeonAbility> allAbilities = new List<DungeonAbility>();

        // �нú� �ɷ� �߰�
        var passiveAbilities = PassiveAbilityFactory.CreateAllPassiveAbilities();
        allAbilities.AddRange(passiveAbilities.Cast<DungeonAbility>());

        // ���� �ɷ� �߰�
        var attackAbilities = AttackAbilityFactory.CreateAllAttackAbilities();
        allAbilities.AddRange(attackAbilities.Cast<DungeonAbility>());

        // �̵� �ɷ� �߰�
        var movementAbilities = MovementAbilityFactory.CreateAllMovementAbilities();
        allAbilities.AddRange(movementAbilities.Cast<DungeonAbility>());

        // Ư�� �ɷ� �߰� (���� �߰��� �κ�)
        var specialAbilities = SpecialAbilityFactory.CreateAllSpecialAbilities();
        allAbilities.AddRange(specialAbilities.Cast<DungeonAbility>());

        return allAbilities;
    }

    // �������� ���� �ɷ� ������ ����
    // �������� ���� �ɷ� ������ ����
    private DungeonAbility CreateUpgradedAbilityCopy(DungeonAbility templateAbility, DungeonAbility existingAbility)
    {
        // �нú� �ɷ��� ���
        if (templateAbility is PassiveAbility passiveTemplate)
        {
            PassiveAbility upgraded = new PassiveAbility();
            upgraded.Initialize(
                ((PassiveAbility)existingAbility).passiveType,
                ((PassiveAbility)existingAbility).effectValue,
                existingAbility.name,
                $"{existingAbility.description} (���� Lv.{existingAbility.level})",
                existingAbility.rarity
            );
            upgraded.id = existingAbility.id;
            upgraded.level = existingAbility.level;

            // ������ ���� �߰�
            upgraded.icon = existingAbility.icon;

            return upgraded;
        }
        // ���� �ɷ��� ���
        else if (templateAbility is AttackAbility attackTemplate)
        {
            AttackAbility upgraded = new AttackAbility();
            upgraded.Initialize(
                ((AttackAbility)existingAbility).attackType,
                ((AttackAbility)existingAbility).effectValue,
                existingAbility.name,
                $"{existingAbility.description} (���� Lv.{existingAbility.level})",
                existingAbility.rarity
            );
            upgraded.id = existingAbility.id;
            upgraded.level = existingAbility.level;

            // ������ ���� �߰�
            upgraded.icon = existingAbility.icon;

            return upgraded;
        }
        // �̵� �ɷ��� ���
        else if (templateAbility is MovementAbility movementTemplate)
        {
            MovementAbility upgraded = new MovementAbility();
            upgraded.Initialize(
                ((MovementAbility)existingAbility).movementType,
                ((MovementAbility)existingAbility).effectValue,
                existingAbility.name,
                $"{existingAbility.description} (���� Lv.{existingAbility.level})",
                existingAbility.rarity
            );
            upgraded.id = existingAbility.id;
            upgraded.level = existingAbility.level;

            // ������ ���� �߰�
            upgraded.icon = existingAbility.icon;

            return upgraded;
        }
        // Ư�� �ɷ��� ���
        else if (templateAbility is SpecialAbility specialAbility)
        {
            SpecialAbility upgraded = new SpecialAbility();
            upgraded.Initialize(
                ((SpecialAbility)existingAbility).specialType,
                ((SpecialAbility)existingAbility).effectValue,
                existingAbility.name,
                $"{existingAbility.description} (���� Lv.{existingAbility.level})",
                existingAbility.rarity
            );
            upgraded.id = existingAbility.id;
            upgraded.level = existingAbility.level;

            // ������ ���� �߰�
            upgraded.icon = existingAbility.icon;

            return upgraded;
        }

        Debug.LogWarning($"�ɷ� {templateAbility.id}�� ���׷��̵� ���纻�� ������ �� �����ϴ�. �������� �ʴ� Ÿ��: {templateAbility.GetType().Name}");
        return null;
    }

    // ����ġ ��� ���� ����
    private DungeonAbility SelectWeightedRandomAbility(List<DungeonAbility> abilities)
    {
        // ��͵����� �׷�ȭ
        Dictionary<Rarity, List<DungeonAbility>> groupedByRarity = new Dictionary<Rarity, List<DungeonAbility>>();
        foreach (Rarity rarity in System.Enum.GetValues(typeof(Rarity)))
        {
            groupedByRarity[rarity] = abilities.Where(a => a.rarity == rarity).ToList();
        }

        // ����ġ ��� ��͵� ����
        float totalWeight = rarityWeights.Sum();
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float weightSum = 0;
        Rarity selectedRarity = Rarity.Common; // �⺻��

        for (int i = 0; i < rarityWeights.Length; i++)
        {
            weightSum += rarityWeights[i];
            if (randomValue <= weightSum)
            {
                selectedRarity = (Rarity)i;
                break;
            }
        }

        // ���õ� ��͵��� �ɷ��� ���ٸ�, �� ���� ��͵����� ����
        while (groupedByRarity[selectedRarity].Count == 0 && (int)selectedRarity > 0)
        {
            selectedRarity = (Rarity)((int)selectedRarity - 1);
        }

        // �׷��� ���ٸ� ��ü���� ���� ����
        if (groupedByRarity[selectedRarity].Count == 0)
        {
            return abilities[Random.Range(0, abilities.Count)];
        }

        // ���õ� ��͵����� ���� ����
        return groupedByRarity[selectedRarity][Random.Range(0, groupedByRarity[selectedRarity].Count)];
    }

    // �ɷ� ���� ó��
    public void AcquireAbility(DungeonAbility selectedAbility)
    {
        // �̹� �ִ� �ɷ����� Ȯ��
        DungeonAbility existingAbility = currentAbilities.Find(a => a.id == selectedAbility.id);
        Debug.Log($"�ɷ�ġ ȹ�� ����: ID={selectedAbility.id}, �̸�={selectedAbility.name}, ����={selectedAbility.level}");
        Debug.Log($"���� ���� �ɷ� ��: {currentAbilities.Count}");

        if (existingAbility != null)
        {
            // ������
            existingAbility.OnLevelUp(playerClass);

            // �̺�Ʈ �߻�
            OnAbilityLevelUp?.Invoke();
        }
        else
        {
            // �� �ɷ� �߰�
            selectedAbility.OnAcquire(playerClass);
            currentAbilities.Add(selectedAbility);

            // �̺�Ʈ �߻�
            OnAbilityAcquired?.Invoke();
        }
    }

    // ���� ���� ���� �ɷ� ��� ��������
    public List<DungeonAbility> GetCurrentAbilities()
    {
        return new List<DungeonAbility>(currentAbilities);
    }

    // Ư�� ID�� �ɷ� ��������
    public DungeonAbility GetAbilityById(string id)
    {
        return currentAbilities.Find(a => a.id == id);
    }
    // ���� �ɷ� ���� Ȯ�� ������ ����
    public void SetRareAbilityChanceMultiplier(float multiplier)
    {
        rareAbilityChanceMultiplier = multiplier;
        Debug.Log($"���� �ɷ� ���� Ȯ�� ������ ����: {multiplier}");
    }
    public List<DungeonAbility> GetSmartAbilitySelection()
    {
        List<DungeonAbility> selection = new List<DungeonAbility>();

        // ��͵� ���ʽ� ��� - ���� ���׷��̵�� �칰 ȿ�� �ջ�
        float rarityBonus = 0f;
        if (StatUpgradeManager.Instance != null)
        {
            rarityBonus = StatUpgradeManager.Instance.GetTotalRarityBonus();
            Debug.Log($"����Ʈ ����: ���� ��͵� ���ʽ� +{rarityBonus:F1}% ����");
        }

        // �ð� ��ġ ȿ�� ����: ��ü���� ��͵� ����
        float deviceBonus = 0f;
        if (rareAbilityChanceMultiplier > 1f)
        {
            deviceBonus = (rareAbilityChanceMultiplier - 1f) * 100f;
            Debug.Log($"����Ʈ ����: �ð� ��ġ ��͵� ���ʽ� +{deviceBonus:F1}% ����");
        }

        // �� ���ʽ� ���
        float totalBonus = rarityBonus + deviceBonus;
        Debug.Log($"����Ʈ ����: �� ��͵� ���ʽ� +{totalBonus:F1}% ����");

        // 1. ���� ��Ȳ�� ���� ��͵� Ǯ ����
        float dungeonProgress = 0f;
        if (DungeonManager.Instance != null)
        {
            dungeonProgress = DungeonManager.Instance.GetDungeonProgress();
        }

        // ��͵� Ǯ ���� (20% �� 20+���ʽ�% Ȯ���� ȥ�� Ǯ ����)
        Rarity[] rarityPool;
        float randomRoll = UnityEngine.Random.value;

        // ���ʽ��� ���� ȥ�� ��͵� Ǯ Ȯ�� ���� (�ִ� +10%)
        if (randomRoll < 0.2f + (totalBonus * 0.005f))
        {
            // �ð� ��ġ ȿ��: ȥ�� Ǯ�� ����/����/�������� ���� ����
            if (deviceBonus > 0)
            {
                // ���ʽ��� Ŭ���� ��� ��͵� ���� ����
                rarityPool = new Rarity[] {
                Rarity.Common,
                Rarity.Uncommon, Rarity.Uncommon,
                Rarity.Rare, Rarity.Rare,
                Rarity.Epic,
                Rarity.Legendary
            };
                Debug.Log("����Ʈ ����: ���� ȥ�� ��͵� Ǯ ��� (��� ��͵� ���� ����)");
            }
            else
            {
                rarityPool = new Rarity[] {
                Rarity.Common, Rarity.Common,
                Rarity.Uncommon, Rarity.Uncommon,
                Rarity.Rare,
                Rarity.Epic,
                Rarity.Legendary
            };
                Debug.Log("����Ʈ ����: �Ϲ� ȥ�� ��͵� Ǯ ���");
            }
        }
        else // �ϰ��� ��͵� Ǯ
        {
            Rarity selectedRarity;
            float rarityRoll = UnityEngine.Random.value;

            // ���൵�� ���ʽ��� ���� Ȯ�� ����
            // �ð� ��ġ ȿ��: ��� ��͵� ���� Ȯ�� ���� (deviceBonus �ݿ�)
            float legendaryChance = 0.02f + (dungeonProgress * 0.05f) + (totalBonus * 0.0025f);
            float epicChance = 0.05f + (dungeonProgress * 0.07f) + (totalBonus * 0.004f);
            float rareChance = 0.10f + (dungeonProgress * 0.08f) + (totalBonus * 0.005f);

            // �ð� ��ġ ȿ���� ������ �� ���� Ȯ�� ���
            if (deviceBonus > 0)
            {
                // �߰� ���ʽ� (�ð� ��ġ ȿ���� Ŭ���� �� ū ���ʽ�)
                float extraBonus = deviceBonus * 0.005f; // �ִ� 15%���� 0.075 �߰�
                legendaryChance += extraBonus;
                epicChance += extraBonus * 1.5f;
                rareChance += extraBonus * 2f;
            }

            // Ȯ�� ���� (�ִ밪 ����)
            legendaryChance = Mathf.Min(legendaryChance, 0.15f); // �ִ� 15%
            epicChance = Mathf.Min(epicChance, 0.25f); // �ִ� 25%
            rareChance = Mathf.Min(rareChance, 0.35f); // �ִ� 35%

            float uncommonChance = 0.35f - (dungeonProgress * 0.1f) - (totalBonus * 0.002f);
            uncommonChance = Mathf.Max(uncommonChance, 0.15f); // �ּ� 15% ����

            float commonChance = 1.0f - (legendaryChance + epicChance + rareChance + uncommonChance);
            commonChance = Mathf.Max(commonChance, 0.1f); // �ּ� 10% ����

            // Ȯ�� �α�
            Debug.Log($"��͵� Ȯ��: �������� {legendaryChance:P1}, ���� {epicChance:P1}, ���� {rareChance:P1}, ��Ŀ�� {uncommonChance:P1}, Ŀ�� {commonChance:P1}");

            // ���� �� ������� ��͵� ����
            if (rarityRoll < commonChance) selectedRarity = Rarity.Common;
            else if (rarityRoll < commonChance + uncommonChance) selectedRarity = Rarity.Uncommon;
            else if (rarityRoll < commonChance + uncommonChance + rareChance) selectedRarity = Rarity.Rare;
            else if (rarityRoll < commonChance + uncommonChance + rareChance + epicChance) selectedRarity = Rarity.Epic;
            else selectedRarity = Rarity.Legendary;

            rarityPool = Enumerable.Repeat(selectedRarity, 6).ToArray();
            Debug.Log($"����Ʈ ����: ���� ��͵� Ǯ ��� ({selectedRarity})");
        }

        // 2. ������ ��ȸ �߰� (20% �⺻ Ȯ��)
        if (currentAbilities.Count > 0)
        {
            var upgradable = currentAbilities.Where(a => a.level < 5).ToList();

            if (upgradable.Count > 0)
            {
                // �ð� ��ġ ȿ��: ������ Ȯ�� ����
                float levelUpChance = 0.2f + (totalBonus * 0.004f);
                levelUpChance = Mathf.Min(levelUpChance, 0.35f); // �ִ� 35%�� ����

                if (UnityEngine.Random.value < levelUpChance)
                {
                    Debug.Log($"����Ʈ ����: ������ Ȯ�� {levelUpChance:P1}�� ���õ�");
                    var toUpgrade = upgradable[UnityEngine.Random.Range(0, upgradable.Count)];
                    var upgraded = CreateUpgradedAbilityCopy(toUpgrade, toUpgrade);
                    selection.Add(upgraded);
                }
            }
        }

        // 3. ���� ���� ä��� (���� ���� - ���� �ڵ� ����)
        List<DungeonAbility> allAvailable = FilterAvailableAbilities();

        // �̹� ���õ� �����Ƽ ����
        foreach (var ability in selection)
        {
            allAvailable.RemoveAll(a => a.id == ability.id);
        }

        // �� ��͵� ���Կ� �´� �����Ƽ ����
        for (int i = selection.Count; i < abilitiesPerSelection; i++)
        {
            if (allAvailable.Count == 0)
            {
                Debug.Log("����Ʈ ����: �� �̻� ��� ������ �����Ƽ�� ����");
                break;
            }

            // �ش� ��͵� �ε��� (�迭 ���� �ʰ� ����)
            int rarityIndex = Mathf.Min(i, rarityPool.Length - 1);
            Rarity targetRarity = rarityPool[rarityIndex];
            Debug.Log($"����Ʈ ����: ���� {i + 1} - ��ǥ ��͵�: {targetRarity}");

            // �ش� ��͵��� �����Ƽ ���͸�
            var abilitiesOfRarity = allAvailable.Where(a => a.rarity == targetRarity).ToList();
            Debug.Log($"����Ʈ ����: {targetRarity} ��͵��� ��� ������ �����Ƽ �� = {abilitiesOfRarity.Count}");

            // �ش� ��͵� �����Ƽ�� ������ �� ���� ��͵� �õ�
            Rarity originalRarity = targetRarity;
            while (abilitiesOfRarity.Count == 0 && (int)targetRarity > 0)
            {
                targetRarity = (Rarity)((int)targetRarity - 1);
                abilitiesOfRarity = allAvailable.Where(a => a.rarity == targetRarity).ToList();
                Debug.Log($"����Ʈ ����: ��͵� �϶� {originalRarity} -> {targetRarity}, ��� ���� �� = {abilitiesOfRarity.Count}");
            }

            // �����Ƽ ���� (ī�װ� �پ缺 ���)
            if (abilitiesOfRarity.Count > 0)
            {
                // ī�װ� �ߺ� ����
                var currentTypes = selection.Select(a => a.GetType()).ToList();
                string currentCategoriesStr = string.Join(", ", currentTypes.Select(t => t.Name));
                Debug.Log($"����Ʈ ����: ���� ���õ� ī�װ� = {currentCategoriesStr}");

                // �ߺ����� �ʴ� ī�װ� �켱 ����
                var nonDuplicateTypes = abilitiesOfRarity
                    .Where(a => !currentTypes.Contains(a.GetType()))
                    .ToList();

                Debug.Log($"����Ʈ ����: �ߺ����� �ʴ� ī�װ��� �����Ƽ �� = {nonDuplicateTypes.Count}");

                if (nonDuplicateTypes.Count > 0)
                {
                    var selected = nonDuplicateTypes[UnityEngine.Random.Range(0, nonDuplicateTypes.Count)];
                    selection.Add(selected);
                    allAvailable.Remove(selected);
                    Debug.Log($"����Ʈ ����: �ߺ����� �ʴ� ī�װ����� ������ - {selected.name} ({selected.GetType().Name}, {selected.rarity})");
                }
                else // �ߺ��� ī�װ��� ������
                {
                    var selected = abilitiesOfRarity[UnityEngine.Random.Range(0, abilitiesOfRarity.Count)];
                    selection.Add(selected);
                    allAvailable.Remove(selected);
                    Debug.Log($"����Ʈ ����: �ߺ� ī�װ����� ������ - {selected.name} ({selected.GetType().Name}, {selected.rarity})");
                }
            }
            else
            {
                Debug.Log("����Ʈ ����: ������ ��͵��� �����Ƽ�� ����");
            }
        }

        // ���� ��� �α�
        Debug.Log($"����Ʈ ����: ���� ���õ� �����Ƽ �� = {selection.Count}");
        foreach (var ability in selection)
        {
            Debug.Log($"����Ʈ ���� ���: {ability.name} (Ÿ��: {ability.GetType().Name}, ��͵�: {ability.rarity}, ����: {ability.level})");
        }

        return selection;
    }

    // �����Ƽ Ÿ������ ���͸��ϴ� ���� �޼���
    private List<DungeonAbility> FilterAvailableAbilitiesByType(System.Type abilityType)
    {
        return FilterAvailableAbilities()
            .Where(a => a.GetType() == abilityType)
            .ToList();
    }
}
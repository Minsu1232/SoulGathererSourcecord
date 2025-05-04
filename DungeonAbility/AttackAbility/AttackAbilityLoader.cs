// AttackAbilityLoader.cs - CSV ������ �δ� Ŭ����
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AttackAbilityLoader : MonoBehaviour
{
    // Singleton ����
    public static AttackAbilityLoader Instance { get; private set; }

    // ���� �ɷ� �����ͺ��̽�
    private Dictionary<string, Dictionary<string, string>> attackAbilityData = new Dictionary<string, Dictionary<string, string>>();

    // CSV ���� ���
    private string attackAbilitiesPath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // CSV ���� ��� ����
            attackAbilitiesPath = Path.Combine(Application.persistentDataPath, "AttackAbilities.csv");

            // CSV ���� ���� �� �ε�
            CopyCSVFromStreamingAssets();
            LoadAttackAbilitiesFromCSV();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // StreamingAssets���� CSV ���� ����
    private void CopyCSVFromStreamingAssets()
    {
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "AttackAbilities.csv");

        if (File.Exists(streamingPath))
        {
            File.Copy(streamingPath, attackAbilitiesPath, true);
            Debug.Log("���� �ɷ� CSV ���� ���� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("StreamingAssets���� AttackAbilities.csv ������ ã�� �� �����ϴ�.");
        }
    }

    // CSV���� ���� �ɷ� ������ �ε�
    private void LoadAttackAbilitiesFromCSV()
    {
        if (!File.Exists(attackAbilitiesPath))
        {
            Debug.LogError($"���� �ɷ� CSV ������ ã�� �� �����ϴ�: {attackAbilitiesPath}");
            return;
        }

        string[] lines = File.ReadAllLines(attackAbilitiesPath);
        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(',');

            // ���� ������� Ȯ��
            if (values.Length < headers.Length)
            {
                Debug.LogWarning($"���� �ɷ� CSV �� {i + 1}�� ���� �����մϴ�. �ǳʶݴϴ�.");
                continue;
            }

            Dictionary<string, string> abilityData = new Dictionary<string, string>();
            for (int j = 0; j < headers.Length; j++)
            {
                abilityData[headers[j]] = values[j];
            }

            string id = values[0];
            attackAbilityData[id] = abilityData;
        }

        Debug.Log($"���� �ɷ� ������ �ε� �Ϸ�: {attackAbilityData.Count}�� �ɷ�");
    }

    // ID�� ���� �ɷ� ������ ��������
    public Dictionary<string, string> GetAttackAbilityData(string id)
    {
        if (attackAbilityData.TryGetValue(id, out Dictionary<string, string> data))
        {
            return data;
        }

        Debug.LogWarning($"���� �ɷ� ID�� ã�� �� ����: {id}");
        return null;
    }

    // ��� ���� �ɷ� ������ ��������
    public List<Dictionary<string, string>> GetAllAttackAbilityData()
    {
        List<Dictionary<string, string>> allData = new List<Dictionary<string, string>>();
        foreach (var data in attackAbilityData.Values)
        {
            allData.Add(data);
        }
        return allData;
    }

    // ID�� ���� �ɷ� �ν��Ͻ� ����
    public AttackAbility CreateAttackAbility(string id)
    {
        Dictionary<string, string> data = GetAttackAbilityData(id);
        if (data != null)
        {            
            return AttackAbility.FromCSVData(data);
        }
        return null;
    }

    // ��� ���� �ɷ� �ν��Ͻ� ����
    public List<AttackAbility> CreateAllAttackAbilities()
    {
        List<AttackAbility> abilities = new List<AttackAbility>();
        foreach (var data in attackAbilityData.Values)
        {
            AttackAbility ability = AttackAbility.FromCSVData(data);
            abilities.Add(ability);
            
        }
        return abilities;
    }
}
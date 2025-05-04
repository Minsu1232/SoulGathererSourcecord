// SpecialAbilityLoader.cs - CSV ������ �δ� Ŭ����
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpecialAbilityLoader : MonoBehaviour
{
    // Singleton ����
    public static SpecialAbilityLoader Instance { get; private set; }

    // Ư�� �ɷ� �����ͺ��̽�
    private Dictionary<string, Dictionary<string, string>> specialAbilityData = new Dictionary<string, Dictionary<string, string>>();

    // CSV ���� ���
    private string specialAbilitiesPath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // CSV ���� ��� ����
            specialAbilitiesPath = Path.Combine(Application.persistentDataPath, "SpecialAbilities.csv");

            // CSV ���� ���� �� �ε�
            CopyCSVFromStreamingAssets();
            LoadSpecialAbilitiesFromCSV();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // StreamingAssets���� CSV ���� ����
    private void CopyCSVFromStreamingAssets()
    {
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "SpecialAbilities.csv");

        if (File.Exists(streamingPath))
        {
            File.Copy(streamingPath, specialAbilitiesPath, true);
            Debug.Log("Ư�� �ɷ� CSV ���� ���� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("StreamingAssets���� SpecialAbilities.csv ������ ã�� �� �����ϴ�.");
        }
    }

    // CSV���� Ư�� �ɷ� ������ �ε�
    private void LoadSpecialAbilitiesFromCSV()
    {
        if (!File.Exists(specialAbilitiesPath))
        {
            Debug.LogError($"Ư�� �ɷ� CSV ������ ã�� �� �����ϴ�: {specialAbilitiesPath}");
            return;
        }

        string[] lines = File.ReadAllLines(specialAbilitiesPath);
        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(',');

            // ���� ������� Ȯ��
            if (values.Length < headers.Length)
            {
                Debug.LogWarning($"Ư�� �ɷ� CSV �� {i + 1}�� ���� �����մϴ�. �ǳʶݴϴ�.");
                continue;
            }

            Dictionary<string, string> abilityData = new Dictionary<string, string>();
            for (int j = 0; j < headers.Length; j++)
            {
                abilityData[headers[j]] = values[j];
            }

            string id = values[0];
            specialAbilityData[id] = abilityData;
        }

        Debug.Log($"Ư�� �ɷ� ������ �ε� �Ϸ�: {specialAbilityData.Count}�� �ɷ�");
    }

    // ID�� Ư�� �ɷ� ������ ��������
    public Dictionary<string, string> GetSpecialAbilityData(string id)
    {
        if (specialAbilityData.TryGetValue(id, out Dictionary<string, string> data))
        {
            return data;
        }

        Debug.LogWarning($"Ư�� �ɷ� ID�� ã�� �� ����: {id}");
        return null;
    }

    // ��� Ư�� �ɷ� ������ ��������
    public List<Dictionary<string, string>> GetAllSpecialAbilityData()
    {
        List<Dictionary<string, string>> allData = new List<Dictionary<string, string>>();
        foreach (var data in specialAbilityData.Values)
        {
            allData.Add(data);
        }
        return allData;
    }

    // ID�� Ư�� �ɷ� �ν��Ͻ� ����
    public SpecialAbility CreateSpecialAbility(string id)
    {
        Dictionary<string, string> data = GetSpecialAbilityData(id);
        if (data != null)
        {
            return SpecialAbility.FromCSVData(data);
        }
        return null;
    }

    // ��� Ư�� �ɷ� �ν��Ͻ� ����
    public List<SpecialAbility> CreateAllSpecialAbilities()
    {
        List<SpecialAbility> abilities = new List<SpecialAbility>();
        foreach (var data in specialAbilityData.Values)
        {
            SpecialAbility ability = SpecialAbility.FromCSVData(data);
            abilities.Add(ability);
        }
        return abilities;
    }
}
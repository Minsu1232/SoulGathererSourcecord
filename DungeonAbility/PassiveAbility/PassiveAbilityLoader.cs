// 3. CSV ������ �δ� Ŭ����
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PassiveAbilityLoader : MonoBehaviour
{
    // Singleton ����
    public static PassiveAbilityLoader Instance { get; private set; }

    // �нú� �ɷ� �����ͺ��̽�
    private Dictionary<string, Dictionary<string, string>> passiveAbilityData = new Dictionary<string, Dictionary<string, string>>();

    // CSV ���� ���
    private string passiveAbilitiesPath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // CSV ���� ��� ����
            passiveAbilitiesPath = Path.Combine(Application.persistentDataPath, "PassiveAbilities.csv");

            // CSV ���� ���� �� �ε�
            CopyCSVFromStreamingAssets();
            LoadPassiveAbilitiesFromCSV();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // StreamingAssets���� CSV ���� ����
    private void CopyCSVFromStreamingAssets()
    {
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "PassiveAbilities.csv");

        if (File.Exists(streamingPath))
        {
            File.Copy(streamingPath, passiveAbilitiesPath, true);
            Debug.Log("�нú� �ɷ� CSV ���� ���� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("StreamingAssets���� PassiveAbilities.csv ������ ã�� �� �����ϴ�.");
        }
    }

    // CSV���� �нú� �ɷ� ������ �ε�
    private void LoadPassiveAbilitiesFromCSV()
    {
        if (!File.Exists(passiveAbilitiesPath))
        {
            Debug.LogError($"�нú� �ɷ� CSV ������ ã�� �� �����ϴ�: {passiveAbilitiesPath}");
            return;
        }

        string[] lines = File.ReadAllLines(passiveAbilitiesPath);
        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(',');

            // ���� ������� Ȯ��
            if (values.Length < headers.Length)
            {
                Debug.LogWarning($"�нú� �ɷ� CSV �� {i + 1}�� ���� �����մϴ�. �ǳʶݴϴ�.");
                continue;
            }

            Dictionary<string, string> abilityData = new Dictionary<string, string>();
            for (int j = 0; j < headers.Length; j++)
            {
                abilityData[headers[j]] = values[j];
            }

            string id = values[0];
            passiveAbilityData[id] = abilityData;
        }

        Debug.Log($"�нú� �ɷ� ������ �ε� �Ϸ�: {passiveAbilityData.Count}�� �ɷ�");
    }

    // ID�� �нú� �ɷ� ������ ��������
    public Dictionary<string, string> GetPassiveAbilityData(string id)
    {
        if (passiveAbilityData.TryGetValue(id, out Dictionary<string, string> data))
        {
            return data;
        }

        Debug.LogWarning($"�нú� �ɷ� ID�� ã�� �� ����: {id}");
        return null;
    }

    // ��� �нú� �ɷ� ������ ��������
    public List<Dictionary<string, string>> GetAllPassiveAbilityData()
    {
        List<Dictionary<string, string>> allData = new List<Dictionary<string, string>>();
        foreach (var data in passiveAbilityData.Values)
        {
            allData.Add(data);
        }
        return allData;
    }

    // ID�� �нú� �ɷ� �ν��Ͻ� ����
    public PassiveAbility CreatePassiveAbility(string id)
    {
        Dictionary<string, string> data = GetPassiveAbilityData(id);
        if (data != null)
        {
            Debug.Log("���⼭ ������");
            return PassiveAbility.FromCSVData(data);
            
        }
        return null;
    }

    // ��� �нú� �ɷ� �ν��Ͻ� ����
    public List<PassiveAbility> CreateAllPassiveAbilities()
    {
        List<PassiveAbility> abilities = new List<PassiveAbility>();
        foreach (var data in passiveAbilityData.Values)
        {
            Debug.Log("���⼭ ������2");
            PassiveAbility ability = PassiveAbility.FromCSVData(data);
            abilities.Add(ability);
        }
        return abilities;
    }
}
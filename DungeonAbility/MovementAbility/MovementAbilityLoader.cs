// MovementAbilityLoader.cs - CSV ������ �δ� Ŭ����
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MovementAbilityLoader : MonoBehaviour
{
    // Singleton ����
    public static MovementAbilityLoader Instance { get; private set; }

    // �̵� �ɷ� �����ͺ��̽�
    private Dictionary<string, Dictionary<string, string>> movementAbilityData = new Dictionary<string, Dictionary<string, string>>();

    // CSV ���� ���
    private string movementAbilitiesPath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // CSV ���� ��� ����
            movementAbilitiesPath = Path.Combine(Application.persistentDataPath, "MovementAbilities.csv");

            // CSV ���� ���� �� �ε�
            CopyCSVFromStreamingAssets();
            LoadMovementAbilitiesFromCSV();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // StreamingAssets���� CSV ���� ����
    private void CopyCSVFromStreamingAssets()
    {
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "MovementAbilities.csv");

        if (File.Exists(streamingPath))
        {
            File.Copy(streamingPath, movementAbilitiesPath, true);
            Debug.Log("�̵� �ɷ� CSV ���� ���� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("StreamingAssets���� MovementAbilities.csv ������ ã�� �� �����ϴ�.");
        }
    }

    // CSV���� �̵� �ɷ� ������ �ε�
    private void LoadMovementAbilitiesFromCSV()
    {
        if (!File.Exists(movementAbilitiesPath))
        {
            Debug.LogError($"�̵� �ɷ� CSV ������ ã�� �� �����ϴ�: {movementAbilitiesPath}");
            return;
        }

        string[] lines = File.ReadAllLines(movementAbilitiesPath);
        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(',');

            // ���� ������� Ȯ��
            if (values.Length < headers.Length)
            {
                Debug.LogWarning($"�̵� �ɷ� CSV �� {i + 1}�� ���� �����մϴ�. �ǳʶݴϴ�.");
                continue;
            }

            Dictionary<string, string> abilityData = new Dictionary<string, string>();
            for (int j = 0; j < headers.Length; j++)
            {
                abilityData[headers[j]] = values[j];
            }

            string id = values[0];
            movementAbilityData[id] = abilityData;
        }

        Debug.Log($"�̵� �ɷ� ������ �ε� �Ϸ�: {movementAbilityData.Count}�� �ɷ�");
    }

    // ID�� �̵� �ɷ� ������ ��������
    public Dictionary<string, string> GetMovementAbilityData(string id)
    {
        if (movementAbilityData.TryGetValue(id, out Dictionary<string, string> data))
        {
            return data;
        }

        Debug.LogWarning($"�̵� �ɷ� ID�� ã�� �� ����: {id}");
        return null;
    }

    // ��� �̵� �ɷ� ������ ��������
    public List<Dictionary<string, string>> GetAllMovementAbilityData()
    {
        List<Dictionary<string, string>> allData = new List<Dictionary<string, string>>();
        foreach (var data in movementAbilityData.Values)
        {
            allData.Add(data);
        }
        return allData;
    }

    // ID�� �̵� �ɷ� �ν��Ͻ� ����
    public MovementAbility CreateMovementAbility(string id)
    {
        Dictionary<string, string> data = GetMovementAbilityData(id);
        if (data != null)
        {
            return MovementAbility.FromCSVData(data);
        }
        return null;
    }

    // ��� �̵� �ɷ� �ν��Ͻ� ����
    public List<MovementAbility> CreateAllMovementAbilities()
    {
        List<MovementAbility> abilities = new List<MovementAbility>();
        foreach (var data in movementAbilityData.Values)
        {
            MovementAbility ability = MovementAbility.FromCSVData(data);
            abilities.Add(ability);
        }
        return abilities;
    }
}
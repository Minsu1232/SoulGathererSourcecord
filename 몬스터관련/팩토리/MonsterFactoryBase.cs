using System;
using System.Collections.Generic; // Dictionary ����� ���� �߰�
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
/// <summary>
/// ���� ������ ����ϴ� �߻� ���丮 ��� Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 2�� '���� ������ ����' > '���丮 ����(Factory)'
/// 
/// �ֿ� ���:
/// - ���� �ν��Ͻ� ���� �� �ʱ�ȭ
/// - Addressables �ý����� ���� ���� ������ �񵿱� �ε�
/// - ���� �����Ϳ� �ν��Ͻ� ����
/// - ���� ������ ���丮 Ȯ�� ����
/// </remarks>
public abstract class MonsterFactoryBase
{
    // ��ȯ Ƚ���� �����ϴ� ���� ��ųʸ� �߰�
    private static Dictionary<string, int> spawnCounts = new Dictionary<string, int>();

    protected abstract IMonsterClass CreateMonsterInstance(ICreatureData data);
    protected abstract string GetMonsterDataKey();
    protected abstract bool IsEliteAvailable();
    // ��ü���� ������ Ÿ���� ��� ���� �߻� �޼��� �߰�
    protected abstract Type GetDataType();

    public virtual ICreatureStatus CreateMonster(Vector3 spawnPosition, Action<ICreatureStatus> onMonsterCreated)
    {
        string key = GetMonsterDataKey();

        // �� Ÿ���� ���Ͱ� �� �� ��ȯ�Ǿ����� Ȯ�� �� ī��Ʈ ����
        if (!spawnCounts.ContainsKey(key))
        {
            spawnCounts[key] = 1;
            Debug.Log($"ù ��° ��ȯ: {key}");
        }
        else
        {
            spawnCounts[key]++;
            Debug.Log($"{spawnCounts[key]}��° ��ȯ: {key}, Ű: {key}");

            // �� ��° ��ȯ�� ��� ����� �ߴ�
            if (spawnCounts[key] == 2)
            {
                Debug.Log("�� ��° ��ȯ ������ - ����� �ߴ��� ����");
               
            }
        }

        LoadMonsterData(spawnPosition, onMonsterCreated);
        return null;
    }

    private void LoadMonsterData(Vector3 spawnPosition, Action<ICreatureStatus> onMonsterCreated)
    {
        string key = GetMonsterDataKey();
        bool isSecondSpawn = spawnCounts.ContainsKey(key) && spawnCounts[key] >= 2;

        // �� ��° �̻� ��ȯ�� ��� ���������� ó��
        if (isSecondSpawn)
        {
            var data = Addressables.LoadAssetAsync<MonsterData>(GetMonsterDataKey()).WaitForCompletion();
            if (data != null && data is ICreatureData creatureData)
            {
                var prefab = Addressables.LoadAssetAsync<GameObject>(creatureData.monsterPrefabKey).WaitForCompletion();
                if (prefab != null)
                {
                    var monsterObject = GameObject.Instantiate(prefab, spawnPosition, Quaternion.identity);
                    FinalizeMonsterCreation(monsterObject, creatureData, onMonsterCreated);
                    return;
                }
            }
        }

        // ù ��° ��ȯ �Ǵ� ���� ó�� ���� �� ���� �񵿱� ��� ���
        var loadOperation = Addressables.LoadAssetAsync<MonsterData>(GetMonsterDataKey());
        loadOperation.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded &&
                handle.Result is ICreatureData data)
            {
                InstantiatePrefab(data, spawnPosition, onMonsterCreated, isSecondSpawn);
            }
            else
            {
                Debug.LogError($"Failed to load MonsterData with Key: {GetMonsterDataKey()}");
                onMonsterCreated?.Invoke(null);
            }
        };
    }

    private void InstantiatePrefab(ICreatureData data, Vector3 position, Action<ICreatureStatus> onMonsterCreated, bool isSecondSpawn)
    {
        if (data == null || string.IsNullOrEmpty(data.monsterPrefabKey))  // �빮�ڷ� ����
        {
            Debug.LogError("MonsterData is null or PrefabKey is missing.");
            return;
        }

        Addressables.InstantiateAsync(data.monsterPrefabKey, position, Quaternion.identity)
            .Completed += handle =>
            {
                if (isSecondSpawn)
                {
                    Debug.Log("InstantiatePrefab �ݹ� ���� - ����� �ߴ�");
                    
                }

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("@@@@@@@@@@@@@@@" + "������ �����");
                    FinalizeMonsterCreation(handle.Result, data, onMonsterCreated);
                }
            };
    }

    protected virtual void FinalizeMonsterCreation(GameObject monsterObject, ICreatureData data, Action<ICreatureStatus> onMonsterCreated)
    {


        ICreatureStatus status = monsterObject.AddComponent<MonsterStatus>();
        IMonsterClass monster = CreateMonsterInstance(data);
       
        Debug.Log("@@@@@@@@@@@@@@@" + "�߰��� �Ǿ���");
        status.Initialize(monster);
        if (monster is EliteMonster)
        {
            monsterObject.AddComponent<EliteMonsterController>();
        }
        onMonsterCreated?.Invoke(status);
    }

   
}
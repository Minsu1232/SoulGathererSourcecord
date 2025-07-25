using System;
using System.Collections.Generic; // Dictionary 사용을 위해 추가
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
/// <summary>
/// 몬스터 생성을 담당하는 추상 팩토리 기반 클래스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 2절 '적용 디자인 패턴' > '팩토리 패턴(Factory)'
/// 
/// 주요 기능:
/// - 몬스터 인스턴스 생성 및 초기화
/// - Addressables 시스템을 통한 몬스터 프리팹 비동기 로드
/// - 몬스터 데이터와 인스턴스 연결
/// - 몬스터 종류별 팩토리 확장 지원
/// </remarks>
public abstract class MonsterFactoryBase
{
    // 소환 횟수를 추적하는 정적 딕셔너리 추가
    private static Dictionary<string, int> spawnCounts = new Dictionary<string, int>();

    protected abstract IMonsterClass CreateMonsterInstance(ICreatureData data);
    protected abstract string GetMonsterDataKey();
    protected abstract bool IsEliteAvailable();
    // 구체적인 데이터 타입을 얻기 위한 추상 메서드 추가
    protected abstract Type GetDataType();

    public virtual ICreatureStatus CreateMonster(Vector3 spawnPosition, Action<ICreatureStatus> onMonsterCreated)
    {
        string key = GetMonsterDataKey();

        // 이 타입의 몬스터가 몇 번 소환되었는지 확인 및 카운트 증가
        if (!spawnCounts.ContainsKey(key))
        {
            spawnCounts[key] = 1;
            Debug.Log($"첫 번째 소환: {key}");
        }
        else
        {
            spawnCounts[key]++;
            Debug.Log($"{spawnCounts[key]}번째 소환: {key}, 키: {key}");

            // 두 번째 소환인 경우 디버그 중단
            if (spawnCounts[key] == 2)
            {
                Debug.Log("두 번째 소환 감지됨 - 디버그 중단점 실행");
               
            }
        }

        LoadMonsterData(spawnPosition, onMonsterCreated);
        return null;
    }

    private void LoadMonsterData(Vector3 spawnPosition, Action<ICreatureStatus> onMonsterCreated)
    {
        string key = GetMonsterDataKey();
        bool isSecondSpawn = spawnCounts.ContainsKey(key) && spawnCounts[key] >= 2;

        // 두 번째 이상 소환일 경우 동기적으로 처리
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

        // 첫 번째 소환 또는 동기 처리 실패 시 기존 비동기 방식 사용
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
        if (data == null || string.IsNullOrEmpty(data.monsterPrefabKey))  // 대문자로 수정
        {
            Debug.LogError("MonsterData is null or PrefabKey is missing.");
            return;
        }

        Addressables.InstantiateAsync(data.monsterPrefabKey, position, Quaternion.identity)
            .Completed += handle =>
            {
                if (isSecondSpawn)
                {
                    Debug.Log("InstantiatePrefab 콜백 진입 - 디버그 중단");
                    
                }

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("@@@@@@@@@@@@@@@" + "지금은 여기다");
                    FinalizeMonsterCreation(handle.Result, data, onMonsterCreated);
                }
            };
    }

    protected virtual void FinalizeMonsterCreation(GameObject monsterObject, ICreatureData data, Action<ICreatureStatus> onMonsterCreated)
    {


        ICreatureStatus status = monsterObject.AddComponent<MonsterStatus>();
        IMonsterClass monster = CreateMonsterInstance(data);
       
        Debug.Log("@@@@@@@@@@@@@@@" + "추가가 되었다");
        status.Initialize(monster);
        if (monster is EliteMonster)
        {
            monsterObject.AddComponent<EliteMonsterController>();
        }
        onMonsterCreated?.Invoke(status);
    }

   
}
using System;
/// <summary>
/// 쥐 몬스터 생성을 담당하는 팩토리 클래스
/// </summary>
/// <remarks>
/// 기술문서 참조: 2장 '몬스터 설계 구조' > 2절 '적용 디자인 패턴' > '팩토리 패턴(Factory)'
/// 
/// 주요 기능:
/// - 일반 및 엘리트 쥐 몬스터 생성
/// - 확률 기반 엘리트 몬스터 등장 처리
/// - 몬스터 데이터 키 관리
/// </remarks>
public class RatMonsterFactory : MonsterFactoryBase
{

    private const float ELITE_CHANCE = 0.1f;
    protected override Type GetDataType()
    {
        return typeof(MonsterData);  // 일반 몬스터는 MonsterData 사용
    }
    protected override IMonsterClass CreateMonsterInstance(ICreatureData data)
    {
        return UnityEngine.Random.value < ELITE_CHANCE && IsEliteAvailable()
            ? new EliteMonster(data)
            : new DummyMonster(data);
    }

    protected override string GetMonsterDataKey() => "MonsterData_8";
    protected override bool IsEliteAvailable() => true;
}

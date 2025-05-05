using System;
/// <summary>
/// �� ���� ������ ����ϴ� ���丮 Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 2�� '���� ������ ����' > '���丮 ����(Factory)'
/// 
/// �ֿ� ���:
/// - �Ϲ� �� ����Ʈ �� ���� ����
/// - Ȯ�� ��� ����Ʈ ���� ���� ó��
/// - ���� ������ Ű ����
/// </remarks>
public class RatMonsterFactory : MonsterFactoryBase
{

    private const float ELITE_CHANCE = 0.1f;
    protected override Type GetDataType()
    {
        return typeof(MonsterData);  // �Ϲ� ���ʹ� MonsterData ���
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

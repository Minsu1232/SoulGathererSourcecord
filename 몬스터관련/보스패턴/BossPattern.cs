using DG.Tweening;
using UnityEngine;
using static IMonsterState;
/// <summary>
/// ���� ������ ��� Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 3�� '���� �ý��� ���� �� Ȯ�� ���' > 1�� '���� ���� �ý���'
/// 
/// �ֿ� ���:
/// - ����: �̴ϰ��� + ���� ���� ������ ���� ���� ����
/// - ������ ���� ���� ���� �� ����
/// - �̴ϰ��� ����/���п� ���� ���� ���� ����
/// - ���� ���� ������ �������� ������ ���� ����
/// </remarks>
public abstract class BossPattern : BasePhysicalAttackStrategy
{
    protected BasicAttackStrategy basicAttack;
    protected JumpAttackStrategy jumpAttack;
    protected MiniGameManager miniGameManager;
    protected BossPatternManager patternManager;
    protected BossStatus bossStatus;
    protected AttackPatternData patternData;
    protected bool isRunning;
    protected Sequence patternSequence;
    protected BossData bossData_;
    protected Animator animator;
    protected CreatureAI owner;

    private float patternLastAttackTime;  // ���ϸ��� Ÿ�̸�
    protected virtual bool IsExecutingPattern { get; }

    protected BossPattern(
        MiniGameManager miniGameManager, 
        GameObject shockwaveEffectPrefab, 
        float shockwaveRadius, 
        BossData bossData, 
        Animator animator,
        CreatureAI owner,        
        AttackPatternData patternData
       )
    {
        bossStatus = owner.GetComponent<BossStatus>();
        basicAttack = new BasicAttackStrategy();
        jumpAttack = new JumpAttackStrategy(shockwaveEffectPrefab, shockwaveRadius,owner);
        bossData_ = bossData;
        this.animator = animator;
        this.miniGameManager = miniGameManager;
        this.owner = owner;
        this.patternManager = new BossPatternManager(bossStatus);
        this.patternData = patternData;        
        miniGameManager.OnMiniGameComplete += HandleMiniGameComplete;

        patternLastAttackTime = Time.time; // �߰�: ���� Ÿ�̸Ӹ� ���� �ð����� �ʱ�ȭ
    }

    public override void Attack(Transform transform, Transform target, IMonsterClass monsterData)
    {
        if (!isRunning)
        {
            isAttacking = true;
            StartPattern(transform, target, monsterData);
            return;
        }
        ExecutePattern(transform, target, monsterData);
    }
    public override bool CanAttack(float distanceToTarget, IMonsterClass monsterData)
    {
        float timeSinceLastPattern = Time.time - patternLastAttackTime;
        return timeSinceLastPattern >= patternData.patternCooldown &&
               distanceToTarget <= monsterData.CurrentAttackRange;
    }
    protected abstract void StartPattern(Transform transform, Transform target, IMonsterClass monsterData);
    public abstract void ExecutePattern(Transform transform, Transform target, IMonsterClass monsterData);

    protected virtual void CompletePattern()
    {
        //// ���� ���� �� �̺�Ʈ ���� ����
        //miniGameManager.OnMiniGameComplete -= HandleMiniGameComplete;

        isRunning = false;
        if (patternSequence != null)
        {
            if (patternSequence.IsActive())
            {
                patternSequence.Kill();
            }
            patternSequence = null;
        }
        patternLastAttackTime = Time.time;
        isAttacking = false;
    }

    protected void StartMiniGame(MiniGameType type)
    {
        float difficulty = patternManager.GetPatternDifficulty(patternData);
        miniGameManager.StartMiniGame(type, difficulty);
    }

    protected virtual void HandleMiniGameComplete(MiniGameType type, MiniGameResult result)
    {
             
            StopAttack();
            bool enterGroggy = patternManager.HandleMiniGameSuccess(result, patternData);
            miniGameManager.HandleDodgeReward(result);
           
            if (enterGroggy)
            {             
                owner.ChangeState(MonsterStateType.Groggy);
                return;
            }

            CompletePattern();
        
    }

    public override void StopAttack()
    {
        CompletePattern();
        base.StopAttack();
    }

    public void CleanAll()
    {
        if (miniGameManager != null)
        {
            miniGameManager.OnMiniGameComplete -= HandleMiniGameComplete;
        }
    }
}
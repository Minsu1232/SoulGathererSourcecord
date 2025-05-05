using System;
using UnityEditor.Rendering;
using UnityEngine;
using static IMonsterState;
/// <summary>
/// ������ ��ų ��� ���¸� �����ϴ� Ŭ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����' > 2�� '���� ������ ����' > '���� ����(FSM)'
/// 
/// �ֿ� ���:
/// - ��ų ���� �ܰ�(Phase) ������ ���� ������ ��ų ���� ����
/// - �ִϸ��̼� �̺�Ʈ�� ������ ��ų ȿ�� �ߵ� Ÿ�̹� ����
/// - Ÿ�Ӿƿ� �� �ߴ�(Interrupt) ����� ���� �������� ���� ����
/// - ���� ���� ��ų �ߴ� �Ұ� ��� ����
/// </remarks>
public class SkillState : MonsterBaseState
{
    private readonly ISkillStrategy skillStrategy;
    private Animator animator;
    private readonly float skillTimeoutDuration;
    private float skillTimer;
    private bool isSkillAnimationComplete;
    private bool hasSkillStarted;
    private bool isInterrupted;

    // ��ų ���¸� ��Ÿ���� ������
    private enum SkillPhase
    {
        NotStarted,      // ��ų ���� ��
        Starting,        // ��ų ���� �ִϸ��̼� ��� ��
        Executing,       // ��ų ȿ�� ���� �� (���� ��)
        Finishing,       // ��ų ���� �ִϸ��̼� ��� ��
        Completed,       // ��ų ���� ����
        Interrupted      // ��ų ���� �ߴ�
    }
    private SkillPhase currentPhase = SkillPhase.NotStarted;

    public SkillState(CreatureAI owner, ISkillStrategy strategy, float timeout = 5f) : base(owner)
    {
        skillStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        animator = owner.GetComponent<Animator>();
        // �ִϸ��̼� ���̿� ���� Ÿ�Ӿƿ� ����
        var animController = animator.runtimeAnimatorController;
        foreach (var clip in animController.animationClips)
        {
            if (clip.name.Contains("Skill"))
            {
                // �ִϸ��̼� ���� + �ణ�� ���� �ð����� Ÿ�Ӿƿ� ����
                skillTimeoutDuration = clip.length + 0.5f;
                Debug.Log($"��ų �ִϸ��̼� ���� ��� Ÿ�Ӿƿ� ����: {skillTimeoutDuration}��");
                break;
            }
        }

        // �ִϸ��̼� ���̸� ã�� ���ߴٸ� �⺻�� ���
        if (skillTimeoutDuration <= 0)
        {
            skillTimeoutDuration = timeout;
        }
        ResetSkillState();
    }

    private void ResetSkillState()
    {
        Debug.Log("��ų�ð��ʱ�ȭ");
        skillTimer = 0f;
        isSkillAnimationComplete = false;
        hasSkillStarted = false;
        isInterrupted = false;
        currentPhase = SkillPhase.NotStarted;
    }

    public override void Enter()
    {
        ResetSkillState();

        // �ִϸ��̼��� �̹� ��� ���̸� �����, �ƴϸ� Ʈ���� ����
        var currentAnimState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimState.IsName("SkillAttack"))
        {
            animator.Play("SkillAttack", 0, 0f);
            LogStateTransition("Enter", "Restarting SkillAttack animation");
        }
        else
        {
            animator.SetTrigger("SkillAttack");
            LogStateTransition("Enter", "Setting SkillAttack trigger");
        }

        currentPhase = SkillPhase.Starting;
    }

    public override void Execute()
    {
        if (isInterrupted) return;

        skillTimer += Time.deltaTime;

        // Ÿ�Ӿƿ� üũ
        if (skillTimer >= skillTimeoutDuration)
        {
            LogStateTransition(currentPhase.ToString(), "Timeout");
            ForceCompleteSkill();
            return;
        }

        // ��ų ���� ���� �� �÷��̾� ���� ����
        if (player != null && currentPhase == SkillPhase.Executing)
        {
            UpdateRotation();
        }

        // ��ų �Ϸ� ���� üũ:
        // �ִϸ��̼��� �Ϸ�Ǿ���, ��ų ����(���ο��� �߻� Ƚ�� ����)�� �Ϸ�Ǿ����� ����
        if (IsSkillComplete() && currentPhase != SkillPhase.Completed)
        {
            CompleteSkill();
        }
    }

    private void UpdateRotation()
    {
        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // �ּ� �Ÿ� ���� (�� ���� ���� ũ�⿡ ���� ����)
        float minDistance = 2.0f;

        // �ʹ� ������ ������ ȸ������ ����
        if (distanceToPlayer < minDistance)
            return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (directionToPlayer != Vector3.zero)
        {
            // Y�ุ ȸ���ϵ��� ����
            Vector3 flatDirection = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
            if (flatDirection.magnitude > 0.01f) // ���� ���Ͱ� �ʹ� ���� ������ Ȯ��
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    // �ִϸ��̼� �̺�Ʈ�� ���� ȣ��: ��ų �ߵ� ���� (�� ���� ȣ��)
    public void OnSkillStart()
    {
        if (isInterrupted || currentPhase != SkillPhase.Starting) return;

        try
        {
            // �� ���� �����
            if (skillStrategy == null) Debug.LogError("skillStrategy is null");
            if (transform == null) Debug.LogError("transform is null");
            if (player == null) Debug.LogError("player is null");
            if (monsterClass == null) Debug.LogError("monsterClass is null");

            // ���� �˻� �߰�
            if (player == null || monsterClass == null)
            {
                Debug.LogError("�ʼ� ������ �����ϴ�: player �Ǵ� monsterClass�� null�Դϴ�.");
                ForceCompleteSkill();
                return;
            }

            // ���⼭ ��ų �������� ���������� �߻� Ƚ���� �����ϵ��� ����
            skillStrategy.StartSkill(transform, player, monsterClass);
            hasSkillStarted = true;
            currentPhase = SkillPhase.Executing;
            LogStateTransition("OnSkillStart", "Skill started");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in OnSkillStart: {e.Message}\nStackTrace: {e.StackTrace}");
            ForceCompleteSkill();
        }
    }

    // �ִϸ��̼� �̺�Ʈ�� ���� ȣ��: ���� �߻縦 ���� �� ������ Ȥ�� ���ϴ� �����Ӹ��� ȣ��
    public void OnSkillEffect()
    {
        if (isInterrupted || currentPhase != SkillPhase.Executing)
        {
            Debug.Log("���ư���~");
            return;
        }

        try
        {
            // �� ���� �˻�
            if (player == null || monsterClass == null)
            {
                Debug.LogError("OnSkillEffect: player �Ǵ� monsterClass�� null�Դϴ�.");
                return;
            }

            // ��ų ���� ���ο��� �߻� Ƚ���� ī�����ϰ�,
            // �ִ� Ƚ�� ���� �� ���������� CompleteSkill�� ó���ϵ��� �Ѵ�.
            skillStrategy.UpdateSkill(transform, player, monsterClass);
            Debug.Log("����~");
            LogStateTransition("OnSkillEffect", "Shot fired");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in OnSkillEffect: {e.Message}\nStackTrace: {e.StackTrace}");
            ForceCompleteSkill();
        }
    }

    // �ִϸ��̼� �̺�Ʈ�� ���� ȣ��: ��ų �ִϸ��̼��� �Ϸ�Ǿ����� �˸�
    public void OnSkillAnimationComplete()
    {
        if (isInterrupted) return;

        isSkillAnimationComplete = true;
        currentPhase = SkillPhase.Finishing;

        Debug.Log("�ִϸ��̼� �Ϸ�: ��ų ���� ó�� ����");

        // ��� ��ų ������ ���� �ִϸ��̼� �Ϸ� �� ��ų ����
        if (hasSkillStarted)
        {
            // ��ų ȿ�� ���Ḧ ���� ���� �۾�
            if (skillStrategy != null)
            {
                // UpdateSkill ���� ȣ�� (ȿ�� ������ ����)
                skillStrategy.UpdateSkill(transform, player, monsterClass);
            }

            //��ų �Ϸ� ó��
            CompleteSkill();
        }

        LogStateTransition("OnSkillAnimationComplete", "Animation complete");
    }

    // ��ų �Ϸ� ����: �ִϸ��̼� �Ϸ�� ��ų �������� �߻� Ƚ�� �� �Ϸ� ���ΰ� ��� �����Ǹ� �Ϸ�
    private bool IsSkillComplete()
    {
        return isSkillAnimationComplete;
    }

    private void CompleteSkill()
    {
        // �ߺ� ȣ�� ����
        if (currentPhase == SkillPhase.Completed)
        {
            Debug.Log("�̹� �Ϸ�� ��ų�� �ٽ� �Ϸ��Ϸ��� �õ���");
            return;
        }

        Debug.Log("��ų�Ϸ��� - ����");
        currentPhase = SkillPhase.Completed;

        // CanTransition ���� Ȯ��
        

     
        owner.ChangeState(MonsterStateType.Move);

        animator.ResetTrigger("SkillAttack");
        
        LogStateTransition("CompleteSkill", "Skill completed");
    }

    private void ForceCompleteSkill()
    {        
        isSkillAnimationComplete = true;
        CompleteSkill();
        LogStateTransition("ForceCompleteSkill", "Forced completion");
    }

    public void InterruptSkill(InterruptReason reason)
    {
        if (isInterrupted) return;

        isInterrupted = true;
        currentPhase = SkillPhase.Interrupted;
        LogStateTransition("InterruptSkill", reason.ToString());

        switch (reason)
        {
            case InterruptReason.Damaged:
                var bossMonster = monsterClass as BossMonster;
                if (bossMonster != null && !bossMonster.CanBeInterrupted)
                {
                    return; // ������ ���ͷ�Ʈ �Ұ���
                }
                animator.ResetTrigger("SkillAttack");
                animator.Play("Hit");
                owner.ChangeState(MonsterStateType.Hit);
                break;

            case InterruptReason.PhaseChange:
            case InterruptReason.Death:
                ForceCompleteSkill();
                break;
        }
    }

    public override void Exit()
    {
        if (currentPhase != SkillPhase.Completed && currentPhase != SkillPhase.Interrupted)
        {
            LogStateTransition("Exit", "Forced exit");
            animator.ResetTrigger("SkillAttack");
        }
    }

    public override bool CanTransition()
    {
        return currentPhase == SkillPhase.Completed || currentPhase == SkillPhase.Interrupted;
    }

}

public enum InterruptReason
{
    Damaged,
    PhaseChange,
    Death
}
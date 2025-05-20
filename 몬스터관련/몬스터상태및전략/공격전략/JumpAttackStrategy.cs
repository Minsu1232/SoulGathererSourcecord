
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// ���� ���� ���� ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����'
/// 
/// �ֿ� ���:
/// - DOTween�� Ȱ���� ������ ���� �ִϸ��̼�
/// - ���� �� ���� ����� ���� �� ������ ����
/// </remarks>
public class JumpAttackStrategy : BasePhysicalAttackStrategy
{
    public override PhysicalAttackType AttackType => PhysicalAttackType.Jump;
    public override float GetAttackPowerMultiplier() => 1.5f; // ���� ������ 50% �� ����

    private Vector3 jumpStartPosition;
    private bool isJumping;
    private float jumpHeight = 3f;
    private float jumpDuration = 1f;
    private DG.Tweening.Sequence jumpSequence;
    private GameObject shockwaveEffect; // ����� ����Ʈ ������
    private float shockwaveRadius;
    private CreatureAI owner;
    public JumpAttackStrategy(GameObject shockwaveEffectPrefab, float shockwaveRadius, CreatureAI owner)
    {
        this.shockwaveEffect = shockwaveEffectPrefab;
        this.shockwaveRadius = shockwaveRadius;
        lastAttackTime = Time.time - 100f;  // ù ������ �ٷ� �����ϵ���
        this.owner = owner;
    }

    public override bool CanAttack(float distanceToTarget, IMonsterClass monsterData)
    {
        float timeCheck = Time.time - lastAttackTime;
        bool isInRange = distanceToTarget <= monsterData.CurrentAttackRange * 10f;
        bool isCooldownReady = Time.time >= lastAttackTime + monsterData.CurrentAttackSpeed;

   

        return isInRange && isCooldownReady;
    }


    private void CreateShockwave(Vector3 position, IMonsterClass monsterData)
    {
        if (shockwaveEffect != null)
        {
            GameObject effect = GameObject.Instantiate(shockwaveEffect, position, Quaternion.identity);

            // �ֺ� �÷��̾� ã�Ƽ� ������ ����
            
            Collider[] hitColliders = Physics.OverlapSphere(position, shockwaveRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    
                    IDamageable player = GameInitializer.Instance.GetPlayerClass();

                        ApplyDamage(player, monsterData);
                        break;
                    
                }
            }

            GameObject.Destroy(effect, 2f);
        }
    }

    public override void Attack(Transform transform, Transform target, IMonsterClass monsterData)
    {
       
        Debug.Log("Executing Jump Attack Strategy...");
        // ���� �غ�
        StartAttack();
        FaceTarget(transform, target);       
        jumpStartPosition = transform.position;

        // ���� ���� ���
        Vector3 startPos = transform.position;
        Vector3 endPos = target.position;  // ���� ������ Ÿ���� ���� ��ġ�� ���� ���� �ְ�, Y��ǥ�� startPos.y�� ���� �� ��Ȳ�� ���� ����


        // ������ ����
        jumpSequence = DOTween.Sequence().SetId("JumpAttack");
        DOTween.logBehaviour = LogBehaviour.Verbose;

        // 0 ~ 1���� t�� �ø��鼭 X, Z�� ���� �̵�, Y�� ���������� ���
        jumpSequence.Append(
            DOTween.To(
                () => 0f,   // ���۰�
                t =>
                {
                    // t: 0 ~ 1
                    float newX = Mathf.Lerp(startPos.x, endPos.x, t);
                    float newZ = Mathf.Lerp(startPos.z, endPos.z, t);

                    // Y�� �⺻ ���� ���� + jumpHeight�� Ȱ���� ������ ����
                    float baseY = Mathf.Lerp(startPos.y, endPos.y, t);

                    // ������ ������ ��: 4t(1 - t) = �ִ�ġ�� t=0.5�� �� �߻�
                    float parabola = 4f * t * (1f - t) * jumpHeight;

                    // ���� ��ǥ ����
                    transform.position = new Vector3(newX, baseY + parabola, newZ);
                },
                1f,           // ��ǥġ
                jumpDuration  // �ɸ��� �ð�
            )
            .SetEase(Ease.Linear)
        );

        // ���� �� �����
        jumpSequence.AppendCallback(() =>
        {
            CreateShockwave(transform.position, monsterData);
        });

        // �Ϸ� ����
        jumpSequence.OnComplete(() =>
        {
            Debug.Log("JumpAttack OnComplete - Before StopAttack");
            OnAttackAnimationEnd();  // �ִϸ��̼� ���� ó�� �߰�
            StopAttack();            

            var strategy = owner.GetAttackStrategy();
            if (strategy is BossMultiAttackStrategy physicalStrategy)
            {
                physicalStrategy.StopAttack();
                physicalStrategy.UpdateLastAttackTime();
                Debug.Log("�Ĵٺ�������" + physicalStrategy.IsAttacking);
            }
            UpdateLastAttackTime();
            Debug.Log("JumpAttack OnComplete - After StopAttack - IsAttacking: " + IsAttacking);
            jumpSequence.Kill();
        });

        jumpSequence.Play();
    }

    public override void StopAttack()
    {
        Debug.Log("JumpAttackStrategy: Before StopAttack - IsAttacking: " + IsAttacking);
        base.StopAttack();
        Debug.Log("JumpAttackStrategy: After StopAttack - IsAttacking: " + IsAttacking);

    }
}
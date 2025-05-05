using System;
using UnityEngine;
using static IMonsterState;
/// <summary>
/// ���� ���� ���� ����
/// </summary>
/// <remarks>
/// ������� ����: 2�� '���� ���� ����'
/// 
/// �ֿ� ���:
/// - �غ� �ܰ迡�� ���� �ε������� ǥ�� �� ����Ʈ ����
/// - ���� �ܰ迡�� ��� �̵� �� �浹 ó��
/// - ��/�÷��̾� �浹 �� ����Ʈ �� ������ ó��
/// - ���� ��ȭ�� ���� �̺�Ʈ �߻�
/// </remarks>
public class ChargeAttackStrategy : BasePhysicalAttackStrategy
{
    public override PhysicalAttackType AttackType => PhysicalAttackType.Charge;
    public event System.Action OnChargeStateChanged;

    private GameObject prepareDustEffectPrefab;
    private GameObject startEffectPrefab;
    private GameObject trailEffectPrefab;
    private GameObject wallImpactEffectPrefab;
    private GameObject playerImpactEffectPrefab;

    private GameObject prepareDustEffect;
    private GameObject trailEffect;

    private float targetChargeDistance; // ��ǥ �̵� �Ÿ�
    private float currentChargeDistance; // ������� �̵��� �Ÿ�
    private enum ChargeState
    {
        None,
        Preparing,
        Charging,
        End
    }

    private ChargeState currentChargeState = ChargeState.None;
    private Vector3 chargeDirection;
    private float chargeSpeed;
    private float chargeDuration;
    private float currentChargeTime;
    private float prepareTime;
    private float currentPrepareTime = 0f;
    private GameObject chargeIndicator;
    private GameObject indicatorPrefab;
    private CreatureAI owner;
    private Animator animator;

    public ChargeAttackStrategy(CreatureAI owner, ICreatureData data, Animator animator)
    {
        this.owner = owner;
        chargeSpeed = data.chargeSpeed;
        chargeDuration = data.chargeDuration;
        prepareTime = data.prepareTime;
        indicatorPrefab = data.chargeIndicatorPrefab;
        this.animator = animator;
        Debug.Log("@#$@#$@#$####" + indicatorPrefab);
        // ����Ʈ ������ ����
        prepareDustEffectPrefab = data.ChargePrepareDustEffect;
        startEffectPrefab = data.ChargeStartEffect;
        trailEffectPrefab = data.ChargeTrailEffect;
        wallImpactEffectPrefab = data.WallImpactEffect;
        playerImpactEffectPrefab = data.PlayerImpactEffect;
    }

    public override void Attack(Transform transform, Transform target, IMonsterClass monsterData)
    {
        if (!CanAttack(Vector3.Distance(transform.position, target.position), monsterData))
            return;

        StartAttack();
        FaceTarget(transform, target);

        Vector3 directionXZ = target.position - transform.position;
        directionXZ.y = 0;
        chargeDirection = directionXZ.normalized;

        currentChargeState = ChargeState.Preparing;
        currentPrepareTime = 0f;

        // �ε������� ����
        if (chargeIndicator == null && indicatorPrefab != null)
        {
            chargeIndicator = GameObject.Instantiate(indicatorPrefab, transform.position, Quaternion.identity);
            var renderer = chargeIndicator.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material.SetFloat("_FillAmount", 0f);
            }
        }

        // �غ� �ܰ� ���� ����Ʈ ����
        if (prepareDustEffectPrefab != null)
        {
            prepareDustEffect = GameObject.Instantiate(
                prepareDustEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            prepareDustEffect.transform.parent = transform;

         
        }
    }

    public void UpdateCharge(Transform transform)
    {
        switch (currentChargeState)
        {
            case ChargeState.Preparing:
                HandlePreparingState(transform);
                break;
            case ChargeState.Charging:
                HandleChargingState(transform);
                break;
            case ChargeState.End:
                HandleChargingStateEnd();
                break;
        }
    }

    private void HandleChargingStateEnd()
    {
        // �ʿ��� ��� ���� ���� �߰�
    }

    private void HandlePreparingState(Transform transform)
    {
        currentPrepareTime += Time.deltaTime;

        if (chargeIndicator != null)
        {
            Transform playerTransform = GameInitializer.Instance.GetPlayerClass().playerTransform;

            Vector3 directionXZ = playerTransform.position - transform.position;
            directionXZ.y = 0;
            chargeDirection = directionXZ.normalized;

            // ������ �÷��̾ �ٶ󺸵��� ȸ��
            transform.rotation = Quaternion.Lerp(transform.rotation,
                                               Quaternion.LookRotation(chargeDirection),
                                               Time.deltaTime * 5f);

            float distanceToPlayer = directionXZ.magnitude;

            Vector3 indicatorPosition = transform.position;
            indicatorPosition.y += 0.5f;

            // �ǹ��� ���������� �̵�
            indicatorPosition += chargeDirection * (distanceToPlayer * 0.5f);

            chargeIndicator.transform.position = indicatorPosition;

            float angle = Mathf.Atan2(chargeDirection.x, chargeDirection.z) * Mathf.Rad2Deg;
            chargeIndicator.transform.rotation = Quaternion.Euler(90f, angle, 0f);

            // �� ���� �����Ϸ� ���� (�ʺ� 2���)
            chargeIndicator.transform.localScale = new Vector3(2f, distanceToPlayer, 1f);

            var renderer = chargeIndicator.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                // �⺻ ����
                renderer.material.SetFloat("_FillAmount", currentPrepareTime / prepareTime);
            }
        }

        if (currentPrepareTime >= prepareTime)
        {
            // ��¡ ���� ����Ʈ
            if (startEffectPrefab != null)
            {
                GameObject startEffect = GameObject.Instantiate(
                    startEffectPrefab,
                    transform.position,
                    Quaternion.LookRotation(chargeDirection)
                );

                // ��ƼŬ �ý��� ���� ����
                ParticleSystem ps = startEffect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    float duration = ps.main.duration;
                    if (ps.main.loop)
                    {
                        duration = 2f; // ���� ����� ��� �⺻ 2��
                    }
                    else if (duration == 0)
                    {
                        duration = ps.main.startLifetimeMultiplier;
                    }

                    GameObject.Destroy(startEffect, duration + 0.5f); // ���� �ð� �߰�
                }
                else
                {
                    GameObject.Destroy(startEffect, 2f);
                }
            }

            // Ʈ���� ����Ʈ ����
            if (trailEffectPrefab != null && trailEffect == null)
            {
                trailEffect = GameObject.Instantiate(
                    trailEffectPrefab,
                    transform.position,
                    Quaternion.identity
                );
                trailEffect.transform.parent = transform;
            }

            // �غ� ����Ʈ ����
            if (prepareDustEffect != null)
            {
                GameObject.Destroy(prepareDustEffect);
                prepareDustEffect = null;
            }

            // �÷��̾� ������ ���������� �� �� �� ������Ʈ
            Transform playerTransform = GameInitializer.Instance.GetPlayerClass().playerTransform;
            Vector3 directionXZ = playerTransform.position - transform.position;
            directionXZ.y = 0;
            chargeDirection = directionXZ.normalized;

            // ������ �÷��̾ ��Ȯ�� �ٶ󺸵��� ȸ��
            transform.rotation = Quaternion.LookRotation(chargeDirection);
            currentChargeState = ChargeState.Charging;
            Debug.Log("��¡");
            currentChargeTime = 0f;
            float distanceToPlayer = directionXZ.magnitude;
            targetChargeDistance = distanceToPlayer + 2f; // ��: �÷��̾� �Ÿ� + 2����
            currentChargeDistance = 0f; // �̵� �Ÿ� �ʱ�ȭ
            OnChargeStateChanged?.Invoke();  // ���� ���·� ���� �� �̺�Ʈ �߻�

            if (chargeIndicator != null)
            {
                GameObject.Destroy(chargeIndicator);
                chargeIndicator = null;
            }
        }
    }

    private void HandleChargingState(Transform transform)
    {
        currentChargeTime += Time.deltaTime;

        // ��ǥ �Ÿ� ���� üũ �߰�
        if (currentChargeDistance >= targetChargeDistance)
        {
            StopAttack();
            return;
        }

        // �� üũ
        if (Physics.Raycast(transform.position, chargeDirection, out RaycastHit hit, 1f))
        {
            if (hit.collider.CompareTag("CrashWall"))
            {
                Debug.Log("��");
                // �� �浹 ����Ʈ ����
                if (wallImpactEffectPrefab != null)
                {
                    GameObject wallEffect = GameObject.Instantiate(
                        wallImpactEffectPrefab,
                        hit.point,
                        Quaternion.LookRotation(hit.normal)
                    );

                    // ��ƼŬ �ý��� ���� ����
                    ParticleSystem ps = wallEffect.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        float duration = ps.main.duration;
                        if (ps.main.loop)
                        {
                            duration = 2f; // ���� ����� ��� �⺻ 2��
                        }
                        else if (duration == 0)
                        {
                            duration = ps.main.startLifetimeMultiplier;
                        }

                        GameObject.Destroy(wallEffect, duration + 0.5f);
                    }
                    else
                    {
                        GameObject.Destroy(wallEffect, 2f);
                    }
                }

                // ������ �ణ �ڷ� �б�
                transform.position -= chargeDirection * 3f;
                CameraShakeManager.TriggerShake(1f, 0.2f);
                StopAttack();
                owner.ChangeState(MonsterStateType.Groggy);
                
                return;
            }
        }

        // �÷��̾� �浹 üũ
        Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
        foreach (var hitCollider in hits)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // �÷��̾� �浹 ����Ʈ ����
                if (playerImpactEffectPrefab != null)
                {
                    GameObject playerEffect = GameObject.Instantiate(
                        playerImpactEffectPrefab,
                        hitCollider.transform.position,
                        Quaternion.LookRotation(-chargeDirection)
                    );

                    // ��ƼŬ �ý��� ���� ����
                    ParticleSystem ps = playerEffect.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        float duration = ps.main.duration;
                        if (ps.main.loop)
                        {
                            duration = 2f; // ���� ����� ��� �⺻ 2��
                        }
                        else if (duration == 0)
                        {
                            duration = ps.main.startLifetimeMultiplier;
                        }

                        GameObject.Destroy(playerEffect, duration + 0.5f);
                    }
                    else
                    {
                        GameObject.Destroy(playerEffect, 2f);
                    }
                }

                IDamageable player = GameInitializer.Instance.GetPlayerClass();
                ApplyDamage(player, owner.GetStatus().GetMonsterClass());
                StopAttack();
                return;
            }
        }
    
        // ���� �̵�
        float moveDistance = chargeSpeed * Time.deltaTime;
        transform.position += chargeDirection * moveDistance;
        currentChargeDistance += moveDistance; // �̵� �Ÿ� ����
    }

    public override string GetAnimationTriggerName()
    {
        return currentChargeState == ChargeState.Preparing ? "ChargePrepare" : "Attack_Charge";
    }

    public override void StopAttack()
    {
        base.StopAttack();
        animator.SetTrigger("ChargingComplete");
        currentChargeTime = 0f;
        currentPrepareTime = 0f;
        currentChargeState = ChargeState.None;

        // ��� ����Ʈ ����
        if (chargeIndicator != null)
        {
            GameObject.Destroy(chargeIndicator);
            chargeIndicator = null;
        }

        if (prepareDustEffect != null)
        {
            GameObject.Destroy(prepareDustEffect);
            prepareDustEffect = null;
        }

        if (trailEffect != null)
        {
            GameObject.Destroy(trailEffect);
            trailEffect = null;
        }
    }
}
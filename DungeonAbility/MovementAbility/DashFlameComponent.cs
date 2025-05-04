// DashFlameComponent.cs - ��� ��ο� ���� �Ҳ� ���� ������Ʈ (�ϼ���)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DashFlameComponent : MonoBehaviour
{
    private float damageMultiplier = 0f;    // �÷��̾� ���ݷ� ��� ������ ����
    private PlayerClass playerClass;
    private PlayerDashComponent dashComponent;
    private Rarity flameRarity = Rarity.Common; // �⺻ ��͵�

    [SerializeField] private float flameDuration = 3f;      // �Ҳ� ���� �ð�
    [SerializeField] private float flameRadius = 2f;        // �Ҳ� ȿ�� �ݰ�
    [SerializeField] private float flameHeight = 0.1f;     // �Ҳ��� �ٴڿ��� �󸶳� �� ������

    private Vector3 dashStartPosition;      // ��� ���� ��ġ
    private List<GameObject> activeFlames = new List<GameObject>();
    // �Ҳ� ����Ʈ ������
    private GameObject flameEffectPrefab;

    private void Start()
    {
        playerClass = GameInitializer.Instance.GetPlayerClass();
        dashComponent = GetComponent<PlayerDashComponent>();

        // �ʿ��� ���� üũ
        if (playerClass == null || dashComponent == null)
        {
            Debug.LogError("DashFlameComponent: �ʿ��� ������Ʈ�� �����ϴ�.");
            enabled = false;
            return;
        }

        // �Ҳ� ������ ����
        if (GameInitializer.Instance.flameWall != null)
        {
            flameEffectPrefab = GameInitializer.Instance.flameWall;
        }

        // �̺�Ʈ ����
        dashComponent.OnDashStart.AddListener(OnDashStart);
        dashComponent.OnDashEnd.AddListener(OnDashEnd);
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        if (dashComponent != null)
        {
            dashComponent.OnDashStart.RemoveListener(OnDashStart);
            dashComponent.OnDashEnd.RemoveListener(OnDashEnd);
        }

        // Ȱ��ȭ�� �Ҳ� ����
        foreach (var flame in activeFlames)
        {
            if (flame != null)
            {
                Destroy(flame);
            }
        }
        activeFlames.Clear();
    }

    // ��� ���� �� ��ġ ����
    private void OnDashStart()
    {
        dashStartPosition = transform.position;
    }

    // ��� ���� �� �Ҳ� ����
    private void OnDashEnd()
    {
        if (damageMultiplier > 0f && flameEffectPrefab != null)
        {
            CreateFlame();
        }
    }

    // ��� ��ο� �Ҳ� ����
    private void CreateFlame()
    {
        Vector3 dashEndPosition = transform.position;
        Vector3 dashVector = dashEndPosition - dashStartPosition;

        // y�� ���� ����
        dashVector.y = 0;

        // �ʹ� ª�� ��ô� ����
        float dashDistance = dashVector.magnitude;
        if (dashDistance < 0.5f)
            return;

        // ��� ���� ��� (XZ ��鿡��)
        Vector3 dashDirection = dashVector.normalized;

        // �Ҳ� ���� ��ġ (��� ����� �߰�)
        Vector3 flamePosition = dashStartPosition + (dashVector * 0.5f);
        flamePosition.y += flameHeight; // �ٴڿ��� �ణ ���� �ø�

        // Quaternion ȸ�� - ���� ���⿡ �°� 90�� ȸ�� ����
        Quaternion rotation = Quaternion.LookRotation(dashDirection) * Quaternion.Euler(0, 90, 0);

        GameObject flameObj = Instantiate(flameEffectPrefab, flamePosition, rotation);

        // �⺻ ������ �������� (�������� ���� ũ��)
        Vector3 originalScale = flameEffectPrefab.transform.localScale;

        // ������ ��� (��� �Ÿ��� 50%�� ����)
        float scaleFactor = dashDistance * 0.5f / originalScale.x;  // X���� ���� �����̶�� ����

        // �� ������ ���� - X������ �����ϸ�
        Vector3 newScale = new Vector3(
            scaleFactor * originalScale.x,
            originalScale.y,
           scaleFactor* originalScale.z
        );

        // ������ ����
        flameObj.transform.localScale = newScale;

        // ��͵��� ���� ���� ����
        SetFlameColorByRarity(flameObj, flameRarity);

        // ������ ���
        float damage = playerClass.PlayerStats.AttackPower * damageMultiplier;

        // �Ҳ� ������ ���� ������Ʈ �߰�
        FlameAreaEffect flameEffect = flameObj.AddComponent<FlameAreaEffect>();
        flameEffect.Initialize(damage, flameDuration, dashDistance / 2.0f);

        // Ȱ��ȭ�� �Ҳ� ��Ͽ� �߰�
        activeFlames.Add(flameObj);

        // ���� �ð� �� ����
        StartCoroutine(RemoveFlameAfterDuration(flameObj, flameDuration));

        Debug.Log($"��� �Ҳ� ����: ���� {dashDirection}, �Ÿ� {dashDistance}m, ������ {damage}, ��͵�: {flameRarity}");
    }

    // ���� �ð� �� �Ҳ� ����
    private IEnumerator RemoveFlameAfterDuration(GameObject flame, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (activeFlames.Contains(flame))
        {
            activeFlames.Remove(flame);
        }
        Destroy(flame);
    }

    // ��͵��� ���� ���� ���� (�µ� ���)
    private void SetFlameColorByRarity(GameObject flameObj, Rarity rarity)
    {
        // �µ� ��� ���� (���� ��͵� = ���� �µ�, ���� ��͵� = ���� �µ�)
        Color flameColor;
        Debug.Log($"���� �� ��͵� {rarity}");
        switch (rarity)
        {
            case Rarity.Common:
                flameColor = new Color(1f, 0.3f, 0f, 0.7f); // ���� ��Ȳ�� (���� �µ�)
                break;
            case Rarity.Uncommon:
                flameColor = new Color(1f, 0.7f, 0f, 0.7f); // ��Ȳ��
                break;
            case Rarity.Rare:
                flameColor = new Color(1f, 0.9f, 0.2f, 0.7f); // �����
                break;
            case Rarity.Epic:
                flameColor = new Color(0.9f, 0.9f, 0.9f, 0.7f); // ���
                break;
            case Rarity.Legendary:
                flameColor = new Color(0.3f, 0.5f, 1f, 0.7f); // �Ķ��� (���� ���� �µ�)
                break;
            default:
                flameColor = new Color(1f, 0.5f, 0f, 0.7f); // �⺻ ��Ȳ��
                break;
        }

        // ��ƼŬ �ý��� ã��
        ParticleSystem[] particleSystems = flameObj.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems)
        {
            var main = ps.main;
            main.startColor = flameColor;
        }
    }

    // ������ ���� ���� (���ݷ� ��� ����)
    public void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = Mathf.Max(0f, multiplier);
        Debug.Log($"��� �Ҳ� ������ ����: ���ݷ��� {damageMultiplier * 100}%");
    }

    // ���� ������ ���� ��ȯ
    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }

    // �Ҳ� ���� �ð� ����
    public void SetFlameDuration(float duration)
    {
        flameDuration = Mathf.Max(1f, duration);
    }

    // �Ҳ� �ݰ� ����
    public void SetFlameRadius(float radius)
    {
        flameRadius = Mathf.Max(1f, radius);
    }

    // ��͵� ���� �޼���
    public void SetFlameRarity(Rarity rarity)
    {
        flameRarity = rarity;
        Debug.Log($"��� �Ҳ� ��͵� ����: {rarity}");
    }
}
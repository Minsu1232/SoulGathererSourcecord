// DashInvincibleComponent.cs - ��� �� ���� ��� ������Ʈ
using UnityEngine;
using DG.Tweening;  // DOTween ����� ���� �߰�

public class DashInvincibleComponent : MonoBehaviour
{
    private float invincibleDuration = 0f;    // ��� �� ���� ���� �ð�
    private PlayerClass playerClass;
    private PlayerDashComponent dashComponent;
    private bool isInvincible = false;

    // ���� ȿ�� �ð�ȭ�� ���� ����
    [SerializeField] private Color invincibleColor = new Color(1f, 1f, 1f, 0.5f);
    private Renderer[] renderers;
    private Color[] originalColors;

    private void Awake()
    {
        playerClass = GameInitializer.Instance.GetPlayerClass();
        dashComponent = GetComponent<PlayerDashComponent>();

        if (playerClass == null || dashComponent == null)
        {
            Debug.LogError("DashInvincibleComponent: �ʿ��� ������Ʈ�� �����ϴ�.");
            enabled = false;
            return;
        }

        // ������ ĳ��
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = renderers[i].material.color;
            }
        }

        // ��� �̺�Ʈ ����
        dashComponent.OnDashStart.AddListener(OnDashStart);
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        if (dashComponent != null)
        {
            dashComponent.OnDashStart.RemoveListener(OnDashStart);
        }

        // ���� ���� ���� (������ ����)
        if (isInvincible)
        {
            EndInvincibility();
        }
    }

    // ��� ���� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯
    private void OnDashStart()
    {
        if (invincibleDuration > 0f)
        {
            StartInvincibility();
        }
    }

    // ���� ���� ����
    private void StartInvincibility()
    {
        if (!isInvincible)
        {
            isInvincible = true;
            playerClass.isInvicible = true;

            // �ð��� �ǵ�� (DOTween ���)
            ApplyInvincibilityVisuals(true);

            // ���� ���� �ð� �� ���� ���·� ����
            DOVirtual.DelayedCall(invincibleDuration, EndInvincibility);

            Debug.Log($"��� ���� ����: {invincibleDuration}��");
        }
    }

    // ���� ���� ����
    private void EndInvincibility()
    {
        if (isInvincible)
        {
            isInvincible = false;
            playerClass.isInvicible = false;

            // �ð��� �ǵ�� ����
            ApplyInvincibilityVisuals(false);

            Debug.Log("��� ���� ����");
        }
    }

    // ���� �� �ð��� �ǵ�� ����/����
    private void ApplyInvincibilityVisuals(bool active)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;

            if (renderers[i].material.HasProperty("_Color"))
            {
                if (active)
                {
                    // ���� ����: ���� ���� (DOTween ���)
                    renderers[i].material.DOColor(invincibleColor, 0.2f);
                }
                else
                {
                    // ���� ����: ���� �������� ����
                    renderers[i].material.DOColor(originalColors[i], 0.2f);
                }
            }
        }
    }

    // ���� ���ӽð� ����
    public void SetInvincibleDuration(float duration)
    {
        invincibleDuration = Mathf.Max(0f, duration);
        Debug.Log($"��� ���� ���ӽð� ����: {invincibleDuration}��");
    }

    // ���� ���� ���ӽð� ��ȯ
    public float GetInvincibleDuration()
    {
        return invincibleDuration;
    }

    // ���� ���� ���� ��ȯ
    public bool IsInvincible()
    {
        return isInvincible;
    }
}
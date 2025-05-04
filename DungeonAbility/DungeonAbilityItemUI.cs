using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class DungeonAbilityItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI ���")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image abilityIconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelAndRarityText; // ������ ��͵��� �Բ� ǥ��

    [Header("�ִϸ��̼� ����")]
    [SerializeField] private float hoverScaleAmount = 1.05f;
    [SerializeField] private float hoverScaleDuration = 0.2f;
    [SerializeField] private float pulseDuration = 1.5f;
    [SerializeField] private float pulseScaleAmount = 1.02f;

    [Header("ȿ����")]
    [SerializeField] private AudioClip hoverSound;

    private DungeonAbility ability;
    private AudioSource audioSource;
    private Vector3 originalScale;
    private Tween currentHoverTween;
    private Tween pulseTween;

    private void Awake()
    {
        // ����� �ҽ� ������Ʈ �������ų� ����
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        // �޽� �ִϸ��̼� ���� (��͵��� ���� �����۸�)
        StartPulseEffect();
    }

    private void OnDisable()
    {
        // ��� �ִϸ��̼� ����
        if (currentHoverTween != null && currentHoverTween.IsActive())
            currentHoverTween.Kill();

        if (pulseTween != null && pulseTween.IsActive())
            pulseTween.Kill();
    }

    // �ɷ� ���� ����
    public void SetupAbility(DungeonAbility ability, Color rarityColor)
    {
        this.ability = ability;

        // �⺻ ���� ����
        nameText.text = ability.name;
        descriptionText.text = ability.description;

        // ������ ��͵� �Բ� ǥ��
        levelAndRarityText.text = $"Lv.{ability.level} ({ability.rarity})";
        levelAndRarityText.color = rarityColor;

        // �ؽ�Ʈ �ƿ����� ����
        levelAndRarityText.outlineWidth = 0.2f;
        levelAndRarityText.outlineColor = new Color(0.1f, 0.1f, 0.1f, 1f); // ������ �ƿ�����

        // ��� ���� ���� (�� ���� ��͵� ����)
        if (backgroundImage != null)
        {
            Color bgColor = rarityColor;
            bgColor.a = 0.8f; // �� �����ϰ� ����
            backgroundImage.color = bgColor;
        }

        // ������ ���� (�ִ� ��쿡��)
        if (ability.icon != null && abilityIconImage != null)
        {
            abilityIconImage.sprite = ability.icon;
            abilityIconImage.enabled = true;
        }
        else if (abilityIconImage != null)
        {
            // �������� ���� ��� �⺻ ������ �Ǵ� ��Ȱ��ȭ
            abilityIconImage.enabled = false;
        }

        // ��͵��� �������� Ư���� ȿ�� ����
        ApplyRarityEffects(ability.rarity);

        Debug.Log($"�����Ƽ UI ����: {ability.name}, ����: {ability.level}, ��͵�: {ability.rarity}");
    }

    // ��͵��� ���� Ư�� ȿ�� ����
    private void ApplyRarityEffects(Rarity rarity)
    {
        // ��͵��� �������� �� ���� ȿ�� ����
        switch (rarity)
        {
            case Rarity.Epic:
            case Rarity.Legendary:
                // �޽� ȿ�� Ȱ��ȭ
                StartPulseEffect();
                break;
        }
    }

    // �޽� �ִϸ��̼� ����
    private void StartPulseEffect()
    {
        // �ɷ��� ���� ��� �������� ����
        if (ability == null)
            return;

        // ���� �̻� ��͵��� �޽� ȿ�� ����
        if (ability.rarity == Rarity.Epic || ability.rarity == Rarity.Legendary)
        {
            if (pulseTween != null && pulseTween.IsActive())
                pulseTween.Kill();

            // �޽� �ִϸ��̼� ����
            pulseTween = transform.DOScale(originalScale * pulseScaleAmount, pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    // ���콺 ȣ�� �� ȣ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ȣ�� ����
        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound);

        // ȣ�� ȿ�� (ũ�� Ȯ��)
        if (currentHoverTween != null && currentHoverTween.IsActive())
            currentHoverTween.Kill();

        currentHoverTween = transform.DOScale(originalScale * hoverScaleAmount, hoverScaleDuration)
            .SetEase(Ease.OutBack);

        // ��� ����
        if (backgroundImage != null)
        {
            Color currentColor = backgroundImage.color;
            Color brighterColor = currentColor;
            brighterColor.a = 0.4f; // �� �����ϰ�
            backgroundImage.DOColor(brighterColor, hoverScaleDuration);
        }
    }

    // ���콺 ȣ�� ���� �� ȣ��
    public void OnPointerExit(PointerEventData eventData)
    {
        // ���� ũ��� ����
        if (currentHoverTween != null && currentHoverTween.IsActive())
            currentHoverTween.Kill();

        currentHoverTween = transform.DOScale(originalScale, hoverScaleDuration)
            .SetEase(Ease.OutQuad);

        // ��� �������
        if (backgroundImage != null && ability != null)
        {
            // ���� �������� ���� (���İ� ����)
            Color currentColor = backgroundImage.color;
            currentColor.a = 0.2f;
            backgroundImage.DOColor(currentColor, hoverScaleDuration);
        }

        // �ʿ��ϸ� �޽� ȿ�� �ٽ� ����
        StartPulseEffect();
    }
}
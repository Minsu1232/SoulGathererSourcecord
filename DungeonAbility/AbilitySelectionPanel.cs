// AbilitySelectionPanel.cs - �ɷ� ���� �г� ������
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AbilitySelectionPanel : MonoBehaviour
{
    [Header("�г� ���� ���")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject abilityCardPrefab;

    [Header("�ִϸ��̼� ����")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float cardAppearDelay = 0.15f;
    [SerializeField] private float cardAppearDuration = 0.4f;
    [SerializeField] private float panelScaleDuration = 0.4f;

    [Header("���� ȿ��")]
    [SerializeField] private AudioClip panelOpenSound;
    [SerializeField] private AudioClip confirmSound;

    private AudioSource audioSource;
    private List<GameObject> activeCards = new List<GameObject>();
    private System.Action onSelectionCompleted;

    private void Awake()
    {
        // ����� �ҽ� ������Ʈ �������ų� ����
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // �ʱ� ���� ����
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        // �г� �ʱ� ����
        gameObject.SetActive(false);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    // �ɷ� ���� �г� ǥ��
    public void ShowSelectionPanel(List<DungeonAbility> abilities, System.Action onCompleted = null)
    {
        Debug.Log($"ShowSelectionPanel ȣ���: �ɷ� �� = {abilities.Count}");

        onSelectionCompleted = onCompleted;

        // ���� ī�� ����
        ClearCards();

        // �г� Ȱ��ȭ
        gameObject.SetActive(true);

        // ���������� ��� �ڽ� ������Ʈ Ȱ��ȭ
        SetAllChildrenActive(transform, true);

        Debug.Log("�г� �� �ڽ� ���ӿ�����Ʈ Ȱ��ȭ �Ϸ�");

        // ������ ����
        if (panelOpenSound != null && audioSource != null)
            audioSource.PlayOneShot(panelOpenSound);

        // �г� �ִϸ��̼�
        StartPanelAnimation();

        // ī�� �����̳� Ȯ��
        if (cardContainer == null)
        {
            Debug.LogError("ī�� �����̳ʰ� null�Դϴ�!");
            cardContainer = transform;
        }

        // ī�� ���� Ȯ��
        Debug.Log($"ī�� ���� ����: container = {(cardContainer != null ? "����" : "����")}, prefab = {(abilityCardPrefab != null ? "����" : "����")}");

        // ī�� ���� �� �ִϸ��̼�
        StartCoroutine(CreateCardsWithAnimation(abilities));
    }

    // �г� ������ �ִϸ��̼�
    private void StartPanelAnimation()
    {
       
        canvasGroup.gameObject.SetActive(true);
        
        // ĵ���� �׷� ���̵� ��
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            canvasGroup.DOFade(1f, fadeInDuration)
                .SetEase(Ease.OutQuad);
        }

        // �г� ������ �ִϸ��̼�
        if (panelRect != null)
        {
            panelRect.localScale = Vector3.one * 0.9f;
            panelRect.DOScale(Vector3.one, panelScaleDuration)
                .SetEase(Ease.OutBack);
        }
    }

    // ī�� ���� �� �ִϸ��̼�
    private System.Collections.IEnumerator CreateCardsWithAnimation(List<DungeonAbility> abilities)
    { // ù ��° ī�� �ε� ���� �ణ�� ���� �߰� (��� ������ �ε� �Ϸ� ���)
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < abilities.Count; i++)
        {
            // ī�� ����
            GameObject cardObj = Instantiate(abilityCardPrefab, cardContainer);
            AbilityCardUI cardUI = cardObj.GetComponent<AbilityCardUI>();

            // �ʱ� ���� (������ �ʰ�)
            cardObj.transform.localScale = Vector3.zero;

            // ī�� ����
            if (cardUI != null)
            {
                cardUI.Setup(abilities[i], OnAbilitySelected);
                activeCards.Add(cardObj);
            }

            // ī�� ���� �ִϸ��̼�
            cardObj.transform.DOScale(abilityCardPrefab.gameObject.transform.localScale, cardAppearDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(i * cardAppearDelay);

            // ���� ī�� ���� �� ��� ���
            yield return new WaitForSeconds(cardAppearDelay);
        }
    }

    // �ɷ� ���� ó��
    private void OnAbilitySelected(DungeonAbility ability)
    {
        // ���� ȿ����
        if (confirmSound != null && audioSource != null)
            audioSource.PlayOneShot(confirmSound);

        // �ɷ� ����
        DungeonAbilityManager.Instance.AcquireAbility(ability);
        Debug.Log("�����Ƽ ���õ�: " + ability.name);
        // �г� �ݱ� �ִϸ��̼�
        ClosePanelWithAnimation();
    }

    // �г� �ݱ� �ִϸ��̼�
    // �г� �ݱ� �ִϸ��̼� (����)
    private void ClosePanelWithAnimation()
    {
        // ���ͷ��� ���� (gameObject ��Ȱ��ȭ ����)
        if (canvasGroup != null)
        {
            // canvasGroup.gameObject.SetActive(false); // �� �� ����
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        // ���õ��� ���� ī�� ���̵� �ƿ�
        foreach (var card in activeCards)
        {
            card.transform.DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBack);
        }

        // �г� ���̵� �ƿ�
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, fadeInDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => {
                    // �г� ��Ȱ��ȭ
                    gameObject.SetActive(false);

                    // �Ϸ� �ݹ� ȣ��
                    onSelectionCompleted?.Invoke();
                });
        }
    }
    // ��� �ڽ� ������Ʈ Ȱ��ȭ/��Ȱ��ȭ�ϴ� ����� �޼��� (���� �߰�)
    private void SetAllChildrenActive(Transform parent, bool active)
    {
        // ���� Transform�� ���� ������Ʈ Ȱ��ȭ
        parent.gameObject.SetActive(active);

        // ��� �ڽ� ������Ʈ�� ���� ��������� ȣ��
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            child.gameObject.SetActive(active);

            // �ڽ��� �� ������ ��������� ó��
            if (child.childCount > 0)
            {
                SetAllChildrenActive(child, active);
            }
        }
    }
    // ī�� ����
    private void ClearCards()
    {
        foreach (var card in activeCards)
        {
            Destroy(card);
        }
        activeCards.Clear();
    }

    // Ÿ��Ʋ ����
    public void SetTitle(string title)
    {
        if (titleText != null)
            titleText.text = title;
    }
}
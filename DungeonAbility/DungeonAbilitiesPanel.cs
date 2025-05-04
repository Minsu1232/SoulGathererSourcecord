using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DungeonAbilitiesPanel : MonoBehaviour
{
    [Header("�г� ����")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Button toggleButton;
    [SerializeField] private TextMeshProUGUI toggleButtonText;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Transform abilitiesContainer;
    [SerializeField] private GameObject abilityItemPrefab;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    [Header("��͵� ����")]
    [SerializeField] private Color commonColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color uncommonColor = new Color(0.3f, 0.8f, 0.3f);
    [SerializeField] private Color rareColor = new Color(0.3f, 0.3f, 0.8f);
    [SerializeField] private Color epicColor = new Color(0.8f, 0.3f, 0.8f);
    [SerializeField] private Color legendaryColor = new Color(1.0f, 0.8f, 0.2f);

    private bool isPanelOpen = false;
    private float panelWidth;
    private Vector2 openPosition;
    private Vector2 closedPosition;
    private List<GameObject> abilityItems = new List<GameObject>();

    private void Awake()
    {
        if (panelRect == null)
            panelRect = GetComponent<RectTransform>();

        if (toggleButton != null)
            toggleButton.onClick.AddListener(TogglePanel);

        // �г� �ʺ� ��� �� ��ġ ����
        panelWidth = panelRect.sizeDelta.x;
        openPosition = panelRect.anchoredPosition;
        closedPosition = new Vector2(openPosition.x + panelWidth, openPosition.y);

        // �ʱ� ����: ����
        panelRect.anchoredPosition = closedPosition;
        isPanelOpen = false;

        if (toggleButtonText != null)
            toggleButtonText.text = "��";

        // �׸��� ���̾ƿ� ����
        if (gridLayoutGroup == null && abilitiesContainer != null)
        {
            gridLayoutGroup = abilitiesContainer.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                gridLayoutGroup = abilitiesContainer.gameObject.AddComponent<GridLayoutGroup>();
                // �⺻ �׸��� ����
                gridLayoutGroup.cellSize = new Vector2(130, 180);
                gridLayoutGroup.spacing = new Vector2(10, 10);
                gridLayoutGroup.padding = new RectOffset(10, 10, 10, 10);
                gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayoutGroup.constraintCount = 2; // �� �࿡ 2�� ǥ��
            }
        }
        if (DungeonAbilityManager.Instance != null)
        {
            DungeonAbilityManager.Instance.OnAbilityAcquired += RefreshAbilitiesList;
            DungeonAbilityManager.Instance.OnAbilityLevelUp += RefreshAbilitiesList;

            // �ʱ� ������Ʈ
            RefreshAbilitiesList();
        }
       
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ����
        if (DungeonAbilityManager.Instance != null)
        {
            DungeonAbilityManager.Instance.OnAbilityAcquired -= RefreshAbilitiesList;
            DungeonAbilityManager.Instance.OnAbilityLevelUp -= RefreshAbilitiesList;
        }
    }

    public void TogglePanel()
    {
        isPanelOpen = !isPanelOpen;

        // �г� �ִϸ��̼�
        Vector2 targetPosition = isPanelOpen ? openPosition : closedPosition;
        panelRect.DOAnchorPos(targetPosition, animationDuration).SetEase(Ease.OutBack);

        // ��ư �ؽ�Ʈ ����
        if (toggleButtonText != null)
            toggleButtonText.text = isPanelOpen ? "��" : "��";
    }

    // �����Ƽ ��� ���ΰ�ħ
    public void RefreshAbilitiesList()
    {
        // ���� �׸� ����
        ClearAbilityItems();

        // DungeonAbilityManager���� ���� �����Ƽ ��������
        List<DungeonAbility> abilities = new List<DungeonAbility>();
        if (DungeonAbilityManager.Instance != null)
        {
            abilities = DungeonAbilityManager.Instance.GetCurrentAbilities();
        }
        else
        {
          
            Debug.Log("DungeonAbilityManager�� �����ϴ�.");
           
        }

        // �� �����Ƽ�� ���� UI �׸� ����
        foreach (var ability in abilities)
        {
            CreateAbilityItem(ability);
        }

        // ��ũ�� ��ġ �ʱ�ȭ
        if (scrollRect != null)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }
    
   
    // �����Ƽ �׸� ����
    private void CreateAbilityItem(DungeonAbility ability)
    {
        if (abilityItemPrefab == null || abilitiesContainer == null)
            return;

        // ������ �ν��Ͻ� ����
        GameObject itemObj = Instantiate(abilityItemPrefab, abilitiesContainer, false);
        abilityItems.Add(itemObj);

        // ������Ʈ ����
        DungeonAbilityItemUI itemUI = itemObj.GetComponent<DungeonAbilityItemUI>();
        if (itemUI != null)
        {
            // ��͵��� ���� ���� ��������
            Color rarityColor = GetRarityColor(ability.rarity);

            // �����Ƽ ���� ����
            itemUI.SetupAbility(ability, rarityColor);
        }
        else
        {
            Debug.LogError("DungeonAbilityItemUI ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    // ��͵��� ���� ���� ��ȯ
    private Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return commonColor;
            case Rarity.Uncommon:
                return uncommonColor;
            case Rarity.Rare:
                return rareColor;
            case Rarity.Epic:
                return epicColor;
            case Rarity.Legendary:
                return legendaryColor;
            default:
                return commonColor;
        }
    }

    // ���� �׸� ����
    private void ClearAbilityItems()
    {
        foreach (var item in abilityItems)
        {
            Destroy(item);
        }
        abilityItems.Clear();
    }
}
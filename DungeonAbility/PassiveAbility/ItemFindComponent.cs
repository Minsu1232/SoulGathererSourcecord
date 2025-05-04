// ItemFindComponent.cs - ������ ã�� Ȯ�� ���� ������Ʈ
using UnityEngine;

public class ItemFindComponent : MonoBehaviour
{
    private float itemFindBonus = 0f;

    private void Awake()
    {
        // ���� �ν��Ͻ��� ������ ã�� ���ʽ� ���
        RegisterGlobalBonus();
    }

    private void OnDestroy()
    {
        // ���� �ν��Ͻ����� ���ʽ� ����
        UnregisterGlobalBonus();
    }

    private void RegisterGlobalBonus()
    {
        if (GlobalItemFindManager.Instance != null)
        {
            GlobalItemFindManager.Instance.AddItemFindBonus(itemFindBonus);
        }
        else
        {
            Debug.LogWarning("GlobalItemFindManager�� �������� �ʽ��ϴ�.");

            // �Ŵ����� ���� ��� �ڵ����� ����
            GameObject managerObj = new GameObject("GlobalItemFindManager");
            managerObj.AddComponent<GlobalItemFindManager>();

            // ���� �� �ٽ� �õ�
            GlobalItemFindManager.Instance.AddItemFindBonus(itemFindBonus);
        }
    }

    private void UnregisterGlobalBonus()
    {
        if (GlobalItemFindManager.Instance != null)
        {
            GlobalItemFindManager.Instance.RemoveItemFindBonus(itemFindBonus);
        }
    }

    // ������ ã�� ���ʽ� ����
    public void AddItemFindBonus(float bonus)
    {
        // ���� �� ����
        float oldBonus = itemFindBonus;

        // �� �� ����
        itemFindBonus += bonus;

        // ���� �Ŵ����� ������� �ݿ�
        if (GlobalItemFindManager.Instance != null)
        {
            GlobalItemFindManager.Instance.RemoveItemFindBonus(oldBonus); // ���� ���ʽ� ����
            GlobalItemFindManager.Instance.AddItemFindBonus(itemFindBonus); // �� ���ʽ� �߰�
        }

        Debug.Log($"������ ã�� ���ʽ� �߰�: +{bonus * 100}%, ����: +{itemFindBonus * 100}%");
    }

    // ������ ã�� ���ʽ� ����
    public void RemoveItemFindBonus(float bonus)
    {
        // ���� �� ����
        float oldBonus = itemFindBonus;

        // �� �� ����
        itemFindBonus -= bonus;
        itemFindBonus = Mathf.Max(0f, itemFindBonus);

        // ���� �Ŵ����� ������� �ݿ�
        if (GlobalItemFindManager.Instance != null)
        {
            GlobalItemFindManager.Instance.RemoveItemFindBonus(oldBonus); // ���� ���ʽ� ����
            GlobalItemFindManager.Instance.AddItemFindBonus(itemFindBonus); // �� ���ʽ� �߰�
        }

        Debug.Log($"������ ã�� ���ʽ� ����: -{bonus * 100}%, ����: +{itemFindBonus * 100}%");
    }

    // ���� ������ ã�� ���ʽ� ��ȯ
    public float GetItemFindBonus()
    {
        return itemFindBonus;
    }
}
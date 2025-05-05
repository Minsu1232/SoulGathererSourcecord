/// <summary>
/// �̴ϰ��� �Ŵ���
/// </summary>
/// <remarks>
/// ������� ����: 3�� '���� �ý��� ���� �� Ȯ�� ���' > 1�� '���� ���� �ý���' > '����'
/// 
/// �ֿ� ���:
/// - �پ��� �̴ϰ��� Ÿ���� ���� �� ����
/// - �̴ϰ��� UI ����
/// - �̴ϰ��� ��� ó�� �� ���� ����
/// - �ð� ������ ������ ���� ���ο� ��� ȿ��
/// </remarks>
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class MiniGameManager : MonoBehaviour
{
    public DodgeMiniGameUI dodgeUI;  // Inspector���� �Ҵ�
                                     
    public event Action<MiniGameType, MiniGameResult> OnMiniGameComplete;
    [SerializeField] private MiniGameResultUI resultUI;  // ���⼭ �Ҵ�
    private IMiniGame currentMiniGame;
    private Dictionary<MiniGameType, IMiniGame> miniGameCache = new Dictionary<MiniGameType, IMiniGame>();
    public Dictionary<MiniGameType, MonoBehaviour> miniGameUIs = new Dictionary<MiniGameType, MonoBehaviour>();
    private void Awake()
    {
        if (dodgeUI != null)
        {
            RegisterUI(MiniGameType.Dodge, dodgeUI);
            Debug.Log("DodgeGameUI registered from inspector reference");
        }
        foreach (var kvp in miniGameUIs)
        {
            Debug.Log($"Registered UI - Type: {kvp.Key} value : {kvp.Value}");
        }
    }
    private void CreateMiniGame(MiniGameType type)
    {
        if (!miniGameCache.ContainsKey(type))
        {
            miniGameCache[type] = type switch
            {
                MiniGameType.Dodge => new DodgeMiniGameWrapper(this),
                //MiniGameType.Parry => new ParryMiniGame(),
                //MiniGameType.QuickTime => new QuickTimeMiniGame(),
                _ => throw new ArgumentException($"Unknown mini game type: {type}")
            };
        }
        currentMiniGame = miniGameCache[type];
    }

    public void StartMiniGame(MiniGameType type, float difficulty)
    {
        
        Debug.Log($"StartMiniGame called - Type: {type}, Difficulty: {difficulty}");
        Debug.Log($"miniGameUIs count after StartMiniGame: {miniGameUIs.Count}");
        Debug.Log($"Registering UI for type: {type}, Type value: {(int)type}");
        Debug.Log($"Trying to fetch UI for type: {type}, Type value: {(int)type}");
        foreach (var pair in miniGameUIs)
        {
            Debug.Log($"Key: {pair.Key}, UI Type: {pair.Value.GetType().Name}");
        }
        CreateMiniGame(type);
        currentMiniGame.Initialize(difficulty);
        Time.timeScale = 0.5f; // ���ο� ��� ȿ��
        Debug.Log(miniGameUIs.TryGetValue(type, out var uii));
        // UI Ȱ��ȭ
        if (miniGameUIs.TryGetValue(type, out var ui))
        {

            Debug.Log($"Found UI for type {type}, activating...");
            ui.gameObject.SetActive(true);  // UI GameObject ��ü�� Ȱ��ȭ
            if (ui is DodgeMiniGameUI dodgeUI && currentMiniGame is DodgeMiniGameWrapper dodgeGame)
            {
                Debug.Log("Initializing DodgeMiniGameUI");
                dodgeUI.gameObject.SetActive(true);  // UI�� ���� Ȱ��ȭ
                dodgeUI.Initialize(dodgeGame);       // �� ���� �ʱ�ȭ
            }
        }
        else
        {
            Debug.LogError($"No UI found for game type: {type}");
        }

        currentMiniGame.Start();
    }

    private void Update()
    {
        if (currentMiniGame != null && !currentMiniGame.IsComplete)
        {
            currentMiniGame.Update();
            if (currentMiniGame.IsComplete)
            {
                CompleteMiniGame();
            }
        }
    }

    private void CompleteMiniGame()
    {
        Time.timeScale = 1f;
        var result = currentMiniGame.GetResult();
        OnMiniGameComplete?.Invoke(currentMiniGame.Type, result);

        // UI ��Ȱ��ȭ
        if (miniGameUIs.TryGetValue(currentMiniGame.Type, out var ui))
        {
            ui.gameObject.SetActive(false);
        }

        currentMiniGame = null;
        Debug.Log("�̴ϰ��� ��");
    }

    public void RegisterUI(MiniGameType type, MonoBehaviour ui)
    {
        Debug.Log("RegisterUI Called!");
        Debug.Log($"Dictionary count before: {miniGameUIs.Count}");
        miniGameUIs[type] = ui;
        ui.gameObject.SetActive(false);
        Debug.Log($"Dictionary count after: {miniGameUIs.Count}");
        Debug.Log($"UI registered? {miniGameUIs.ContainsKey(type)}");
    }
    public void ShowMiniGameResult(DodgeMiniGame.DodgeResult result)
    {
        switch (result)
        {
            case DodgeMiniGame.DodgeResult.Perfect:
                resultUI.ShowResult("Perfect!", Color.magenta);
                break;
            case DodgeMiniGame.DodgeResult.Good:
                resultUI.ShowResult("Good!", Color.green);
                break;
            default:
                resultUI.ShowResult("Miss!", Color.red);
                break;
        }
    }
    public void HandleDodgeReward(MiniGameResult result)
    {
        var player = GameInitializer.Instance.GetPlayerClass();
        float invincibleDuration = 2f; // ���� �ð��� ���� �ڷ�ƾ �ʿ�
       
        switch (result)
        {
            case MiniGameResult.Perfect:
                // �Ϻ� ȸ��
                Debug.Log(player.GetStats().AttackPower);
                StartCoroutine(GrantInvincibility(player, invincibleDuration));
                player.ModifyPower(attackAmount: 30);
                Debug.Log(player.GetStats().AttackPower);
                break;

            case MiniGameResult.Good:
                // �Ϲ� ȸ��
                Debug.Log(player.GetStats().Speed);
                StartCoroutine(GrantInvincibility(player, invincibleDuration * 0.5f));
                player.ModifyPower(speedAmount: 2);
                Debug.Log(player.GetStats().Speed);
                break;

            case MiniGameResult.Miss:
                // ���� - �ƹ� ���� ����, ���� ���ݿ� ����
                break;
        }
    }

    private IEnumerator GrantInvincibility(PlayerClass player, float duration)
    {
        player.isInvicible = true;
        Debug.Log($"{duration}���� ����");
        yield return new WaitForSeconds(duration);
        player.isInvicible = false;
    }
}
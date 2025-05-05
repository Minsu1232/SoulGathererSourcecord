using UnityEngine;
using System;
/// <summary>
/// ȸ�� �̴ϰ��� ����
/// </summary>
/// <remarks>
/// ������� ����: 3�� '���� �ý��� ���� �� Ȯ�� ���' > 1�� '���� ���� �ý���' > '����'
/// 
/// �ֿ� ���:
/// - ���̵��� ���� ���� ���� ���� ����
/// - Ÿ�̹� ��� ȸ�� ����
/// - �Ϻ�/����/���� ��� ����
/// - �ð� ���� �� ���ο� ��� ȿ��
/// </remarks>
public class DodgeMiniGame
{
    public enum DodgeResult
    {
        Perfect,
        Good,
        Miss
    }
     
    private float successWindowStart;
    private float successWindowEnd;
    private float currentProgress;
    private bool isMovingRight = true;
    private float moveSpeed = 1f;

    private float totalTime = 3f;          // �̴ϰ��� ���� �ð�
    private float remainingTime;
    private float slowMotionScale = 0.3f;  // (UI ������ ���� ���ο� ��� ���� - �������� ������� ����)

    public event Action<DodgeResult> OnDodgeResultReceived;
    public event Action OnMiniGameEnded;

    /// <summary>
    /// �̴ϰ��� ���� �� ���� ������ �⺻ ���¸� �ʱ�ȭ�Ѵ�.
    /// ���� ������ �׻� �߾�(0.5)�� �������� �����Ǹ�, ���̵��� ���� �ʺ� ��������.
    /// </summary>
    public void StartDodgeMiniGame(float difficulty = 1f)
    {
        // ���̵��� ���� ���� ������ �ʺ� ������ ����
        // ���̵� 1������ �ʺ� ��������� �а�, ���̵� 3������ ���� ������
        float maxWidth = Mathf.Lerp(0.4f, 0.3f, (difficulty - 1f) / 2f);
        float minWidth = Mathf.Lerp(0.3f, 0.2f, (difficulty - 1f) / 2f);
        // �����ϰ� �ʺ� ���� (�� �κ��� ���������ε� ������ �� ����)
        float windowWidth = UnityEngine.Random.Range(minWidth, maxWidth);

        // ���� ������ �׻� �߾ӿ� ��ġ�ϵ��� ���� (�߾� 0.5 ����)
        successWindowStart = 0.5f - windowWidth / 2f;
        successWindowEnd = 0.5f + windowWidth / 2f;

        // ȭ��ǥ �̵� �ӵ��� ���� �ð� ���� (���̵��� ���� ����)
        moveSpeed = 1f + (difficulty - 1f);  // ���̵� 1~3�� ���� 2~4�� ����
        totalTime = Mathf.Lerp(3f, 1.5f, (difficulty - 1f) / 2f);  // ���̵� 1~3�� ���� 3~1.5�ʷ� ����

        // �⺻ ���� �ʱ�ȭ
        currentProgress = 0f;
        isMovingRight = true;
        remainingTime = totalTime;

        // ���ο� ��� ȿ�� ���� (UI �����, ���� ������Ʈ�� unscaledDeltaTime ���)
        Time.timeScale = slowMotionScale;
        AudioListener.pause = true;

        Debug.Log($"DodgeMiniGame Started - Difficulty: {difficulty}, " +
                  $"Window: {successWindowStart:F2} ~ {successWindowEnd:F2}, " +
                  $"Move Speed: {moveSpeed}, Total Time: {totalTime}");
    }

    /// <summary>
    /// ����ڰ� �Է��� ���� ��, ���� ���� ��Ȳ�� ���� ������ �������� �����Ѵ�.
    /// </summary>
    public DodgeResult ProcessInput(float inputTiming)
    {
        // �Է� ������ ���� ���¸� �α׷� ����� UI�� ������ ��ġ�ϴ��� Ȯ��
        Debug.Log($"[Input Process] InputTiming: {inputTiming:F2}, CurrentProgress: {currentProgress:F2}, " +
                  $"SuccessWindow: {successWindowStart:F2} ~ {successWindowEnd:F2}");

        DodgeResult result;
        bool isInSuccessWindow = inputTiming >= successWindowStart && inputTiming <= successWindowEnd;

        if (isInSuccessWindow)
        {
            float center = (successWindowStart + successWindowEnd) / 2f;
            float distanceFromCenter = Mathf.Abs(inputTiming - center);
            result = distanceFromCenter < 0.05f ? DodgeResult.Perfect : DodgeResult.Good;
        }
        else
        {
            result = DodgeResult.Miss;
        }

        OnDodgeResultReceived?.Invoke(result);
        EndMiniGame(result != DodgeResult.Miss);

        return result;
    }

    /// <summary>
    /// �� �����Ӹ��� ȭ��ǥ�� �̵���Ű��, ���� �ð��� üũ�Ѵ�.
    /// ������Ʈ �� Time.unscaledDeltaTime�� ����Ͽ� timeScale ���濡 ������� �ϰ��� �ð� ����� �Ѵ�.
    /// </summary>
    public bool Update()
    {
        // ��� ���� ������Ʈ�� unscaledDeltaTime�� ���
        float unscaledDeltaTime = Time.unscaledDeltaTime;

        // ȭ��ǥ �̵�
        if (isMovingRight)
        {
            currentProgress += moveSpeed * unscaledDeltaTime;
            if (currentProgress >= 1f)
            {
                currentProgress = 1f;
                isMovingRight = false;
            }
        }
        else
        {
            currentProgress -= moveSpeed * unscaledDeltaTime;
            if (currentProgress <= 0f)
            {
                currentProgress = 0f;
                isMovingRight = true;
            }
        }

        // ���� �ð� ���� (unscaledDeltaTime ���)
        remainingTime -= unscaledDeltaTime;

        if (remainingTime <= 0)
        {
            EndMiniGame(false);
            return false;
        }

        return true;
    }

    /// <summary>
    /// �̴ϰ��� ���� ó�� (Ÿ�ӽ����� ����, ����� ������ �簳 ��).
    /// </summary>
    public void EndMiniGame(bool success)
    {
        // ������ �������Ƿ� timeScale�� ����� ���¸� �����Ѵ�.
        Time.timeScale = 1f;
        AudioListener.pause = false;
        OnMiniGameEnded?.Invoke();
    }

    // -------------------------------------------------
    // Getters
    // -------------------------------------------------
    public float GetCurrentProgress() => currentProgress;          // 0 ~ 1
    public float GetSuccessWindowStart() => successWindowStart;    // 0 ~ 1
    public float GetSuccessWindowEnd() => successWindowEnd;        // 0 ~ 1
    public float GetRemainingTimeNormalized() => Mathf.Clamp01(remainingTime / totalTime);
    public float GetSlowMotionScale() => slowMotionScale;
}

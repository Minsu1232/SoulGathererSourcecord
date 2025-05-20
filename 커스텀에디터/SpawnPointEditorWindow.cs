#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class SpawnPointEditorWindow : EditorWindow
{
    private SpawnPointContainer spawnContainer;
    private SpawnPointMarker.SpawnType currentSpawnType = SpawnPointMarker.SpawnType.Fixed;
    private int monsterID = 1;
    private float spawnWeight = 1f;
    private bool isBoss = false;
    private Color spawnColor = Color.red;

    private bool isPlacementMode = false; // ��ġ ��� Ȱ��ȭ ����
    private Vector3 lastHitPosition; // ���������� ������ ��ġ
    private bool hasValidHit = false; // ��ȿ�� ��Ʈ�� �ִ���

    [MenuItem("Tools/���� ����Ʈ ������")]
    public static void ShowWindow()
    {
        GetWindow<SpawnPointEditorWindow>("���� ����Ʈ ������");
    }

    private void OnEnable()
    {
        // ������ �����찡 Ȱ��ȭ�� �� �� GUI �̺�Ʈ ���
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        // ������ �����찡 ��Ȱ��ȭ�� �� �̺�Ʈ ����
        SceneView.duringSceneGui -= OnSceneGUI;

        // ��ġ ��� ���� ����
        isPlacementMode = false;
    }

    private void OnGUI()
    {
        GUILayout.Label("���� ����Ʈ ������", EditorStyles.boldLabel);

        // �����̳� ����
        spawnContainer = (SpawnPointContainer)EditorGUILayout.ObjectField(
            "���� �����̳�", spawnContainer, typeof(SpawnPointContainer), true);

        if (spawnContainer == null)
        {
            EditorGUILayout.HelpBox("���� SpawnPointContainer�� �����ϼ���", MessageType.Warning);

            if (GUILayout.Button("�� ���� �����̳� ����"))
            {
                CreateNewContainer();
            }

            return;
        }

        EditorGUILayout.Space();

        // �������� ����
        spawnContainer.stageID = EditorGUILayout.TextField("�������� ID", spawnContainer.stageID);
        spawnContainer.stageName = EditorGUILayout.TextField("�������� �̸�", spawnContainer.stageName);

        EditorGUILayout.Space();

        // ���� ����Ʈ ����
        GUILayout.Label("���� ����Ʈ �߰�", EditorStyles.boldLabel);
        currentSpawnType = (SpawnPointMarker.SpawnType)EditorGUILayout.EnumPopup("���� Ÿ��", currentSpawnType);

        if (currentSpawnType != SpawnPointMarker.SpawnType.Portal)
        {
            monsterID = EditorGUILayout.IntField("���� ID", monsterID);

            if (currentSpawnType == SpawnPointMarker.SpawnType.Random)
            {
                spawnWeight = EditorGUILayout.FloatField("����ġ", spawnWeight);
            }

            isBoss = EditorGUILayout.Toggle("����", isBoss);
        }

        spawnColor = EditorGUILayout.ColorField("����", spawnColor);

        // ���� ���: ������Ʈ ���� �� ���� �߰�
        GUI.enabled = Selection.activeGameObject != null;
        if (GUILayout.Button("������ ������Ʈ ��ġ�� ���� ����Ʈ �߰�"))
        {
            AddSpawnPointAtSelectedPosition();
        }
        GUI.enabled = true;

        EditorGUILayout.Space();

        // ���ο� ���: �ݶ��̴� ���� ���
        EditorGUILayout.BeginHorizontal();

        // ��ġ ��� ��� ��ư
        GUIStyle toggleStyle = new GUIStyle(GUI.skin.button);
        toggleStyle.normal.textColor = isPlacementMode ? Color.green : Color.white;
        toggleStyle.fontStyle = isPlacementMode ? FontStyle.Bold : FontStyle.Normal;

        if (GUILayout.Button(isPlacementMode ? "��ġ ��� ����" : "��ġ ��� ����", toggleStyle))
        {
            isPlacementMode = !isPlacementMode;
            SceneView.RepaintAll(); // �� �� ����
        }

        // ��ġ ��忡���� �˸�
        if (isPlacementMode)
        {
            EditorGUILayout.HelpBox("�� �信�� Shift+Ŭ������ ���� ����Ʈ ��ġ", MessageType.Info);
        }

        EditorGUILayout.EndHorizontal();

        // ������ ��Ʈ ��ġ ǥ�� (����׿�)
        if (hasValidHit)
        {
            EditorGUILayout.LabelField("������ ��Ʈ ��ġ:", lastHitPosition.ToString("F2"));
        }

        EditorGUILayout.Space();

        // ��������/�������� ��ư
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("CSV�� ��������"))
        {
            spawnContainer.ExportToCSV();
        }

        if (GUILayout.Button("CSV���� ��������"))
        {
            spawnContainer.ImportFromCSV();
        }
        EditorGUILayout.EndHorizontal();

        // ���� ����Ʈ ��Ȳ ǥ��
        if (spawnContainer != null)
        {
            EditorGUILayout.Space();
            GUILayout.Label("���� ����Ʈ ��Ȳ", EditorStyles.boldLabel);

            Transform fixedParent = spawnContainer.transform.Find("FixedSpawns");
            Transform randomParent = spawnContainer.transform.Find("RandomSpawns");

            int fixedCount = fixedParent != null ? fixedParent.childCount : 0;
            int randomCount = randomParent != null ? randomParent.childCount : 0;

            EditorGUILayout.LabelField($"���� ���� ����Ʈ: {fixedCount}��");
            EditorGUILayout.LabelField($"���� ���� ����Ʈ: {randomCount}��");

            // �÷��̾� ��ġ ǥ��
            if (spawnContainer.playerSpawnPoint != null)
            {
                EditorGUILayout.LabelField("�÷��̾� ���� ��ġ:",
                    spawnContainer.playerSpawnPoint.position.ToString("F2"));
            }

            // ��Ż ��ġ ǥ��
            if (spawnContainer.portalSpawnPoint != null)
            {
                EditorGUILayout.LabelField("��Ż ���� ��ġ:",
                    spawnContainer.portalSpawnPoint.position.ToString("F2"));
            }
        }
    }

    // �� GUI �̺�Ʈ ó��
    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isPlacementMode || spawnContainer == null) return;

        Event e = Event.current;

        // ���콺 �̵� �� ����ĳ��Ʈ�� �ݶ��̴� ����
        if (e.type == EventType.MouseMove)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // �ݶ��̴� ��Ʈ ������
                lastHitPosition = hit.point;
                hasValidHit = true;
                sceneView.Repaint(); // �� �� ����
            }
            else
            {
                hasValidHit = false;
            }
        }

        // ��ġ ��忡�� Shift + Ŭ������ ���� ����Ʈ ��ġ
        if (e.type == EventType.MouseDown && e.button == 0 && e.shift && hasValidHit)
        {
            // ���� ����Ʈ ����
            AddSpawnPointAtPosition(lastHitPosition);
            e.Use(); // �̺�Ʈ ��� �Ϸ� ó��
        }

        // ��ġ ��� �ð�ȭ
        if (hasValidHit)
        {
            // ������ �ڵ� �׸���
            Handles.color = spawnColor;

            switch (currentSpawnType)
            {
                case SpawnPointMarker.SpawnType.Fixed:
                    Handles.DrawWireCube(lastHitPosition, Vector3.one * 0.5f);
                    break;
                case SpawnPointMarker.SpawnType.Random:
                    Handles.SphereHandleCap(0, lastHitPosition, Quaternion.identity, 0.5f, EventType.Repaint);
                    break;
                case SpawnPointMarker.SpawnType.Portal:
                    Handles.SphereHandleCap(0, lastHitPosition, Quaternion.identity, 0.5f, EventType.Repaint);
                    Handles.DrawLine(lastHitPosition, lastHitPosition + Vector3.up * 1.0f);
                    break;
            }

            // �� ǥ��
            Handles.Label(lastHitPosition + Vector3.up * 0.5f,
                currentSpawnType == SpawnPointMarker.SpawnType.Portal
                    ? "��Ż"
                    : $"ID: {monsterID}");
        }
    }

    private void CreateNewContainer()
    {
        GameObject containerObj = new GameObject("SpawnPointContainer");
        spawnContainer = containerObj.AddComponent<SpawnPointContainer>();

        // �ڽ� �����̳� ����
        GameObject fixedContainer = new GameObject("FixedSpawns");
        fixedContainer.transform.parent = containerObj.transform;

        GameObject randomContainer = new GameObject("RandomSpawns");
        randomContainer.transform.parent = containerObj.transform;

        // �÷��̾� ���� ����Ʈ
        GameObject playerSpawn = new GameObject("PlayerSpawn");
        playerSpawn.transform.parent = containerObj.transform;
        spawnContainer.playerSpawnPoint = playerSpawn.transform;

        // ��Ż ���� ����Ʈ
        GameObject portalSpawn = new GameObject("PortalSpawn");
        portalSpawn.transform.parent = containerObj.transform;
        spawnContainer.portalSpawnPoint = portalSpawn.transform;

        Selection.activeGameObject = containerObj;
    }

    // ������ ������Ʈ ��ġ�� ���� ����Ʈ �߰� (���� ���)
    private void AddSpawnPointAtSelectedPosition()
    {
        if (Selection.activeGameObject == null)
        {
            EditorUtility.DisplayDialog("����", "������Ʈ�� �����ؾ� �մϴ�", "Ȯ��");
            return;
        }

        Vector3 position = Selection.activeGameObject.transform.position;
        AddSpawnPointAtPosition(position);
    }

    // ������ ��ġ�� ���� ����Ʈ �߰� (���� �޼���)
    private void AddSpawnPointAtPosition(Vector3 position)
    {
        if (spawnContainer == null) return;

        GameObject spawnObj = new GameObject(GetSpawnName());
        spawnObj.transform.position = position;

        // �θ� ����
        Transform parent = null;

        switch (currentSpawnType)
        {
            case SpawnPointMarker.SpawnType.Fixed:
                parent = spawnContainer.transform.Find("FixedSpawns");
                break;
            case SpawnPointMarker.SpawnType.Random:
                parent = spawnContainer.transform.Find("RandomSpawns");
                break;
            case SpawnPointMarker.SpawnType.Portal:
                // ��Ż ��ġ ������Ʈ
                if (spawnContainer.portalSpawnPoint != null)
                {
                    spawnContainer.portalSpawnPoint.position = position;
                    DestroyImmediate(spawnObj); // �ӽ� ������Ʈ ����
                    return;
                }
                else
                {
                    // ��Ż ���� ����Ʈ�� ������ ���� ����
                    spawnObj.name = "PortalSpawn";
                    spawnContainer.portalSpawnPoint = spawnObj.transform;
                    parent = spawnContainer.transform;
                }
                break;
            case SpawnPointMarker.SpawnType.Player:
                // �÷��̾� ��ġ ������Ʈ
                if (spawnContainer.playerSpawnPoint != null)
                {
                    spawnContainer.playerSpawnPoint.position = position;
                    DestroyImmediate(spawnObj); // �ӽ� ������Ʈ ����
                    return;
                }
                else
                {
                    // �÷��̾� ���� ����Ʈ�� ������ ���� ����
                    spawnObj.name = "PlayerSpawn";
                    spawnContainer.playerSpawnPoint = spawnObj.transform;
                    parent = spawnContainer.transform;
                }
                break;
        }

        if (parent != null)
        {
            spawnObj.transform.parent = parent;
        }
        else
        {
            spawnObj.transform.parent = spawnContainer.transform;
        }

        // ��Ŀ ������Ʈ �߰� (��Ż�̳� �÷��̾ �ƴ� ���)
        if (currentSpawnType != SpawnPointMarker.SpawnType.Portal &&
            currentSpawnType != SpawnPointMarker.SpawnType.Player)
        {
            SpawnPointMarker marker = spawnObj.AddComponent<SpawnPointMarker>();
            marker.spawnType = currentSpawnType;
            marker.monsterID = monsterID;
            marker.spawnWeight = spawnWeight;
            marker.isBoss = isBoss;
            marker.gizmoColor = spawnColor;
        }

        // ���� ����Ʈ �߰� ���� �α�
        Debug.Log($"���� ����Ʈ �߰���: {spawnObj.name}, ��ġ: {position}");

        // �����Ϳ��� ����
        Selection.activeGameObject = spawnObj;

        // �����̳ʰ� ������� ��� (Undo ����)
        EditorUtility.SetDirty(spawnContainer);
    }

    private string GetSpawnName()
    {
        switch (currentSpawnType)
        {
            case SpawnPointMarker.SpawnType.Fixed:
                return isBoss ? $"Boss_{monsterID}" : $"Fixed_{monsterID}";
            case SpawnPointMarker.SpawnType.Random:
                return $"Random_{monsterID}";
            case SpawnPointMarker.SpawnType.Portal:
                return "Portal";
            case SpawnPointMarker.SpawnType.Player:
                return "PlayerSpawn";
            default:
                return "Spawn";
        }
    }
}
#endif
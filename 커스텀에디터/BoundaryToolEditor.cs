using UnityEngine;
using UnityEditor;

public class BoundaryToolEditor : EditorWindow
{
    // ��� �� ����Ʈ
    private Vector3[] boundaryPoints;
    private GameObject boundaryPrefab;

    [MenuItem("Tools/Boundary Tool")]
    public static void ShowWindow()
    {
        GetWindow<BoundaryToolEditor>("Boundary Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Boundary Tool", EditorStyles.boldLabel);

        // ��� Prefab ����
        boundaryPrefab = (GameObject)EditorGUILayout.ObjectField("Boundary Prefab", boundaryPrefab, typeof(GameObject), false);

        // ��� �� ���� ����
        int pointCount = EditorGUILayout.IntField("Number of Points", boundaryPoints != null ? boundaryPoints.Length : 0);

        if (boundaryPoints == null || boundaryPoints.Length != pointCount)
        {
            boundaryPoints = new Vector3[pointCount];
        }

        // ��� �� ��ġ �Է�
        for (int i = 0; i < pointCount; i++)
        {
            boundaryPoints[i] = EditorGUILayout.Vector3Field($"Point {i + 1}", boundaryPoints[i]);
        }

        // ��ư: ��� ����
        if (GUILayout.Button("Generate Boundaries"))
        {
            if (ValidateInputs())
                GenerateBoundaries();
        }

        // ��ư: ��� ����
        if (GUILayout.Button("Clear Boundaries"))
        {
            ClearBoundaries();
        }
    }

    private bool ValidateInputs()
    {
        if (boundaryPrefab == null)
        {
            Debug.LogError("Boundary Prefab is not assigned!");
            return false;
        }

        if (boundaryPoints == null || boundaryPoints.Length < 2)
        {
            Debug.LogError("Not enough points to create boundaries! At least 2 points are required.");
            return false;
        }

        return true;
    }

    private void GenerateBoundaries()
    {
        for (int i = 0; i < boundaryPoints.Length - 1; i++) // ������ ������ ù ��° ������ ���ư��� ����
        {
            Vector3 start = boundaryPoints[i];
            Vector3 end = boundaryPoints[i + 1];

            CreateBoundarySegment(start, end);
        }

        Debug.Log("Boundaries generated successfully!");
    }

    private void CreateBoundarySegment(Vector3 start, Vector3 end)
    {
        if (boundaryPrefab == null)
            return;

        // �� ���� �߾� ��ġ
        Vector3 midPoint = (start + end) / 2;

        // �� �� ������ �Ÿ�
        float distance = Vector3.Distance(start, end);

        // ���� ���: X�� �������� ȸ���ϵ��� ����
        Vector3 direction = (end - start).normalized;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction); // X���� �������� ���� ���

        // Boundary Prefab ����
        GameObject boundary = Instantiate(boundaryPrefab, midPoint, rotation);

        // Box Collider ũ�� ����
        BoxCollider collider = boundary.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.size = new Vector3(distance, collider.size.y, collider.size.z); // X�� ũ�⸸ ����
                                                                                     // ������Ʈ �̸��� �±� ����
            boundary.name = "Boundary Segment";
            boundary.tag = "Boundary"; // �±׸� �����Ͽ� Clear ��ɰ� ����
        }

      
      
    }

    private void ClearBoundaries()
    {
        GameObject[] boundaries = GameObject.FindGameObjectsWithTag("Boundary");
        foreach (GameObject boundary in boundaries)
        {
            DestroyImmediate(boundary); // ��� ����
        }

        Debug.Log("Boundaries cleared!");
    }
}

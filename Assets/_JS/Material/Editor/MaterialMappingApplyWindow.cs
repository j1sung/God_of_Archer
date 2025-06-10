using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;   // �� �� ��Ƽ ǥ�ø� ���� �߰�
#endif

public class MaterialMappingApplyWindow : EditorWindow
{
    // �� �̸� ����� ��Ƽ���� ���� ������ ���� SO
    private MaterialMappingProfile _targetProfile;

    // �� ������ �� ������ �𵨵��� Drag & Drop�ϰų�, Hierarchy���� ���� ������ �� ��ư Ŭ��
    private GameObject[] _targetModels = new GameObject[0];

    [MenuItem("Tools/Material Mapping/Apply Mapping Window")]
    public static void OpenWindow()
    {
        GetWindow<MaterialMappingApplyWindow>("Apply Material Mapping");
    }

    private void OnGUI()
    {
        GUILayout.Label("1) Mapping Profile (ScriptableObject)", EditorStyles.boldLabel);
        _targetProfile = (MaterialMappingProfile)EditorGUILayout.ObjectField(
            "Profile SO",
            _targetProfile,
            typeof(MaterialMappingProfile),
            false
        );

        EditorGUILayout.Space();

        GUILayout.Label("2) Target Models (GameObjects)", EditorStyles.boldLabel);
        // Hierarchy���� ���� ���õ� GameObject���� �������� ���
        if (GUILayout.Button("Load From Hierarchy Selection"))
        {
            _targetModels = Selection.gameObjects;
            Repaint();
        }

        // �Ǵ� �迭 ũ�⸦ ���� ������ �巡�� �� ����� �� �ֵ��� ó��
        int newSize = Mathf.Max(0, EditorGUILayout.IntField("Models Count", _targetModels.Length));
        if (newSize != _targetModels.Length)
        {
            System.Array.Resize(ref _targetModels, newSize);
        }
        for (int i = 0; i < _targetModels.Length; i++)
        {
            _targetModels[i] = (GameObject)EditorGUILayout.ObjectField($"Model {i}", _targetModels[i], typeof(GameObject), true);
        }

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(_targetProfile == null || _targetModels.Length == 0);
        if (GUILayout.Button("Apply Mapping to Models"))
        {
            ApplyProfileToModels();
        }
        EditorGUI.EndDisabledGroup();
    }

    private void ApplyProfileToModels()
    {
        if (_targetProfile == null || _targetModels.Length == 0) return;

        int appliedCount = 0;
        foreach (var go in _targetModels)
        {
            if (go == null) continue;
            bool applied = ApplyProfileToModel(_targetProfile, go);
            if (applied) appliedCount++;
        }

        // �� ���� ������Ʈ�� ���������� �ش� ���� ��dirty���� ǥ���� �Ӵϴ�.
        foreach (var go in _targetModels)
        {
            if (go == null) continue;
            if (go.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(go.scene);
            }
        }

        // ��Ƽ���� ���� ��ü�� �ٲ� ���� �ƴϹǷ� AssetDatabase.SaveAssets()�� ���� ����������,
        // Ȥ�� Profile SO�� ���� ������ �־��ٸ� ����
        AssetDatabase.SaveAssets();

        Debug.Log($"[MaterialMapping] Applied '{_targetProfile.name}' to {appliedCount} model(s).");
    }

    /// <summary>
    /// �ش� ��(root) ������ Profile�� mappings�� �����ϰ�, ������ �ϳ��� �ٲ������ true�� ����
    /// </summary>
    private bool ApplyProfileToModel(MaterialMappingProfile profile, GameObject root)
    {
        bool didApply = false;

        // ��Ʈ ������ Renderer�� �ϳ��� ������ ��ŵ
        var anyRenderer = root.GetComponentInChildren<Renderer>(true);
        if (anyRenderer == null)
        {
            Debug.LogWarning($"[MaterialMapping] '{root.name}' has no Renderer children. Skipped.");
            return false;
        }

        foreach (var entry in profile.mappings)
        {
            // ����: "Body/Torso" �� ��� ��θ� �̿��� Transform ã�ư�
            Transform targetT = root.transform.Find(entry.transformPath);
            if (targetT == null)
            {
                Debug.LogWarning($"[MaterialMapping] Path '{entry.transformPath}' not found under '{root.name}'.");
                continue;
            }

            // ���� SkinnedMeshRenderer üũ
            var smr = targetT.GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                // Undo ��� (Ctrl+Z ����)
                Undo.RecordObject(smr, "Apply Material Mapping (Skinned)");

                // �޽�(�޽�)�� ����޽� ���� ȹ��
                Mesh mesh = smr.sharedMesh;
                int subCount = (mesh != null) ? mesh.subMeshCount : 1;

                // ���� sharedMaterials ���� (���̴� subCount�� ���ƾ� ��)
                Material[] original = smr.sharedMaterials;
                Material[] newMats = new Material[subCount];

                // ���� ������ subCount���� ������, ������ �κ��� 0�� ���� ��Ƽ����� ä��
                for (int i = 0; i < subCount; i++)
                {
                    newMats[i] = (i < original.Length && original[i] != null) ? original[i] : original.Length > 0 ? original[0] : null;
                }

                // ���⿡ entry.material(�������� ��Ƽ����)�� **��� ����**�� ������� �Ʒ�ó��
                for (int i = 0; i < subCount; i++)
                {
                    newMats[i] = entry.material;
                }

                // ���� ��Ư�� ���� �ε����� �ٲٰ� �������� �����д١� ������ ���� �ʹٸ�
                // newMats[���ϴ��ε���] = entry.material; �� ���� �ٲٸ� �˴ϴ�.

                smr.sharedMaterials = newMats;
                didApply = true;
                continue;
            }

            // �״��� MeshRenderer üũ
            var mr = targetT.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Undo.RecordObject(mr, "Apply Material Mapping (Mesh)");

                // MeshFilter�� �޽ö� subMesh ���� �ľ�
                MeshFilter mf = targetT.GetComponent<MeshFilter>();
                int subCount = (mf != null && mf.sharedMesh != null) ? mf.sharedMesh.subMeshCount : 1;

                Material[] original = mr.sharedMaterials;
                Material[] newMats = new Material[subCount];

                for (int i = 0; i < subCount; i++)
                {
                    newMats[i] = (i < original.Length && original[i] != null) ? original[i] : original.Length > 0 ? original[0] : null;
                }

                // ��� ���Կ� ���� ��Ƽ���� �����
                for (int i = 0; i < subCount; i++)
                {
                    newMats[i] = entry.material;
                }

                mr.sharedMaterials = newMats;
                didApply = true;
                continue;
            }

            Debug.LogWarning($"[MaterialMapping] '{entry.transformPath}' in '{root.name}' has no MeshRenderer/SkinnedMeshRenderer.");
        }

        return didApply;
    }
}
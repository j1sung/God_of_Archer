using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;   // ★ 씬 더티 표시를 위해 추가
#endif

public class MaterialMappingApplyWindow : EditorWindow
{
    // ① 미리 저장된 머티리얼 매핑 정보를 담은 SO
    private MaterialMappingProfile _targetProfile;

    // ② 눌렀을 때 적용할 모델들을 Drag & Drop하거나, Hierarchy에서 복수 선택한 뒤 버튼 클릭
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
        // Hierarchy에서 다중 선택된 GameObject들을 가져오는 방법
        if (GUILayout.Button("Load From Hierarchy Selection"))
        {
            _targetModels = Selection.gameObjects;
            Repaint();
        }

        // 또는 배열 크기를 직접 조절해 드래그 앤 드롭할 수 있도록 처리
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

        // 씬 안의 오브젝트를 변경했으면 해당 씬을 “dirty”로 표시해 둡니다.
        foreach (var go in _targetModels)
        {
            if (go == null) continue;
            if (go.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(go.scene);
            }
        }

        // 머티리얼 에셋 자체를 바꾼 것은 아니므로 AssetDatabase.SaveAssets()는 선택 사항이지만,
        // 혹시 Profile SO에 대한 변경이 있었다면 저장
        AssetDatabase.SaveAssets();

        Debug.Log($"[MaterialMapping] Applied '{_targetProfile.name}' to {appliedCount} model(s).");
    }

    /// <summary>
    /// 해당 모델(root) 하위에 Profile의 mappings를 적용하고, 실제로 하나라도 바뀌었으면 true를 리턴
    /// </summary>
    private bool ApplyProfileToModel(MaterialMappingProfile profile, GameObject root)
    {
        bool didApply = false;

        // 루트 하위에 Renderer가 하나도 없으면 스킵
        var anyRenderer = root.GetComponentInChildren<Renderer>(true);
        if (anyRenderer == null)
        {
            Debug.LogWarning($"[MaterialMapping] '{root.name}' has no Renderer children. Skipped.");
            return false;
        }

        foreach (var entry in profile.mappings)
        {
            // 예시: "Body/Torso" 등 상대 경로를 이용해 Transform 찾아감
            Transform targetT = root.transform.Find(entry.transformPath);
            if (targetT == null)
            {
                Debug.LogWarning($"[MaterialMapping] Path '{entry.transformPath}' not found under '{root.name}'.");
                continue;
            }

            // 먼저 SkinnedMeshRenderer 체크
            var smr = targetT.GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                // Undo 기록 (Ctrl+Z 가능)
                Undo.RecordObject(smr, "Apply Material Mapping (Skinned)");

                // 메시(메쉬)와 서브메시 개수 획득
                Mesh mesh = smr.sharedMesh;
                int subCount = (mesh != null) ? mesh.subMeshCount : 1;

                // 기존 sharedMaterials 복사 (길이는 subCount와 같아야 함)
                Material[] original = smr.sharedMaterials;
                Material[] newMats = new Material[subCount];

                // 원래 슬롯이 subCount보다 작으면, 부족한 부분은 0번 슬롯 머티리얼로 채움
                for (int i = 0; i < subCount; i++)
                {
                    newMats[i] = (i < original.Length && original[i] != null) ? original[i] : original.Length > 0 ? original[0] : null;
                }

                // 여기에 entry.material(프로필의 머티리얼)을 **모든 슬롯**에 덮어쓰려면 아래처럼
                for (int i = 0; i < subCount; i++)
                {
                    newMats[i] = entry.material;
                }

                // 만약 “특정 슬롯 인덱스만 바꾸고 나머지는 원래둔다” 식으로 쓰고 싶다면
                // newMats[원하는인덱스] = entry.material; 과 같이 바꾸면 됩니다.

                smr.sharedMaterials = newMats;
                didApply = true;
                continue;
            }

            // 그다음 MeshRenderer 체크
            var mr = targetT.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Undo.RecordObject(mr, "Apply Material Mapping (Mesh)");

                // MeshFilter로 메시랑 subMesh 개수 파악
                MeshFilter mf = targetT.GetComponent<MeshFilter>();
                int subCount = (mf != null && mf.sharedMesh != null) ? mf.sharedMesh.subMeshCount : 1;

                Material[] original = mr.sharedMaterials;
                Material[] newMats = new Material[subCount];

                for (int i = 0; i < subCount; i++)
                {
                    newMats[i] = (i < original.Length && original[i] != null) ? original[i] : original.Length > 0 ? original[0] : null;
                }

                // 모든 슬롯에 같은 머티리얼 덮어쓰기
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
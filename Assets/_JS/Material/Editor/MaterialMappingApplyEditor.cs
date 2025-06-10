using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

[CustomEditor(typeof(MaterialMappingProfile))]
public class MaterialMappingApplyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(6);
        if (GUILayout.Button("Apply to Selected Models"))
        {
            ApplyToSelection();
        }
    }

    private void ApplyToSelection()
    {
        MaterialMappingProfile profile = (MaterialMappingProfile)target;
        if (profile.mappings == null || profile.mappings.Length == 0)
        {
            Debug.LogWarning("[MaterialMapping] Profile has no entries to apply.");
            return;
        }

        // 선택된 모든 GameObject에 대해 반복
        foreach (GameObject go in Selection.gameObjects)
        {
            ApplyProfileToModel(profile, go);
        }

        // 변경 사항 저장
        EditorUtility.SetDirty(profile);
        AssetDatabase.SaveAssets();
    }

    private void ApplyProfileToModel(MaterialMappingProfile profile, GameObject root)
    {
        // 먼저, 모델이 맞는지 간단히 검사(루트가 비어 있거나, 렌더러가 하나도 없으면 스킵)
        var anyRenderer = root.GetComponentInChildren<Renderer>(true);
        if (anyRenderer == null)
        {
            Debug.LogWarning($"[MaterialMapping] '{root.name}' has no Renderer component in children. Skipped.");
            return;
        }

        // 하위 모든 MeshRenderer/SkinnedMeshRenderer를 한번에 가져옴
        var meshRenderers = root.GetComponentsInChildren<MeshRenderer>(true);
        var skinnedRenders = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        // 지금부터 SO에 저장된 각 Entry(경로→머티리얼)를 순회
        foreach (var entry in profile.mappings)
        {
            // 1) entry.transformPath를 root 하위에서 찾기
            Transform targetTransform = root.transform.Find(entry.transformPath);
            if (targetTransform == null)
            {
                Debug.LogWarning($"[MaterialMapping] Path '{entry.transformPath}' not found under '{root.name}'.");
                continue;
            }

            // 2) 해당 Transform 위에 MeshRenderer 또는 SkinnedMeshRenderer가 붙어 있는지 검사
            var mr = targetTransform.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Undo.RecordObject(mr, "Apply Material Mapping");
                mr.sharedMaterial = entry.material;
                continue;
            }

            var smr = targetTransform.GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                Undo.RecordObject(smr, "Apply Material Mapping");
                smr.sharedMaterial = entry.material;
                continue;
            }

            Debug.LogWarning($"[MaterialMapping] No MeshRenderer/SkinnedMeshRenderer on '{entry.transformPath}' in '{root.name}'.");
        }

        Debug.Log($"[MaterialMapping] Applied profile '{profile.name}' to '{root.name}'.");
    }
}
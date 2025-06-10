using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using System.Collections.Generic;

public class MaterialMappingEditor : EditorWindow
{
    private MaterialMappingProfile _targetProfile;
    private GameObject _targetModel;

    [MenuItem("Tools/Material Mapping/Save Current Mapping")]
    public static void ShowWindow()
    {
        var window = GetWindow<MaterialMappingEditor>("Material Mapping");
        window.minSize = new Vector2(300, 120);
    }

    private void OnGUI()
    {
        GUILayout.Space(8);
        EditorGUILayout.LabelField("1) Profile (SO) to Save Into", EditorStyles.boldLabel);
        _targetProfile = (MaterialMappingProfile)EditorGUILayout.ObjectField(
            "Mapping Profile",
            _targetProfile,
            typeof(MaterialMappingProfile),
            false
        );

        GUILayout.Space(6);
        EditorGUILayout.LabelField("2) Model (GameObject) to Read From", EditorStyles.boldLabel);
        _targetModel = (GameObject)EditorGUILayout.ObjectField(
            "Model GameObject",
            _targetModel,
            typeof(GameObject),
            true
        );

        GUILayout.Space(10);
        GUI.enabled = (_targetProfile != null && _targetModel != null);
        if (GUILayout.Button("Save Current Mapping"))
        {
            SaveMappingToProfile();
        }
        GUI.enabled = true;
    }

    private void SaveMappingToProfile()
    {
        if (_targetProfile == null || _targetModel == null) return;

        // 하위에 있는 모든 MeshRenderer와 SkinnedMeshRenderer를 모아서,
        // “루트 대비 상대 경로 + 공유된 머티리얼” 리스트를 생성
        List<MaterialMappingProfile.Entry> entries = new List<MaterialMappingProfile.Entry>();

        // MeshRenderer 대상
        foreach (var mr in _targetModel.GetComponentsInChildren<MeshRenderer>(true))
        {
            // 루트에서부터 상대 경로 추출
            string path = GetRelativePath(_targetModel.transform, mr.transform);

            // 첫 번째(0번) 머티리얼만 가져온다고 가정(여러 슬롯이 있으면 반복문으로 빼내도 됨)
            var mat = mr.sharedMaterial;
            if (mat != null)
            {
                entries.Add(new MaterialMappingProfile.Entry
                {
                    transformPath = path,
                    material = mat
                });
            }
        }

        // SkinnedMeshRenderer 대상
        foreach (var smr in _targetModel.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            string path = GetRelativePath(_targetModel.transform, smr.transform);
            var mat = smr.sharedMaterial; // SkinnedMeshRenderer도 sharedMaterial로 가져옴 (단일 슬롯이라 가정)
            if (mat != null)
            {
                entries.Add(new MaterialMappingProfile.Entry
                {
                    transformPath = path,
                    material = mat
                });
            }
        }

        // SO에 덮어쓰기
        Undo.RecordObject(_targetProfile, "Save Material Mapping");
        _targetProfile.mappings = entries.ToArray();
        EditorUtility.SetDirty(_targetProfile);
        AssetDatabase.SaveAssets();

        Debug.Log($"[MaterialMapping] Saved {entries.Count} entries into '{_targetProfile.name}'.");
    }

    // rootTransform에서부터 targetTransform까지의 상대 경로를 "A/B/C" 형태로 리턴
    private string GetRelativePath(Transform rootTransform, Transform targetTransform)
    {
        if (rootTransform == targetTransform)
            return "";
        List<string> parts = new List<string>();
        Transform cur = targetTransform;
        while (cur != null && cur != rootTransform)
        {
            parts.Add(cur.name);
            cur = cur.parent;
        }
        parts.Reverse();
        return string.Join("/", parts);
    }
}
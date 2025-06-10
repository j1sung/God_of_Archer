// Assets/Scripts/MaterialMapping/MaterialMappingProfile.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterialMappingProfile", menuName = "Material Mapping/Profile")]
public class MaterialMappingProfile : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        // “루트 모델 바로 아래에서부터 시작하는 경로”
        // 예) "Body/Torso", "Body/LeftArm/Shoulder" 같은 식으로 기록
        public string transformPath;

        // 적용할 머티리얼 에셋
        public Material material;
    }

    // 여러 개의 (경로 → 머티리얼) 정보
    public Entry[] mappings;
}
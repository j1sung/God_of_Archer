using UnityEngine;
/// <summary>
/// "개별 봉화 1개"의 상태와 동작만을 책임지는 코드이다.
/// 
public class BeaconController : MonoBehaviour
{
    public enum BeaconState { Active, Inactive, Ignited, Extinguished }
    public string beaconName;
    public BeaconState State { get; private set; } = BeaconState.Active;

    public bool CanIgnite => State == BeaconState.Active;
    public bool CanExtinguish => State == BeaconState.Ignited;

    // 점화
    public void Ignite()
    {
        if (!CanIgnite) return;
        State = BeaconState.Ignited;
        Debug.Log($"{beaconName} 봉화 점화됨(Ignited)!");
    }

    // 소화
    public void Extinguish()
    {
        if (!CanExtinguish) return;
        State = BeaconState.Extinguished;
        Debug.Log($"{beaconName} 봉화 소화됨(Extinguished)!");
    }

    // 다른 봉화가 점화됐을 때 비활성화
    public void SetInactive()
    {
        State = BeaconState.Inactive;
        Debug.Log($"{beaconName} 봉화 비활성화(Inactive)");
    }

    // 초기화
    public void ResetState()
    {
        State = BeaconState.Active;
    }
}
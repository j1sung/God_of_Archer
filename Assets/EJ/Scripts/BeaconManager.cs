using UnityEngine;

/// <summary>
/// "모든 봉화"를 한꺼번에 통합 관리하는 코드이다. 
/// </summary>
public class BeaconManager : MonoBehaviour
{
    public BeaconController beaconA;
    public BeaconController beaconB;

    // 봉화 점화 시도
    public void TryIgnite(BeaconController target)
    {
        // 이미 다른 봉화가 점화된 경우 아무 일도 안 함
        if ((beaconA.State == BeaconController.BeaconState.Ignited) ||
            (beaconB.State == BeaconController.BeaconState.Ignited))
            return;

        target.Ignite();

        // 다른 봉화 비활성화
        if (target == beaconA && beaconB != null)
            beaconB.SetInactive();
        else if (target == beaconB && beaconA != null)
            beaconA.SetInactive();
    }

    // 봉화 소화 시도
    public void TryExtinguish(BeaconController target)
    {
        target.Extinguish();
        // GameManager가 봉화 상태를 감지해서 승패 판정 처리
    }

    // 봉화 전체 초기화 (게임 시작/재시작)
    public void ResetAllBeacons()
    {
        if (beaconA != null) beaconA.ResetState();
        if (beaconB != null) beaconB.ResetState();
    }

    // 현재 점화된 봉화 반환 (게임 매니저에서 점화된 봉화를 체크하기 위함)
    public BeaconController GetIgnitedBeacon()
    {
        if (beaconA.State == BeaconController.BeaconState.Ignited) return beaconA;
        if (beaconB.State == BeaconController.BeaconState.Ignited) return beaconB;
        return null;
    }
}
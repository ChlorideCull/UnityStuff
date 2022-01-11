
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CalibratedSeat : UdonSharpBehaviour
{
    public Transform seatEdge;
    private VRC.SDK3.Components.VRCStation _station;
    private Vector3 _initialPosition;
    
    void Start()
    {
        _station = (VRC.SDK3.Components.VRCStation)GetComponent(typeof(VRC.SDK3.Components.VRCStation));
        _initialPosition = _station.stationEnterPlayerLocation.position;
    }

    public override void Interact()
    {
        Networking.SetOwner(Networking.LocalPlayer, _station.stationEnterPlayerLocation.gameObject);
        RecalculateSeatPosition();
        Networking.LocalPlayer.UseAttachedStation();
    }

    public override void OnStationEntered(VRCPlayerApi player)
    {
        /*if (!player.isLocal)
            return;
        Networking.SetOwner(player, _station.stationEnterPlayerLocation.gameObject);
        SendCustomEventDelayedSeconds(nameof(RecalculateSeatPosition), 0.5f);*/
    }

    public void RecalculateSeatPosition()
    {
        var lowerLegPos = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftLowerLeg);
        var upperLegPos = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftUpperLeg);
        if (lowerLegPos == Vector3.zero || upperLegPos == Vector3.zero)
        {
            Debug.Log("Not adjusting seat, since we can't get the leg bone positions");
            _station.stationEnterPlayerLocation.position = _initialPosition;
            return;
        }

        var upperLegLength = (upperLegPos - lowerLegPos).magnitude;
        
        var playerPos = Networking.LocalPlayer.GetPosition();
        
        var ySeatEdgeOffset = lowerLegPos.y - playerPos.y;
        var zSeatEdgeOffset = upperLegLength;

        var seatEdgePosition = seatEdge.position;
        var enterLocationScale = _station.stationEnterPlayerLocation.lossyScale;
        _station.stationEnterPlayerLocation.position = seatEdgePosition;
        _station.stationEnterPlayerLocation.Translate(0, -ySeatEdgeOffset*enterLocationScale.y, -zSeatEdgeOffset*enterLocationScale.z, Space.Self);
    }
}

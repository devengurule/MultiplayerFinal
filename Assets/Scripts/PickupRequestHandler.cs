using UnityEngine;
using Unity.Netcode;
using System;

public class PickupRequestHandler : NetworkBehaviour
{
    public static Action<PickupRequest> PickupRequestApproved;



    private void ApproveRequest(PickupRequest request)
    {
        PickupRequestApproved?.Invoke(request);
    }
}

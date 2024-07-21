using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    //movement
    public Vector3 direction;

    //bullet fuction
    public const int MOUSEBUTTON0 = 1;
    public const int MOUSEBUTTON1 = 2;
    public NetworkButtons buttons;
}

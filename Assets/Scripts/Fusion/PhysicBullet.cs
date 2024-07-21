using Fusion;
using UnityEngine;

public class PhysicBullet : NetworkBehaviour
{
    [Networked] TickTimer life { get; set; }

    public void Initial(Vector3 forward)
    {
        life = TickTimer.CreateFromSeconds(Runner, 5f);

        GetComponent<Rigidbody>().velocity = forward;
    }
    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}

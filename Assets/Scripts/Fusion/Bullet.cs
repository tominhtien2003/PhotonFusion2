using Fusion;

public class Bullet : NetworkBehaviour
{
    [Networked] TickTimer life { get; set; }
    
    public void Initial()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5f);
    }
    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            transform.position += 5 * transform.forward * Runner.DeltaTime;
        }
    }
}

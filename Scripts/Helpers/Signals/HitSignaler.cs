using Godot;

public class HitSignaler : Node
{
    [Signal] public delegate void OnHitRelay();
    
    public void OnHit(float damage)
    {
        EmitSignal(nameof(OnHitRelay), damage);
    }    
}
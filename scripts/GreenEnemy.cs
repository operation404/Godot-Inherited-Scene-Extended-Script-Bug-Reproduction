using Godot;

public partial class GreenEnemy : BaseEnemy
{
    public override void _Ready()
    {
        base._Ready();
        // GetNode<MoveComponent>("MoveComponent").Actor = this;
        GD.Print("GreenEnemy ready.");
    }
}

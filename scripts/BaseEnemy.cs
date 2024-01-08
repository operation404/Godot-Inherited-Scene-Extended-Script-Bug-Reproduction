using Godot;

public partial class BaseEnemy : Node2D
{
    public override void _Ready()
    {
        // GetNode<MoveComponent>("MoveComponent").Actor = this;
        GD.Print("BaseEnemy ready.");
    }
}

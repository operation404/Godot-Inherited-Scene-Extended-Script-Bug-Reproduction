using Godot;

[GlobalClass]
public partial class MoveComponent : Node
{
    [Export]
    public Node2D Actor;
    [Export]
    public Vector2 Velocity;

    public override void _Process(double delta)
    {
        Actor.Translate(Velocity * (float)delta);
    }
}


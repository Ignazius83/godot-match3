using Godot;
using System;

public class concrete_holder : Node2D
{
    [Signal]
    delegate void remove_concrete(Vector2 boardPosition);

    private ice[,] concrete_pieces;
    private int width = 8;
    private int height = 10;
    private PackedScene concrete = ResourceLoader.Load("res://scenes/concrete.tscn") as PackedScene;

    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
    private void _on_grid_damage_concrete(Vector2 boardPosition)
    {
        if (concrete_pieces ==  null) return;

        if (concrete_pieces[(int)boardPosition.x, (int)boardPosition.y] != null)
        {
            concrete_pieces[(int)boardPosition.x, (int)boardPosition.y].takeDamage(1);

            if (concrete_pieces[(int)boardPosition.x, (int)boardPosition.y].health <= 0)
            {
                concrete_pieces[(int)boardPosition.x, (int)boardPosition.y].QueueFree();
                concrete_pieces[(int)boardPosition.x, (int)boardPosition.y] = null;
                EmitSignal("remove_concrete", boardPosition);
            }

        }
    }

    private void _on_grid_make_concrete(Vector2 boardPosition)
    {
        if (concrete_pieces == null)
            concrete_pieces = new ice[width, height];
        var current = (ice)concrete.Instance();

        AddChild(current);
        current.Position = new Vector2(boardPosition.x * 64 + 64, -boardPosition.y * 64 + 800);
        concrete_pieces[(int)boardPosition.x, (int)boardPosition.y] = current;

    }
}

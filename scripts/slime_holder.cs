using Godot;
using System;

public class slime_holder : Node2D
{
    [Signal]
    delegate void remove_slime(Vector2 boardPosition);

    private ice[,] slime_pieces;
    private int width = 8;
    private int height = 10;
    private PackedScene slime = ResourceLoader.Load("res://scenes/slime.tscn") as PackedScene;

    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
    private void _on_grid_damage_slime(Vector2 boardPosition)
    {
        if (slime_pieces == null) return;

        if (slime_pieces[(int)boardPosition.x, (int)boardPosition.y] != null)
        {
            slime_pieces[(int)boardPosition.x, (int)boardPosition.y].takeDamage(1);

            if (slime_pieces[(int)boardPosition.x, (int)boardPosition.y].health <= 0)
            {
                slime_pieces[(int)boardPosition.x, (int)boardPosition.y].QueueFree();
                slime_pieces[(int)boardPosition.x, (int)boardPosition.y] = null;
                EmitSignal("remove_slime", boardPosition);
            }

        }
    }

    private void _on_grid_make_slime(Vector2 boardPosition)
    {
        if (slime_pieces == null)
            slime_pieces = new ice[width, height];
        var current = (ice)slime.Instance();

        AddChild(current);
        current.Position = new Vector2(boardPosition.x * 64 + 64, -boardPosition.y * 64 + 800);
        slime_pieces[(int)boardPosition.x, (int)boardPosition.y] = current;

    }
}

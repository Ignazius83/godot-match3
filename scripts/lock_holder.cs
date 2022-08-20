using Godot;
using System;

public class lock_holder : Node2D
{
    [Signal]
    delegate void remove_lock(Vector2 boardPosition);

    private ice[,] lock_pieces;
    private int width = 8;
    private int height = 10;
    private PackedScene licorice = ResourceLoader.Load("res://scenes/licorice.tscn") as PackedScene;

    public override void _Ready()
    {
        
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
    private void _on_grid_damage_lock(Vector2 boardPosition)
    {
        if (lock_pieces[(int)boardPosition.x, (int)boardPosition.y] != null)
        {
            lock_pieces[(int)boardPosition.x, (int)boardPosition.y].takeDamage(1);         
           
            if (lock_pieces[(int)boardPosition.x, (int)boardPosition.y].health <= 0)
            {
                lock_pieces[(int)boardPosition.x, (int)boardPosition.y].QueueFree();
                lock_pieces[(int)boardPosition.x, (int)boardPosition.y] = null;
                EmitSignal("remove_lock", boardPosition);
            }

        }
    }

    private void _on_grid_make_lock(Vector2 boardPosition)
    {
        if (lock_pieces == null)
            lock_pieces = new ice[width, height];
        var current = (ice)licorice.Instance();

        AddChild(current);
        current.Position = new Vector2(boardPosition.x * 64 + 64, -boardPosition.y * 64 + 800);       
        lock_pieces[(int)boardPosition.x, (int)boardPosition.y] = current;

    }
}

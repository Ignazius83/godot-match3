using Godot;
using System;

public class ice_holder : Node2D
{
    private ice[,] ice_pieces;
    private int width = 8;
    private int height = 10;
    private PackedScene ice = ResourceLoader.Load("res://scenes/ice.tscn") as PackedScene;

    public override void _Ready()
    {
        
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
    private void _on_grid_damage_ice(Vector2 boardPosition)
    {
      
        if (ice_pieces[(int)boardPosition.x, (int)boardPosition.y] != null)
        {
            ice_pieces[(int)boardPosition.x, (int)boardPosition.y].takeDamage(1);           
            if (ice_pieces[(int)boardPosition.x, (int)boardPosition.y].health <= 0)
            {
                ice_pieces[(int)boardPosition.x, (int)boardPosition.y].QueueFree();
                ice_pieces[(int)boardPosition.x, (int)boardPosition.y] = null;
            }
                
        }
    }

    private void _on_grid_make_ice(Vector2 boardPosition)
    {
        if (ice_pieces == null)
            ice_pieces = new ice[width, height];
        var current = (ice)ice.Instance();      

        AddChild(current);
        current.Position = new Vector2(boardPosition.x * 64 + 64, -boardPosition.y * 64 + 800);        
        ice_pieces[(int)boardPosition.x, (int)boardPosition.y] = current;

    }
}

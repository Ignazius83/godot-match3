using Godot;
using System;

public class LevelBackdrop : TextureRect
{
    [Signal] delegate void save_scroll_value();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

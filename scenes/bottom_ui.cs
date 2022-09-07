using Godot;
using System;

public class bottom_ui : TextureRect
{
    [Signal]
    delegate void pause_game();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    private void _on_Pause_pressed()
    {
        EmitSignal("pause_game");
        GetTree().Paused = true;
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

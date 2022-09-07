using Godot;
using System;

public class Pause : BaseMenuPanel
{
    private void _on_Quit_pressed()
    {
        GetTree().Quit();
    }

    private void _on_Continue_pressed()
    {
        GetTree().Paused = false;
        slide_out();
    }

    private void _on_bottom_ui_pause_game()
    {
        slide_in();
    }

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

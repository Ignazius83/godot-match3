using Godot;
using System;

public class MainMenuPanel : BaseMenuPanel
{
    [Signal]
    delegate void play_pressed();
    [Signal]
    delegate void setting_pressed();

    private void _on_Button1_pressed()
    {
        EmitSignal("play_pressed");

    }
    private void _on_Button2_pressed()
    {
        EmitSignal("setting_pressed");
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

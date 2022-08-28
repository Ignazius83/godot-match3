using Godot;
using System;

public class SettingsPanel : BaseMenuPanel
{
    [Signal]
    delegate void sound_change();
    [Signal]
    delegate void back_button();

    private void _on_Button1_pressed()
    {
        EmitSignal("sound_change");

    }
    private void _on_Button2_pressed()
    {
        EmitSignal("back_button");
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

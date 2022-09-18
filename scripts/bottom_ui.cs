using Godot;
using System;

public class bottom_ui : TextureRect
{
    [Signal]
    delegate void pause_game();
    [Signal] delegate void booster(string booster_type);

    private BoosterInfo boosterInfo;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        boosterInfo = GetNode<BoosterInfo>("/root/BoosterInfo");
    }

    private void _on_Booster_1_pressed()
    {
        EmitSignal("booster", GetNode<Booster>("MarginContainer/HBoxContainer/Booster 1").booster_type);
    }
    private void _on_Booster_2_pressed()
    {
        EmitSignal("booster", GetNode<Booster>("MarginContainer/HBoxContainer/Booster 2").booster_type);
    }
    private void _on_Booster_3_pressed()
    {
        EmitSignal("booster", GetNode<Booster>("MarginContainer/HBoxContainer/Booster 3").booster_type);
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

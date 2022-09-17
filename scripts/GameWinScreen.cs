using Godot;
using System;

public class GameWinScreen : BaseMenuPanel
{
    private bool is_aut;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    private void _on_goal_holder_game_won()
    {
        if (!is_aut)
        {
            is_aut = true;
            slide_in();
        }
    }
   
    private void _on_ContunueButton_pressed()
    {
        GetTree().ChangeScene("res://scenes/LevelSelectScene.tscn");
    }
}

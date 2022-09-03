using Godot;
using System;

public class GameWinScreen : BaseMenuPanel
{   

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    private void _on_goal_holder_game_won()
    {
        slide_in();
    }
   
    private void _on_ContunueButton_pressed()
    {
        GetTree().ReloadCurrentScene();
    }
}

using Godot;
using System;

public class GameOverPanel : BaseMenuPanel
{
    private void _on_GameManager_game_lost()
    {
        slide_in();
    }
    private void _on_QuitButton_pressed ()
    {
        GetTree().ChangeScene("res://scenes/LevelSelectScene.tscn");
    }
    private void _on_Restart_pressed()
    {
        GetTree().ReloadCurrentScene();
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

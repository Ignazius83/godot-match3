using Godot;
using System;

public class GameWinScreen : BaseMenuPanel
{
    private bool is_aut;
    private Label scoreLabel;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        scoreLabel = GetNode<Label>("MarginContainer/TextureRect/VBoxContainer/Label");


    }

    private void _on_GameManager_game_won(int scoreToDisplay)
    {
        scoreLabel.Text = scoreToDisplay.ToString();
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

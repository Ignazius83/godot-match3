using Godot;
using Godot.Collections;
using System;

public class LevelButton : Node2D
{
    [Export] private int level;
    [Export] private string levelToLoad;
    [Export] private bool enabled;
    [Export] private bool scoreGoalMeet;

    [Export] private Texture blockedTexture;
    [Export] private Texture openTexture;
    [Export] private Texture goalMeetTexture;
    [Export] private Texture goalNotMeetTexture;

    private Label levelLabel;
    private TextureButton button;
    private Sprite star;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        levelLabel = GetNode<Label>("TextureButton/Label");
        button = GetNode<TextureButton>("TextureButton");
        star = GetNode<Sprite>("Sprite");
        var gameDataManager = GetNode<GameDataManager>("/root/GameDataManager");
        if (gameDataManager.levelInfo.Contains(level))
            enabled = (bool)((Dictionary)gameDataManager.levelInfo[level])["unlocked"];
        else
            enabled = false;
        setup();
    }
    private void _on_TextureButton_pressed()
    {
        if (enabled)
           GetTree().ChangeScene(levelToLoad);

    }
    private void setup()
    {
        levelLabel.Text = level.ToString();
        if (enabled)
            button.TextureNormal = openTexture;
        else
            button.TextureNormal = blockedTexture;
        if (scoreGoalMeet)
            star.Texture = goalMeetTexture;
        else
            star.Texture = goalNotMeetTexture;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

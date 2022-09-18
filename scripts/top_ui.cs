using Godot;
using Godot.Collections;
using System;

public class top_ui : TextureRect
{
    private Label scoreLabel;
    private Label counterLabel;
    private TextureProgress scoreBar;
    private HBoxContainer goalContainer;
    private int currentScore = 0;
    private int currentCount = 0;
    private int max_counter = 0;
    [Export] private int currentLevel;
    [Export] private PackedScene goalPrefab;
    private GameDataManager gameDataManager;

    [Signal] delegate void notify_of_level(int curentLevel);
    public top_ui()
    {
        
    }

    private void _on_ice_holder_break_ice(string goal_type)
    {
        for (var i = 0; i < goalContainer.GetChildCount(); i++)
            goalContainer.GetChild<GoalPrefab>(i).updateGoalValues(goal_type);
    }

    private void _on_grid_check_goal(string goal_type)
    {
        for (var i = 0; i < goalContainer.GetChildCount(); i++)
            goalContainer.GetChild<GoalPrefab>(i).updateGoalValues(goal_type);

    }

    private void _on_goal_holder_create_goal(int newmax, Texture newtexture, string newvalue)
    {
        makeGoal(newmax, newtexture, newvalue);
    }
    private void _on_grid_setup_max_score(int maxScore)
    {
        setupScoreBar(maxScore);
    }

    private void _on_grid_set_max_counter(int new_max_counter)
    {
        max_counter = new_max_counter;
    }
    private void _on_grid_update_counter(int amauntToChange)
    {
        currentCount += amauntToChange;
        if (currentCount > max_counter)
            currentCount = max_counter;
        counterLabel.Text = currentCount.ToString();
        GD.Print(amauntToChange);
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        scoreLabel = GetNode("MarginContainer/HBoxContainer/VBoxContainer/ScoreLabel") as Label;
        counterLabel = GetNode("MarginContainer/HBoxContainer/CounterLabel") as Label;
        scoreBar = GetNode("MarginContainer/HBoxContainer/VBoxContainer/TextureProgress") as TextureProgress;
        goalContainer = GetNode<HBoxContainer>("MarginContainer/HBoxContainer/HBoxContainer");
        gameDataManager = GetNode<GameDataManager>("/root/GameDataManager");
        EmitSignal("notify_of_level", currentLevel);
        _on_grid_update_score(currentScore);
    }
    private void _on_grid_update_score(int amauntToChange)
    {
        currentScore += amauntToChange;
        updateScoreBar();
        scoreLabel.Text = currentScore.ToString();
        ((Dictionary)gameDataManager.levelInfo[currentLevel])["high_score"] = currentScore;
         if (currentScore>=scoreBar.MaxValue)
           ((Dictionary)gameDataManager.levelInfo[currentLevel])["stars_unlocked"] = 1;


        GD.Print(gameDataManager.levelInfo[currentLevel]);

    }

    private void setupScoreBar(int maxScore)
    {
        scoreBar.MaxValue = maxScore;
    }
    private void makeGoal(int newmax, Texture newtexture, string newvalue)
    {
        var current = goalPrefab.Instance<GoalPrefab>();
        goalContainer.AddChild(current);
        current.setGoalValues(newmax, newtexture, newvalue);

    }

    private void updateScoreBar()
    {
        scoreBar.Value = currentScore;
    }
}

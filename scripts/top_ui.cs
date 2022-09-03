using Godot;
using System;

public class top_ui : TextureRect
{
    private Label scoreLabel;
    private Label counterLabel;
    private TextureProgress scoreBar;
    private HBoxContainer goalContainer;
    private int currentScore = 0;
    private int currentCount = 0;
    [Export] private PackedScene goalPrefab;
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
    private void _on_grid_update_counter(int amauntToChange)
    {
        currentCount += amauntToChange;
        counterLabel.Text = currentCount.ToString();
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        scoreLabel = GetNode("MarginContainer/HBoxContainer/VBoxContainer/ScoreLabel") as Label;
        counterLabel = GetNode("MarginContainer/HBoxContainer/CounterLabel") as Label;
        scoreBar = GetNode("MarginContainer/HBoxContainer/VBoxContainer/TextureProgress") as TextureProgress;
        goalContainer = GetNode<HBoxContainer>("MarginContainer/HBoxContainer/HBoxContainer");
        _on_grid_update_score(currentScore);
    }
    private void _on_grid_update_score(int amauntToChange)
    {
        currentScore += amauntToChange;
        updateScoreBar();
        scoreLabel.Text = currentScore.ToString();
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

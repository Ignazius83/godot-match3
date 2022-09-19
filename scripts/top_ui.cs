using Godot;
using Godot.Collections;
using System;

public class top_ui : TextureRect
{
    private Label scoreLabel;
    private Label counterLabel;
    private TextureProgress scoreBar;
    private HBoxContainer goalContainer;    
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

    private void _on_GameManager_create_goal(int newmax, Texture newtexture, string newvalue)
    {
        makeGoal(newmax, newtexture, newvalue);
    } 

    private void makeGoal(int newmax, Texture newtexture, string newvalue)
    {
        if (goalContainer == null)
            goalContainer = GetNode<HBoxContainer>("MarginContainer/HBoxContainer/HBoxContainer");

        var current = goalPrefab.Instance<GoalPrefab>();
        goalContainer.AddChild(current);
        current.setGoalValues(newmax, newtexture, newvalue);

    }

    private void _on_GameManager_set_counter_info(int current_counter)
    {
        if (counterLabel == null)
            counterLabel = GetNode<Label>("MarginContainer/HBoxContainer/CounterLabel");
        counterLabel.Text = current_counter.ToString();
    }

    private void _on_GameManager_set_score_info(int max_score, int current_score)
    {
        if (scoreLabel == null)
            scoreLabel = GetNode<Label>("MarginContainer/HBoxContainer/VBoxContainer/ScoreLabel");
        if (scoreBar == null)
            scoreBar = GetNode<TextureProgress>("MarginContainer/HBoxContainer/VBoxContainer/TextureProgress");
      
        scoreBar.MaxValue = max_score;
        scoreBar.Value = current_score;
        scoreLabel.Text = current_score.ToString();
    }

  
}

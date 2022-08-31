using Godot;
using System;

public class top_ui : TextureRect
{
    private Label scoreLabel;
    private Label counterLabel;
    private int currentScore = 0;
    private int currentCount = 0;
    public top_ui()
    {
        
    }
    private void _on_grid_update_counter(int amauntToChange)
    {
        currentCount += amauntToChange;
        counterLabel.Text = currentCount.ToString();
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        scoreLabel = GetNode("MarginContainer/HBoxContainer/ScoreLabel") as Label;
        counterLabel = GetNode("MarginContainer/HBoxContainer/CounterLabel") as Label;
        _on_grid_update_score(currentScore);
    }
    private void _on_grid_update_score(int amauntToChange)
    {
        currentScore += amauntToChange;
        scoreLabel.Text = currentScore.ToString();
    }
}

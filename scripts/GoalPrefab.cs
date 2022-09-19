using Godot;
using System;

public class GoalPrefab : TextureRect
{
    private int currentNumber;
    private int maxValue;
    private string goalValue;
    private Texture goalTexture;
    private Label goalLabel;
    private TextureRect this_texture;
  

    public void setGoalValues(int newmax, Texture newtexture, string newvalue)
    {
        if (goalLabel == null)
            goalLabel = GetNode<Label>("VBoxContainer/Label");
        if (this_texture == null)
            this_texture = GetNode<TextureRect>("VBoxContainer/TextureRect");

        this_texture.Texture = newtexture;
        maxValue = newmax;
        goalValue = newvalue;
        goalLabel.Text = $"{currentNumber} / {maxValue}";

    }

    public void updateGoalValues(string goalType)
    {
        if (goalType == goalValue)
        {
            currentNumber += 1;
            if (currentNumber <= maxValue)
                goalLabel.Text = $"{currentNumber} / {maxValue}";
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

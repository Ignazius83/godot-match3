using Godot;
using System;

public class Goal : Node2D
{
    [Export] public Texture goalTexture { get; set; }
    [Export] public int max_needed { get; set; }
    [Export] public string goal_string { get; set; }
    private int numberCollected = 0;
    public bool goalmeet = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void checkGoal( string goal_type)
    {
        if (goal_type == goal_string)
           updateGoal();
       
    }

    public void updateGoal()
    {
        if (numberCollected < max_needed)
            numberCollected += 1;
        if (numberCollected == max_needed)
        {
            if (!goalmeet)
                goalmeet = true;
        }
           
    }
       


}

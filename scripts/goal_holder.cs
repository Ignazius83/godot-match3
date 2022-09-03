using Godot;
using System;

public class goal_holder : Node2D
{
   [Signal] delegate void create_goal(int newmax, Texture newtexture, string newvalue);
   [Signal] delegate void game_won();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        createGoals();
    }

    private void createGoals()
    {
        for (int i = 0; i < GetChildCount(); i++)
        {
            var current = GetChild<Goal>(i);
            EmitSignal("create_goal", current.max_needed,current.goalTexture,current.goal_string);
        }
    }

    private bool goalsMeet()
    {
        for (int i = 0; i < GetChildCount(); i++)        
            if (!GetChild<Goal>(i).goalmeet)
                return false;
        return true;
    }

    private void checkGameWin()
    {
        if (goalsMeet())
            EmitSignal("game_won");
    }


    private void checkGoals(string goal_type)
    {
        for (int i = 0; i < GetChildCount(); i++)
            ((Goal)GetChild(i)).checkGoal(goal_type);
        checkGameWin();
    }


    private void _on_grid_check_goal(string goal_type)
    {
        checkGoals(goal_type);
    }

    private void _on_ice_holder_break_ice(string goal_type)
    {
        checkGoals(goal_type);
    }


    }

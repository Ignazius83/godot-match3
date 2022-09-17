using Godot;
using Godot.Collections;
using System;

public class goal_holder : Node2D
{
   [Signal] delegate void create_goal(int newmax, Texture newtexture, string newvalue);
   [Signal] delegate void game_won();
    private GameDataManager gameDataManager;
    private int current_level = 0;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        gameDataManager = GetNode<GameDataManager>("/root/GameDataManager");
        createGoals();
    }

    private void _on_top_ui_notify_of_level(int curentLevel)
    {
        current_level = curentLevel;
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
        {
            EmitSignal("game_won");
            gameDataManager.levelInfo.Add(current_level + 1, new Dictionary
            {
                ["unlocked"] = true,
                ["high_score"] = 0,
                ["stars_unlocked"] = 0
            });

        }
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

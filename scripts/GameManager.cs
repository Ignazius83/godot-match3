using Godot;
using Godot.Collections;
using System;

public class GameManager : Node
{
    //grid
    [Export] private int width;
    [Export] private int height;

    //level
    [Export] private int level;
    [Export] private bool is_moves;
    [Export] private int max_counter;
    private int current_counter;

    //score
    private int current_high_score;
    private int current_score;
    [Export] private int max_score;
    [Export] private int points_per_piece;

    //goals
    private Node2D _goal_holder;

    //signals
    [Signal]
    delegate void set_dimentions(int width, int height);
    [Signal]
    delegate void set_score_info(int max_score, int current_score);
    [Signal]
    delegate void set_counter_info(int current_counter);
    [Signal] delegate void create_goal(int newmax, Texture newtexture, string newvalue);
    [Signal] delegate void game_won();

    private GameDataManager dataManager;
    private bool game_Won = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
         dataManager  = GetNode<GameDataManager>("/root/GameDataManager");
        _goal_holder = GetNode<Node2D>("goal_holder");
        setup();
    }

    private void setup()
    {
        if (!is_moves)
            GetNode<Timer>("MoveTimer").Start();
        current_counter = max_counter;
        current_score = 0;
        if (dataManager.levelInfo.Contains(level))            
            current_high_score = (int)((Dictionary)dataManager.levelInfo[level])["high_score"];

        EmitSignal("set_score_info", max_score, current_score);
        EmitSignal("set_dimentions", width, height);
        EmitSignal("set_counter_info", current_counter);
        createGoals();

    }

    private void createGoals()
    {
        for (int i = 0; i < _goal_holder.GetChildCount(); i++)
        {
            var current = _goal_holder.GetChild<Goal>(i);
            EmitSignal("create_goal", current.max_needed, current.goalTexture, current.goal_string);
        }
    }

    private bool goalsMeet()
    {
        for (int i = 0; i < _goal_holder.GetChildCount(); i++)
            if (!_goal_holder.GetChild<Goal>(i).goalmeet)
                return false;
        return true;
    }

    private void checkGameWin()
    {
        if (goalsMeet())
        {
            EmitSignal("game_won");
            if (!dataManager.levelInfo.Contains(level + 1))
               dataManager.levelInfo.Add(level + 1, new Dictionary
              {
                ["unlocked"] = true,
                ["high_score"] = 0,
                ["stars_unlocked"] = 0
              });
            game_Won = true;

        }
    }
    private void _on_grid_check_goal(string goal_type)
    {
        checkGoals(goal_type);
    }

    private void _on_ice_holder_break_ice(string goal_type)
    {
        checkGoals(goal_type);
    }

    private void checkGoals(string goal_type)
    {
        for (int i = 0; i < _goal_holder.GetChildCount(); i++)
            ((Goal)_goal_holder.GetChild(i)).checkGoal(goal_type);
        checkGameWin();
    }

    private void _on_grid_update_score(int streak)
    {
        current_score += streak * points_per_piece;
        EmitSignal("set_score_info", max_score, current_score);
    }

    private void _on_grid_update_counter()
    {
        if (is_moves)
        {
            current_counter -= 1;
            if (current_counter < 0)
                current_counter = 0;
            EmitSignal("set_counter_info", current_counter);
        }
    }

    private void _on_MoveTimer_timeout()
    {
        if (!is_moves && !game_Won)
        {
            current_counter -= 1;
            if (current_counter < 0)
                current_counter = 0;
            EmitSignal("set_counter_info", current_counter);
        }
    }


}

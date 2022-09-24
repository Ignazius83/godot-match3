using Godot;
using Godot.Collections;
using System;

public class GameManager : Node2D
{
    //grid
    [Export] private int width;
    [Export] private int height;
    private bool boardStable = true;

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

    [Signal]
    delegate void screen_fade_in();
    [Signal]
    delegate void screen_fade_out();

    [Signal] delegate void create_goal(int newmax, Texture newtexture, string newvalue);
   
    [Signal] delegate void game_won(int scoreToDisplay);
    [Signal] delegate void game_lost();

    private GameDataManager dataManager;
    private bool game_Won = false;
    private bool game_Lost = false;
    [Signal] delegate void grid_change_move();

    private bool boosterActive = false;
    private string currentBooster = "";
    [Signal] delegate void color_bomb(Vector2 position);
    [Signal] delegate void destroy_piece(Vector2 position);
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
         dataManager  = GetNode<GameDataManager>("/root/GameDataManager");
        _goal_holder = GetNode<Node2D>("goal_holder");
        setup();
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("ui_touch") && currentBooster != "")
            booster_input();
    }

    private void booster_input()
    {
        if (currentBooster == "ColorBomb")
            EmitSignal("color_bomb", GetGlobalMousePosition());
        else if (currentBooster == "DestroyPiece")
            EmitSignal("destroy_piece", GetGlobalMousePosition());

        else if (currentBooster == "AddCounter")
        {
            var temp = GetGlobalMousePosition();

            if (temp.x > 20 && temp.x < 556)
                if (temp.y > 200 && temp.y < 1000)
                {
                    current_counter += 10;
                    if (current_counter > max_counter)
                        current_counter = max_counter;
                    EmitSignal("set_counter_info", current_counter);
                    _on_bottom_ui_booster(currentBooster);
                }                   
        }
       
    }
    private void changeBoardState()
    {
        boardStable = !boardStable;
        checkGameWin();
    }
    private void resetBooster()
    {
        currentBooster = "";
        EmitSignal("screen_fade_out");
        boosterActive = false;
    }

    private void _on_grid_reset_booster()
    {
        resetBooster();
    }
    private void _on_grid_change_move_state()
    {        
        changeBoardState();
    }   

    private void _on_bottom_ui_booster(string booster_type)
    {
        GD.Print(booster_type);
        if (boosterActive && boardStable)
        {
            currentBooster = "";
            EmitSignal("screen_fade_out");
            EmitSignal("grid_change_move");
            boosterActive = false;
        }
        else if (!boosterActive && boardStable)
        {
            currentBooster = booster_type;
            EmitSignal("screen_fade_in");
            EmitSignal("grid_change_move");
            boosterActive = true;
           
        }
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
        if (goalsMeet() && boardStable)
        {
            EmitSignal("game_won",current_score);
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

    private void updateCounter()
    {
        if (!boosterActive)
        {
            current_counter -= 1;
            if (current_counter <= 0)
            {
                current_counter = 0;
                if (!game_Lost && boardStable)
                {
                    EmitSignal("game_lost");
                    game_Lost = true;
                    GetNode<Timer>("MoveTimer").Stop();
                }
            }
            EmitSignal("set_counter_info", current_counter);
        }
    }

    private void _on_grid_update_counter()
    {
        if (is_moves)
            updateCounter();
    }

    private void _on_MoveTimer_timeout()
    {
        if (!is_moves && !game_Won)
            updateCounter();
    }


}

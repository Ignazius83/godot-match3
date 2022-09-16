using Godot;
using System;

public class GameMenu : Control
{
    private MainMenuPanel main;
    private SettingsPanel setting;
    [Signal] delegate void read_sound();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        main = GetNode("Main") as MainMenuPanel;
        setting = GetNode("Settings") as SettingsPanel;
        main.slide_in();
    }
   
    private void _on_Settings_sound_change()
    {
      
      //  config.saveConfig();
    }
    private void _on_Main_setting_pressed()
    {
        EmitSignal("read_sound");
        main.slide_out();
        setting.slide_in();
    }

    private void _on_Settings_back_button()
    {
        main.slide_in();
        setting.slide_out();
    }

    private void _on_Main_play_pressed()
    {
        GetTree().ChangeScene("res://scenes/LevelSelectScene.tscn");
    }



    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}

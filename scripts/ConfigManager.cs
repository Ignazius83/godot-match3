using Godot;
using System;

public class ConfigManager : Node
{
    private readonly string path = "user://config.ini";

    public  bool sound_on = true;
    public  bool music_on = true;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        loadConfig();
    }

    public void saveConfig()
    {
        var config = new ConfigFile();
        config.SetValue("audio", "sound", sound_on);
        config.SetValue("audio", "music", music_on);
        var err = config.Save(path);
        if (err != Error.Ok)
            GD.Print("error save");
    }
    public void loadConfig()
    {
        var config = new ConfigFile();
        var default_options = new {
            music = true,
            sound = true
        };     
        var err = config.Load(path);
        if (err != Error.Ok)           
              return;

        music_on = (bool)config.GetValue("audio", "music", default_options.music);
        sound_on = (bool)config.GetValue("audio", "sound", default_options.sound);
        
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}

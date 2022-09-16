using Godot;
using System;

public class SettingsPanel : BaseMenuPanel
{
    [Signal]
    delegate void sound_change();
    [Signal]
    delegate void back_button();

    [Export] private Texture sound_on;
    [Export] private Texture sound_off;

    private ConfigManager config;
    private SoundManager soundManager;


    private void _on_Button1_pressed()
    {       
        config.sound_on = !config.sound_on;
        changesoundTexture();
        config.saveConfig();
        soundManager.setVolume();
        soundManager.playFixedSound(0);
        EmitSignal("sound_change");

    }
    private void _on_GameMenu_read_sound()
    {
        changesoundTexture();
    }
    private void  changesoundTexture()
    {
        if (config.sound_on)
            GetNode<TextureButton>("MarginContainer/GraphicAndButtons/Buttons/Button1").TextureNormal = sound_on;
        else
            GetNode<TextureButton>("MarginContainer/GraphicAndButtons/Buttons/Button1").TextureNormal = sound_off;

    }
    private void _on_Button2_pressed()
    {
        soundManager.playFixedSound(0);
        EmitSignal("back_button");
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        config = (ConfigManager)GetNode("/root/ConfigManager");
        soundManager = GetNode<SoundManager>("/root/SoundManager");


    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

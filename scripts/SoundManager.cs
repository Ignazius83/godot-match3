using Godot;
using System;

public class SoundManager : Node
{
    private AudioStreamPlayer musicPlayer;
    private ConfigManager configManager;
    private AudioStreamPlayer soundPlayer;
    private AudioStream[] posible_music= new AudioStream[]{
          ResourceLoader.Load<AudioStream>("res://Music/theme-1.ogg"),
          ResourceLoader.Load<AudioStream>("res://Music/theme-2.ogg"),
          ResourceLoader.Load<AudioStream>("res://Music/theme-3.ogg"),
          ResourceLoader.Load<AudioStream>("res://Music/theme-4.ogg"),
        };
    private AudioStream[] posible_sound = new AudioStream[]{
          ResourceLoader.Load<AudioStream>("res://Sounds/1.ogg"),
          ResourceLoader.Load<AudioStream>("res://Sounds/3.ogg"),
          ResourceLoader.Load<AudioStream>("res://Sounds/4.ogg"),
          ResourceLoader.Load<AudioStream>("res://Sounds/5.ogg"),
        };

    private void playRandomTheam()
    {
        var temp = (int)Math.Floor(GD.RandRange(0, posible_music.Length));
        musicPlayer.Stream = posible_music[temp];
        musicPlayer.Play();
    }

    public void playRandomSound()
    {
        var temp = (int)Math.Floor(GD.RandRange(0, posible_sound.Length));
        soundPlayer.Stream = posible_sound[temp];      
        soundPlayer.Play();
    }
    public void playFixedSound(int sound)
    {       
        soundPlayer.Stream = posible_sound[sound];       
        soundPlayer.Play();
    }
    public override void _Ready()
    {
        musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
        soundPlayer = GetNode<AudioStreamPlayer>("SoundPlayer");
        configManager = GetNode<ConfigManager>("/root/ConfigManager");
        setVolume();
        GD.Randomize();
        playRandomTheam();
    }

    public void setVolume()
    {
      if (configManager.sound_on)
      {
            musicPlayer.VolumeDb = -15;
            soundPlayer.VolumeDb = -15;
        }
      else
      {
            musicPlayer.VolumeDb = -80;
            soundPlayer.VolumeDb = -80;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

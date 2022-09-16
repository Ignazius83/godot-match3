using Godot;
using System;

public class MusicPlayer : Node
{
    private SoundManager soundManager;

    private void _on_grid_play_sound()
    {
        soundManager.playRandomSound();
    }
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        soundManager = GetNode<SoundManager>("/root/SoundManager");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

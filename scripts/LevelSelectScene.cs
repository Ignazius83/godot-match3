using Godot;
using System;

public class LevelSelectScene : CanvasLayer
{
    private void _on_TextureButton_pressed()
    {
        GetTree().ChangeScene("res://scenes/GameMenu.tscn");
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

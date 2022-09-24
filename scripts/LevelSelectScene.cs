using Godot;
using System;

public class LevelSelectScene : CanvasLayer
{
   
    private void _on_TextureButton_pressed()
    {
        GetTree().ChangeScene("res://scenes/GameMenu.tscn");
    }

    private void _on_LevelBackdrop_save_scroll_value()
    {
        var currentScrollValue = GetNode<ScrollContainer>("ScrollContainer").ScrollVertical;
        GetNode<GameDataManager>("/root/GameDataManager").changeLevelScrollValue(currentScrollValue);
    }
    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        await ToSignal(GetTree(), "idle_frame");
        GetNode<ScrollContainer>("ScrollContainer").ScrollVertical = GetNode<GameDataManager>("/root/GameDataManager").levelScrollValue;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

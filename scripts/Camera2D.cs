using Godot;
using System;

public class Camera2D : Godot.Camera2D
{
    private Tween screen_kick;

    private void moveCamera(Vector2 placement)
    {
        Offset = placement;        
    }

    private void cameraEffect()
    {
        screen_kick.InterpolateProperty(this, "zoom", new Vector2(0.9f, 0.9f), new Vector2(1, 1), 0.2f, Tween.TransitionType.Back, Tween.EaseType.InOut);
        screen_kick.Start();
    }
    
    private void _on_grid_place_camera(Vector2 placement)
    {
        moveCamera(placement);
    }

    private void _on_grid_camera_effect()
    {
        cameraEffect();
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        screen_kick = GetNode<Tween>("ScreenKick");


    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

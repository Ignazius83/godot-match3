using Godot;
using System;

public class DestroyParticle : Particles2D
{
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Emitting = true;

    }

    private void _on_Timer_timeout()
    {
        QueueFree();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}

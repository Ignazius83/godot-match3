using Godot;
using System;

public class ice : Node2D
{
    [Export] public int health;

    public override void _Ready()
    {
        
    }

    public void takeDamage(int damage)
    {
        health -= damage;
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

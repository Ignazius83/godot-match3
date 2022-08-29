using Godot;
using System;

public class AnimationExplosion : Node2D
{
    private void _on_AnimatedSprite_animation_finished()
    {
        QueueFree();
    }
    public override void _Ready()
    {
        var sprite = GetNode("AnimatedSprite") as AnimatedSprite;
        sprite.Playing = true;        
    }
}

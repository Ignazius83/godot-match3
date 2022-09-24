using Godot;
using System;

public class Screen : TextureRect
{

    private Tween fadeTween;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        fadeTween = GetNode<Tween>("FadeTween");
        Modulate = new Color(1, 1, 1, 0);
    }


    public void fade_in()
    {
        fadeTween.InterpolateProperty(this, "modulate", new Color(1, 1, 1, 0),
            new Color(1, 1, 1, 1), 0.3f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
        fadeTween.Start();
    }

    public void fade_out()
    {
        fadeTween.InterpolateProperty(this, "modulate", new Color(1, 1, 1, 1),
            new Color(1, 1, 1, 0), 0.3f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
        fadeTween.Start();
    }

    private void _on_GameManager_screen_fade_in()
    {
        fadeTween.StopAll();
        fade_in();
    }
    private void _on_GameManager_screen_fade_out()
    {
        fadeTween.StopAll();
        fade_out();
    }

}

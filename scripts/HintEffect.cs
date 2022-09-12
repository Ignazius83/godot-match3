using Godot;
using System;

public class HintEffect : Node2D
{
    private Sprite _sprite;
    private Tween _sizeTween;
    private Tween _colorTwin;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _sprite =  GetNode<Sprite>("Sprite");
        _sizeTween = GetNode<Tween>("SizeTween");
        _colorTwin = GetNode<Tween>("ColorTween");
        setup(_sprite.Texture);
    }
    public void setup(Texture sprite_texture)
    {
        _sprite.Texture = sprite_texture;
        slowlyLarger();
        slowlyDimmer();

    }
    private void _on_SizeTween_tween_completed(Godot.Object @object,NodePath key)
    {
        slowlyLarger();

    }
    private void _on_ColorTween_tween_completed(Godot.Object @object, NodePath key)
    {
        slowlyDimmer();
    }


    private void slowlyLarger()
    {
        _sizeTween.InterpolateProperty(_sprite, "scale", new Vector2(0.5f, 0.5f), new Vector2(1f, 1f), 2.0f, Tween.TransitionType.Sine, Tween.EaseType.Out);
        _sizeTween.Start();
    }
    private void slowlyDimmer()
    {
        _colorTwin.InterpolateProperty(_sprite, "modulate", new Color(1,1,1,1), new Color(1, 1, 1, 0.2f), 2.0f, Tween.TransitionType.Sine, Tween.EaseType.Out);
        _colorTwin.Start();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}

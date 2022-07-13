using Godot;
using static Godot.Tween;

public class Piece : Node2D
{   
	[Export] public string color;
	private Tween  moveTween;
	public bool matched = false;
	public override void _Ready()
	{
		moveTween = GetNode("moveTween") as Tween;
	}
	
	public void move(Vector2 target)
	{
		moveTween.InterpolateProperty(this, "position", Position, target, .3f, TransitionType.Elastic, EaseType.Out);
		moveTween.Start();
	}

	public void dim()
	{
		var sprite = GetNode("Sprite") as Sprite;
		sprite.Modulate = new Color(1, 1, 1, .5f);

	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

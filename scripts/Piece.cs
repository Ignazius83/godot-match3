using Godot;
using static Godot.Tween;

public class Piece : Node2D
{   
	[Export] public string color;
	private Tween  moveTween;
	public override void _Ready()
	{
		moveTween = GetNode("moveTween") as Tween;
	}
	
	public void move(Vector2 target)
	{
		moveTween.InterpolateProperty(this, "position", Position, target, .3f, TransitionType.Elastic, EaseType.Out);
		moveTween.Start();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

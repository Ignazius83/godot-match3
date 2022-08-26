using Godot;
using static Godot.Tween;

public class Piece : Node2D
{   
	[Export] public string color;
	[Export] private Texture rowTexture;
	[Export] private Texture colTexture;
	[Export] private Texture adjacentTexture;
	[Export] private Texture colorBombTexture;

	public bool isRowBomb { get; set; } = false;
	public bool isColBomb { get; set; } = false;
	public bool isAdjacentBomb { get; set; } = false;
	public bool isColorBomb { get; set; } = false;

	private Tween  moveTween;
	private Sprite sprite;
	public bool matched = false;
	public override void _Ready()
	{
		moveTween = GetNode("moveTween") as Tween;
		sprite = GetNode("Sprite") as Sprite;
	}
	
	public void move(Vector2 target)
	{
		moveTween.InterpolateProperty(this, "position", Position, target, .3f, TransitionType.Elastic, EaseType.Out);
		moveTween.Start();
	}

	public void makeColumnBomb()
    {
		isColBomb = true;
		sprite.Texture = colTexture;
		sprite.Modulate = new Color(1, 1, 1);
    }
	public void makeRowBomb()
	{
		isRowBomb = true;
		sprite.Texture = rowTexture;
		sprite.Modulate = new Color(1, 1, 1);
	}
	public void makeAdjacentBomb()
	{
		isAdjacentBomb = true;
		sprite.Texture = adjacentTexture;
		sprite.Modulate = new Color(1, 1, 1);
	}

	public void makeColorBomb()
	{
		isColorBomb = true;
		sprite.Texture = colorBombTexture;
		sprite.Modulate = new Color(1, 1, 1);
		color = "Color";
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

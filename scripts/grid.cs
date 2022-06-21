using Godot;
using System;

public class grid : Node2D
{
	[Export] private int width;
	[Export] private int height;
	[Export] private int x_start;
	[Export] private int y_start;
	[Export] private int offset;

	private PackedScene[]possible_pieces = new PackedScene[]
	{
		ResourceLoader.Load("res://scenes/blue_piece.tscn") as PackedScene,
		ResourceLoader.Load("res://scenes/green_piece.tscn") as PackedScene,
		ResourceLoader.Load("res://scenes/pink_piece.tscn") as PackedScene,
		ResourceLoader.Load("res://scenes/orange_piece.tscn") as PackedScene,
		ResourceLoader.Load("res://scenes/light_green_piece.tscn") as PackedScene,
		ResourceLoader.Load("res://scenes/yellow_piece.tscn") as PackedScene,

	};
	private Piece [,] all_pieces;
	private Vector2 firstTouch = new Vector2(0,0);
	private Vector2 finalTouch = new Vector2(0, 0);
	private bool controlling = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Randomize();
		all_pieces = new Piece [width,height];
		spawnPieces();
	}
	
	private void spawnPieces()
	{
		for (int i =0; i<width; i++)
		   for(int j=0; j<height; j++)
		{
			var rand = (int)Math.Floor(GD.RandRange(0,possible_pieces.Length));
			var piece = (Piece)possible_pieces[rand].Instance();
			
			var loops = 0;
			while (mathAt(i,j,piece.color)&& loops<100)
			{
				rand = (int)Math.Floor(GD.RandRange(0,possible_pieces.Length));
				piece = (Piece)possible_pieces[rand].Instance();
				loops+=1;
			}
			AddChild(piece);
			piece.Position = gridToPixel(i, j);
			all_pieces[i,j] = piece;
		}
	}
	
	private Vector2 gridToPixel(int coll, int row)
	{
		return new Vector2(x_start+offset*coll, y_start-offset*row);
	}

	private Vector2 pixelToGrid(float x, float y)
	{
		return new Vector2((float)Math.Round((x- x_start) / offset),
						   (float)Math.Round((y- y_start) / -offset));
	}
	public bool isInGrid(int coll, int row)
	{
		if (coll >= 0 && coll < width)
			if (row >= 0 && row < height)
				return true;
		return false;
	}

	private void touchInput()
	{
		if (Input.IsActionJustPressed("ui_touch"))
		{
			firstTouch = GetGlobalMousePosition();
			var gridPosition = pixelToGrid(firstTouch.x, firstTouch.y);
			if (isInGrid((int)gridPosition.x, (int)gridPosition.y))
				controlling = true;
	
		}

		if (Input.IsActionJustReleased("ui_touch"))
		{
			finalTouch = GetGlobalMousePosition();
			var gridPosition = pixelToGrid(finalTouch.x, finalTouch.y);
			if (isInGrid((int)gridPosition.x, (int)gridPosition.y)&& controlling)
				touchDifference(pixelToGrid(firstTouch.x, firstTouch.y), gridPosition);
		}
	}

	private void swapPieces(int coll, int row, Vector2 direction)
	{
		var firstPiece = all_pieces[coll, row];
		var otherPiece = all_pieces[coll+(int)direction.x, row+(int)direction.y];
		all_pieces[coll, row] = otherPiece;
		all_pieces[coll + (int)direction.x, row + (int)direction.y] = firstPiece;
		firstPiece.move(gridToPixel(coll + (int)direction.x, row + (int)direction.y));
		otherPiece.move(gridToPixel(coll,row));

	}

	private void touchDifference(Vector2 grid1, Vector2 grid2)
	{
		var difference = grid2 - grid1;
		if (Math.Abs(difference.x) > Math.Abs(difference.y))
		{
			if (difference.x > 0)
				swapPieces((int)grid1.x, (int)grid1.y, new Vector2(1, 0));
			else
			if (difference.x < 0)
				swapPieces((int)grid1.x, (int)grid1.y, new Vector2(-1, 0));
		}
		else
		{
			if (Math.Abs(difference.y) > Math.Abs(difference.x))
				if (difference.y > 0)
					swapPieces((int)grid1.x, (int)grid1.y, new Vector2(0, 1));
				else
				if (difference.y < 0)
					swapPieces((int)grid1.x, (int)grid1.y, new Vector2(0, -1));
		}
	}

	private bool mathAt(int i, int j, string color )
	{
	 if ( i > 1)
	   if (all_pieces[i-1,j] != null && all_pieces[i-2,j]!= null)
		  if (all_pieces[i-1,j].color == all_pieces[i-2,j].color && all_pieces[i - 2, j].color == color)
			 return true;
	
	  if ( j > 1)
	   if (all_pieces[i,j-1] != null && all_pieces[i,j-2]!= null)
		  if (all_pieces[i,j-1].color == all_pieces[i,j-2].color && all_pieces[i, j - 2].color == color)
			 return true;
			
	 return false;	
	}

   public override void _Process(float delta)
   {
	 touchInput();
	  
   }
}

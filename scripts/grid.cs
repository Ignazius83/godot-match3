using Godot;
using System;
using System.Collections.Generic;

enum GameStates {
	WAIT,
	MOVE
}


public class grid : Node2D
{
	private GameStates state;

	[Export] private int width;
	[Export] private int height;
	[Export] private int x_start;
	[Export] private int y_start;
	[Export] private int offset;
	[Export] private int y_offset;
	[Export] private Vector2[] empty_spaces;
	[Export] private Vector2[] ice_spaces;
	[Export] private Vector2[] lock_spaces;

	[Signal]
	delegate void damage_ice(Vector2 boardPosition);
	[Signal]
	delegate void make_ice(Vector2 boardPosition);
	[Signal]
	delegate void damage_lock(Vector2 boardPosition);
	[Signal]
	delegate void make_lock(Vector2 boardPosition);

	private PackedScene[] possible_pieces = new PackedScene[]
	{
		ResourceLoader.Load("res://scenes/blue_piece.tscn") as PackedScene,
		ResourceLoader.Load("res://scenes/green_piece.tscn") as PackedScene,
		ResourceLoader.Load("res://scenes/pink_piece.tscn") as PackedScene,
		ResourceLoader.Load("res://scenes/orange_piece.tscn") as PackedScene,
		ResourceLoader.Load("res://scenes/light_green_piece.tscn") as PackedScene,
		ResourceLoader.Load("res://scenes/yellow_piece.tscn") as PackedScene,

	};
	private Piece[,] all_pieces;
	private Piece pieceOne;
	private Piece pieceTwo;
	private bool move_checked = false;
	private Vector2 lastPlace = new Vector2(0, 0);
	private Vector2 lastDirection = new Vector2(0, 0);

	private Vector2 firstTouch = new Vector2(0, 0);
	private Vector2 finalTouch = new Vector2(0, 0);
	private bool controlling = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		state = GameStates.MOVE;

		GD.Randomize();
		all_pieces = new Piece[width, height];
		spawnPieces();
		spawnIce();
		spawnLock();
	}
	private void spawnIce()
    {
		for (int i = 0; i < ice_spaces.Length; i++)
			EmitSignal("make_ice", ice_spaces[i]);

	}

	private void spawnLock()
	{
		for (int i = 0; i < lock_spaces.Length; i++)
			EmitSignal("make_lock", lock_spaces[i]);

	}

	private void spawnPieces()
	{
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (!restrictedFill(new Vector2(i, j)))
				{
					var rand = (int)Math.Floor(GD.RandRange(0, possible_pieces.Length));
					var piece = (Piece)possible_pieces[rand].Instance();

					var loops = 0;
					while (mathAt(i, j, piece.color) && loops < 100)
					{
						rand = (int)Math.Floor(GD.RandRange(0, possible_pieces.Length));
						piece = (Piece)possible_pieces[rand].Instance();
						loops += 1;
					}
					AddChild(piece);
					piece.Position = gridToPixel(i, j);
					all_pieces[i, j] = piece;
				}
			}
	}

	private Vector2 gridToPixel(int coll, int row)
	{
		return new Vector2(x_start + offset * coll, y_start - offset * row);
	}

	private Vector2 pixelToGrid(float x, float y)
	{
		return new Vector2((float)Math.Round((x - x_start) / offset),
						   (float)Math.Round((y - y_start) / -offset));
	}
	public bool isInGrid(Vector2 gridPosition)
	{
		if (gridPosition.x >= 0 && gridPosition.x < width)
			if (gridPosition.y >= 0 && gridPosition.y < height)
				return true;
		return false;
	}

	private void touchInput()
	{
		if (Input.IsActionJustPressed("ui_touch"))
		{
			if (isInGrid(pixelToGrid(GetGlobalMousePosition().x, GetGlobalMousePosition().y)))
			{
				firstTouch = pixelToGrid(GetGlobalMousePosition().x, GetGlobalMousePosition().y);
				controlling = true;
			}


		}

		if (Input.IsActionJustReleased("ui_touch"))
		{
			if (isInGrid(pixelToGrid(GetGlobalMousePosition().x, GetGlobalMousePosition().y)) && controlling)
			{
				controlling = false;
				finalTouch = pixelToGrid(GetGlobalMousePosition().x, GetGlobalMousePosition().y);
				touchDifference(firstTouch, finalTouch);

			}



		}
	}
	private void swapBack()
    {
		if (pieceOne != null && pieceTwo != null)
		{
			swapPieces((int)lastPlace.x, (int)lastPlace.y, lastDirection);
			state = GameStates.MOVE;
			move_checked = false;
		}

    }

	private bool restrictedFill(Vector2 place)
    {
		if (isInArray(empty_spaces,place))
			return true;
       
		return false;
    }
	private bool restrictedMove(Vector2 place)
	{
		if (isInArray(lock_spaces, place))
			return true;

		return false;
	}


	private bool isInArray(Vector2[] array,Vector2 item)
    {
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == item)
				return true;
		}
		return false;
	}
	private void storeInfo( Piece firstPiece, Piece otherPiece, Vector2 place, Vector2 direction)
    {
		pieceOne = firstPiece;
		pieceTwo = otherPiece;
		lastPlace = place;
		lastDirection = direction;
    }

	private void swapPieces(int coll, int row, Vector2 direction)
	{
		var firstPiece = all_pieces[coll, row];
		var otherPiece = all_pieces[coll + (int)direction.x, row + (int)direction.y];
		if (firstPiece !=null && otherPiece!=null)
		{
			if (!restrictedMove(new Vector2(coll,row)) && !restrictedMove(new Vector2(coll, row)+direction))
            {
				storeInfo(firstPiece, otherPiece, new Vector2(coll, row), direction);
				state = GameStates.WAIT;

				all_pieces[coll, row] = otherPiece;
				all_pieces[coll + (int)direction.x, row + (int)direction.y] = firstPiece;
				firstPiece.move(gridToPixel(coll + (int)direction.x, row + (int)direction.y));
				otherPiece.move(gridToPixel(coll, row));
				if (!move_checked)
					findmatches();
			}
			
		}
		
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

	private void findmatches()
	{
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (all_pieces[i, j] != null)
				{
					var currentColor = all_pieces[i, j].color;
					if (i > 0 && i < width - 1)
					{
						if (!isPieceNull(i - 1, j) && !isPieceNull(i + 1, j))
						{
							if (all_pieces[i - 1, j].color == currentColor &&
								all_pieces[i + 1, j].color == currentColor)
							{
								matchAndDim(all_pieces[i - 1, j]);
								matchAndDim(all_pieces[i + 1, j]);
								matchAndDim(all_pieces[i, j]);							

							}
						}
					}
					if (j > 0 && j < height - 1)
					{
						if (!isPieceNull(i, j-1) && !isPieceNull(i, j+1))
						{
							if (all_pieces[i, j - 1].color == currentColor &&
								all_pieces[i, j + 1].color == currentColor)
							{
								matchAndDim(all_pieces[i, j-1]);
								matchAndDim(all_pieces[i, j+1]);
								matchAndDim(all_pieces[i, j]);								

							}
						}
					}


				}
			}
		(GetParent().GetNode("destroy_timer") as Timer).Start();
	}

	private bool isPieceNull(int col, int row)
    {
		if (all_pieces[col, row] == null)
			return true;
		return false;
    }

	private void matchAndDim(Piece item)
	{
		item.matched = true;
		item.dim();
	}
	private bool mathAt(int i, int j, string color)
	{
		if (i > 1)
			if (all_pieces[i - 1, j] != null && all_pieces[i - 2, j] != null)
				if (all_pieces[i - 1, j].color == all_pieces[i - 2, j].color && all_pieces[i - 2, j].color == color)
					return true;

		if (j > 1)
			if (all_pieces[i, j - 1] != null && all_pieces[i, j - 2] != null)
				if (all_pieces[i, j - 1].color == all_pieces[i, j - 2].color && all_pieces[i, j - 2].color == color)
					return true;

		return false;
	}

	public override void _Process(float delta)
	{
		if (state == GameStates.MOVE)
		   touchInput();

	}


	private void destroyMached()
	{
		bool was_mached = false;
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (all_pieces[i, j] != null)
				{
					if (all_pieces[i, j].matched)
					{
						
						damageSpecial(i, j);
						was_mached = true;
						all_pieces[i, j].QueueFree();
						all_pieces[i, j] = null;
					}					
						
				}
			}
		move_checked = true;
		if (was_mached)
			(GetParent().GetNode("collapse_timer") as Timer).Start();
		else
			swapBack();

	}
	private void damageSpecial(int column, int row)
    {
		EmitSignal("damage_ice", new Vector2(column, row));
		EmitSignal("damage_lock", new Vector2(column, row));
	}

	public void collapseColumn()
	{
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (all_pieces[i,j] == null && !restrictedFill(new Vector2(i, j)))
					foreach (int k in GD.Range(j + 1, height))
						if (all_pieces[i,k] != null)
						{
							all_pieces[i,k].move(gridToPixel(i, j));
							all_pieces[i,j] = all_pieces[i,k];
							all_pieces[i,k] = null;
							break;
						}
			}
		(GetParent().GetNode("refill_timer") as Timer).Start();
	}


	private void _on_destroy_timer_timeout()
	{
		destroyMached();
	}

	private void _on_collapse_timer_timeout()
	{
		collapseColumn();
	}

	private void _on_refill_timer_timeout()
    {
		refillColumns();
    }

    private void refillColumns()
    {
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (all_pieces[i, j] == null && !restrictedFill(new Vector2(i, j)))
				{
					var rand = (int)Math.Floor(GD.RandRange(0, possible_pieces.Length));
					var piece = (Piece)possible_pieces[rand].Instance();

					var loops = 0;
					while (mathAt(i, j, piece.color) && loops < 100)
					{
						rand = (int)Math.Floor(GD.RandRange(0, possible_pieces.Length));
						piece = (Piece)possible_pieces[rand].Instance();
						loops += 1;
					}
					AddChild(piece);
					piece.Position = gridToPixel(i, j-y_offset);
					piece.move(gridToPixel(i, j));
					all_pieces[i, j] = piece;
				}
			}
		afterRefillColumns();
	}

	private void _on_lock_holder_remove_lock(Vector2 boardPosition)
	{
		var lockList = new List<Vector2>(lock_spaces);
		foreach (int i in GD.Range(lockList.Count-1 ,-1,-1))
        {
			if (lock_spaces[i] == boardPosition)
				lockList.RemoveAt(i);
        }

		lock_spaces = lockList.ToArray();
	}

		public void afterRefillColumns()
	{
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (all_pieces[i, j] != null)
				{
					if (mathAt(i, j, all_pieces[i, j].color))
					{
						findmatches();
						(GetParent().GetNode("destroy_timer") as Timer).Start();
						return;
					}

				}

			}
		state = GameStates.MOVE;
		move_checked = false;
	}
}

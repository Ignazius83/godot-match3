using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

enum GameStates {
	WAIT,
	MOVE,
	WIN,
	BOOSTER
}


public class grid : Node2D
{
	private GameStates state;

	private bool canMove= true;
	[Signal]
	delegate void change_move_state();

	[Signal]
	delegate void reset_booster();

	private int width;
	 private int height;
	[Export] private int x_start;
	[Export] private int y_start;
	[Export] private int offset;
	[Export] private int y_offset;
	[Export] private Vector2[] empty_spaces;
	[Export] private Vector2[] ice_spaces;
	[Export] private Vector2[] lock_spaces;
	[Export] private Vector2[] concrete_spaces;
	[Export] private Vector2[] slime_spaces;
	[Export] private Vector3[] preset_spaces;
	

	[Signal]
	delegate void damage_ice(Vector2 boardPosition);
	[Signal]
	delegate void make_ice(Vector2 boardPosition);
	[Signal]
	delegate void damage_lock(Vector2 boardPosition);
	[Signal]
	delegate void make_lock(Vector2 boardPosition);
	[Signal]
	delegate void damage_concrete(Vector2 boardPosition);
	[Signal]
	delegate void make_concrete(Vector2 boardPosition);
	[Signal]
	delegate void damage_slime(Vector2 boardPosition);
	[Signal]
	delegate void make_slime(Vector2 boardPosition);

	[Signal]
	delegate void update_score(int streak);	
	private int streak = 1;

	[Signal]
	delegate void update_counter();

	[Signal]
	delegate void check_goal(string goal_type);
	private bool color_momb_used = false;


	// Collectible/Sikers
	[Export] private PackedScene sinkerPiece;
	[Export] private bool  sinkerInScene;
	[Export] private int maxSinkers;
	private int currentSinkers = 0;
	// Effects
	[Export] private PackedScene hintEffect;
	private HintEffect _hint;
	private string hintColor = "";
	private PackedScene particleEffect = ResourceLoader.Load("res://scenes/ParticleEffect.tscn") as PackedScene;
	private PackedScene animatedEffect = ResourceLoader.Load("res://scenes/AnimateExplosion.tscn") as PackedScene;
	// sound 
	[Signal] delegate void play_sound();

	//camera
	[Signal] delegate void place_camera(Vector2 placement);
	[Signal] delegate void camera_effect();

	[Export] private string[] possible_pieces;
	
	private Piece[,] all_pieces;
	private Piece[,] clone_array;
	private Vector2[] current_matches = new Vector2[] { };
	private Piece pieceOne;
	private Piece pieceTwo;
	private bool move_checked = false;
	private Vector2 lastPlace = new Vector2(0, 0);
	private Vector2 lastDirection = new Vector2(0, 0);

	private Vector2 firstTouch = new Vector2(0, 0);
	private Vector2 finalTouch = new Vector2(0, 0);
	private bool controlling = false;

	private bool damageSlime = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Randomize();
		moveCamera();
		all_pieces = new Piece[width, height];
		clone_array = new Piece[width, height];
		spawnPresets();
		if (sinkerInScene)
			spawnSinker(maxSinkers);
		spawnPieces();
		spawnIce();
		spawnLock();
		spawnConcrete();
		spawnSlime();		
	}

	private void _on_GameManager_grid_change_move()
    {
		canMove = !canMove;
    }

    private void moveCamera()
    {
		var newPos = gridToPixel((int)(width/2 - 0.5), (int)(height/2 -0.5));
		EmitSignal("place_camera", newPos);
    }

	private void _on_ShuffleTimer_timeout()
    {
		shuffleBoard();
    }

	private void camEffect()
    {
		EmitSignal("camera_effect");
    }

	private Piece[,] copyArray(Piece[,] arrayToCopy)
	{
		var newArray = new Piece[width, height];
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				newArray[i, j] = arrayToCopy[i, j];
			}
		return newArray;
	}
	private bool isDeadLocked()
    {
		clone_array = copyArray(all_pieces);
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
            {
				//right
				if (switchAndCheck(new Vector2(i, j), new Vector2(1, 0), ref clone_array))
					return false;
				//up
				if (switchAndCheck(new Vector2(i, j), new Vector2(0, 1), ref clone_array))
					return false;
				
			}
		return true;


	}
	private List<Piece> clearAndStoreBoard()
    {
		List<Piece> holder = new List<Piece>();
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (all_pieces[i, j] != null)
				{
					holder.Add(all_pieces[i, j]);
					all_pieces[i, j] = null;
				}
			}
		return holder;
	}

	private void shuffleBoard()
    {
		var holder = clearAndStoreBoard();
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (!restrictedFill(new Vector2(i, j)) && all_pieces[i, j] == null)
				{
					var rand = (int)Math.Floor(GD.RandRange(0, holder.Count));
					var piece = holder[rand];

					var loops = 0;
					while (mathAt(i, j, piece.color) && loops < 100)
					{
						rand = (int)Math.Floor(GD.RandRange(0, holder.Count));
						piece = holder[rand];
						loops += 1;
					}
					
					piece.move(gridToPixel(i, j));
					all_pieces[i, j] = piece;
					holder.RemoveAt(rand);
				}
			}
		if (isDeadLocked())
			shuffleBoard();
		canMove = true;
		EmitSignal("change_move_state");
	}
	private List<Piece> findAllMatches()
    {
		List<Piece> holder_array = new List<Piece>();
		clone_array = copyArray(all_pieces);
		GD.Print("hintColor="+hintColor);
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (!restrictedMove(new Vector2(i, j)) && clone_array[i, j] != null)
				{
					if (switchAndCheck(new Vector2(i, j), new Vector2(1, 0), ref clone_array) &&  isInGrid(new Vector2(i + 1, j)) && !restrictedMove(new Vector2(i + 1, j)))
					{
						if (hintColor != "")
						{
							if (hintColor == clone_array[i, j].color)
								holder_array.Add(clone_array[i, j]);
							else
								holder_array.Add(clone_array[i + 1, j]);
						}
					}

					if (switchAndCheck(new Vector2(i, j), new Vector2(0, 1), ref clone_array) && isInGrid(new Vector2(i, j+1)) && !restrictedMove(new Vector2(i, j+1)))
					{
						if (hintColor != "")
						{
							if (hintColor == clone_array[i, j].color)
								holder_array.Add(clone_array[i, j]);
							else
								holder_array.Add(clone_array[i, j + 1]);
						}
					}
				}
			}
		return holder_array;

	}
	private void _on_HintTimer_timeout()
    {
		generate_hint();
    }

	private void destroy_hint()
	{
		if (_hint !=null)
		{
			_hint.QueueFree();
			_hint = null;
		}
	}
	private void generate_hint()
	{		
		var hints = findAllMatches();

		if (hints.Count > 0)
		{
			//destroy_hint();
			var rand = Math.Floor(GD.RandRange(0, hints.Count));
			_hint = hintEffect.Instance<HintEffect>();
			AddChild(_hint);
			_hint.Position = hints[(int)rand].Position;
			_hint.setup(hints[(int)rand].GetNode<Sprite>("Sprite").Texture);
		}
		GD.Print(hints.Count);
		
	}
	private void switchPieces(Vector2 place, Vector2 direction, ref Piece[,] array)
    {
		if (isInGrid(place) && !restrictedFill(place))
			if (isInGrid(place+direction)&& !restrictedFill(place+direction))
		    {
				var holder = array[(int)place.x + (int)direction.x, (int)place.y + (int)direction.y];
				array[(int)place.x + (int)direction.x, (int)place.y + (int)direction.y] = array[(int)place.x , (int)place.y];
				array[(int)place.x, (int)place.y] = holder;
			}				
    }
	private bool switchAndCheck(Vector2 place, Vector2 direction, ref Piece[,] array)
    {
		switchPieces(place, direction, ref array);
		if (existMatches(array))
        {
			switchPieces(place, direction, ref array);
			return true;
        }
		switchPieces(place, direction, ref array);
		return false;

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

	private void spawnConcrete()
	{
		for (int i = 0; i < concrete_spaces.Length; i++)
			EmitSignal("make_concrete", concrete_spaces[i]);

	}

	private void spawnSlime()
	{
		for (int i = 0; i < slime_spaces.Length; i++)
			EmitSignal("make_slime", slime_spaces[i]);

	}

	private void spawnPieces()
	{
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (!restrictedFill(new Vector2(i, j)) && all_pieces[i,j] == null)
				{
					var rand = (int)Math.Floor(GD.RandRange(0, possible_pieces.Length));
					var piece = (Piece)ResourceLoader.Load<PackedScene>(possible_pieces[rand]).Instance();

					var loops = 0;
					while (mathAt(i, j, piece.color) && loops < 100)
					{
						rand = (int)Math.Floor(GD.RandRange(0, possible_pieces.Length));
						piece = (Piece)ResourceLoader.Load<PackedScene>(possible_pieces[rand]).Instance();
						loops += 1;
					}
					AddChild(piece);
					piece.Position = gridToPixel(i, j);
					all_pieces[i, j] = piece;
				}
			}

		if (isDeadLocked())
			shuffleBoard();

		(GetNode("HintTimer") as Timer).Start();
	}

	private bool isPieceSinker(int column, int row)
    {
		if (all_pieces[column, row] != null)
			if (all_pieces[column, row].color == "None")
				return true;
		return false;
    }

	private void spawnPresets()
    {
		if (preset_spaces == null) return;

		for (int i = 0; i < preset_spaces.Length; i++)
        {
			var piece = ResourceLoader.Load<PackedScene>(possible_pieces[(int)preset_spaces[i].z]).Instance<Piece>();
			AddChild(piece);
			piece.Position = gridToPixel((int)preset_spaces[i].x, (int)preset_spaces[i].y);
			all_pieces[(int)preset_spaces[i].x, (int)preset_spaces[i].y] = piece;
		}

	}
	
	private void spawnSinker(int number_to_spawn)
    {
       for (int i=0; i< number_to_spawn; i++)
        {
			var column = (int)Math.Floor(GD.RandRange(0, width));
			while(all_pieces[column,height-1]!=null || restrictedFill(new Vector2(column, height - 1)))
		    {
			  column = (int)Math.Floor(GD.RandRange(0, width));
		    }

			var current = sinkerPiece.Instance<Piece>();
			AddChild(current);
			current.Position = gridToPixel(column, height - 1);
			all_pieces[column, height - 1] = current;
			currentSinkers += 1;
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
				destroy_hint();
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
			swapPieces((int)lastPlace.x, (int)lastPlace.y, lastDirection);
			
		canMove = true;
		EmitSignal("change_move_state");
		move_checked = false;
	    (GetNode("HintTimer") as Timer).Start();


	}

	private bool restrictedFill(Vector2 place)
    {
		if (isInArray(empty_spaces,place))
			return true;
		if (isInArray(concrete_spaces, place))
			return true;
		if (isInArray(slime_spaces, place))
			return true;
		return false;
    }

	private void check_concrete(int column, int row)
    {
		if (column < width - 1)
			EmitSignal("damage_concrete", new Vector2(column + 1, row));
		if (column > 0)
			EmitSignal("damage_concrete", new Vector2(column - 1, row));
		if (row < height - 1)
			EmitSignal("damage_concrete", new Vector2(column, row+1));
		if (row > 0)
			EmitSignal("damage_concrete", new Vector2(column, row-1));
	}

	private void check_slime(int column, int row)
	{
		if (column < width - 1)
			EmitSignal("damage_slime", new Vector2(column + 1, row));
		if (column > 0)
			EmitSignal("damage_slime", new Vector2(column - 1, row));
		if (row < height - 1)
			EmitSignal("damage_slime", new Vector2(column, row + 1));
		if (row > 0)
			EmitSignal("damage_slime", new Vector2(column, row - 1));
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
				if (isColorBomb(firstPiece,otherPiece))
                {				

					if (isPieceSinker(coll, row) || isPieceSinker(coll + (int)direction.x, row + (int)direction.y))
                    {
						swapBack();
						return;
					}

					if (firstPiece.color == "Color" && otherPiece.color == "Color")
						clearBoard();
					else
					{
						if (firstPiece.color == "Color")
						{
							matchColor(otherPiece.color);
							matchAndDim(firstPiece);
							addToArray(new Vector2(coll, row), ref current_matches);
						}
						else
						{
							matchColor(firstPiece.color);
							matchAndDim(otherPiece);
							addToArray(new Vector2(coll + direction.x, row + direction.y), ref current_matches);
						}
					}
						
				}
				storeInfo(firstPiece, otherPiece, new Vector2(coll, row), direction);
				canMove = false;
				EmitSignal("change_move_state");

				all_pieces[coll, row] = otherPiece;
				all_pieces[coll + (int)direction.x, row + (int)direction.y] = firstPiece;
				firstPiece.move(gridToPixel(coll + (int)direction.x, row + (int)direction.y));
				otherPiece.move(gridToPixel(coll, row));
				if (!move_checked)
					findmatches();
			}
			
		}
		
	}

	private bool isColorBomb(Piece pieceOne,Piece pieceTwo)
    {
		if (pieceOne.color == "Color" || pieceTwo.color == "Color")
		{
			color_momb_used = true;
			return true;
		}
		return false;
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

	private bool existMatches( Piece[,] array)
    {
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (array[i, j] != null)
				{
					var currentColor = array[i, j].color;
					if (i > 0 && i < width - 1)
					{
						if (array[i - 1, j] != null && array[i + 1, j]!=null)
						{
							if (array[i - 1, j].color == currentColor &&
								array[i + 1, j].color == currentColor)
							{

								hintColor = currentColor;
								return true;
							}
							
						}
					}
					if (j > 0 && j < height - 1)
					{
						if (array[i, j - 1]!=null && array[i, j + 1]!=null)
						{
							if (array[i, j - 1].color == currentColor &&
								array[i, j + 1].color == currentColor)
							{
								hintColor = currentColor;
								return true;
							}
								
						}
					}


				}
			}

		return false;
	}
	
	private void findmatches()
	{
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (all_pieces[i, j] != null && !isPieceSinker(i,j))
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

								addToArray(new Vector2(i, j), ref current_matches);
								addToArray(new Vector2(i+1, j),ref current_matches);
								addToArray(new Vector2(i-1, j),ref current_matches);
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

								addToArray(new Vector2(i, j), ref current_matches);
								addToArray(new Vector2(i, j+1), ref current_matches);
								addToArray(new Vector2(i, j-1), ref current_matches);

							}
						}
					}


				}
			}
		getBombPieces();
		(GetParent().GetNode("destroy_timer") as Timer).Start();
	}

	private void addToArray(Vector2 value, ref Vector2[] array)
    {
		if (!array.Contains(value))
			array = array.Append(value).ToArray();
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

		if (canMove)
			touchInput();
	 
	}

	private void  _on_GameManager_color_bomb( Vector2 position)
    {
		var temp = pixelToGrid(position.x, position.y);
		if (isInGrid(temp))
			makeColorBomb(temp);
	}
	private void _on_GameManager_destroy_piece(Vector2 position)
	{
		var temp = pixelToGrid(position.x, position.y);
		if (isInGrid(temp))
			if (all_pieces[(int)temp.x, (int)temp.y] != null)
			{
				all_pieces[(int)temp.x, (int)temp.y].matched = true;
				destroyMached();
				EmitSignal("reset_booster");

			}
	}

	private void makeColorBomb(Vector2 gridPos)
    {
		if (isInGrid(gridPos))
		{
			if (all_pieces[(int)gridPos.x, (int)gridPos.y] != null)
			{
				all_pieces[(int)gridPos.x, (int)gridPos.y].makeColorBomb();				
				EmitSignal("reset_booster");
				canMove = true;
			}
		}
	}

    private void findBombs()
	{ 
		if (color_momb_used) return;
		
		for (int i = 0; i < current_matches.Length; i++)
		{
			var current_column = current_matches[i].x;
			var current_row =    current_matches[i].y;
			var current_color = all_pieces[(int)current_column, (int)current_row].color;
			var col_matched = 0;
			var row_matched = 0;
			for (int j = 0; j < current_matches.Length; j++)
            {
				var this_column = current_matches[j].x;
				var this_row = current_matches[j].y;
				var this_color = all_pieces[(int)this_column, (int)this_row].color;
				if (this_column == current_column && this_color == current_color)
					col_matched++;
				if (this_row == current_row && this_color == current_color)
					row_matched++;
			}
			if (col_matched == 5 || row_matched == 5)
			{
				makeBomb(3, current_color);
				continue;
			}
			else if (col_matched >= 3 && row_matched >= 3)
			{
				makeBomb(0, current_color);
				continue;
			}
			else if (col_matched == 4)
			{
				makeBomb(1, current_color);
				continue;
			}
			else if (row_matched == 4)
			{
				makeBomb(2, current_color);
				continue;
			}
			
			

		}
	}

	private void matchAllInColumn(int column)
	{
		for (int i = 0; i < height; i++)
		{
			if (all_pieces[column, i] != null && !isPieceSinker(column, i))
            {
				if (all_pieces[column, i].isRowBomb)
					matchAllInRow(i);
				if (all_pieces[column, i].isAdjacentBomb)
					findAdjacentPieces(column,i);
				if (all_pieces[column, i].isColorBomb)
					matchColor(all_pieces[column, i].color);

				all_pieces[column, i].matched = true;
			}				
				
		}
	}

	private void getBombPieces()
	{
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (all_pieces[i, j] != null)
					if (all_pieces[i, j].matched)
                    {
						if (all_pieces[i, j].isColBomb)
							matchAllInColumn(i);
						else if (all_pieces[i, j].isRowBomb)
							matchAllInRow(j);
						else if (all_pieces[i, j].isAdjacentBomb)
							findAdjacentPieces(i, j);
					}
			}
	}

	private void  findAdjacentPieces(int column, int row)
    {
		foreach (int i in GD.Range(-1, 2))
			foreach (int j in GD.Range(-1, 2))
			{
				if (isInGrid(new Vector2(column + i, row + j)))
					if (all_pieces[column + i, row + j] != null && !isPieceSinker(column + i, row + j))
					{
						if (all_pieces[column+i, row+j].isRowBomb)
							matchAllInRow(row + j);
						if (all_pieces[column + i, row + j].isColBomb)
							matchAllInColumn(column + i);
						if (all_pieces[column+i, row+j].isColorBomb)
							matchColor(all_pieces[column+i, row+j].color);

						all_pieces[column + i, row + j].matched = true;
					}

			}
    }

	private void matchColor(string color)
	{
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
				if (all_pieces[i, j] != null && !isPieceSinker(i,j))
				{

					if (all_pieces[i, j].color == color)
                    {
						if (all_pieces[i, j].isColBomb)                       
							matchAllInColumn(i);
						if (all_pieces[i, j].isRowBomb)
							matchAllInRow(j);
						if (all_pieces[i, j].isAdjacentBomb)
							findAdjacentPieces(i,j);

						matchAndDim(all_pieces[i, j]);
						addToArray(new Vector2(i, j), ref current_matches);

					}

				}
	}

	private void clearBoard()
	{
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
				if (all_pieces[i, j] != null && !isPieceSinker(i, j))
				{					
						matchAndDim(all_pieces[i, j]);
						addToArray(new Vector2(i, j), ref current_matches);
				}
	}

	private void matchAllInRow(int row)
	{
		for (int i = 0; i < width; i++)
		{
			if (all_pieces[i, row] != null && !isPieceSinker(i, row))
            {
				if (all_pieces[i, row].isColBomb)
					matchAllInColumn(i);
				if (all_pieces[i,row].isAdjacentBomb)
					findAdjacentPieces(i, row);
				if (all_pieces [i,row].isColorBomb)
					matchColor(all_pieces[i, row].color);

				all_pieces[i, row].matched = true;
			}
				
		}
	}

	private void makeBomb(int bombType, string color)
	{
		for (int i = 0; i < current_matches.Length; i++)
		{
			var current_column =(int)current_matches[i].x;
			var current_row =(int) current_matches[i].y;
			if (all_pieces[current_column, current_row] == pieceOne && pieceOne.color == color)
			{
				damageSpecial(current_column, current_row);
				EmitSignal("check_goal", pieceOne.color);
				pieceOne.matched = false;
				changeBomb(bombType, pieceOne);
			}else
			if (all_pieces[current_column, current_row] == pieceTwo && pieceTwo.color == color)
            {
				pieceTwo.matched = false;
				damageSpecial(current_column, current_row);
				EmitSignal("check_goal", pieceTwo.color);
				changeBomb(bombType, pieceTwo);
			}

		}
	}
	private void changeBomb(int bombType, Piece piece)
    {
		switch(bombType)
        {
			case 0: piece.makeAdjacentBomb();
				break;
			case 1: piece.makeColumnBomb();
				break;
			case 2: piece.makeRowBomb(); 
				break;
			case 3:
				piece.makeColorBomb();
				break;
		}
    }

	private void destroyMached()
	{
		findBombs();
		bool was_mached = false;
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (all_pieces[i, j] != null)
				{
					if (all_pieces[i, j].matched)
					{
						
						EmitSignal("check_goal", all_pieces[i, j].color);
						damageSpecial(i, j);
						was_mached = true;
						all_pieces[i, j].QueueFree();
						all_pieces[i, j] = null;
						makeEffect(particleEffect, i, j);
						makeEffect(animatedEffect, i, j);
						EmitSignal("play_sound");
						camEffect();
						EmitSignal("update_score", streak);
					}					
						
				}
			}
		move_checked = true;
		if (was_mached)
		{
			destroy_hint();
			(GetParent().GetNode("collapse_timer") as Timer).Start();
		}
		else
			swapBack();

		current_matches = new Vector2[] { };

	}

	private void makeEffect(PackedScene effect, int column, int row)
    {
		var current = effect.Instance() as Node2D;
		current.Position = gridToPixel(column, row);
		AddChild(current);
	}
	private void damageSpecial(int column, int row)
    {
		EmitSignal("damage_ice", new Vector2(column, row));
		EmitSignal("damage_lock", new Vector2(column, row));
		check_concrete(column, row);
		check_slime(column, row);
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
		destroySinkers();
		(GetParent().GetNode("refill_timer") as Timer).Start();
	}

	private void destroySinkers()
    {
		for (int i=0; i<width; i++)
        {
			if (all_pieces[i, 0] != null)
				if (all_pieces[i, 0].color == "None")
				{
					all_pieces[i, 0].matched = true;
					currentSinkers -= 1;
				}
					


		}
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
		streak += 1;
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				if (all_pieces[i, j] == null && !restrictedFill(new Vector2(i, j)))
				{
					var rand = (int)Math.Floor(GD.RandRange(0, possible_pieces.Length));
					var piece = (Piece)ResourceLoader.Load<PackedScene>(possible_pieces[rand]).Instance();

					var loops = 0;
					while (mathAt(i, j, piece.color) && loops < 100)
					{
						rand = (int)Math.Floor(GD.RandRange(0, possible_pieces.Length));
						piece = (Piece)ResourceLoader.Load<PackedScene>(possible_pieces[rand]).Instance();
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
	private void _on_slime_holder_remove_slime(Vector2 boardPosition)
	{
		damageSlime = true;
		var lockList = new List<Vector2>(slime_spaces);
		foreach (int i in GD.Range(lockList.Count - 1, -1, -1))
		{
			if (lockList[i] == boardPosition)
				lockList.RemoveAt(i);
		}

		slime_spaces = lockList.ToArray();

	}

	private void _on_lock_holder_remove_lock(Vector2 boardPosition)
	{
		var lockList = new List<Vector2>(lock_spaces);
		foreach (int i in GD.Range(lockList.Count - 1, -1, -1))
		{
			if (lockList[i] == boardPosition)
				lockList.RemoveAt(i);
		}

		lock_spaces = lockList.ToArray();
			
	}
	private void _on_concrete_holder_remove_concrete(Vector2 boardPosition)
	{
		var lockList = new List<Vector2>(concrete_spaces);
		foreach (int i in GD.Range(lockList.Count - 1, -1, -1))
		{
			if (lockList[i] == boardPosition)
				lockList.RemoveAt(i);
		}

		concrete_spaces = lockList.ToArray();		
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
		if (!damageSlime)
			generateSlime();
		
		streak = 1;
		move_checked = false;
		damageSlime = false;
		color_momb_used = false;
		if (isDeadLocked())
			GetNode<Timer>("ShuffleTimer").Start();		
		
		canMove = true;
		EmitSignal("change_move_state");
		EmitSignal("update_counter");
		(GetNode("HintTimer") as Timer).Start();
	}

	private void generateSlime()
    {
		if (slime_spaces.Length>0)
        {
			var slime_made = false;
			var tracker = 0;
			while(!slime_made && tracker<100)
            {
				var random_num =(int)Math.Floor(GD.RandRange(0, slime_spaces.Length));
				GD.Print(random_num);
				int curr_x = (int)slime_spaces[random_num].x;
				int curr_y = (int) slime_spaces[random_num].y;
				var neighbar = findNormalNeighbar((int)curr_x, (int)curr_y);
				GD.Print(neighbar);
				GD.Print(slime_spaces.Length);
				if (neighbar != null)
				{
					
					all_pieces[(int)((Vector2)neighbar).x, (int)((Vector2)neighbar).y].QueueFree();
					all_pieces[(int)((Vector2)neighbar).x, (int)((Vector2)neighbar).y] = null;
					slime_spaces = slime_spaces.Append(new Vector2(((Vector2)neighbar).x, ((Vector2)neighbar).y)).ToArray();
					EmitSignal("make_slime", new Vector2(((Vector2)neighbar).x, ((Vector2)neighbar).y));
					slime_made = true;
				}
				tracker += 1;
			}
        }

    }

	private Vector2? findNormalNeighbar(int column, int row)
    {
		if (isInGrid(new Vector2(column + 1, row)))
		{
			if (all_pieces[column + 1, row] != null && !isPieceSinker(column+1, row))
				return new Vector2(column + 1, row);
		}
		if (isInGrid(new Vector2(column - 1, row)))
		{
			if (all_pieces[column - 1, row] != null && !isPieceSinker(column-1, row))
				return new Vector2(column - 1, row);
		}
		if (isInGrid(new Vector2(column, row + 1)))
		{
			if (all_pieces[column, row + 1] != null && !isPieceSinker(column, row+1))
				return new Vector2(column, row + 1);
		}
		if (isInGrid(new Vector2(column, row - 1)))
		{
			if (all_pieces[column, row - 1] != null && !isPieceSinker(column, row-1))
				return new Vector2(column, row - 1);
		}

		return null;

	}

  
	private void _on_GameManager_game_lost()
    {
		canMove = false;
	}

	private void _on_GameManager_game_won(int scoreToDisplay)
    {
		canMove = false;		
	}

	private void _on_GameManager_set_dimentions(int new_width, int new_height)
    {
		width = new_width;
		height = new_height;
    }
}

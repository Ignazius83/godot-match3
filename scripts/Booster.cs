using Godot;
using System;


public class Booster : TextureButton
{
   
    private bool active = false;

    [Export] private Texture colorBombTexture;
    [Export] private Texture addCounterTexture;
    [Export] private Texture destroyPieceTexture;

    public string booster_type;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void checkActive(bool is_active,string type)
    {
        if (is_active)
        {
            if (type == "ColorBomb")
            {
                TextureNormal = colorBombTexture;
                booster_type = "ColorBomb";
            }
            else if (type == "AddCounter")
            {
                TextureNormal = addCounterTexture;
                booster_type = "AddCounter";
            }
            else if (type == "DestroyPiece")
            {
                TextureNormal = destroyPieceTexture;
                booster_type = "DestroyPiece";
            }
        }
        else
            TextureNormal = null;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

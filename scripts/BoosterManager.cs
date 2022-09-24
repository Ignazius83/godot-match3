using Godot;
using System;

public class BoosterManager : HBoxContainer
{

    private BoosterInfo boosterInfo;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        boosterInfo = GetNode<BoosterInfo>("/root/BoosterInfo");
        boosterInfo.boosterInfo[1] = "AddCounter";
        boosterInfo.boosterInfo[2] = "ColorBomb";
        boosterInfo.boosterInfo[3] = "DestroyPiece";
        activateBoosterButtons();
    }
    public void activateBoosterButtons()
    {
        for (int i = 1; i < GetChildCount(); i++)
            if (GetChild(i).IsInGroup("Boosters"))
            { 
                if (boosterInfo.boosterInfo[i].ToString() == "")                          
                    GetChild<Booster>(i).checkActive(false,"");
                else
                    GetChild<Booster>(i).checkActive(true, boosterInfo.boosterInfo[i].ToString());
            }
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

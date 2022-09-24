using Godot;
using Godot.Collections;
using System;

public class GameDataManager : Node
{
    public Dictionary levelInfo = new Dictionary{};
    private readonly string path = "user://save.dat";
    public Dictionary defaultInfo = new Dictionary
     {
        [1] =  new Dictionary
         {
            ["unlocked"] = true,
            ["high_score"] =0,
            ["stars_unlocked"] = 0
         }
     };

    public int levelScrollValue = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        levelInfo = loadData();      
      
    }

    public void saveData()
    {
        var file = new File();
        var err = file.Open(path, File.ModeFlags.Write);
        if (err != Error.Ok)
        {
            GD.Print("wrong data");
            return;
        }
        file.StoreVar(levelInfo);
        file.Close();

    }
    public Dictionary loadData()
    {
        var file = new File();
        var err = file.Open(path, File.ModeFlags.Read);
        if (err != Error.Ok)
        {
            GD.Print("wrong data");
            return defaultInfo;
        }

        var read = (Dictionary)file.GetVar();
        return read;


    }

    public void  changeLevelScrollValue(int value)
    {
        levelScrollValue = value;
    }


}

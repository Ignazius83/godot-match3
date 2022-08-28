using Godot;
using System;

public class BaseMenuPanel : CanvasLayer
{
	private AnimationPlayer animationPlayer;
	public override void _Ready()
	{
			
	}
	public void slide_in()
    {
		animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
		animationPlayer.Play("slide_in");
    }
	public void slide_out()
	{
		animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
		animationPlayer.PlayBackwards("slide_in");
	}
}

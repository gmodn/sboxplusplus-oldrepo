using Sandbox;
using System;

[Library( "ent_textscreen", Title = "Text Screen", Spawnable = true )]
public partial class TextScreen : Prop
{
    [Net] public string Text { get; set; } = "Default Text";
    public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/torch/torch.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );

        //var worldtext = Entity.Create("point_worldtext");
        ConsoleSystem.Run("ent_create point_worldtext");
        ConsoleSystem.Run("ent_fire", "point_worldtext", "SetMessage", "cock");
	}
}
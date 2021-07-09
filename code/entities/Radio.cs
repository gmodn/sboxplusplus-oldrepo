using Sandbox;
using System;

[Library( "ent_radio", Title = "Radio", Spawnable = true )]
public partial class RadioEntity : Prop, IUse
{
	public float MaxSpeed { get; set; } = 1000.0f;
	public float SpeedMul { get; set; } = 1.2f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/food/glizzy.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}

	public bool OnUse( Entity user )
	{
		if ( user is Player player )
		{
			PlaySound( "sboxsong" );
		}

		return false;
	}
}

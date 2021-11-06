using Sandbox;
using System.Linq;

[Library( "generic_actor", Title = "Npc Test", Spawnable = true )]
[Hammer.EditorModel( "models/citizen/citizen.vmdl" )]
public partial class CAI_BaseNpc : AnimEntity
{
	[ConVar.Replicated]
	public static bool nav_drawpath { get; set; }

	public string[] clothesList = new string[] { "trousers/trousers.smart.vmdl", "jacket/labcoat.vmdl", "shirt/shirt_longsleeve.scientist.vmdl" };

	[ServerCmd( "npc_clear" )]
	public static void NpcClear()
	{
		foreach ( var npc in Entity.All.OfType<CAI_BaseNpc>().ToArray() )
			npc.Delete();
	}

	float m_fHealth = 16;

	float Speed;

	public NavSteer Steer;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen/citizen.vmdl" );
		EyePos = Position + Vector3.Up * 64;
		CollisionGroup = CollisionGroup.Player;
		SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 72, 8 ) );

		EnableHitboxes = true;

		SetBodyGroup( 1, 0 );

		Speed = 50;
	}

	public override void TakeDamage( DamageInfo info )
	{
		m_fHealth -= info.Damage;
		Log.Info( m_fHealth );
		if ( m_fHealth <= 0 )
		{
			ModelEntity ragdoll = new ModelEntity();
			ragdoll.SetModel( GetModel() );
			ragdoll.Position = Position;
			ragdoll.Rotation = EyeRot;
			ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			ragdoll.PhysicsGroup.Velocity = (Position - info.Position) * info.Force;
			ragdoll.Spawn();
			Delete();
		}
		base.TakeDamage( info );
	}

	public Sandbox.Debug.Draw Draw => Sandbox.Debug.Draw.Once;

	Vector3 InputVelocity;

	Vector3 LookDir;

	[Event.Tick.Server]
	public void Tick()
	{
		using var _a = Sandbox.Debug.Profile.Scope( "NpcTest::Tick" );

		InputVelocity = 0;

		if ( Steer != null )
		{
			using var _b = Sandbox.Debug.Profile.Scope( "Steer" );

			Steer.Tick( Position );

			if ( !Steer.Output.Finished )
			{
				InputVelocity = Steer.Output.Direction.Normal;
				Velocity = Velocity.AddClamped( InputVelocity * Time.Delta * 500, Speed );
			}

			if ( nav_drawpath )
			{
				Steer.DebugDrawPath();
			}
		}

		using ( Sandbox.Debug.Profile.Scope( "Move" ) )
		{
			Move( Time.Delta );
		}

		var walkVelocity = Velocity.WithZ( 0 );
		if ( walkVelocity.Length > 0.5f )
		{
			var turnSpeed = walkVelocity.Length.LerpInverse( 0, 100, true );
			var targetRotation = Rotation.LookAt( walkVelocity.Normal, Vector3.Up );
			Rotation = Rotation.Lerp( Rotation, targetRotation, turnSpeed * Time.Delta * 20.0f );
		}

		var animHelper = new CitizenAnimationHelper( this );

		LookDir = Vector3.Lerp( LookDir, InputVelocity.WithZ( 0 ) * 1000, Time.Delta * 100.0f );
		animHelper.WithLookAt( EyePos + LookDir );
		animHelper.WithVelocity( Velocity );
		animHelper.WithWishVelocity( InputVelocity );
	}

	protected virtual void Move( float timeDelta )
	{
		var bbox = BBox.FromHeightAndRadius( 64, 4 );
		//DebugOverlay.Box( Position, bbox.Mins, bbox.Maxs, Color.Green );

		MoveHelper move = new( Position, Velocity );
		move.MaxStandableAngle = 50;
		move.Trace = move.Trace.Ignore( this ).Size( bbox );

		if ( !Velocity.IsNearlyZero( 0.001f ) )
		{
			//	Sandbox.Debug.Draw.Once
			//						.WithColor( Color.Red )
			//						.IgnoreDepth()
			//						.Arrow( Position, Position + Velocity * 2, Vector3.Up, 2.0f );

			using ( Sandbox.Debug.Profile.Scope( "TryUnstuck" ) )
				move.TryUnstuck();

			using ( Sandbox.Debug.Profile.Scope( "TryMoveWithStep" ) )
				move.TryMoveWithStep( timeDelta, 30 );
		}

		using ( Sandbox.Debug.Profile.Scope( "Ground Checks" ) )
		{
			var tr = move.TraceDirection( Vector3.Down * 10.0f );

			if ( move.IsFloor( tr ) )
			{
				GroundEntity = tr.Entity;

				if ( !tr.StartedSolid )
				{
					move.Position = tr.EndPos;
				}

				if ( InputVelocity.Length > 0 )
				{
					var movement = move.Velocity.Dot( InputVelocity.Normal );
					move.Velocity = move.Velocity - movement * InputVelocity.Normal;
					move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
					move.Velocity += movement * InputVelocity.Normal;

				}
				else
				{
					move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
				}
			}
			else
			{
				GroundEntity = null;
				move.Velocity += Vector3.Down * 900 * timeDelta;
				Sandbox.Debug.Draw.Once.WithColor( Color.Red ).Circle( Position, Vector3.Up, 10.0f );
			}
		}

		Position = move.Position;
		Velocity = move.Velocity;
	}
}


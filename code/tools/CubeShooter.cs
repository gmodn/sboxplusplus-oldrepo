namespace Sandbox.Tools
{
	[Library( "tool_cubegun", Title = "Aperture Science Weighted Companion Cube Shooter", Description = "ASWCCS Will generate any number of Companions for you price free! (Do not let it get close to fire)", Group = "fun" )]
	public class CubeShooter : BaseTool
	{
		TimeSince timeSinceShoot;

		public override void Simulate()
		{
			if ( Host.IsServer )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					ShootBox();
				}

				if ( Input.Down( InputButton.Attack2 ) && timeSinceShoot > 0.05f )
				{
					timeSinceShoot = 0;
					ShootBox();
				}
			}
		}

		void ShootBox()
		{
			var ent = new Prop
			{
				Position = Owner.EyePos + Owner.EyeRot.Forward * 50,
				Rotation = Owner.EyeRot
			};

			ent.SetModel( "models/citizen_props/crate01.vmdl" );
			ent.Velocity = Owner.EyeRot.Forward * 1000;
		}
	}

}

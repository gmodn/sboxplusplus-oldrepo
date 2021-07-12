namespace Sandbox.Tools
{
	[Library( "tool_boxgun", Title = "Car Shooter", Description = "It's a hit and run", Group = "fun" )]
	public class CarShooter : BaseTool
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
			var ent = new CarEntity
			{
				Position = Owner.EyePos + Owner.EyeRot.Forward * 150,
				Rotation = Owner.EyeRot
			};

			ent.Velocity = Owner.EyeRot.Forward * 1000;
		}
	}

}

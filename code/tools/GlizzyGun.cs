namespace Sandbox.Tools
{
	[Library( "tool_glizzygun", Title = "Glizzy Gun", Description = "You're Favorite", Group = "fun" )]
	public class GlizyGun : BaseTool
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

				if ( Input.Down( InputButton.Attack2 ) && timeSinceShoot > 0f )
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

			ent.SetModel( "models/citizen_props/hotdog01.vmdl" );
			ent.Velocity = Owner.EyeRot.Forward * 1000;
		}
	}

}

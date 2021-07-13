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
					ShootGlizzy();
				}

				if ( Input.Down( InputButton.Attack2 ) && timeSinceShoot > 0.05f )
				{
					timeSinceShoot = 0;
					ShootGlizzy();
				}
			}
		}

		void ShootGlizzy()
		{
			var ent = new GlizzyEntity()
			{
				Position = Owner.EyePos + Owner.EyeRot.Forward * 150,
				Rotation = Owner.EyeRot
			};

			ent.Velocity = Owner.EyeRot.Forward * 1000;
			
			var player = Owner as SandboxPlayer;
			player.AddToUndo( ent );
		}
	}

}

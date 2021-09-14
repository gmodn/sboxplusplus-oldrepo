namespace Sandbox.Tools
{
	[Library( "tool_keygun", Title = "s&box key gen 2021", Description = "s&box key gen 100% virus free no torrent", Group = "fun" )]
	public class KeyGen: BaseTool
	{
		TimeSince timeSinceShoot;

		public override void Simulate()
		{
			if ( Host.IsServer )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					ShootKey();
				}

				if ( Input.Down( InputButton.Attack2 ) && timeSinceShoot > 0f )
				{
					timeSinceShoot = 0;
					ShootKey();
				}
			}
		}

		void ShootKey()
		{
			var ent = new KeyEntity()
			{
				Position = Owner.EyePos + Owner.EyeRot.Forward * 30,
				Rotation = Owner.EyeRot
			};

			ent.Velocity = Owner.EyeRot.Forward * 1000;
			
			var player = Owner as SandboxPlayer;
			player.AddToUndo( ent );
		}
	}

}

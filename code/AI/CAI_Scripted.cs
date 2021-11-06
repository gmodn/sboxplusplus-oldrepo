using Sandbox;

namespace CampaignBase.AI
{
	[Library( "scripted_sequence" )]
	[Hammer.EditorModel( "models/editor/scripted_sequence.vmdl" )]
	public partial class CAI_ScriptedSequence : Prop
	{
		[Property( Title = "Target" )]
		public string npcName { get; set; } = "";

		[Property( Title = "Always face final direction" )]
		public bool lookDir { get; set; } = false;

		[Property( Title = "Face final direction at end" )]
		public bool lookDirEnd { get; set; } = false;

		public CAI_BaseNpc target;

		public bool isSequence;

		public bool isCanceled;

		public int m_iTicks;

		public override void Spawn()
		{
			base.Spawn();

			EnableDrawing = false;
			EnableAllCollisions = false;
			PhysicsEnabled = false;
			SetModel( "models/editor/scripted_sequence.vmdl" );
		}

		[Input( "BeginSequence" )]
		public void beginSequence()
		{
			target = (CAI_BaseNpc)FindByName( npcName );

			if ( target != null )
			{
				target.Steer = new NavSteer();
				target.Steer.Target = Position;

				isSequence = true;
			}
			else
			{
				OnCancelFailedSequence.Fire( this );
			}
		}

		[Event.Tick]
		void Tick()
		{
			if(isSequence)
			{
				target = (CAI_BaseNpc)FindByName( npcName );
				if ( target != null )
				{
					if( lookDir || (lookDirEnd && m_iTicks > 0) )
						target.Rotation = Rotation.Lerp(target.Rotation,Rotation,.25f);

					if(m_iTicks==0)
					{
						if ( target.Steer.Output.Finished )
						{
							if ( lookDir || lookDirEnd )
								m_iTicks = 15;
							else
								endSequence();
						}
						else if( isCanceled )
						{
							isSequence = false;
							target.Steer.Target = target.Position;
							OnCancelSequence.Fire( this );
						}
					}
					else
					{
						if(m_iTicks>1)
							m_iTicks--;
						else
						{
							m_iTicks = 0;
							endSequence();
						}
					}
				}
				else
				{
					isSequence = false;
					OnCancelFailedSequence.Fire( this );
				}
			}
		}

		void endSequence()
		{
			isSequence = false;
			OnEndSequence.Fire( this );
		}

		[Input("CancelSequence")]
		public void cancelSequence()
		{
			isCanceled = true;
		}

		protected Output OnEndSequence { get; set; }

		/// <summary>
		/// Fires when the sequence failed.
		/// </summary>
		protected Output OnCancelFailedSequence { get; set; }

		/// <summary>
		/// Fires when the sequence was canceled.
		/// </summary>
		protected Output OnCancelSequence { get; set; }
	}
}

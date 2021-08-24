using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Sandbox.UI.Tests;
using System.Linq;

[Library]
public partial class WeaponList : Panel
{
	VirtualScrollPanel Canvas;
	
	public bool IsVehicle(Sandbox.LibraryAttribute x)
	{
		if (!x.Spawnable) return false;
		if (!x.Name.StartsWith("weapon_")) return false;
		return true;
	}

	public WeaponList()
	{
		AddClass( "spawnpage" );
		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemSize = new Vector2( 100, 100 );
		Canvas.OnCreateCell = ( cell, data ) =>
		{
			var entry = (LibraryAttribute)data;
			var btn = cell.Add.Button( entry.Title );
			btn.AddClass( "icon" );
			btn.AddEventListener( "onclick", () => ConsoleSystem.Run( "spawn_entity", entry.Name ) );
			btn.Style.Background = new PanelBackground
			{
				Texture = Texture.Load( $"icons/weapons/{entry.Name}.png", false )
			};
		};

		var guns = Library.GetAllAttributes<Entity>().Where( x => x.Spawnable ).OrderBy( x => x.Title ).ToArray();

		foreach ( var entry in guns )
		{
			Canvas.AddItem( entry );
		}
	}
}

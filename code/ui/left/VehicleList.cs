using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Sandbox.UI.Tests;
using System.Linq;

[Library]
public partial class VehicleList : Panel
{
	VirtualScrollPanel Canvas;
	
	public bool IsVehicle(Sandbox.LibraryAttribute x)
	{
		if (!x.Spawnable) return false;
		if (!x.Name.StartsWith("vehicle_")) return false;
		return true;
	}

	public VehicleList()
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
				Texture = Texture.Load( $"icons/vehicles/{entry.Name}.png", false )
			};
		};

		var cars = Library.GetAllAttributes<Entity>().Where( x => x.Spawnable ).OrderBy( x => x.Title ).ToArray();

		foreach ( var entry in cars )
		{
			Canvas.AddItem( entry );
		}
	}
}

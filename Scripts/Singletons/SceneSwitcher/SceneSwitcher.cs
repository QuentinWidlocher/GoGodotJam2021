using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Helpers;

public class SceneSwitcher : Node
{
    private readonly Dictionary<Scene, PackedScene> _sceneList = new Dictionary<Scene, PackedScene>
    {
        {Scene.Hub, GD.Load<PackedScene>("res://Scenes/Levels/Zones/Hub.tscn")},
        {Scene.FirstZone, GD.Load<PackedScene>("res://Scenes/Levels/Zones/FirstZone.tscn")},
        {Scene.SecondZone, GD.Load<PackedScene>("res://Scenes/Levels/Zones/SecondZone.tscn")},
    };

    private Node? _currentSceneNode;

    public void Switch(Scene sceneToSwitchTo, LoadingZone? loadingZone = null)
    {
        if (!_sceneList.ContainsKey(sceneToSwitchTo))
        {
            GD.PrintErr($"Could not switch to \"{sceneToSwitchTo}\" because it does not exist in the SceneSwitcher");
            return;
        }
        
        // We use CallDeferred so the current level can safely end what it's doing before changing
        CallDeferred(nameof(GoToScene), sceneToSwitchTo, loadingZone);
    }

    public void GoToScene(Scene sceneToGoTo, LoadingZone? loadingZone = null)
    {
        // We create the instance of our next scene 
        var newScene = _sceneList[sceneToGoTo].Instance();
        
        // We remove the current scene and add the new at its place
        GetTree().Root.RemoveChild(GetTree().CurrentScene);
        GetTree().Root.AddChild(newScene);
        
        // We tell godot this is the current scene now
        GetTree().CurrentScene = newScene;

        // We remember the now current scene
        _currentSceneNode = newScene;

        // If we're using a loading zone and it has the information to get a custom spawn point
        if (loadingZone?.FromId != null)
        {
            // We find a loading zone that has a ID connected to the one we used
            LoadingZone? loadingZoneTo = _currentSceneNode.FindInChildrenWhere<LoadingZone>(zone => zone.Id == loadingZone.FromId);

            if (loadingZoneTo != null)
            {
                // We find the player in the new scene
                Player? player = _currentSceneNode.FindInChildrenWhere<Player>();

                if (player != null)
                {
                    // If we find a connected loading zone, we place the player at its spawn point
                    var locationAtSpawn = player.Transform;
                    locationAtSpawn.origin = loadingZoneTo.Spawn.GlobalTransform.origin;
                    player.Transform = locationAtSpawn;      
                }
            }
        }
    }
}
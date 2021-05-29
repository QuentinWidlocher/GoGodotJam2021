using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Helpers;
using static Helpers.TaskHelpers;

public class SceneSwitcher : Node
{
    private readonly Dictionary<Scene, PackedScene> _sceneList = new Dictionary<Scene, PackedScene>
    {
        {Scene.MainMenu, GD.Load<PackedScene>("res://Scenes/UI/Menu/MainMenu.tscn")},
        {Scene.Hub, GD.Load<PackedScene>("res://Scenes/Levels/Zones/Hub.tscn")},
        {Scene.FirstZone, GD.Load<PackedScene>("res://Scenes/Levels/Zones/FirstZone.tscn")},
        {Scene.SecondZone, GD.Load<PackedScene>("res://Scenes/Levels/Zones/SecondZone.tscn")},
    };

    public Scene? CurrentScene;
    private Node? _currentSceneNode;
    private SaveSystem _saveSystem = null!;

    public override void _Ready()
    {
        _saveSystem = GetNode<SaveSystem>("/root/SaveSystem");
    }

    public void Switch(Scene sceneToSwitchTo, string? loadingZoneFromId = null)
    {
        if (!_sceneList.ContainsKey(sceneToSwitchTo))
        {
            GD.PrintErr($"Could not switch to \"{sceneToSwitchTo}\" because it does not exist in the SceneSwitcher");
            return;
        }
        
        // We use CallDeferred so the current level can safely end what it's doing before changing
        CallDeferred(nameof(GoToScene), sceneToSwitchTo, loadingZoneFromId);
    }
    

    private void GoToScene(Scene sceneToGoTo, string? loadingZoneFromId = null)
    {
        GD.Print("GoToScene | ", sceneToGoTo, ", ", loadingZoneFromId);
        // We create the instance of our next scene 
        var newScene = _sceneList[sceneToGoTo].Instance();
        
        // We remove the current scene and add the new at its place
        GetTree().Root.RemoveChild(GetTree().CurrentScene);
        GetTree().Root.AddChild(newScene);
        
        // We tell godot this is the current scene now
        GetTree().CurrentScene = newScene;

        // We remember the now current scene
        _currentSceneNode = newScene;
        CurrentScene = sceneToGoTo;

        // If we're using a loading zone and it has the information to get a custom spawn point
        if (loadingZoneFromId != null)
        {
            // We find a loading zone that has a ID connected to the one we used
            LoadingZone? loadingZoneTo = _currentSceneNode.FindInChildrenWhere<LoadingZone>(zone => zone.Id == loadingZoneFromId, true);

            if (loadingZoneTo != null)
            {
                // We find the player in the new scene
                Player player = GetNode<Player>("/root/Player");
                
                // If we find a connected loading zone, we place the player at its spawn point
                player.Position = loadingZoneTo.Spawn.GlobalTransform.origin;

                var camera = player.FindInChildren<Camera2D>();
                if (camera != null)
                {
                    camera.SmoothingEnabled = false;
                    RunAfterDelay(() => camera.SmoothingEnabled = true, 100);
                }
            }
        }
        
        if (CurrentScene != Scene.MainMenu)
            _saveSystem.Save();
    }
}
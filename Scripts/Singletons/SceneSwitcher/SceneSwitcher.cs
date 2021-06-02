using System.Collections.Generic;
using Godot;
using Helpers;

public class SceneSwitcher : Node
{
    private readonly Dictionary<Scene, PackedScene> _sceneList = new Dictionary<Scene, PackedScene>
    {
        {Scene.MainMenu, GD.Load<PackedScene>("res://Scenes/UI/Menu/MainMenu.tscn")},
        {Scene.Guide, GD.Load<PackedScene>("res://Scenes/UI/HelpScreen.tscn")},
        {Scene.IdleSystem, GD.Load<PackedScene>("res://Scenes/UI/Idle/IdleMenu.tscn")},
        {Scene.Hub, GD.Load<PackedScene>("res://Scenes/Levels/Zones/Hub.tscn")},
        {Scene.FirstZone, GD.Load<PackedScene>("res://Scenes/Levels/Zones/FirstZone.tscn")},
        {Scene.GameEnd, GD.Load<PackedScene>("res://Scenes/Levels/WinScreen.tscn")},
    };

    public Scene? CurrentScene;
    private Node? _currentSceneNode;
    private AnimationPlayer _transition = null!;
    
    private readonly float _maxSwitchDelay = 0.75f;
    private float _switchDelay;
    
    private readonly float _maxFadeDelay = 1.5f;
    private float _fadeDelay;
    
    private float _smoothingDelay;
    private float _smoothingMaxDelay = 0.1f;

    public override void _Ready()
    {
        _transition = GetNode<CanvasLayer>("/root/Transition").GetNode<AnimationPlayer>("AnimationPlayer");
        PauseMode = PauseModeEnum.Process;
    }

    private Scene _sceneToSwitchTo = Scene.Hub;
    private string? _loadingZoneToId;

    public override void _Process(float delta)
    {
        if (_switchDelay > 0)
        {
            _switchDelay -= delta;

            if (_switchDelay <= 0)
            {
                _switchDelay = 0;
                CallDeferred(nameof(GoToScene), _sceneToSwitchTo, _loadingZoneToId);
            }
        }
        
        if (_fadeDelay > 0)
        {
            _fadeDelay -= delta;

            if (_fadeDelay <= 0)
            {
                _fadeDelay = 0;
                GetTree().Paused = false;
            }
        }
        
        if (_smoothingDelay > 0)
        {
            _smoothingDelay -= delta;

            if (_smoothingDelay <= 0)
            {
                GetNode<Player>("/root/Player").FindInChildren<Camera2D>().SmoothingEnabled = true;
            }
        }
    }

    public void Switch(Scene sceneToSwitchTo, string? loadingZoneToId = null)
    {
        if (!_sceneList.ContainsKey(sceneToSwitchTo))
        {
            GD.PrintErr($"Could not switch to \"{sceneToSwitchTo}\" because it does not exist in the SceneSwitcher");
            return;
        }

        GetTree().Paused = true;
        _transition.Stop();
        _transition.Play("Transition");

        _fadeDelay = _maxFadeDelay;
        _switchDelay = _maxSwitchDelay;
        _sceneToSwitchTo = sceneToSwitchTo;
        _loadingZoneToId = loadingZoneToId;
    }


    private void GoToScene(Scene sceneToGoTo, string? loadingZoneToId = null)
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
        CurrentScene = sceneToGoTo;

        // If we're using a loading zone and it has the information to get a custom spawn point
        if (loadingZoneToId != null)
        {
            // We find a loading zone that has a ID connected to the one we used
            LoadingZone? loadingZoneTo =
                _currentSceneNode.FindInChildrenWhere<LoadingZone>(zone => zone.Id == loadingZoneToId, true);

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

                    _smoothingDelay = _smoothingMaxDelay;
                }
            }
        }
    }
}
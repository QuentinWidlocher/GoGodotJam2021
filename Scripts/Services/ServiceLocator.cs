using Godot;

public class ServiceLocator : Node
{
    public static IdleSystem IdleSystemService = null!;
    public static StatSystem StatSystemService = null!;
    public static SceneSwitcher SceneSwitcherService = null!;
    public static SaveSystem SaveSystemService = null!;

    public static Player PlayerInstance = null!;

    public override void _Ready()
    {
        IdleSystemService = GetNode<IdleSystem>("/root/IdleSystem");
        StatSystemService = GetNode<StatSystem>("/root/StatSystem");
        SceneSwitcherService = GetNode<SceneSwitcher>("/root/SceneSwitcher");
        SaveSystemService = GetNode<SaveSystem>("/root/SaveSystem");
        
        PlayerInstance = GetNode<Player>("/root/Player");
    }
}
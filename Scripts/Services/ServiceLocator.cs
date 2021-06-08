using Godot;

public class ServiceLocator : Node
{
    public static IdleSystem IdleSystemService;
    public static StatSystem StatSystemService;
    public static SceneSwitcher SceneSwitcherService;
    public static SaveSystem SaveSystemService;

    public static Player PlayerInstance;

    public ServiceLocator()
    {
        IdleSystemService = GetNode<IdleSystem>("/root/IdleSystem");
        StatSystemService = GetNode<StatSystem>("/root/StatSystem");
        SceneSwitcherService = GetNode<SceneSwitcher>("/root/SceneSwitcher");
        SaveSystemService = GetNode<SaveSystem>("/root/SaveSystem");
        
        PlayerInstance = GetNode<Player>("/root/Player");
    }
}
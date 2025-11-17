using Godot;

public partial class Ui : CanvasLayer
{
    // Référence au FactoryManager
    private FactoryManager _factoryManager;
    
    // Chemins vers les scènes de bâtiments
    private const string DrillScenePath = "res://Factory/Scenes/Buildings/Drill.tscn";
    private const string FurnaceScenePath = "res://Factory/Scenes/Buildings/Furnace.tscn";
    private const string FactoryScenePath = "res://Factory/Scenes/Buildings/Factory.tscn";

    

    
    
    public override void _Ready()
    {
        // Récupérer la référence au FactoryManager
        _factoryManager = GetNode<FactoryManager>("../FactoryManager");

        // Connecter les boutons (à faire depuis l'éditeur ou en code)
        ConnectButtons();
    }
    
    private void ConnectButtons()
    {
        // Si tu as créé des boutons dans l'éditeur, connecte-les ici
        Button drillButton = GetNode<Button>("BuildingButtons/DrillButton");
        Button furnaceButton = GetNode<Button>("BuildingButtons/FurnaceButton");
        Button factoryButton = GetNode<Button>("BuildingButtons/FactoryButton");
        
        drillButton.Pressed += OnDrillButtonPressed;
        furnaceButton.Pressed += OnFurnaceButtonPressed;
        factoryButton.Pressed += OnFactoryButtonPressed;
    }

    private void OnDrillButtonPressed()
    {
        // Charger la scène du bâtiment Drill
        PackedScene buildingScene = GD.Load<PackedScene>(DrillScenePath);

        // Activer le mode placement dans le FactoryManager
        _factoryManager.StartPlacement(buildingScene);
        GD.Print("Mode placement : Foreuse");
        _factoryManager.BuildingType = "Drill";
    }

    private void OnFurnaceButtonPressed()
    {
        PackedScene buildingScene = GD.Load<PackedScene>(FurnaceScenePath);
        _factoryManager.StartPlacement(buildingScene);
        GD.Print("Mode placement : Four");
        _factoryManager.BuildingType = "Furnace";
    }

    private void OnFactoryButtonPressed()
    {
        PackedScene buildingScene = GD.Load<PackedScene>(FactoryScenePath);
        _factoryManager.StartPlacement(buildingScene);
        GD.Print("Mode placement : Usine");
        _factoryManager.BuildingType = "Factory";
    }
}
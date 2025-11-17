
using Godot;
using System.Collections.Generic;
using System;
using test.Building;

public partial class FactoryManager : Node2D
{
    [Export] public int GridWidth = 5;
    [Export] public int GridHeight = 5;
    [Export] public int CellSize = 64;

    // Dictionnaire pour stocker les bâtiments sur la grille
    private Dictionary<Vector2I, Building> _grid = new Dictionary<Vector2I, Building>();

    // Référence au bâtiment en cours de placement (preview)
    private PackedScene _currentBuildingScene;
    private bool _isPlacementMode = false;

    // Sprite pour visualiser la grille
    private Sprite2D _previewSprite;
    private bool _canPlace = true;

    private string currentBuildingType;
    public string BuildingType = "None";



    // Delimitation des zones
    private (int MinX, int MaxY, int MaxX, int MinY, string Type)[] Zones = {
        (-4, 2, 0, -3, "Drill"),
        (-5, 5, -4, 4, "Factory"),
        (2, 2, 4, 1, "Furnace")
        
    };

    public override void _Ready()
    {
        // Créer le sprite de preview
        _previewSprite = new Sprite2D();
        _previewSprite.Modulate = new Color(1, 1, 1, 0.5f);
        _previewSprite.Visible = false;
        _previewSprite.Scale = new Vector2(0.5f, 0.5f);
        AddChild(_previewSprite);

    }

    public override void _Process(double delta)
    {
        if (_isPlacementMode)
        {
            UpdatePreview();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left && _isPlacementMode)
            {
                TryPlaceBuilding();
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Right)
            {
                CancelPlacement();
            }
        }
    }

    // Convertit une position du monde en coordonnées de grille
    public Vector2I WorldToGrid(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.X / CellSize);
        int y = Mathf.FloorToInt(worldPos.Y / CellSize);
        return new Vector2I(x, y);
    }

    // Convertit des coordonnées de grille en position du monde (centre de la cellule)
    public Vector2 GridToWorld(Vector2I gridPos)
    {

        return new Vector2(gridPos.X * CellSize + CellSize / 2, gridPos.Y * CellSize + CellSize / 2);
    }

    // Vérifie si une position est valide et libre
    public bool IsPositionValid(Vector2I gridPos)
    {

        // Vérifier si la case est occupée
        if (_grid.ContainsKey(gridPos))
            return false;

        //Vérifier les limites des zones
        for (int e = 0; e < Zones.Length; e++)
        {
            var zone = Zones[e];
            if (gridPos.X >= zone.MinX && gridPos.X < zone.MaxX && gridPos.Y >= zone.MinY && gridPos.Y < zone.MaxY && BuildingType == zone.Type)
                return true;
        }


        return false;
    }

    // Active le mode placement avec un type de bâtiment
    public void StartPlacement(PackedScene buildingScene)
    {
        _isPlacementMode = true;
        _currentBuildingScene = buildingScene;

        // Créer un preview temporaire pour obtenir la texture
        var temp = buildingScene.Instantiate<Building>();
        AddChild(temp);
        currentBuildingType = temp.BuildingType;

        // Attendre que _Ready() soit appelé
        if (temp.HasNode("Sprite2D"))
        {
            Sprite2D tempSprite = temp.GetNode<Sprite2D>("Sprite2D");
            _previewSprite.Texture = tempSprite.Texture;
        }

        _previewSprite.Visible = true;

        // Libérer le preview temporaire
        temp.QueueFree();
    }

    // Met à jour la position du preview
    private void UpdatePreview()
    {
        Vector2 mousePos = GetGlobalMousePosition();
        Vector2I gridPos = WorldToGrid(mousePos);
        Vector2 worldPos = GridToWorld(gridPos);

        _previewSprite.GlobalPosition = worldPos;

        // Changer la couleur selon si le placement est valide
        _canPlace = IsPositionValid(gridPos);
        _previewSprite.Modulate = _canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
    }

    // Tente de placer le bâtiment
    private void TryPlaceBuilding()
    {
        Vector2 mousePos = GetGlobalMousePosition();
        Vector2I gridPos = WorldToGrid(mousePos);

        if (!_canPlace)
        {
            GD.Print("Position invalide !");

            return;
        }

        // Créer le vrai bâtiment
        Building building = _currentBuildingScene.Instantiate<Building>();

        if (PlaceBuilding(building, gridPos))
        {
            GD.Print($"Bâtiment '{building.BuildingName}' placé à {gridPos}");
        }

        CancelPlacement();
    }

    // Place un bâtiment à une position donnée
    public bool PlaceBuilding(Building building, Vector2I gridPos)
    {
        if (!IsPositionValid(gridPos))
            return false;

        _grid[gridPos] = building;
        building.GlobalPosition = GridToWorld(gridPos);
        building.GridPosition = gridPos;
        AddChild(building);

        return true;
    }

    // Retire un bâtiment
    public void RemoveBuilding(Vector2I gridPos)
    {
        if (_grid.ContainsKey(gridPos))
        {
            Building building = _grid[gridPos];
            _grid.Remove(gridPos);
            building.QueueFree();
        }
    }

    // Annule le placement en cours
    private void CancelPlacement()
    {
        _isPlacementMode = false;
        _previewSprite.Visible = false;
    }

    public override void _Draw()
    {
        // Couleur Grille (Vert/Jaune moche)
        Color gridColor = new Color(0.3f, 1, 0.5f);

        for (int e = 0; e < Zones.Length; e++)
        {
            // Ca marche mais cherche pas pk
            // PSP
            var zone = Zones[e];
            int left = Math.Min(zone.MinX, zone.MaxX);
            int right = Math.Max(zone.MinX, zone.MaxX);
            int top = Math.Min(zone.MinY, zone.MaxY);
            int bottom = Math.Max(zone.MinY, zone.MaxY);

            for (int x = left; x <= right; x++)
            {
                Vector2 from = new Vector2(x * CellSize, top * CellSize);
                Vector2 to = new Vector2(x * CellSize, bottom * CellSize);
                DrawLine(from, to, gridColor, 2);
            }

            for (int y = top; y <= bottom; y++)
            {
                Vector2 from = new Vector2(left * CellSize, y * CellSize);
                Vector2 to = new Vector2(right * CellSize, y * CellSize);
                DrawLine(from, to, gridColor, 2);
            }
        }
    }
}
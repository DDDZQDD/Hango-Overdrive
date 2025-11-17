using Godot;
namespace test.Building;

public partial class Building : Node2D
{
    [Export] public string BuildingName = "Bâtiment";
    [Export] public string BuildingType = "Generic"; // Drill, Furnace, Factory, etc.
    
    // Position sur la grille
    public Vector2I GridPosition { get; set; }
    
    // Référence au sprite (peut être null si pas encore initialisé)
    private Sprite2D _sprite;
    
    public override void _Ready()
    {
        // Vérifier si le Sprite2D existe
        if (HasNode("Sprite2D"))
        {
            _sprite = GetNode<Sprite2D>("Sprite2D");
        }
        else
        {
            GD.PushWarning($"Building '{BuildingName}' n'a pas de node Sprite2D enfant");
        }
    }
    
    // Méthode appelée quand le bâtiment est sélectionné
    public virtual void OnSelected()
    {
        if (_sprite != null)
        {
            // Ajouter un effet visuel de sélection
            _sprite.Modulate = new Color(1.2f, 1.2f, 1.2f);
        }
    }
    
    // Méthode appelée quand le bâtiment est désélectionné
    public virtual void OnDeselected()
    {
        if (_sprite != null)
        {
            _sprite.Modulate = Colors.White;
        }
    }
    
    // Méthode pour obtenir le sprite (utile pour le preview)
    public Sprite2D GetSprite()
    {
        return _sprite;
    }
}
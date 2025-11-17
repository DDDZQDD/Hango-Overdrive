using Godot;
using System;
public partial class Camera2d : Camera2D
    {
    float mouseSpeed = 0.01f;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
        {
        
        }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
        {
        if (Input.IsMouseButtonPressed(MouseButton.Middle))
            {
                var mousePosition = GetGlobalMousePosition();
                System.Threading.Thread.Sleep(10);
                Position += mousePosition - GetGlobalMousePosition();
            }
        }
    }
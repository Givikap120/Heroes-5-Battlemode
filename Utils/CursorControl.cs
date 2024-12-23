using Godot;
using Godot.Collections;

public partial class CursorHandler : Node
{
    public static CursorHandler Instance { get; private set; } = null!;

    private Cursor cursor = Cursor.Default;
    public Cursor InstanceCursor 
    {
        get => cursor;
        set
        {
            if (value == cursor) return;
            cursor = value;
            updateCursor(cursor);
        }
    }

    public static Cursor Cursor { get => Instance.InstanceCursor; set =>  Instance.InstanceCursor = value; }

    private Dictionary<Cursor, Texture2D> cursorTextures = [];

    public override void _EnterTree()
    {
        if (Instance != null)
        {
            GD.PrintErr("Multiple CursorControl instances detected. This is not allowed.");
            return;
        }
        Instance = this;
    }

    public override void _Ready()
    {
        base._Ready();
        loadTextures();
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Instance = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }

    void updateCursor(Cursor cursor) => Input.SetCustomMouseCursor(cursorTextures[cursor]);

    private void loadTextures()
    {
        cursorTextures[Cursor.Default] = GD.Load<Texture2D>("res://Assets/Cursors/Default.png");
        cursorTextures[Cursor.CantCast] = GD.Load<Texture2D>("res://Assets/Cursors/CantCast.png");
        cursorTextures[Cursor.Move] = GD.Load<Texture2D>("res://Assets/Cursors/Move.png");
        cursorTextures[Cursor.Ability] = GD.Load<Texture2D>("res://Assets/Cursors/Ability.png");
        cursorTextures[Cursor.Cast] = GD.Load<Texture2D>("res://Assets/Cursors/Cast.png");
        cursorTextures[Cursor.Shoot] = GD.Load<Texture2D>("res://Assets/Cursors/Shoot.png");
        cursorTextures[Cursor.ShootPenalty] = GD.Load<Texture2D>("res://Assets/Cursors/Shoot_Penalty.png");
        cursorTextures[Cursor.AttackUp] = GD.Load<Texture2D>("res://Assets/Cursors/Attack_Up.png");
        cursorTextures[Cursor.AttackDown] = GD.Load<Texture2D>("res://Assets/Cursors/Attack_Down.png");
        cursorTextures[Cursor.AttackRight] = GD.Load<Texture2D>("res://Assets/Cursors/Attack_Right.png");
        cursorTextures[Cursor.AttackLeft] = GD.Load<Texture2D>("res://Assets/Cursors/Attack_Left.png");
        cursorTextures[Cursor.AttackRightUp] = GD.Load<Texture2D>("res://Assets/Cursors/Attack_RightUp.png");
        cursorTextures[Cursor.AttackRightDown] = GD.Load<Texture2D>("res://Assets/Cursors/Attack_RightDown.png");
        cursorTextures[Cursor.AttaclLeftUp] = GD.Load<Texture2D>("res://Assets/Cursors/Attack_LeftUp.png");
        cursorTextures[Cursor.AttaclLeftDown] = GD.Load<Texture2D>("res://Assets/Cursors/Attack_LeftDown.png");
    }
}

public enum Cursor
{
    Default,
    CantCast,
    Move,
    Ability,
    Cast,
    Shoot,
    ShootPenalty,
    AttackUp,
    AttackDown,
    AttackRight,
    AttackLeft,
    AttackRightUp,
    AttackRightDown,
    AttaclLeftUp,
    AttaclLeftDown,
}

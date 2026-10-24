using Godot;

public partial class GameManager : Node
{
    public enum GameState
    {
        MainMenu,
        Level,
        GameOver,
        Options
    }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    private GameState _previousState;
    private Control _optionsInstance;

    public int Score { get; private set; } = 0;
    public int Lives { get; private set; } = 3;

    [Signal]
    public delegate void ScoreChangedEventHandler(int newScore);

    [Signal]
    public delegate void LivesChangedEventHandler(int newLives);

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed("ui_cancel"))
            return;

        if (CurrentState == GameState.Level)
        {
            OpenOptions();
            GetViewport().SetInputAsHandled();
        }
        else if (CurrentState == GameState.Options)
        {
            CloseOptions();
            GetViewport().SetInputAsHandled();
        }
    }

    public void AddScore(int amount)
    {
        Score += amount;
        EmitSignal(SignalName.ScoreChanged, Score);
    }

    public void LoseLife()
    {
        if (Lives <= 0)
            return;

        Lives--;
        EmitSignal(SignalName.LivesChanged, Lives);

        if (Lives == 0)
        {
            Callable.From(() =>
                ChangeState(GameState.GameOver)
            ).CallDeferred();
        }
    }

    public void StartGame()
    {
        Score = 0;
        Lives = 3;

        EmitSignal(SignalName.ScoreChanged, Score);
        EmitSignal(SignalName.LivesChanged, Lives);

        ChangeState(GameState.Level);
    }

    public void ChangeState(GameState newState)
    {
        if (newState == GameState.Options)
        {
            OpenOptions();
            return;
        }

        if (_optionsInstance != null)
            CloseOptions();

        string scenePath = newState switch
        {
            GameState.MainMenu => "res://Scenes/MainMenu.tscn",
            GameState.Level => "res://Scenes/Levels/Level.tscn",
            GameState.GameOver => "res://Scenes/GameOver.tscn",
            _ => ""
        };

        if (scenePath == "")
        {
            GD.PushError($"Unknown state: {newState}");
            return;
        }

        Error result = GetTree().ChangeSceneToFile(scenePath);

        if (result == Error.Ok)
        {
            CurrentState = newState;
        }
        else
        {
            GD.PushError($"Failed to load scene: {scenePath}");
        }
    }

    public void OpenOptions()
    {
        if (_optionsInstance != null)
            return;

        PackedScene optionsScene =
            GD.Load<PackedScene>("res://Scenes/Options.tscn");

        if (optionsScene == null)
        {
            GD.PushError("Options scene could not be loaded.");
            return;
        }

        _previousState = CurrentState;

        _optionsInstance = optionsScene.Instantiate<Control>();
        _optionsInstance.ProcessMode = ProcessModeEnum.Always;

        GetTree().Root.AddChild(_optionsInstance);

        CurrentState = GameState.Options;
        GetTree().Paused = true;
    }

    public void CloseOptions()
    {
        if (_optionsInstance == null)
            return;

        GetTree().Paused = false;

        _optionsInstance.QueueFree();
        _optionsInstance = null;

        CurrentState = _previousState;
    }
}

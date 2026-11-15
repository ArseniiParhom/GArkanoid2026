using Godot;

public partial class GameManager : Node
{
    public enum GameState
    {
        MainMenu,
        Level,
        GameOver,
        Options,
        Pause
    }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    private GameState _previousState;
    private Control _optionsInstance;

    private AudioStreamPlayer _musicPlayer;
    private AudioStream _mainMenuMusic;
    private AudioStream _gameplayMusic;

    public int Score { get; private set; } = 0;
    public int Lives { get; private set; } = 3;

    [Signal]
    public delegate void ScoreChangedEventHandler(int newScore);

    [Signal]
    public delegate void LivesChangedEventHandler(int newLives);

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        _musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");

        _mainMenuMusic = GD.Load<AudioStream>(
            "res://Audio/Music/main_menu.ogg"
        );

        _gameplayMusic = GD.Load<AudioStream>(
            "res://Audio/Music/gameplay.ogg"
        );

        UpdateMusic(CurrentState);
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
        else if (
            CurrentState == GameState.Options ||
            CurrentState == GameState.Pause
        )
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
        if (
            newState == GameState.Options ||
            newState == GameState.Pause
        )
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
            UpdateMusic(newState);
        }
        else
        {
            GD.PushError($"Failed to load scene: {scenePath}");
        }
    }

    private void UpdateMusic(GameState state)
    {
        AudioStream nextTrack = state switch
        {
            GameState.MainMenu => _mainMenuMusic,
            GameState.Level => _gameplayMusic,
            _ => null
        };

        // Keep the current track if it is already playing
        if (_musicPlayer.Stream == nextTrack)
        {
            if (nextTrack != null && !_musicPlayer.Playing)
                _musicPlayer.Play();

            return;
        }

        _musicPlayer.Stop();
        _musicPlayer.Stream = nextTrack;

        if (nextTrack != null)
            _musicPlayer.Play();
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

        CurrentState = _previousState == GameState.Level
            ? GameState.Pause
            : GameState.Options;

        _optionsInstance = optionsScene.Instantiate<Control>();
        _optionsInstance.ProcessMode = ProcessModeEnum.Always;

        GetTree().Root.AddChild(_optionsInstance);
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

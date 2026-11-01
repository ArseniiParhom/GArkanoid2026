using Godot;

public partial class Options : Control
{
    private Button _closeButton;
    private Button _backToMainMenuButton;
    private GameManager _gameManager;

    private OptionButton _resolutionOptionButton;
    private CheckBox _fullscreenCheckBox;

    private static readonly Vector2I[] Resolutions =
    {
        new Vector2I(640, 360),
        new Vector2I(1280, 720),
        new Vector2I(1920, 1080)
    };

    private static Vector2I _windowedSize = new Vector2I(640, 360);

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("/root/GameManager");

        string basePath = "CenterContainer/VBoxContainer/";

        _closeButton = GetNode<Button>(
            basePath + "CloseButton"
        );

        _backToMainMenuButton = GetNode<Button>(
            basePath + "BackToMainMenuButton"
        );

        _closeButton.Pressed += OnCloseButtonPressed;
        _backToMainMenuButton.Pressed += OnBackToMainMenuPressed;

        SetupAudioSlider(
            "Master",
            "MasterRow",
            "MasterSlider",
            "MasterValueLabel"
        );

        SetupAudioSlider(
            "Music",
            "MusicRow",
            "MusicSlider",
            "MusicValueLabel"
        );

        SetupAudioSlider(
            "SFX",
            "SfxRow",
            "SfxSlider",
            "SfxValueLabel"
        );

        SetupVideoControls();
    }

    private void SetupAudioSlider(
        string busName,
        string rowName,
        string sliderName,
        string valueLabelName)
    {
        string path =
            $"CenterContainer/VBoxContainer/AudioSection/{rowName}/";

        HSlider slider = GetNode<HSlider>(path + sliderName);
        Label valueLabel = GetNode<Label>(path + valueLabelName);

        int busIndex = AudioServer.GetBusIndex(busName);

        if (busIndex < 0)
        {
            GD.PushError($"Audio bus not found: {busName}");
            return;
        }

        int currentVolume = AudioServer.IsBusMute(busIndex)
            ? 0
            : Mathf.Clamp(
                Mathf.RoundToInt(
                    AudioServer.GetBusVolumeLinear(busIndex) * 100f
                ),
                0,
                100
            );

        slider.SetValueNoSignal(currentVolume);
        valueLabel.Text = currentVolume.ToString();

        slider.ValueChanged += value =>
        {
            int volume = (int)value;

            valueLabel.Text = volume.ToString();

            AudioServer.SetBusMute(busIndex, volume == 0);

            if (volume > 0)
            {
                AudioServer.SetBusVolumeLinear(
                    busIndex,
                    volume / 100f
                );
            }
        };
    }

    private void SetupVideoControls()
    {
        string path = "CenterContainer/VBoxContainer/VideoSection/";

        _resolutionOptionButton = GetNode<OptionButton>(
            path + "ResolutionOptionButton"
        );

        _fullscreenCheckBox = GetNode<CheckBox>(
            path + "FullscreenCheckBox"
        );

        DisplayServer.WindowMode currentMode =
            DisplayServer.WindowGetMode();

        bool isFullscreen =
            currentMode == DisplayServer.WindowMode.Fullscreen ||
            currentMode == DisplayServer.WindowMode.ExclusiveFullscreen;

        if (!isFullscreen)
        {
            _windowedSize = DisplayServer.WindowGetSize();
        }

        int selectedIndex = 0;

        for (int i = 0; i < Resolutions.Length; i++)
        {
            if (Resolutions[i] == _windowedSize)
            {
                selectedIndex = i;
                break;
            }
        }

        _resolutionOptionButton.Select(selectedIndex);
        _resolutionOptionButton.Disabled = isFullscreen;

        _fullscreenCheckBox.SetPressedNoSignal(isFullscreen);

        _resolutionOptionButton.ItemSelected += OnResolutionSelected;
        _fullscreenCheckBox.Toggled += OnFullscreenToggled;
    }

    private void OnResolutionSelected(long index)
    {
        if (index < 0 || index >= Resolutions.Length)
            return;

        _windowedSize = Resolutions[(int)index];

        DisplayServer.WindowSetSize(_windowedSize);
        GetWindow().MoveToCenter();
    }

    private void OnFullscreenToggled(bool enabled)
    {
        if (enabled)
        {
            _windowedSize = DisplayServer.WindowGetSize();

            DisplayServer.WindowSetMode(
                DisplayServer.WindowMode.Fullscreen
            );
        }
        else
        {
            DisplayServer.WindowSetMode(
                DisplayServer.WindowMode.Windowed
            );

            DisplayServer.WindowSetFlag(
                DisplayServer.WindowFlags.Borderless,
                false
            );

            DisplayServer.WindowSetSize(_windowedSize);
            GetWindow().MoveToCenter();
        }

        _resolutionOptionButton.Disabled = enabled;
    }

    private void OnBackToMainMenuPressed()
    {
        _gameManager.ChangeState(GameManager.GameState.MainMenu);
    }

    private void OnCloseButtonPressed()
    {
        _gameManager.CloseOptions();
    }
}

using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class OptionsMenu : BaseUI
{
    [SerializeField] private PlayerSoundSO soundSO;

    private Slider _masterVolumeSlider;
    private Slider _sfxVolumeSlider;
    private Slider _musicVolumeSlider;

    private Button _closeButton;

    private void OnEnable()
    {
        UI.Events.UIEventBus.OnOptionsButtonClicked += PushToStack;
    }

    private void OnDisable()
    {
        UI.Events.UIEventBus.OnOptionsButtonClicked -= PushToStack;

        Unregister();
    }

    public override void OnOpen()
    {
        base.OnOpen();
        Register();
    }

    public override void OnClose()
    {
        base.OnClose();
        Unregister();
    }

    private void Register()
    {
        _masterVolumeSlider = Container.Q<Slider>("MasterVolumeSlider");
        _sfxVolumeSlider = Container.Q<Slider>("SFXVolumeSlider");
        _musicVolumeSlider = Container.Q<Slider>("MusicVolumeSlider");
        _closeButton = Container.Q<Button>("CloseButton");

        // Push current SO values onto the sliders before wiring up callbacks
        _masterVolumeSlider?.SetValueWithoutNotify(soundSO.masterVolume);
        _sfxVolumeSlider?.SetValueWithoutNotify(soundSO.sfxVolume);
        _musicVolumeSlider?.SetValueWithoutNotify(soundSO.musicVolume);

        _masterVolumeSlider?.RegisterValueChangedCallback(OnSliderValueChanged);
        _sfxVolumeSlider?.RegisterValueChangedCallback(OnSliderValueChanged);
        _musicVolumeSlider?.RegisterValueChangedCallback(OnSliderValueChanged);
        _masterVolumeSlider?.RegisterCallback<PointerUpEvent>(_ => PlayerPrefs.Save());
        _sfxVolumeSlider?.RegisterCallback<PointerUpEvent>(_ => PlayerPrefs.Save());
        _musicVolumeSlider?.RegisterCallback<PointerUpEvent>(_ => PlayerPrefs.Save());


        _closeButton?.RegisterCallback<ClickEvent>(OnCloseOptionsButtonClicked);
        _closeButton?.RegisterCallback<MouseEnterEvent>(OnCloseButtonHovered);

        // If soundSO changes from elsewhere (e.g. a "reset to default" button), reflect it in UI.
        soundSO.OnMasterVolumeChanged += OnMasterChangedExternally;
        soundSO.OnSFXVolumeChanged += OnSFXChangedExternally;
        soundSO.OnMusicVolumeChanged += OnMusicChangedExternally;
    }

    private void Unregister()
    {
        _masterVolumeSlider?.UnregisterValueChangedCallback(OnSliderValueChanged);
        _sfxVolumeSlider?.UnregisterValueChangedCallback(OnSliderValueChanged);
        _musicVolumeSlider?.UnregisterValueChangedCallback(OnSliderValueChanged);

        _closeButton?.UnregisterCallback<ClickEvent>(OnCloseOptionsButtonClicked);
        _closeButton?.UnregisterCallback<MouseEnterEvent>(OnCloseButtonHovered);
    }

    private void OnSliderValueChanged(ChangeEvent<float> evt)
    {
        if (evt.target == _masterVolumeSlider)
        {
            soundSO.SetMasterVolume(evt.newValue);
        }
        else if (evt.target == _sfxVolumeSlider)
        {
            soundSO.SetSFXVolume(evt.newValue);
        }
        else if (evt.target == _musicVolumeSlider)
        {
            soundSO.SetMusicVolume(evt.newValue);
        }
    }

    private void OnMasterChangedExternally(float v) => _masterVolumeSlider.SetValueWithoutNotify(v);
    private void OnSFXChangedExternally(float v) => _sfxVolumeSlider.SetValueWithoutNotify(v);
    private void OnMusicChangedExternally(float v) => _musicVolumeSlider.SetValueWithoutNotify(v);

    private void OnCloseButtonHovered(MouseEnterEvent evt)
    {
        //SFX
    }

    private void OnCloseOptionsButtonClicked(ClickEvent evt)
    {
        UIManager.Instance.Pop(this);
    }

}

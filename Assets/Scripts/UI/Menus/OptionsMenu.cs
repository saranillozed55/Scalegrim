using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class OptionsMenu : BaseUI
{

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


        _masterVolumeSlider?.RegisterValueChangedCallback(OnSliderValueChanged);
        _sfxVolumeSlider?.RegisterValueChangedCallback(OnSliderValueChanged);
        _musicVolumeSlider?.RegisterValueChangedCallback(OnSliderValueChanged);


        _closeButton?.RegisterCallback<ClickEvent>(OnCloseOptionsButtonClicked);
        _closeButton?.RegisterCallback<MouseEnterEvent>(OnCloseButtonHovered);
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
        if(evt.target == _masterVolumeSlider)
        {
            SoundMixerManager.Instance.SetMasterVolume(evt.newValue);
        }
        else if(evt.target == _sfxVolumeSlider)
        {
            SoundMixerManager.Instance.SetSFXVolume(evt.newValue);
        }
        else if(evt.target == _musicVolumeSlider)
        {
            SoundMixerManager.Instance.SetMusicVolume(evt.newValue);
        }
    }

    private void OnCloseButtonHovered(MouseEnterEvent evt)
    {
        //SFX
    }

    private void OnCloseOptionsButtonClicked(ClickEvent evt)
    {
        UIManager.Instance.Pop(this);
    }

}

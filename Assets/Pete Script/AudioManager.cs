using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/*
 * TODO: Music "Main Menu quick loop" think of it like repeating intro? 
 * TODO: Music "Intensity Music loop"
 * TODO: Music "Lost Music loop "
 * TODO: UI SFX: entry click, exit click
 * TODO: UI SFX: correct answer, incorrect answer
 * TODO: go up streak SFX: level 1, lvl 2, lvl 3
 * TODO: go down streak SFX: lvl 3 down, lvl 2 down
 * TODO: lose all hearts, make sure it works with the lost music
 */
public class AudioManager : MonoBehaviour
{
[SerializeField] private ManagingMath managingMath;
    
    [Space]
    [Header("Audio")]
    [Space]

    public AudioSource uiAudio;
    public AudioClip audio_clickSound;
    public AudioClip audio_correctAnswer;
    public AudioClip audio_incorrectAnswer;
    public AudioClip audio_gameOver;
    public AudioClip uiEntryClip;
    public AudioClip uiExitClip;
    
    [Space]
    [Header("Audio Settings")]
    [Space]
    public Slider sfxSlider;
    public AudioMixer mainMixer;
    public Slider musicSlider;

    private void Start()
    {
        managingMath.OnCorrectAnswer.AddListener(PlayCorrectAnswerSfx);
        managingMath.OnIncorrectAnswer.AddListener(PlayInCorrectAnswerSfx);
        managingMath.OnGameOver.AddListener(PlayGameOverSfx);
    }

    private void PlayGameOverSfx()
    {
        uiAudio.PlayOneShot(audio_gameOver);
    }

    private void PlayCorrectAnswerSfx()
    {
        uiAudio.PlayOneShot(audio_correctAnswer);
    }
    
    private void PlayInCorrectAnswerSfx()
    {
        uiAudio.PlayOneShot(audio_incorrectAnswer);
    }

    public void PlayEntryUI()
    {
        uiAudio.PlayOneShot(uiEntryClip);

    }
    
    public void PlayExitUI()
    {
        uiAudio.PlayOneShot(uiExitClip);
    }
    //AUDIO SETTINGS
    public void openSettings()
    {
        float vol = -1;
        mainMixer.GetFloat("Volume_SFX", out vol);
      
        sfxSlider.value = (vol + 80f) / 80f;

        mainMixer.GetFloat("Volume_BackgroundMusic", out vol);
        musicSlider.value = (vol + 80f) / 80f;
    }

    
    //This will be called while the sliders are moving?
    public void updateAudioSettings()
    {
        mainMixer.SetFloat("Volume_SFX", Mathf.Log10(sfxSlider.value) * 20);
        //mixer_overall.SetFloat("Volume_SFX", slider_audioEffects.value);
        mainMixer.SetFloat("Volume_BackgroundMusic", Mathf.Log10(musicSlider.value) * 20);
    }

  
    
}

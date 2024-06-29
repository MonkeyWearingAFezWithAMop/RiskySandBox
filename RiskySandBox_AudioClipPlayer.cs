using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public partial class RiskySandBox_AudioClipPlayer : MonoBehaviour
{

    public static string audio_clips_folder { get { return Application.streamingAssetsPath + "/RiskySandBox/AudioClips"; } }

    [SerializeField] bool debugging;


    [SerializeField] AudioSource my_AudioSource { get { return GetComponent<AudioSource>(); } }


    [SerializeField] List<AudioClip> deploy_AudioClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> attack_AudioClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> capture_AudioClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> fortify_AudioClips = new List<AudioClip>();


    private void Awake()
    {
        if (this.debugging)
            GlobalFunctions.print("called Awake!",this);

        RiskySandBox_MainGame.Ondeploy += EventReceiver_Ondeploy;
        RiskySandBox_MainGame.Onattack += EventReceiver_Onattack;
        RiskySandBox_MainGame.Onfortify += EventReceiver_Onfortify;
        RiskySandBox_MainGame.Oncapture += EventReceiver_Oncapture;
    }

    private void OnDestroy()
    {
        if (this.debugging)
            GlobalFunctions.print("called OnDestroy!", this);

        RiskySandBox_MainGame.Ondeploy -= EventReceiver_Ondeploy;
        RiskySandBox_MainGame.Onattack -= EventReceiver_Onattack;
        RiskySandBox_MainGame.Onfortify -= EventReceiver_Onfortify;
        RiskySandBox_MainGame.Oncapture -= EventReceiver_Oncapture;
    }


    void TRY_playRandomClip(List<AudioClip> _options)
    {
        if (my_AudioSource.isPlaying)
            return;

        if (_options.Count == 0)
            return;

        //select a random audioclip...
        int _random_index = GlobalFunctions.randomInt(0, _options.Count - 1);
        this.my_AudioSource.PlayOneShot(_options[_random_index]);
    }


    void EventReceiver_Ondeploy(RiskySandBox_MainGame.EventInfo_Ondeploy _EventInfo)
    {
        //TODO - if the player doesnt want the sounds to play? e.g. they have disabled the sfx? or the deploy sfx? - return
        TRY_playRandomClip(deploy_AudioClips);
    }

    void EventReceiver_Onattack(RiskySandBox_MainGame.EventInfo_Onattack _EventInfo)
    {
        TRY_playRandomClip(this.attack_AudioClips);
    }

    void EventReceiver_Onfortify(RiskySandBox_MainGame.EventInfo_Onfortify _EventInfo)
    {
        TRY_playRandomClip(this.fortify_AudioClips);
    }

    void EventReceiver_Oncapture(RiskySandBox_MainGame.EventInfo_Oncapture _EventInfo)
    {
        TRY_playRandomClip(this.capture_AudioClips);
    }





}

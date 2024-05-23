using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public partial class RiskySandBox_AudioClipPlayer : MonoBehaviour
{

    public static string audio_clips_folder { get { return Application.streamingAssetsPath + "/RiskySandBox/AudioClips"; } }

    [SerializeField] bool debugging;


    [SerializeField] AudioSource my_AudioSource { get { return GetComponent<AudioSource>(); } }



    private void Awake()
    {
        if (this.debugging)
            GlobalFunctions.print("called Awake!",this);

        RiskySandBox_MainGame.Ondeploy += EventReceiver_Ondeploy;
        RiskySandBox_MainGame.Onattack += EventReceiver_OnAttack;
    }



    void EventReceiver_Ondeploy(RiskySandBox_MainGame.EventInfo_Ondeploy _EventInfo)
    {
        if (PrototypingAssets.run_client_code.value == false)
        {
            if (this.debugging)
                GlobalFunctions.print("PrototypingAssets.run_client_code.value == false... returning",this);
            return;
        }
            

        //TODO - select a audio clip from the map folder?

        //else select a audio clip folder from the StreamingAssets/RiskySandBox/AudioClips/Deploy
        //get the settings (volume might be different...)

        string[] _deploy_clip_folders = System.IO.Directory.GetDirectories(audio_clips_folder +"/Deploy");

        if (_deploy_clip_folders.Count() <= 0)
        {
            if (this.debugging)
                GlobalFunctions.print("_deploy_clip_folders.Count <= 0 - returning",this);
            return;
        }

        int _random_folder_index = GlobalFunctions.randomInt(0, _deploy_clip_folders.Count() - 1);
        
        string[] _wav_files = System.IO.Directory.GetFiles(_deploy_clip_folders[_random_folder_index], "*.wav");

        if(_wav_files.Count() <= 0)
        {
            return;
        }


        string _full_wav_path = _wav_files[0];
        float _volume = 10f;//TODO - get the volume from the settings file...


        StartCoroutine(LoadAndPlayAudioClip_wav(_full_wav_path, _volume));




    }

    
    IEnumerator LoadAndPlayAudioClip_wav(string file_path,float _volume)
    {
        if (this.debugging)
            GlobalFunctions.print("trying to play the audio clip at '" + file_path + "'", this);
    #if UNITY_ANDROID
        string uri = filePath;
    #else
        string uri = "file://" + file_path;
    #endif

        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error loading audio clip: " + www.error);
            }
            else
            {
                if (this.debugging)
                    GlobalFunctions.print("playing audio clip! " + file_path,this);
                AudioClip _AudioClip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);

                AudioSource.PlayClipAtPoint(_AudioClip, RiskySandBox_CameraControls.instance.GET_cameraPosition(), _volume);
                
            }
        }
    }


    void EventReceiver_OnAttack(RiskySandBox_MainGame.EventInfo_Onattack _EventInfo)
    {
        if (this.debugging)
            GlobalFunctions.print("detected a OnAttack!",this);

        if (PrototypingAssets.run_client_code.value == false)
            return;

        string[] _attack_clip_folders = System.IO.Directory.GetDirectories(audio_clips_folder + "/Attack");

        if (_attack_clip_folders.Count() <= 0)
            return;
        
 
        int _random_folder_index = GlobalFunctions.randomInt(0, _attack_clip_folders.Count() - 1);

        string _random_folder = _attack_clip_folders[_random_folder_index];
        string[] _wav_files = System.IO.Directory.GetFiles(_random_folder, "*.wav");

        if (this.debugging)
            GlobalFunctions.print("searching for .wav files in the folder '" + _random_folder + "'",this);


        if (_wav_files.Count() == 0)
        {
            if (this.debugging)
                GlobalFunctions.print("found no .wav files in the folder '"+_random_folder+"'",this);
            return;
        }
        else
        {
            if (debugging)
                GlobalFunctions.print("found a .wav file! '" + _wav_files[0]+"'",this);
        }
                


        string _full_wav_path = _wav_files[0];
        float _volume = 10f;


        StartCoroutine(LoadAndPlayAudioClip_wav(_full_wav_path,_volume));

    }





}

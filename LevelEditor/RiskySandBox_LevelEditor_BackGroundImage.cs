using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using System.IO;

public partial class RiskySandBox_LevelEditor_BackGroundImage : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] UnityEngine.UI.RawImage background_Image;
    [SerializeField] ObservableFloat PRIVATE_background_image_x_scale;
    [SerializeField] ObservableFloat PRIVATE_background_image_y_scale;
    

    [SerializeField] ObservableBool PRIVATE_show_settings;
    [SerializeField] ObservableBool PRIVATE_show_background_image;

    [SerializeField] ObservableString map_ID;


    private void Awake()
    {
        RiskySandBox_LevelEditor.Onenable += RiskySandBox_LevelEditorEventReceiver_Onenable;

    }

    private void OnDestroy()
    {
        RiskySandBox_LevelEditor.Onenable -= RiskySandBox_LevelEditorEventReceiver_Onenable;
    }


    public void RiskySandBox_MainMenuEventReceiver_Onenable()
    {
        this.background_Image.gameObject.SetActive(false);
    }

    public void RiskySandBox_LevelEditorEventReceiver_Onenable()
    {
        this.background_Image.gameObject.SetActive(PRIVATE_show_background_image);
        
    }




    public void updateImage()
    {

        



        // Path to the PNG file in the StreamingAssets folder
        string _map_folder_path = Path.Combine(RiskySandBox.maps_folder_path, this.map_ID);

        if (Directory.Exists(_map_folder_path) == false)
        {
            background_Image.texture = null;
            return;
        }




        foreach (string file in Directory.GetFiles(_map_folder_path, "*.png"))
        {
            
            // Load the PNG file into a byte array
            byte[] fileData = File.ReadAllBytes(file);

            // Create a new texture
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); // Load the PNG data into the texture


            // Assign the texture to the RawImage component
            background_Image.texture = texture;
        }
    }







}

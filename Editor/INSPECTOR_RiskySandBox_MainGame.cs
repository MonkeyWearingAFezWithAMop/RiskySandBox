using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RiskySandBox_MainGame))]
public class INSPECTOR_RiskySandBox_MainGame : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RiskySandBox_MainGame mainGame = (RiskySandBox_MainGame)target;

        // Add a button to start the game
        if (GUILayout.Button("Start Game"))
        {
            mainGame.startGame();
        }

        if(GUILayout.Button("save map"))
        {

            mainGame.saveMap(RiskySandBox_MainGame.instance.map_ID);
        }


        if(GUILayout.Button("Load Map"))
        {
            mainGame.loadMap(RiskySandBox_MainGame.instance.map_ID);
        }
    }
}

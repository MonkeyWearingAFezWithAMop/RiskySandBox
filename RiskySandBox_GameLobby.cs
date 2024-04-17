using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public partial class RiskySandBox_GameLobby : MonoBehaviourPunCallbacks
{

    [SerializeField] bool debugging;

    [SerializeField] int main_game_scene_id;

    [SerializeField] GameObject text_prefab;
    [SerializeField] Transform ui_root_Transform;


    [SerializeField] List<UnityEngine.UI.Text> current_username_Texts = new List<UnityEngine.UI.Text>();



    private void Start()
    {
        createMyHumanPlayer();
        updatePlayersUI();

    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        createMyHumanPlayer();
    }



    void createMyHumanPlayer()
    {
        RiskySandBox_HumanPlayer _my_HumanPlayer = GET_RiskySandBox_HumanPlayer(PhotonNetwork.LocalPlayer);
        if (_my_HumanPlayer != null)
            return;

        if (PhotonNetwork.InRoom == false)
            return;

        PhotonNetwork.Instantiate(RiskySandBox_Resources.human_player_prefab.name, new Vector3(0, 0, 0), Quaternion.identity);
    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        updatePlayersUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        updatePlayersUI();
    }


    void updatePlayersUI()
    {

        foreach(UnityEngine.UI.Text _Text in current_username_Texts)
        {
            UnityEngine.Object.Destroy(_Text.gameObject);
        }

        List<Photon.Realtime.Player> _Player_list = new List<Photon.Realtime.Player>(PhotonNetwork.PlayerList);

        for (int i = 0; i < _Player_list.Count; i += 1)
        {

            //create a prefab to display this person...
            GameObject _new_text = UnityEngine.Object.Instantiate(text_prefab, ui_root_Transform);

            _new_text.GetComponent<UnityEngine.UI.Text>().text = _Player_list[i].NickName;
            _new_text.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, 210 + (-30 * i));

            current_username_Texts.Add(_new_text.GetComponent<UnityEngine.UI.Text>());

        }
    }





    public void startGame()
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are not the Server...
            return;//we cant start the game... so return


        
        
       
        //assign the players team to be this team...
        List<Photon.Realtime.Player> _Player_list = new List<Photon.Realtime.Player>(PhotonNetwork.PlayerList);


        for (int i = 0; i < _Player_list.Count; i += 1)//foreach human player...
        {
            GameObject _new_Team_GO = PhotonNetwork.InstantiateRoomObject(RiskySandBox_Resources.Team_prefab.name, Vector3.zero, Quaternion.identity);//create a new team...
            RiskySandBox_Team _new_Team = _new_Team_GO.GetComponent<RiskySandBox_Team>();
            _new_Team.ID.value = i;//assign a unique id for the Team...


            //get the human player accociated with this _PlayerList[i].... get the HumanPlayer script and assign the id to be i

            RiskySandBox_HumanPlayer _HumanPlayer = GET_RiskySandBox_HumanPlayer(_Player_list[i]);
            _HumanPlayer.my_Team_ID.value = i;


        }



        //foreach ai player...
        //create a new team...
        //assign a random color and material?

        //todo ok lets also disable the lobby ui
        ui_root_Transform.gameObject.SetActive(false);


        RiskySandBox_MainGame.instance.startGame();
        //TODO -throw up a temp screen that says 10... 9.... 8.... 7... 6... 5... 4... 3... 2... 1... go!
        //this also gives time for the map to get loaded/synced to clients?


    }



    RiskySandBox_HumanPlayer GET_RiskySandBox_HumanPlayer(Photon.Realtime.Player _PhotonPlayer)
    {
        foreach(RiskySandBox_HumanPlayer _RiskySandBox_HumanPlayer in RiskySandBox_HumanPlayer.all_instances)
        {
            if (_RiskySandBox_HumanPlayer.GetComponent<PhotonView>().CreatorActorNr == _PhotonPlayer.ActorNumber)
                return _RiskySandBox_HumanPlayer;
        }
        //unable to find... WTF?!?!?!?!?
        return null;
    }





}

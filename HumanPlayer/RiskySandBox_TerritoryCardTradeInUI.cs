using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_TerritoryCardTradeInUI : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;
    RiskySandBox_Team my_Team { get { return my_HumanPlayer.my_Team; } }

    [SerializeField] GameObject card_trade_in_UI;
    [SerializeField] RectTransform territory_card_start_point;


    [SerializeField] UnityEngine.UI.Text num_cards_Text;

    [SerializeField] List<GameObject> permanent_UI_elements = new List<GameObject>();
    [SerializeField] UnityEngine.UI.Button trade_Button;

    private void Awake()
    {
        RiskySandBox_Team.OnUpdate_territory_card_IDs_STATIC += EventReceiver_OnUpdate_territory_card_IDs_STATIC;
        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state_STATIC;

        RiskySandBox_TerritoryCard.OnVariableUpdate_is_selected_STATIC += EventReceiver_OnVariableUpdate_is_selected_STATIC;

    }

    private void Start()
    {
        closeTerritoryCardsUI();
        updateUI();
    }

    private void OnDestroy()
    {
        RiskySandBox_Team.OnUpdate_territory_card_IDs_STATIC -= EventReceiver_OnUpdate_territory_card_IDs_STATIC;
        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC -= EventReceiver_OnVariableUpdate_current_turn_state_STATIC;

        RiskySandBox_TerritoryCard.OnVariableUpdate_is_selected_STATIC -= EventReceiver_OnVariableUpdate_is_selected_STATIC;
    }

    public void openTerritoryCardsUI()
    {
        card_trade_in_UI.SetActive(true);
        updateUI();
    }


    void updateTradeButton()
    {
        List<int> _selected_cards = RiskySandBox_TerritoryCard.all_instances_is_selected.Select(x => x.tile_ID_READONLY).ToList();
        trade_Button.interactable = RiskySandBox_MainGame.instance.enable_territory_cards && RiskySandBox_MainGame.instance.isValidTrade(_selected_cards);
    }
    

    void updateUI()
    {

        if (this.debugging)
            GlobalFunctions.print("updating the ui!",this);

        List<int> _selected_cards = RiskySandBox_TerritoryCard.all_instances_is_selected.Select(x => x.tile_ID_READONLY).ToList();
        
        updateTradeButton();

        foreach(GameObject _GO in permanent_UI_elements.Where (x => x != null))
        {
            _GO.SetActive(RiskySandBox_MainGame.instance.enable_territory_cards);
        }


        this.num_cards_Text.text = "0";

        RiskySandBox_TerritoryCard.destroyAllCards();
        if (my_Team == null)
        {
            closeTerritoryCardsUI();
            return;
        }

        ObservableList<int> _territory_card_IDs = my_Team.territory_card_IDs;
        this.num_cards_Text.text = "" + _territory_card_IDs.Count();

        for (int i = 0; i < _territory_card_IDs.Count; i += 1)
        {
            //instantiate a new trading card...
            RiskySandBox_TerritoryCard _new_card = RiskySandBox_TerritoryCard.createNew(_territory_card_IDs[i], card_trade_in_UI.transform);

            //_new_card update position...
            _new_card.GetComponent<RectTransform>().anchoredPosition = territory_card_start_point.anchoredPosition + new Vector2(100 * i, 0);

        }


        
    }



    public void closeTerritoryCardsUI()
    {
        card_trade_in_UI.SetActive(false);
    }

    public void toggleTerritoryCardsUI()
    {
        if (card_trade_in_UI.activeInHierarchy == false)
            openTerritoryCardsUI();
        else
            closeTerritoryCardsUI();
    }



    void EventReceiver_OnVariableUpdate_current_turn_state_STATIC(RiskySandBox_Team _Team)
    {
        if (_Team != this.my_Team)
            return;


        if(_Team.current_turn_state == RiskySandBox_Team.turn_state_force_trade_in)
        {

            openTerritoryCardsUI();

            if (this.debugging)
                GlobalFunctions.print("my team entered the force trade in state... updating the ui...",this);
            updateUI();
        }

        // force trade in -> deploy...
        if(_Team.current_turn_state == RiskySandBox_Team.turn_state_deploy && _Team.current_turn_state.previous_value == RiskySandBox_Team.turn_state_force_trade_in)
        {
            //TODO
            //ok if we have no way of trading....
            //we can just quit out of this menu (assuming the player has enabled this shortcut...
        }
            

    }

    void EventReceiver_OnUpdate_territory_card_IDs_STATIC(RiskySandBox_Team _Team)
    {
        if (_Team != this.my_Team)
            return;

        updateUI();


    }

    void EventReceiver_OnVariableUpdate_is_selected_STATIC(RiskySandBox_TerritoryCard _Card)
    {
        updateTradeButton();
    }
}

﻿using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    private Player[] players;
    public int playerCount; //value assigned by UI 'gameManager' object
    public static bool playContinuous; //value assigned by UI 'gameManager' object
    private Queue<Cards> jackPot;
    
    void Start() //called for each instance of the class (On UI scene with this class attached loaded)
    {
        //for war scenario
        jackPot = new Queue<Cards>(); //for war scenario
        
        //initialize a standard deck of cards
        Queue<Cards> deck = Cards.InitializeStandardDeck();        
        
        //initialize the players for the game
        players = new Player[playerCount];
        
        //split the deck into the players' hands
        int index = 0; 
        foreach(Queue<Cards> playerDeck in Cards.SplitDeck(playerCount, deck)){
            players[index] = new Player(playerDeck, (index++)+1); 
        }
    }
    

    public void PlayRound()
    {
        int bestCardPower = 0;
        int winnerCount = 0;
        int winner=0;

        //determine what the best card is and how many players have it
        int index = 0; 
        foreach (Player player in players)
        {
            if (player.Peek().GetPower() == bestCardPower)
                winnerCount++;
            if (player.Peek().GetPower() > bestCardPower)
            {
                winnerCount = 1;
                bestCardPower = player.Peek().GetPower();
                winner = index++; 
            } 
        }
        
        
        if (winnerCount > 1) //enter war scenario
        {
            /*
             * in war scenario, by default, we dequeue 3 cards and then play the fourth...
             * however if one of the players has less than three cards in his/her deck,
             * the amount dequeued for each player is 1 minus that player's total cards
             * (so the player still has a card left to play)
            */
            int warCount = 3;

            foreach (Player player in players)
            {
                if (player.GetDeckCount() < warCount+1)
                {
                    warCount = player.GetDeckCount() - 1; 
                }     
            }

            /*
             * Determine who enters the war scenario and who just adds their losing card into the jackpot
             * if the player's top card == bestCardPower, dequeue the top card and <warCount> amount of cards
             * into the jackpot 
             * this functionality is added in the case that the game should be extended to support more than 2 players
            */
            foreach (Player player in players)
            {                
                if (player.Peek().GetPower() == bestCardPower)
                {
                    for(int i = 0; i < warCount; i++)
                        jackPot.Enqueue(player.DequeueDeck());
                }
                else // just add the top card to the jackpot
                    jackPot.Enqueue(player.DequeueDeck());
            }
        }

        else //there was only one winner, that player gets the cards - and the jackpot if there is one
        {

            //depending on if the card should go into the player deck (player.EnqueueDeck), or into a separate pile (player.EnqueueWinnings)
            while (jackPot.Count > 0)
            {
                if(playContinuous)
                    players[winner].EnqueueDeck(jackPot.Dequeue());
                else
                    players[winner].EnqueueWinnings(jackPot.Dequeue());
            }

            foreach (Player player in players)
            {    
                if(playContinuous)
                    players[winner].EnqueueDeck(player.DequeueDeck());
                else
                    players[winner].EnqueueWinnings(player.DequeueDeck());
            }
        }
        
        RefreshTable();
        CheckEndOfGame();
    }


    public void RefreshTable()
    { 

        /*
         * send the UI elements to their parent GameObject's location in the UI
         */
        
        //place the back of cards
        for (int i = 0; i < playerCount; i++)
        {
            GameObject.Find("backOfCardP" + (i + 1)).GetComponent<GlideController>().SetDestination(GameObject.Find("Player" + (i + 1) + "Hand").transform.position);
        }

        //for each of the players, place the cards in their hand 
        for (int i = 0; i < playerCount; i++)
        {
            if (players[i].GetDeckCount() > 0)
            {
                foreach (Cards card in players[i].getDeck())//move to the location of the parent
                    GameObject.Find(card.GetPower() + card.GetSuit()).GetComponent<GlideController>().SetDestination(GameObject.Find("Player" + (i + 1) + "Hand").transform.position);
                
                //slide top card out of the pile  
                GameObject.Find(players[i].Peek().GetPower() + players[i].Peek().GetSuit()).GetComponent<GlideController>().SetDestination(GameObject.Find("Player" + (i + 1) + "Card").transform.position);

                //set the card count indicators on the player hands
                GameObject.Find("Player" + (i + 1) + "CardCount").GetComponent<Text>().text =(players[i].GetDeckCount()-1).ToString();
            }

            if (!playContinuous)
            {
                foreach (Cards card in players[i].GetWinnings())
                    GameObject.Find(card.GetPower() + card.GetSuit()).GetComponent<GlideController>().SetDestination(GameObject.Find("Player" + (i + 1) + "WinningsCount").transform.position);


                //set the winnings count indicators below the winnings piles
                if (players[i].GetWinningsCount() > 0)
                    GameObject.Find("Player" + (i + 1) + "WinningsCount").GetComponentInChildren<Text>().text =players[i].GetWinningsCount().ToString();
            }

            //set the jackpot hand if there is one
            int offset = 0; //so the cards in the jackpot are nicely staggered, this is iterated by 50
            int index = 0; 
            foreach (Cards card in jackPot)
            {   
                //find the position of the jackpot game object
                Vector2 jackpotPosition = GameObject.Find("Jackpot").transform.position;
                
                //set an offset for the cards so you can see them stacked
                jackpotPosition.x += offset += 50;
                
                //move the cards to the location
                GameObject.Find(card.GetPower()+card.GetSuit()).GetComponent<GlideController>().SetDestination(jackpotPosition);
                
                //maintain the index order so that they stack nicely in the jackpot game object
                GameObject.Find(card.GetPower()+card.GetSuit()).transform.SetSiblingIndex(index++);
            }
        }
    }

    private void CheckEndOfGame()
    {
        /*
         * check if any of the players has only one card left. If so, disappear the top card...
         * (the card back that covers the player deck).
         * If the amount of losers is one less than the total amount of players, end the game
         * This functionality is necessary for the case that the game is extended to more than 2 players
         */
        
        int losers = 0;
        for (int i = 0; i < playerCount; i++)
        {
            /*
             * determine if any card backs need to be disappeared - or reappeared.
             * visibility is changed by placing the item's index in the UI array of objects...
             * and thus in front or behind other objects
             */
            if(players[i].GetDeckCount() < 2)//disappear the back of the card
                GameObject.Find("backOfCardP" + (i+1)).transform.SetSiblingIndex(0);
            else//reappear the back of the card
                GameObject.Find("backOfCardP" + (i+1)).transform.SetSiblingIndex(GameObject.Find("Canvas").transform.childCount);
            
            
            if (players[i].GetDeckCount() < 1)
            {
                losers++;
            }
        }

        //if playerCount = 1 greater than losers, the game is over (applies only if continuous play
        if (playContinuous)
        {
            if (playerCount == losers + 1) //there's a single winner
            {
                foreach (Player player in players)
                {
                    if (player.GetDeckCount() > 0)
                    {
                        //show the game ending prompt
                        int lastIndex = GameObject.Find("Canvas").transform.childCount;
                        GameObject.Find("EOGPrompt").transform.SetSiblingIndex(lastIndex);
                        GameObject.Find("winnerText").GetComponent<Text>().text ="player " + player.getPlayerNumber() + " has won!"; 
                        break;
                    }
                    
                } 
            }
        }

        else // non-continuous play
        {
            //if there is a war scenario on the last card in continuous play, we have to quit without using those cards
            bool war = false;
            if (players[0].GetDeckCount() == 1)
            { 
                HashSet<int>hs = new HashSet<int>();
                foreach (Player player in players)
                {
                    if (hs.Contains(player.Peek().GetPower()))
                        war = true;
                    else
                        hs.Add(player.Peek().GetPower()); 
                }
               
            }
            
            //the deck count for all players is the same... therefore just test the first one for end of Queue
            if (players[0].GetDeckCount() <= 0 || war)
            {
                //show the game ending prompt
                GameObject.Find("EOGPrompt").transform.SetSiblingIndex(GameObject.Find("Canvas").transform.childCount);

                string promptText = ""; 
                foreach (Player player in players)
                {
                    promptText += "Player: " + player.getPlayerNumber() + ": " + player.GetWinningsCount() + " cards" + '\n'; 
                }
                GameObject.Find("winnerText").GetComponent<Text>().text = promptText; 
            }
        }

    }
    
    public void HideSetTableButton()
    {
        //moves the set table button out of view and brings the play round button into view
        GameObject.Find("SetTableButton").GetComponent<GlideController>().SetDestination(GameObject.Find("OffScreenBottom").transform.position);
        GameObject.Find("PlayRoundButton").GetComponent<GlideController>().SetDestination(GameObject.Find("OnScreenButton").transform.position);
    }

    public void PlayAgain()
    {
        //reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadMain()
    {
        SceneManager.LoadScene("Main"); 
    }

    public void loadGame()
    {
        SceneManager.LoadScene("Game"); 
    }

    public void setContinuousPlay(bool cPlay)
    {
        playContinuous = cPlay; 
    }
}

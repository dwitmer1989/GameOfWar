using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    private Queue<Cards>[] players;
    public int playerCount; //value assigned by UI 'gameManager' object
    private Queue<Cards> jackPot;
    void Start() //called for each instance of the class (On UI scene with this class attached loaded)
    {
        jackPot = new Queue<Cards>(); //for war scenario
        Queue<Cards> deck = Cards.initializeStandardDeck();   
        players = Cards.splitDeck(playerCount, deck);
    }

    public void playRound()
    {
        int bestCardPower = 0;
        int winnerCount = 0;
        int winner = 0;
        int index = 0; 
        
        //determine the highest power card and how many players have it
        foreach (Queue<Cards> player in players)
        {
            //if the player has the same card as the best card, increment the amount of round winners
            if (player.Peek().getPower() == bestCardPower)
                winnerCount++; 
            
            //if the player has a better card than the current best card, set the best card power and assign the count of winners to 1
            if (player.Peek().getPower() > bestCardPower)
            {
                bestCardPower = player.Peek().getPower();
                winnerCount = 1;
                winner = index; 
            }
            index++; 
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

            foreach (Queue<Cards> player in players)
            {
                if (player.Count < warCount+1)
                {
                    warCount = player.Count - 1; 
                }     
            }

            /*
             * Determine who enters the war scenario and who just adds their losing card into the jackpot
             * if the player's top card = bestCardPower, dequeue the top card and <warCount> amount of cards
             * into the jackpot 
             * this functionality is added in the case that the game is extended to more than 2 players
            */
            foreach (Queue<Cards> player in players)
            {                
                if (player.Peek().getPower() == bestCardPower)
                {
                    for(int i = 0; i < warCount; i++)
                        jackPot.Enqueue(player.Dequeue());
                }
                else // just add the top card to the jackpot
                    jackPot.Enqueue(player.Dequeue());
            }
        }

        else //there was only one winner, that player gets the cards - and the jackpot if there is one
        {
            while (jackPot.Count > 0)
            {
                players[winner].Enqueue(jackPot.Dequeue());
            }

            foreach (Queue<Cards> player in players)
            {    
                string winnerObject = "Player" + (winner+1) + "Hand";
                players[winner].Enqueue(player.Dequeue());
            }
        }
        RefreshTable();
    }


    public void RefreshTable()
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
            if(players[i].Count < 2)//disappear the back of the card
                GameObject.Find("backOfCardP" + (i+1)).transform.SetSiblingIndex(0);
            else//reappear the back of the card
                GameObject.Find("backOfCardP" + (i+1)).transform.SetSiblingIndex(GameObject.Find("Canvas").transform.childCount);
            
            
            if (players[i].Count < 1)
            {
                losers++;
            }
        }

        //if playerCount = 1 greater than losers, the game is over
        if (playerCount == losers + 1)
        {
            foreach (Queue<Cards> player in players)
            {
                if (player.Count > 0)
                {
                    //show the game ending prompt
                    int lastIndex = GameObject.Find("Canvas").transform.childCount;
                    GameObject.Find("Prompt").transform.SetSiblingIndex(lastIndex);
                    break;
                }
                
            }
            
        }

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
            if (players[i].Count > 0)
            {
                foreach (Cards card in players[i])
                {
                    //move to the location of the parent
                    GameObject.Find(card.getPower() + card.getSuit()).GetComponent<GlideController>().SetDestination(GameObject.Find("Player" + (i + 1) + "Hand").transform.position);
                }

                //slide top card out of the pile  
                GameObject.Find(players[i].Peek().getPower() + players[i].Peek().getSuit()).GetComponent<GlideController>().SetDestination(GameObject.Find("Player" + (i + 1) + "Card").transform.position);

                //set the card count indicators on the player hands
                GameObject.Find("Player" + (i + 1) + "CardCount").GetComponent<Text>().text =(players[i].Count-1).ToString();

            }

            //set the jackpot hand if there is one
            int offset = 0; //so the cards in the jackpot are nicely staggered, this is iterated by 50
            int index = 0; 
            foreach (Cards card in jackPot)
            {
                //find the game object that represents the current card in the UI
                GameObject cardGO = GameObject.Find(card.getPower()+card.getSuit());
                
                //find the position of the jackpot game object
                Vector2 jackpotPosition = GameObject.Find("Jackpot").transform.position;
                
                //set an offset for the cards so you can see them stacked
                jackpotPosition.x += (offset += 50);
                
                //move the cards to the location
                cardGO.GetComponent<GlideController>().SetDestination(jackpotPosition);
                
                //maintain the index order so that they stack nicely in the jackpot game object
                cardGO.transform.SetSiblingIndex(index++);
            }
        }

    }

    public void hideSetTableButton()
    {
        //moves the set table button out of view and brings the play round button into view
        GameObject.Find("SetTableButton").GetComponent<GlideController>().SetDestination(GameObject.Find("OffScreenBottom").transform.position);
        GameObject.Find("PlayRoundButton").GetComponent<GlideController>().SetDestination(GameObject.Find("OnScreenButton").transform.position);
    }

    public void playAgain()
    {
        //reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void exitGame()
    {
        Application.Quit();
    }
}

using System.Collections.Generic;
using Random = System.Random;

namespace DefaultNamespace
{
    public class Cards
    {
        private string suit;  
        private int power; // 2=2, 3=3...10=10, jack=11...ace=14

        public Cards(string suit, int power)
        {
            this.suit = suit; 
            this.power = power; 
        }
        
        //getters
        public string GetSuit()
        {
            return suit; 
        }

        public int GetPower()
        {
            return power; 
        }
        
        //setters
        public void SetSuit(string suit)
        {
            this.suit = suit; 
        }

        public void SetPower(int power)
        {
            this.power = power; 
        }
        
        //auxilliary functions
        public static Queue<Cards> InitializeStandardDeck() //returns a queue that represents a standard deck of cards
        {
            Cards[] deck = new Cards[52];
            string suit=null;
            int deckIndex = 0; 
            for (int i = 0; i < 4; i++)
            {
                for (int j = 2; j <= 14; j++)
                {
                    switch (i)
                    {
                        case 0:
                            suit = "Clubs";
                            break; 
                        case 1:
                            suit = "Diamonds";
                            break;
                        case 2:
                            suit = "Hearts";
                            break;
                        case 3:
                            suit = "Spades";
                            break; 
                    }
                    deck[deckIndex++] = new Cards(suit, j);
                }
            }
            ShuffleDeck(deck);
            return new Queue<Cards>(deck);
        }

        public static Cards[] ShuffleDeck(Cards[] deck)
        // shuffle the deck using the popular Knuth shuffle algorithm
        // for each index in the array, swap the value with the value found at a random index in the array
        {
            Random r = new Random();
            for (int i = 0; i < deck.Length; i++)
            {
                int randomIndex = r.Next() % 52; 
                Cards temp = deck[i];
                deck[i] = deck[randomIndex];
                deck[randomIndex] = temp; 
            }

            return deck; 
        }
        public static Queue<Cards>[] SplitDeck(int playerCount, Queue<Cards> deck)
        {
            Queue<Cards>[] players = new Queue<Cards>[playerCount];
        
            //initialize the players
            for (int i = 0; i < playerCount; i++)
                players[i] = new Queue<Cards>();

            //split the cards so that each player has the same amount of cards (may not use all cards in the deck)
            int splitCount = deck.Count / playerCount; 
            foreach (Queue<Cards> player in players)
            {
                for (int i = 0; i < splitCount; i++)
                    player.Enqueue(deck.Dequeue());
            }
            return players; 
        }
    }
}
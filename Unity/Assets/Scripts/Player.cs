using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Player
    {
        private Queue<Cards> deck;
        private Queue<Cards> winnings;
        private int playerNumber; 

        public Player(Queue<Cards> deck, int playerNumber)
        {
            this.deck = deck;
            winnings = new Queue<Cards>();
            this.playerNumber = playerNumber; 
        }
        public Cards Peek()
        {
            return deck.Peek(); 
        }

        public int GetDeckCount()
        {
            return deck.Count; 
        }

        public int GetWinningsCount()
        {
            return winnings.Count; 
        }

        public Cards DequeueDeck()
        {
            return deck.Dequeue();
        }

        public void EnqueueDeck(Cards c)
        {
            deck.Enqueue(c);
        }

        public Queue<Cards> getDeck()
        {
            return deck; 
        }

        public int getPlayerNumber()
        {
            return playerNumber; 
        }

        public void EnqueueWinnings(Cards c)
        {
            winnings.Enqueue(c);
        }

        public Queue<Cards> GetWinnings()
        {
            return winnings; 
        }

    }
}
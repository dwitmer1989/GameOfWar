using System.Collections.Generic;

namespace DefaultNamespace
{
    public class Player
    {
        private Queue<Cards> deck;
        private Queue<Cards> winnings;

        public Player(Queue<Cards> deck)
        {
            this.deck = deck;
            this.winnings = new Queue<Cards>();
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
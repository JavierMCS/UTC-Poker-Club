namespace ThreeCardPoker
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class Hand
    {
        private readonly List<Card> cards = new List<Card>(5);

        public Hand(bool isDealer = false)
        {
            this.IsDealer = isDealer;
        }

        public event EventHandler Changed;

        public bool IsDealer { get; private set; }

        public ReadOnlyCollection<Card> Cards
        {
            get { return this.cards.AsReadOnly(); }
        }
        
        //get the score of the deck
        public int getScore
        {

            get {
        		int high = 0;
                int score = 0;
                
                foreach (var item in this.Cards)
                {
                    int compare = (int)item.Rank;

                    if (compare > high)
                    {
                        high = compare;
                    }
                    int pair = 0;
                    
                    //if item is an ace, give it a value larger than king
                    if (compare == 1)
                    {
                    	high = 14;
                    }
                    
                    //compare each card to the card we are currently comparing
                    foreach (var item2 in this.Cards)
                    {
                    	if (compare == (int)item2.Rank)
                        {
                            pair += 1;
                        }
                    }
                    
                    //if it matches itself, plus at least one more time, it is a pair
                    if (pair >= 2)
                    {
                        //scale the score to be higher if the user got a pair
                        score = compare + 50;
                        
                        //if it was a pair of aces, treat it as if the aces are worth 14 points
                        if (compare == 1)
                        {
                        	score = 14 + 50;
                        }
                    }                   
                }
                
               
                
                //if the hand was a flush
                int card1 = (int)this.Cards[0].Suite;
                int card2 = (int)this.Cards[1].Suite;
                int card3 = (int)this.Cards[2].Suite;
                
                if ((card1 == card2) && (card1 == card3) && (card2 == card3))
                {
                	//flushes are scored based on the highest card
                	score = high + 100;
                }
                
                //if the hand is not pair, flush, or better, we just take the high card
                if (high > score)
                {
                	score = high;
                }
                
                return score;
            }
        }

        public int SoftValue
        {
            get { return this.cards.Select(c => (int)c.Rank > 1 && (int)c.Rank < 11 ? (int)c.Rank : 10).Sum(); }
        }

        public int TotalValue
        {
            get
            {
                var totalValue = this.SoftValue;
                var aces = this.cards.Count(c => c.Rank == Rank.Ace);

                while (aces-- > 0 && totalValue > 21)
                {
                    totalValue -= 9;
                }

                return totalValue;
            }
        }

        public int FaceValue
        {
            get
            {
                var faceValue = this.cards.Where(c => c.IsFaceUp)
                    .Select(c => (int)c.Rank > 1 && (int)c.Rank < 11 ? (int)c.Rank : 10).Sum();

                var aces = this.cards.Count(c => c.Rank == Rank.Ace);

                while (aces-- > 0 && faceValue > 21)
                {
                    faceValue -= 9;
                }

                return faceValue;
            }
        }

        public bool IsBlackjack
        {
            get { throw new NotImplementedException(); }
        }

        public void AddCard(Card card)
        {
            this.cards.Add(card);

            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }

        public void Show()
        {
            this.cards.ForEach(
                card =>
                {
                    if (!card.IsFaceUp)
                    {
                        card.Flip();

                        if (this.Changed != null)
                        {
                            this.Changed(this, EventArgs.Empty);
                        }
                    }
                });
        }

        public void Clear()
        {
            this.cards.Clear();
        }
    }
}

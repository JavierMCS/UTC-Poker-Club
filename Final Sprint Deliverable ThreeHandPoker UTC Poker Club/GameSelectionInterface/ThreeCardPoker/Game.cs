namespace ThreeCardPoker
{
    using System;

    public class Game
    {
        private GameAction allowedActions;
        private GameState lastState;
        private Deck deck;

        public Game()
        {
            this.Dealer = new Dealer();
            this.Player = new Player();
            this.LastState = GameState.Unknown;
            this.AllowedActions = GameAction.None;
        }

        public event EventHandler LastStateChanged;

        public event EventHandler AllowedActionsChanged;

        public Player Player { get; private set; }

        public Dealer Dealer { get; private set; }

        public GameAction AllowedActions
        {
            get
            {
                return this.allowedActions;
            }

            private set
            {
                if (this.allowedActions != value)
                {
                    this.allowedActions = value;

                    if (this.AllowedActionsChanged != null)
                    {
                        this.AllowedActionsChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public GameState LastState
        {
            get
            {
                return this.lastState;
            }

            private set
            {
                if (this.lastState != value)
                {
                    this.lastState = value;

                    if (this.LastStateChanged != null)
                    {
                        this.LastStateChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public void Play(decimal balance, decimal bet)
        {
            this.Player.Balance = balance;
            this.Player.Bet = bet;
            this.AllowedActions = GameAction.Deal;

            if (this.AllowedActionsChanged != null)
            {
                this.AllowedActionsChanged(this, EventArgs.Empty);
            }
        }

        public void Deal()
        {
            if ((this.AllowedActions & GameAction.Deal) != GameAction.Deal)
            {
                // TODO: Add a descriptive error message
                throw new InvalidOperationException();
            }

            this.LastState = GameState.Unknown;
            
            if (this.deck == null)
            {
                this.deck = new Deck();
            }
            else
            {
                this.deck.Populate();
            }

            this.deck.Shuffle();
            this.Dealer.Hand.Clear();
            this.Player.Hand.Clear();

            this.deck.Deal(this.Dealer.Hand);
            this.deck.Deal(this.Player.Hand);

      
         
            // TODO: Add support of other actions
            this.AllowedActions = GameAction.Hit | GameAction.Stand;
        }

        public void Hit() //Fold
        {
            if ((this.AllowedActions & GameAction.Hit) != GameAction.Hit)
            {
                // TODO: Add a descriptive error message
                throw new InvalidOperationException();
            }
            this.Dealer.Hand.Show();
            this.LastState = GameState.DealerWon;
            this.Player.Balance -= this.Player.Bet;
            this.AllowedActions = GameAction.Deal;
        }

        public void Stand() //Play
        {
            if ((this.AllowedActions & GameAction.Stand) != GameAction.Stand)
            {
                // TODO: Add a descriptive error message
                throw new InvalidOperationException();
            }
            int player = this.Player.Hand.getScore;
            int dealer = this.Dealer.Hand.getScore;
            if (player > dealer)
            {
                this.Player.Balance += this.Player.Bet;
                this.LastState = GameState.PlayerWon;
            }
            else if (dealer == player)
            {
                this.LastState = GameState.Draw;
            }
            else if (dealer > player)
            {
                this.Player.Balance -= this.Player.Bet;
                this.LastState = GameState.DealerWon;
            }

            this.Dealer.Hand.Show();
            this.AllowedActions = GameAction.Deal;
        }
    }
}

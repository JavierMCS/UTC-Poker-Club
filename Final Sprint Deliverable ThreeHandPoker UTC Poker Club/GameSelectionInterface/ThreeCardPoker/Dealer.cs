namespace ThreeCardPoker
{
    public class Dealer : PlayerBase
    {
        public Dealer()
        {
            this.Hand = new Hand(isDealer: true);
        }
    }
}

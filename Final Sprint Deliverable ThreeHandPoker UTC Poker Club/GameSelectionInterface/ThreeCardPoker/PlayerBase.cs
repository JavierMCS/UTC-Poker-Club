namespace ThreeCardPoker
{
    public abstract class PlayerBase
    {
        protected PlayerBase()
        {
        }

        public Hand Hand { get; protected set; }
    }
}

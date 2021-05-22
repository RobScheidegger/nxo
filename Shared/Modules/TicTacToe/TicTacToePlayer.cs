namespace NXO.Shared.Modules
{
    public class TicTacToePlayer
    {
        /// <summary>
        /// The Id of the corresponding player.
        /// </summary>
        public string PlayerId { get; set; }
        /// <summary>
        /// Whether or not this slot is a bot or an actual player.
        /// </summary>
        public bool Bot { get; set; }
        /// <summary>
        /// The playing token that the player wants to play with.
        /// </summary>
        public char Token { get; set; }
    }
}
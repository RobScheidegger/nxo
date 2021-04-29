namespace NXO.Shared.Models
{
    public class CreateLobbyResult
    {
        public bool Success { get; set; }
        public string LobbyCode { get; set; }
        public string PlayerId { get; set; }
        public string Message { get; set; }
    }
}
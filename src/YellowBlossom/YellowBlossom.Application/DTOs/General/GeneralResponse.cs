namespace YellowBlossom.Application.DTOs.General
{
    public class GeneralResponse
    {
        private bool _success;
        private string _message = string.Empty;

        // Constructors
        public GeneralResponse(bool success, string message)
        {
            this._success = success;
            this._message = message;
        }

        // Getters - Setters
        public bool Success { get { return this._success; } set { this._success = value; } }
        public string Message { get { return this._message; } set { this._message = value; } }
    }
}

namespace CharacterCreation.Vtt.Presentation.NetworkSandbox
{
    public readonly struct NetworkSandboxConnectionInput
    {
        public NetworkSandboxConnectionInput(string host, string port, string inviteCode)
        {
            Host = host;
            Port = port;
            InviteCode = inviteCode;
        }

        public string Host { get; }

        public string Port { get; }

        public string InviteCode { get; }
    }
}

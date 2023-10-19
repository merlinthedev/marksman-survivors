namespace Champions.Abilities {
    public interface IChannelable {
        float ChannelTime { get; set; }
        void Channel();
    }
}
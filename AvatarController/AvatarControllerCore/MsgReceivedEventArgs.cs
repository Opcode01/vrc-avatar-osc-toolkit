namespace AvatarController.Infrastructure
{
    public class MsgReceivedEventArgs : EventArgs
    {
        public string Address;
        public List<object> Contents;

        public MsgReceivedEventArgs(string address, List<object> objects)
        {
            Address = address;
            Contents = objects;
        }

        public MsgReceivedEventArgs(string address, object[] objects)
        {
            Address = address;
            Contents = new List<object>(objects);
        }
    }
}

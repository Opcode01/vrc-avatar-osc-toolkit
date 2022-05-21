﻿
namespace OSC
{
    public class OSCMsgReceivedEventArgs : EventArgs
    {
        public string Address;
        public List<object> Contents;

        public OSCMsgReceivedEventArgs(string address, List<object> objects)
        {
            Address = address;
            Contents = objects;
        }

        public OSCMsgReceivedEventArgs(string address, object[] objects)
        {
            Address = address;
            Contents = new List<object>(objects);
        }
    }
}

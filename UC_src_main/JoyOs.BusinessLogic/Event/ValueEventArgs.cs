using System;

namespace JoyOs.BusinessLogic.Event
{
    public class ValueEventArgs : EventArgs
    {
        public string Value { get; private set; }
        public ValueEventArgs(string value)
        {
           Value = value;
        }
    }

    public delegate void ValueEventHandler(object sender, ValueEventArgs e);
}

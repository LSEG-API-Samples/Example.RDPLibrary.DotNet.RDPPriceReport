using System;

namespace ChainExpanderLib.Events
{
    public class ChainErrorEventArgs
    {
        public DateTime TimeStamp { get; set; }
        public string ErrorMessage { get; set; }

    }
}

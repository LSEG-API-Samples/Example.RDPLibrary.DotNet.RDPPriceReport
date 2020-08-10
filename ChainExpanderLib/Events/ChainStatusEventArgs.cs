using System;
using ChainExpanderLib.Models.Enum;
using ChainExpanderLib.Models.Message;

namespace ChainExpanderLib.Events
{
    public class ChainStatusMsgEventArgs
    {
        public DateTime TimeStamp { get; set; }
        public StatusMessage Status { get; set; }

    }
}

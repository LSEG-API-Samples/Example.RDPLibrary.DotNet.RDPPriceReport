using System;
using System.Collections.Generic;

namespace ChainExpanderLib.Events
{
    public class ChainMessageEventArgs
    {
        public DateTime TimeStamp { get; set; }
        public IEnumerable<string> ItemList{ get; set; }
        public IEnumerable<string> ChainList { get; set; }
        // Indicate extraction success or failed, it could be invalid RIC or client does not have permission to retreive the chain so it get status close
        public bool IsSuccess { set; get; } 
        //Additional message
        public string Message { set; get; } 
    }
}

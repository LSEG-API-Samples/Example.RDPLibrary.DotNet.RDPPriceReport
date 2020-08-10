using System.Collections.Generic;
using ChainExpanderLib.Models.Enum;

namespace ChainExpanderLib.Models.Data
{

    internal interface IChain
    {
        int StreamId { get; set; }
        string RDNDISPLAY { get; set; }
        string DSPLY_NAME { get; set; }
        int REF_COUNT { get; set; }
        int RECORD_TYPE { get; set; }
        string PREF_DISP { get; set; }
        // Return list of underlying constituent or ric name under chain ric
        IEnumerable<string> Constituents { get; } 
        bool IsLast { get; }
        bool IsFirst { get; }
        ChainTemplateEnum TemplateType { get; set; }
    }
}

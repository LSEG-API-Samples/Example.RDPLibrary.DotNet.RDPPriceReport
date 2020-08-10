using System.Collections.Generic;
using System.Linq;
using ChainExpanderLib.Utils;
using ChainExpanderLib.Models.Enum;

namespace ChainExpanderLib.Models.Data
{
    internal class ChainBrLink : IChain
    {
        public int StreamId { get; set; }
        public string RDNDISPLAY { get; set; }
        public string DSPLY_NAME { get; set; }
        public int REF_COUNT { get; set; }
        public int RECORD_TYPE { get; set; }
        public string PREF_DISP { get; set; }
        public IEnumerable<string> Constituents
        {
            get
            {
                var ricList = new List<string>();
                if (!string.IsNullOrEmpty(BR_LINK1)) ricList.Add(BR_LINK1.Trim());
                if (!string.IsNullOrEmpty(BR_LINK2)) ricList.Add(BR_LINK2.Trim());
                if (!string.IsNullOrEmpty(BR_LINK3)) ricList.Add(BR_LINK3.Trim());
                if (!string.IsNullOrEmpty(BR_LINK4)) ricList.Add(BR_LINK4.Trim());
                if (!string.IsNullOrEmpty(BR_LINK5)) ricList.Add(BR_LINK5.Trim());
                if (!string.IsNullOrEmpty(BR_LINK6)) ricList.Add(BR_LINK6.Trim());
                if (!string.IsNullOrEmpty(BR_LINK7)) ricList.Add(BR_LINK7.Trim());
                if (!string.IsNullOrEmpty(BR_LINK8)) ricList.Add(BR_LINK8.Trim());
                if (!string.IsNullOrEmpty(BR_LINK9)) ricList.Add(BR_LINK9.Trim());
                if (!string.IsNullOrEmpty(BR_LINK10)) ricList.Add(BR_LINK10.Trim());
                if (!string.IsNullOrEmpty(BR_LINK11)) ricList.Add(BR_LINK11.Trim());
                if (!string.IsNullOrEmpty(BR_LINK12)) ricList.Add(BR_LINK12.Trim());
                if (!string.IsNullOrEmpty(BR_LINK13)) ricList.Add(BR_LINK13.Trim());
                if (!string.IsNullOrEmpty(BR_LINK14)) ricList.Add(BR_LINK14.Trim());
                return ricList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            }
        }
        public bool IsLast => string.IsNullOrEmpty(BR_NEXTLR);
        public bool IsFirst => string.IsNullOrEmpty(BR_PREVLR);
        public ChainTemplateEnum TemplateType { get; set; }
    
        public string BR_LINK1 { get; set; }
        public string BR_LINK2 { get; set; }
        public string BR_LINK3 { get; set; }
        public string BR_LINK4 { get; set; }
        public string BR_LINK5 { get; set; }
        public string BR_LINK6 { get; set; }
        public string BR_LINK7 { get; set; }
        public string BR_LINK8 { get; set; }
        public string BR_LINK9 { get; set; }
        public string BR_LINK10 { get; set; }
        public string BR_LINK11 { get; set; }
        public string BR_LINK12 { get; set; }
        public string BR_LINK13 { get; set; }
        public string BR_LINK14 { get; set; }
        public string BR_PREVLR { get; set; }

        public string BR_NEXTLR { get; set; }
        public string PREV_DISP { get; set; }
    }
}
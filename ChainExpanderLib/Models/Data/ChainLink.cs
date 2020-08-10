using System.Collections.Generic;
using System.Linq;
using ChainExpanderLib.Models.Enum;

namespace ChainExpanderLib.Models.Data
{
    internal class ChainLink : IChain
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
                if (!string.IsNullOrEmpty(LINK_1)) ricList.Add(LINK_1.Trim());
                if (!string.IsNullOrEmpty(LINK_2)) ricList.Add(LINK_2.Trim());
                if (!string.IsNullOrEmpty(LINK_3)) ricList.Add(LINK_3.Trim());
                if (!string.IsNullOrEmpty(LINK_4)) ricList.Add(LINK_4.Trim());
                if (!string.IsNullOrEmpty(LINK_5)) ricList.Add(LINK_5.Trim());
                if (!string.IsNullOrEmpty(LINK_6)) ricList.Add(LINK_6.Trim());
                if (!string.IsNullOrEmpty(LINK_7)) ricList.Add(LINK_7.Trim());
                if (!string.IsNullOrEmpty(LINK_8)) ricList.Add(LINK_8.Trim());
                if (!string.IsNullOrEmpty(LINK_9)) ricList.Add(LINK_9.Trim());
                if (!string.IsNullOrEmpty(LINK_10)) ricList.Add(LINK_10.Trim());
                if (!string.IsNullOrEmpty(LINK_11)) ricList.Add(LINK_11.Trim());
                if (!string.IsNullOrEmpty(LINK_12)) ricList.Add(LINK_12.Trim());
                if (!string.IsNullOrEmpty(LINK_13)) ricList.Add(LINK_13.Trim());
                if (!string.IsNullOrEmpty(LINK_14)) ricList.Add(LINK_14.Trim());
                return ricList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            }
        }
    
        public bool IsLast => string.IsNullOrEmpty(NEXT_LR);
        public bool IsFirst => string.IsNullOrEmpty(PREV_LR);
        public ChainTemplateEnum TemplateType { get; set; }
        public string LINK_1 { get; set; }
        public string LINK_2 { get; set; }
        public string LINK_3 { get; set; }
        public string LINK_4 { get; set; }
        public string LINK_5 { get; set; }
        public string LINK_6 { get; set; }
        public string LINK_7 { get; set; }
        public string LINK_8 { get; set; }
        public string LINK_9 { get; set; }
        public string LINK_10 { get; set; }
        public string LINK_11 { get; set; }
        public string LINK_12 { get; set; }
        public string LINK_13 { get; set; }
        public string LINK_14 { get; set; }
        public string PREV_LR { get; set; }
        public string NEXT_LR { get; set; }
        public string PREF_LINK { get; set; }

    }
}
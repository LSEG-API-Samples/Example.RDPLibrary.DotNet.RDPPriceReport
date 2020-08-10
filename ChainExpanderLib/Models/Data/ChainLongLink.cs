using System.Collections.Generic;
using System.Linq;
using ChainExpanderLib.Models.Enum;

namespace ChainExpanderLib.Models.Data
{
    internal class ChainLongLink : IChain
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
                if(!string.IsNullOrEmpty(LONGLINK1)) ricList.Add(LONGLINK1.Trim());
                if (!string.IsNullOrEmpty(LONGLINK2)) ricList.Add(LONGLINK2.Trim());
                if (!string.IsNullOrEmpty(LONGLINK3)) ricList.Add(LONGLINK3.Trim());
                if (!string.IsNullOrEmpty(LONGLINK4)) ricList.Add(LONGLINK4.Trim());
                if (!string.IsNullOrEmpty(LONGLINK5)) ricList.Add(LONGLINK5.Trim());
                if (!string.IsNullOrEmpty(LONGLINK6)) ricList.Add(LONGLINK6.Trim());
                if (!string.IsNullOrEmpty(LONGLINK7)) ricList.Add(LONGLINK7.Trim());
                if (!string.IsNullOrEmpty(LONGLINK8)) ricList.Add(LONGLINK8.Trim());
                if (!string.IsNullOrEmpty(LONGLINK9)) ricList.Add(LONGLINK9.Trim());
                if (!string.IsNullOrEmpty(LONGLINK10)) ricList.Add(LONGLINK10.Trim());
                if (!string.IsNullOrEmpty(LONGLINK11)) ricList.Add(LONGLINK11.Trim());
                if (!string.IsNullOrEmpty(LONGLINK12)) ricList.Add(LONGLINK12.Trim());
                if (!string.IsNullOrEmpty(LONGLINK13)) ricList.Add(LONGLINK13.Trim());
                if (!string.IsNullOrEmpty(LONGLINK14)) ricList.Add(LONGLINK14.Trim());
                return ricList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            }
        }

        public bool IsLast => string.IsNullOrEmpty(LONGNEXTLR);
        public bool IsFirst => string.IsNullOrEmpty(LONGPREVLR);
        public ChainTemplateEnum TemplateType { get; set; }
        public string LONGLINK1 { get; set; }
        public string LONGLINK2 { get; set; }
        public string LONGLINK3 { get; set; }
        public string LONGLINK4 { get; set; }
        public string LONGLINK5 { get; set; }
        public string LONGLINK6 { get; set; }
        public string LONGLINK7 { get; set; }
        public string LONGLINK8 { get; set; }
        public string LONGLINK9 { get; set; }
        public string LONGLINK10 { get; set; }
        public string LONGLINK11 { get; set; }
        public string LONGLINK12 { get; set; }
        public string LONGLINK13 { get; set; }
        public string LONGLINK14 { get; set; }
        public string LONGPREVLR { get; set; }
        public string LONGNEXTLR { get; set; }
        public string PREF_LINK { get; set; }
        public string PREV_DISP { get; set; }
    }
}
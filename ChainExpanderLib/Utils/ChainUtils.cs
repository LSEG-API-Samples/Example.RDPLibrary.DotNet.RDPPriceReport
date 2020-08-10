using System;
using System.Collections.Generic;
using ChainExpanderLib.Models.Enum;

namespace ChainExpanderLib.Utils
{
    public class ChainUtils
    {
        // Verify if fieldlist (Dictionary<fieldname,data>) contains a Chain and return ChainTemplateEnum
        public static ChainTemplateEnum GetChainTemplate(IDictionary<string, dynamic> fieldlist)
        {
            if (fieldlist.ContainsKey("REF_COUNT") && fieldlist.ContainsKey("LINK_1")
                                                   && fieldlist.ContainsKey("LINK_5") &&
                                                   fieldlist.ContainsKey("LINK_14")
                                                   && fieldlist.ContainsKey("NEXT_LR") &&
                                                   fieldlist.ContainsKey("PREV_LR"))
                return ChainTemplateEnum.LinkEnum;
            else if (fieldlist.ContainsKey("REF_COUNT") && fieldlist.ContainsKey("LONGLINK1")
                                                        && fieldlist.ContainsKey("LONGLINK5") &&
                                                        fieldlist.ContainsKey("LONGLINK14")
                                                        && fieldlist.ContainsKey("LONGNEXTLR") &&
                                                        fieldlist.ContainsKey("LONGPREVLR"))
                return ChainTemplateEnum.LongLinkEnum;
            else if (fieldlist.ContainsKey("REF_COUNT") && fieldlist.ContainsKey("BR_LINK1")
                                                        && fieldlist.ContainsKey("BR_LINK5") &&
                                                        fieldlist.ContainsKey("BR_LINK14")
                                                        && fieldlist.ContainsKey("BR_NEXTLR") &&
                                                        fieldlist.ContainsKey("BR_PREVLR"))
                return ChainTemplateEnum.BrLinkEnum;

            return ChainTemplateEnum.None;
        }

        public static bool IsChainRIC(string ricname)
        {
            if (ricname.Contains("#"))
                return true;
            return false;
        }
        public static async void ChainListToFile(string absolutePathToFile,List<string> itemList)
        {
            if (itemList is null || itemList.Count < 1)
            {
                Console.WriteLine("Operation Aborted. Item List is null or no RIC in the the list");
                return;
            }

            Console.WriteLine($"Writing RIC list to file {absolutePathToFile}");
            using (var file = new System.IO.StreamWriter(@absolutePathToFile))
            {
                foreach (var item in itemList)
                {
                    await file.WriteLineAsync(item);
                }
            }

            Console.WriteLine($"Operation Completed.");
        }


    }
}

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using ChainExpanderLib.Utils;

namespace ChainExpanderLib.Models.Data
{
    internal class ChainData
    {
        public string StartChainRic { get; set; }
        private readonly SortedDictionary<string,IChain> _chains=new SortedDictionary<string, IChain>(new ChainComparer());
        public int GetStreamId(string chain_ric)
        {
            if (string.IsNullOrEmpty(chain_ric) || !_chains.ContainsKey(chain_ric)) return -1;
            return _chains[chain_ric].StreamId;
        }

       
        public SortedDictionary<string, IChain> ChainList => _chains;
        public void Clear()
        {
            _chains.Clear();
        }

        public int Count => _chains.Count;
        public bool Add(string chain_ric,IChain data)
        {
            try
            {
                if (string.IsNullOrEmpty(chain_ric)) return false;
                if (_chains.ContainsKey(chain_ric)) return false;
                _chains.Add(chain_ric, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return true;
        }

        public bool Remove(string chain_ric)
        {
            if (string.IsNullOrEmpty(chain_ric)) return false;
            if (_chains.ContainsKey(chain_ric)) return false;
            _chains.Remove(chain_ric);
            return true;
        }

        public bool Update(string chain_ric, IChain data)
        {
            if (string.IsNullOrEmpty(chain_ric)) return false;
            if (_chains.ContainsKey(chain_ric)) return false;
            _chains[chain_ric] = data;
          
            return true;
        }
        
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChainExpanderLib.Extensions;
using ChainExpanderLib.Events;
using ChainExpanderLib.Models.Data;
using ChainExpanderLib.Models.Enum;
using ChainExpanderLib.Models.Message;
using ChainExpanderLib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refinitiv.DataPlatform.Content;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery;
using Refinitiv.DataPlatform.Delivery.Stream;


namespace ChainExpanderLib
{
    public class ChainExpander
    {
        private bool _isSequentialMode = true;
        private bool _useHex = false;
        private string _subRic = string.Empty;
        private string _indexRic = string.Empty;
        private static int _startIndex = 11;
        private static int _stopIndex = 50;
        private static int _batchSize = 1500;
        private int _processRound = 1;

        private readonly ChainData _chainData=new ChainData();
        private readonly SortedDictionary<string,ChainRequestStatusEnum> _chainList=new SortedDictionary<string, ChainRequestStatusEnum>(new ChainComparer());
        public bool IsOperationCompleted { get; set; } = false;
        private readonly ConcurrentDictionary<string, IStream> _streamCache;
        public int MaxBatchSize { get; set; } = 0;
        internal ISession _session =null;
        public int OverrideStopIndexValue
        {
            get => _stopIndex;
            set => _stopIndex = value;
        }
        public ChainExpander(ISession rdpsession)
        {
            _session = rdpsession;
            _streamCache = new ConcurrentDictionary<string, IStream>();
        }

        public async Task<bool> IsChainRicAsync(string chainRic)
        {
            var isChainRic = false;
            var isComlete = false;
            await Task.Run(() =>
            {
                IEnumerable<string> chainTestFileds = new string[]
                {
                    "REF_COUNT", "LINK_1", "LINK_5", "LINK_14", "NEXT_LR", "PREV_LR", "LONGLINK1", "LONGLINK5",
                    "LONGLINK14", "LONGNEXTLR", "LONGPREVLR", "BR_LINK1", "BR_LINK5", "BR_LINK14", "BR_NEXTLR",
                    "BR_PREVLR"
                };
                
                var itemParams = new ItemStream.Params().Session(_session)
                    .WithStreaming(false)
                    .WithFields(chainTestFileds)
                    .OnRefresh((s, msg) =>
                    {
                        var message = msg.ToObject<MarketPriceRefreshMessage>();
                        if (message?.Fields != null)
                        {
                            var templateEnum = ChainUtils.GetChainTemplate(message.Fields);
                            if (templateEnum != ChainTemplateEnum.None)
                                isChainRic = true;
                        }
                        isComlete = true;
                    })
                    .OnStatus((s, msg) => { isComlete = true; });

                IStream stream = DeliveryFactory.CreateStream(itemParams.Name(chainRic));
                // Open the stream asynchronously and keep track of the task
                stream.OpenAsync();
             
            }).ConfigureAwait(true);
            while (!isComlete) ;
            return isChainRic;
        }
        private async void OpenSnapshot(string ricName)
        {

            var itemParams = new ItemStream.Params().Session(_session)
                    .OnRefresh(ProcessRefresh)
                    .WithStreaming(false)
                    .OnStatus(ProcessStatus);

                var stream = DeliveryFactory.CreateStream(itemParams.Name(ricName));
                if (_streamCache.TryAdd(ricName, stream))
                {
                    await stream.OpenAsync().ConfigureAwait(false);
                }
               
        }

        private async Task OpenBatchSnapshot(string batch_ric_list)
        {
            await Task.Run(() =>
            {
                foreach (var ric in batch_ric_list.Split(','))
                {
                    OpenSnapshot(ric);
                }
            }).ConfigureAwait(false);

        }
       
        void ProcessRefresh(IStream stream, JObject refreshMsg)
        {
            ProcessChainResponseMessage(refreshMsg, MessageTypeEnum.Refresh);
        }

        void ProcessUpdate(IStream stream, JObject updateMsg)
        {
            
            throw new NotImplementedException();
        }
        void ProcessStatus(IStream stream, JObject statusMsg)
        {
            ProcessChainResponseMessage(statusMsg, MessageTypeEnum.Status);
        }
        private static List<string> GenItemList(int startIndex, int stopIndex,string subRic,bool useHex)
        {

            var itemList = new List<string>();

            for (var i = startIndex; i <= stopIndex; i++)
            {
                itemList.Add(!useHex ? $"{i}#{subRic}" : $"{i:X}#{subRic}");
            }
            
   
            return itemList;
        }
        public Task RunExtractionAsync(string chainRic, bool sequentialMode = true)
        {
            return Task.Run(() =>
            {
                _isSequentialMode = sequentialMode;
                IsOperationCompleted = false;
                _startIndex = 0;
                _stopIndex = 50;
                _streamCache.Clear();
                _chainData.Clear();
                _chainList.Clear();
                _chainData.StartChainRic = chainRic;

                if (_isSequentialMode)
                {
                    _chainList.Add(chainRic, ChainRequestStatusEnum.Wait);
                    OpenSnapshot(chainRic);
                    //await _websocketMarketDataMgr.SendMarketPriceRequestAsync(chainRic, _streamId, false).ConfigureAwait(false);
                }
                else
                {
                    //Method 1

                    var tempStr = chainRic.Split('#').ToList();
                    _subRic = tempStr.Count > 1 ? tempStr[1] : chainRic;
                    _indexRic = tempStr.Count > 1 ? "0" : string.Empty;
                    //Out($"Start retrieving {_indexRic}{(string.IsNullOrEmpty(_indexRic) ? string.Empty : "#")}{_subRic}", true);
                    _chainList.Add(chainRic, ChainRequestStatusEnum.Wait);
                    for (var i = 0; i <= 9; i++)
                    {
                        if (!_chainList.ContainsKey($"{i}#{_subRic}"))
                            _chainList.Add($"{i}#{_subRic}", ChainRequestStatusEnum.Wait);
                    }

                    _chainList.Add($"10#{_subRic}", ChainRequestStatusEnum.Wait);
                    _chainList.Add($"A#{_subRic}", ChainRequestStatusEnum.Wait);
                    _chainList.Add($"60#{_subRic}", ChainRequestStatusEnum.Wait);
                    _chainList.Add($"3C#{_subRic}", ChainRequestStatusEnum.Wait);
                    _chainList.Add($"95#{_subRic}", ChainRequestStatusEnum.Wait);
                    _chainList.Add($"5F#{_subRic}", ChainRequestStatusEnum.Wait);
                    _chainList.Add($"1F4#{_subRic}", ChainRequestStatusEnum.Wait);
                    _chainList.Add($"500#{_subRic}", ChainRequestStatusEnum.Wait);
                    _chainList.Add($"3E8#{_subRic}", ChainRequestStatusEnum.Wait);
                    _chainList.Add($"1000#{_subRic}", ChainRequestStatusEnum.Wait);
                    _processRound = 1;
                    var batchItemList = string.Join(",", _chainList.Keys);
                    OpenBatchSnapshot(batchItemList).GetAwaiter().GetResult();

                }
            });
        }
        private static bool AllReceived(SortedDictionary<string, ChainRequestStatusEnum> subscriptionList)
        {
            //return !(subscriptionList.Where(x => x.Value == ChainRequestStatusEnum.Wait).Select(y => y.Key)).Any();
            return subscriptionList.All(data => data.Value != ChainRequestStatusEnum.Wait);
        }

        private void ProcessChainResponseMessage(JToken jsonData, MessageTypeEnum msgType)
        {
            switch (msgType)
            {
                case MessageTypeEnum.Refresh:
                    {
                        var message = jsonData.ToObject<MarketPriceRefreshMessage>();
                        if (message != null)
                        {
                            message.MsgType = MessageTypeEnum.Refresh;
                            var id = message.ID;
                            var ricName = MarketDataUtils.StringListToString(message.Key.Name);

                            if(_chainList.ContainsKey(ricName))
                                _chainList[ricName] = ChainRequestStatusEnum.Received;

                            if (message.Fields != null)
                            {
                                var templateEnum = ChainUtils.GetChainTemplate(message.Fields);
                                string nextric = string.Empty, prevric = String.Empty;
                                IChain fieldData = default;

                                switch (templateEnum)
                                {
                                    case ChainTemplateEnum.None:
                                    {
                                        _chainList.Clear();
                                        _chainData.Clear();
                                        GenerateResult(_chainData.ChainList,
                                            $"Extraction Failed because {ricName} is not a valid Chain RIC.", false);
                                        return;
                                    }
                                    case ChainTemplateEnum.LinkEnum:
                                    {
                                        fieldData = (ChainLink) message.Fields.ToObject<ChainLink>();
                                        fieldData.StreamId = id.GetValueOrDefault();
                                        _chainData.Add(ricName, fieldData);
                                        _chainData.ChainList[ricName].TemplateType = templateEnum;
                                        nextric = ((ChainLink) fieldData).NEXT_LR;
                                        prevric = ((ChainLink) fieldData).PREV_LR;
                                    }
                                        break;
                                    case ChainTemplateEnum.LongLinkEnum:
                                    {
                                        fieldData = message.Fields.ToObject<ChainLongLink>() as ChainLongLink;
                                        fieldData.StreamId = id.GetValueOrDefault();
                                        _chainData.Add(ricName, fieldData);
                                        _chainData.ChainList[ricName].TemplateType = templateEnum;
                                        nextric = ((ChainLongLink) fieldData).LONGNEXTLR;
                                        prevric = ((ChainLongLink) fieldData).LONGPREVLR;
                                    }
                                        break;
                                    case ChainTemplateEnum.BrLinkEnum:
                                    {
                                        fieldData = (ChainBrLink) message.Fields.ToObject<ChainBrLink>();
                                        fieldData.StreamId = id.GetValueOrDefault();
                                        _chainData.Add(ricName, fieldData);
                                        _chainData.ChainList[ricName].TemplateType = templateEnum;
                                        nextric = ((ChainBrLink) fieldData).BR_NEXTLR;
                                        prevric = ((ChainBrLink) fieldData).BR_PREVLR;
                                    }
                                        break;

                                    default:
                                    {
                                        RaiseErrorEvent(DateTime.Now, "Found Unexpected Template Enum {templateEnum}");
                                    }
                                        break;
                                }
                                
                                if (_isSequentialMode)
                                {
                                    if (fieldData != null && (!string.IsNullOrEmpty(nextric) && !fieldData.IsLast &&
                                                              !_chainList.ContainsKey(nextric)))
                                    {
                                        OpenSnapshot(nextric);
                                    }

                                    if (fieldData != null && (!string.IsNullOrEmpty(prevric) && !fieldData.IsFirst &&
                                                              !_chainList.ContainsKey(prevric)))
                                    {
                                        OpenSnapshot(prevric);
                                    }
                                }
                            }
                        }
                    }
                    break;
               
                case MessageTypeEnum.Status:
                    {
                        var message = jsonData.ToObject<StatusMessage>();
                        var ricName = message != null && message.Key != null && message.Key.Name==null ? string.Empty : MarketDataUtils.StringListToString(message.Key.Name);

                        if (string.IsNullOrEmpty(ricName))
                        {
                            throw new InvalidOperationException(
                                "RIC name in status message should not be empty string");
                        }
                        // Console.WriteLine($"Status message Ric Name:{ricName} {message.State.Text}");
                        //Check if item stream is closed or closed recover and resend item request again if Login still open.
                        if (message.State.Stream == StreamStateEnum.Closed ||
                            message.State.Stream == StreamStateEnum.ClosedRecover)
                        {
                            if (_chainList.ContainsKey(ricName))
                            {
                                _chainList[ricName] = ChainRequestStatusEnum.NotFound;
                            }

                            if (ricName == _chainData.StartChainRic)
                            {
                                GenerateResult(_chainData.ChainList, $"Chain Extraction failed {message.State.Text}", false);
                                return;
                            }
                            else
                            {
                                if (_isSequentialMode)
                                {
                                    GenerateResult(_chainData.ChainList, $"Chain Extraction stop because it can't retrieve next ric {ricName}. Code:{message.State.Code}", false);
                                    return;
                                }


                            }
                        }
                    }
                    break;
            }
            if (_chainData.Count > 0 && AllReceived(_chainList))
            {
                if ((_chainData.ChainList.Any(item => item.Value.IsFirst)) &&
                    _chainData.ChainList.Any(item => item.Value.IsLast))
                {
                    GenerateResult(_chainData.ChainList, "Extraction completed successful.");
                    return;
                }


                if (!_isSequentialMode)
                {
                    // First batch received and used to evaluate if the chain use hex or dec and what should be proper incementValue
                    if (_processRound == 1)
                    {
                        if (MaxBatchSize <= 0 && _chainList != null)
                        {
                            if (_chainList[$"A#{_subRic}"] == ChainRequestStatusEnum.Received)
                                _useHex = true;

                            if (_chainList[$"A#{_subRic}"] == ChainRequestStatusEnum.NotFound &&
                                _chainList[$"10#{_subRic}"] == ChainRequestStatusEnum.NotFound)
                                _batchSize = 10;
                            else if (_chainList[$"60#{_subRic}"] == ChainRequestStatusEnum.NotFound &&
                                     _chainList[$"3C#{_subRic}"] == ChainRequestStatusEnum.NotFound)
                                _batchSize = 50;
                            else if (_chainList[$"95#{_subRic}"] == ChainRequestStatusEnum.NotFound &&
                                     _chainList[$"5F#{_subRic}"] == ChainRequestStatusEnum.NotFound)
                                _batchSize = 90;
                            else if (_chainList[$"1F4#{_subRic}"] == ChainRequestStatusEnum.NotFound &&
                                     _chainList[$"500#{_subRic}"] == ChainRequestStatusEnum.NotFound)
                                _batchSize = 500;
                            else if (_chainList[$"3E8#{_subRic}"] == ChainRequestStatusEnum.NotFound &&
                                     _chainList[$"1000#{_subRic}"] == ChainRequestStatusEnum.NotFound)
                                _batchSize = 1000;

                            _stopIndex = _batchSize;
                        }
                        else
                        {
                            _batchSize = MaxBatchSize;
                        }

                        _processRound++;
                    }
                    else
                    {
                        var nextRicInTheBatch = GetNextRicInCurrentBatch();

                        if (_chainList.ContainsKey(nextRicInTheBatch) &&
                            !string.IsNullOrEmpty(nextRicInTheBatch) && _chainList[nextRicInTheBatch] ==
                            ChainRequestStatusEnum.NotFound)
                        {
                            GenerateResult(_chainData.ChainList,
                                $"\n\nChain Extraction stop because application unable to get data for Next RIC {nextRicInTheBatch} and it return status : NotFound",
                                false);
                            return;
                        }
                    }


                    var itemList = GenItemList(_startIndex, _stopIndex, _subRic, _useHex);

                    var batchList = new StringBuilder();
                    foreach (var item in itemList)
                    {
                        if (_chainList.ContainsKey(item)) continue;

                        _chainList.Add(item, ChainRequestStatusEnum.Wait);
                        batchList.Append(item);
                        if (item != itemList.Last())
                            batchList.Append(",");
                    }

                    //_websocketMarketDataMgr.SendMarketPriceRequestAsync(batchList.ToString(), _streamId++, false).GetAwaiter().GetResult();
                    OpenBatchSnapshot(batchList.ToString()).GetAwaiter().GetResult();
                    _startIndex = _stopIndex + 1;
                    _stopIndex += _batchSize;
                }
            }

        }
        private string GetNextRicInCurrentBatch()
        {
            var receiveList = _chainList.Where(y => y.Value == ChainRequestStatusEnum.Received);
            var keyValuePairs = receiveList as KeyValuePair<string, ChainRequestStatusEnum>[] ?? receiveList.ToArray();
            if (!keyValuePairs.Any()) return string.Empty;
            if (!_chainData.ChainList.ContainsKey(keyValuePairs.Last().Key)) return null;
            switch (_chainData.ChainList[keyValuePairs.Last().Key].TemplateType)
            {
                case ChainTemplateEnum.None:
                    throw new InvalidOperationException();
                case ChainTemplateEnum.LinkEnum:
                    return (_chainData.ChainList[keyValuePairs.Last().Key] as ChainLink)?.NEXT_LR;
                case ChainTemplateEnum.LongLinkEnum:
                    return (_chainData.ChainList[keyValuePairs.Last().Key] as ChainLongLink)?.LONGNEXTLR;
                case ChainTemplateEnum.BrLinkEnum:
                    return ((ChainBrLink)_chainData.ChainList[keyValuePairs.Last().Key]).BR_NEXTLR;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }


        private void GenerateResult(SortedDictionary<string, IChain> data, string message, bool isSuccess = true)
        {
            var itemList = new List<string>();
            try
            {

                foreach (var constituent in data.Values)
                {
                    itemList.AddRange(constituent.Constituents);
                }

                var chainList = _chainList.Where(x => x.Value == ChainRequestStatusEnum.Received).Select(y => y.Key).ToList();
                IsOperationCompleted = true;
                RaiseExtractionCompleteEvent(DateTime.Now, itemList, chainList, isSuccess,
                    message);
            }
            catch (Exception ex)
            {
                RaiseErrorEvent(DateTime.Now, $"{ex.Message} {ex.StackTrace}");
            }

        }
        protected void RaiseErrorEvent(DateTime timestamp, string errorMsg)
        {
            var errorCallback = new ChainErrorEventArgs() { TimeStamp = timestamp, ErrorMessage = errorMsg };
            OnError(errorCallback);
        }
        protected void RaiseExtractionCompleteEvent(DateTime timestamp, IEnumerable<string> items, IEnumerable<string> chains, bool isSuccess, string message)
        {
            var messageCallback = new ChainMessageEventArgs() { ChainList = chains, ItemList = items, TimeStamp = timestamp, IsSuccess = isSuccess, Message = message };
            OnMessage(messageCallback);
        }
        protected virtual void OnMessage(ChainMessageEventArgs e)
        {
            var handler = OnExtractionCompleteEvent;
            handler?.Invoke(this, e);
        }
        protected virtual void OnError(ChainErrorEventArgs e)
        {
            var handler = OnExtractionErrorEvent;
            handler?.Invoke(this, e);
        }
        public event EventHandler<ChainMessageEventArgs> OnExtractionCompleteEvent;
        public event EventHandler<ChainErrorEventArgs> OnExtractionErrorEvent;

    }
}
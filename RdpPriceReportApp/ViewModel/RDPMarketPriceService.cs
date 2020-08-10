using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RdpRealTimePricing.Events;
using RdpRealTimePricing.Model.Enum;
using RdpRealTimePricing.Model.MarketData;
using Refinitiv.DataPlatform.Content;
using Refinitiv.DataPlatform.Delivery;
using Refinitiv.DataPlatform.Delivery.Stream;

namespace RdpPriceReportApp.ViewModel
{
    public class RdpMarketPriceService
    {
        private readonly ConcurrentDictionary<string, IStream> _streamCache;

        public async Task<List<string>> GetFiledList(string item, Refinitiv.DataPlatform.Core.ISession RdpSession)
        {
            var fields = new List<string>();
            bool isComplete = false;
            if (RdpSession != null)
                await Task.Run(() =>
                {
                    using (var stream = Pricing.CreateStreamingPrices(new StreamingPrices.Params().WithStreaming(false).Universe(item)
                        .OnStatus((o, item, status) => { isComplete = true; })))
                    {
                        if (stream.Open() == Stream.State.Opened)
                        {
                            // Retrieve a snapshot of the whole cache.  The interface also supports the ability to pull out specific items and fields.
                            var snapshot = stream.GetSnapshotData().FirstOrDefault();
                            if (snapshot.Key == item)
                            {
                                var fieldList = ((IPriceData) snapshot.Value).Fields();
                                foreach (var key in fieldList)
                                {
                                    fields.Add((string)key);
                                }
                               
                            }
                            isComplete = true;
                        }
                    }

                }).ConfigureAwait(false);
            else
                throw new ArgumentNullException("RDP Session is null.");
            while (!isComplete) ;
            return fields;
            
        }
        public async Task OpenItemAsync(string item, Refinitiv.DataPlatform.Core.ISession RdpSession, IEnumerable<string> fieldNameList=null,
            bool streamRequest = false)
        {
            if (RdpSession != null)
                await Task.Run(() =>
                {
                    var itemParams = new ItemStream.Params().Session(RdpSession)
                        .WithStreaming(streamRequest)
                        .OnRefresh(processOnRefresh)
                        .OnUpdate(processOnUpdate)
                        .OnStatus(processOnStatus);
                    var nameList = (fieldNameList ?? Array.Empty<string>()).ToList();
                    if (nameList.Any())
                    {
                        // First, prepare our item stream details including the fields of interest and where to capture events...
                        itemParams.WithFields(nameList);
                    }
                    

                    var stream = DeliveryFactory.CreateStream(itemParams.Name(item));
                    if (_streamCache.TryAdd(item, stream))
                    {
                        stream.OpenAsync();
                    }
                    else
                    {
                        var msg = $"Unable to open new stream for item {item}.";
                        RaiseOnError(msg);
                    }
                }).ConfigureAwait(false);
            else
                throw new ArgumentNullException("RDP Session is null.");
        }

        public async Task OpenItemAsync(string item, Refinitiv.DataPlatform.Core.ISession RdpSession,Type modelType,bool streamRequest=false)
        {
           var fieldNameList = modelType.GetProperties()
                    .SelectMany(p => p.GetCustomAttributes(typeof(JsonPropertyAttribute))
                        .Cast<JsonPropertyAttribute>())
                    .Select(prop => prop.PropertyName)
                    .ToArray();
           await OpenItemAsync(item, RdpSession, fieldNameList, streamRequest);

        }
        public async Task OpenItemAsync(string item, Refinitiv.DataPlatform.Core.ISession RdpSession, bool streamRequest = false)
        {
            await OpenItemAsync(item, RdpSession, new List<string>(), streamRequest);

        }


        public RdpMarketPriceService()
        {
            _streamCache = new ConcurrentDictionary<string, IStream>();
        }
        public Task CloseItemStreamAsync(string item)
        {
            return Task.WhenAll(Task.Run(() =>
            {
                if (_streamCache.TryGetValue(item,out var stream))
                {
                    stream.CloseAsync();
                    if (_streamCache.TryRemove(item, out var removedItem))
                    {
                        removedItem = null;
                    }
                }
            }));
        }

     
        #region ItemEventProcessing
        private void processOnRefresh(IStream s, JObject msg)
       {
           var refreshMsg = msg.ToObject<MarketPriceRefreshMessage>();
           RaiseOnMessage(MessageTypeEnum.Refresh, refreshMsg);
       }
       private void processOnUpdate(IStream s, JObject msg)
       {
           var updateMsg = msg.ToObject<MarketPriceUpdateMessage>();
           RaiseOnMessage(MessageTypeEnum.Update, updateMsg);
       }
       private void processOnStatus(IStream s, JObject msg)
       {
           var statusMsg = msg.ToObject<StatusMessage>();
           var itemName = statusMsg.Key.Name.FirstOrDefault();
           if (statusMsg.State.Stream == StreamStateEnum.Closed ||
               statusMsg.State.Stream == StreamStateEnum.ClosedRecover)
               _streamCache.TryRemove(itemName, out var temp);
           RaiseOnMessage(MessageTypeEnum.Status, statusMsg);

       }
        #endregion
        // Event Handler
        #region EventHandler
        public event EventHandler<OnErrorEventArgs> OnErrorEvents;
       public event EventHandler<OnResponseMessageEventArgs> OnResponeMessageEvents;
       protected void RaiseOnMessage(MessageTypeEnum msgtype, IMessage message)
       {
           var messageEvent = new OnResponseMessageEventArgs() { MessageType = msgtype, RespMessage = message };
           OnRespMessage(messageEvent);
       }
       protected virtual void OnRespMessage(OnResponseMessageEventArgs e)
       {
           var handler = OnResponeMessageEvents;
           handler?.Invoke(this, e);
       }
       protected void RaiseOnError(string message)
       {
           var errorEvent = new OnErrorEventArgs() { Message = message };
           OnError(errorEvent);
       }
       protected virtual void OnError(OnErrorEventArgs e)
       {
           var handler = OnErrorEvents;
           handler?.Invoke(this, e);
       }
        #endregion EventHandler
    }
}

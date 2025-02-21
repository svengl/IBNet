/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;
using System.Collections.Generic;
using System.Linq;
using IBApi;
using System.Threading;
using IBNet.Messages;

namespace IBNet
{
    /// <summary>
    /// Handles all communications from the TWS.
    /// Source: IB sample app.
    /// https://interactivebrokers.github.io/tws-api/historical_bars.html
    /// </summary>
    public partial class IBClient
    {
        public IBClient(bool synchronize  = false)
        {
            ClientSocket = new EClientSocket(this, new EReaderMonitorSignal());

            if (synchronize)
                _sc = SynchronizationContext.Current;
        }

        private SynchronizationContext _sc;

        #region EWrapper ------------------------------------------------------

        /// <summary>
        /// TickerId, ErrorCode, ErrorMessageCode, Exception
        /// </summary>
        public event Action<int, int, string, Exception> Error;

        void EWrapper.error(Exception e)
        {
            var tmp = Error;

            if (tmp != null)
                FireEvent(t => tmp(0, 0, null, e), null);
        }

        void EWrapper.error(string str)
        {
            var tmp = Error;

            if (tmp != null)
                FireEvent(t => tmp(0, 0, str, null), null);
        }

        void EWrapper.error(int id, int errorCode, string errorMsg)
        {
            var tmp = Error;

            if (tmp != null)
                FireEvent(t => tmp(id, errorCode, errorMsg, null), null);
        }

        public event Action ConnectionClosed;

        void EWrapper.connectionClosed()
        {
            var tmp = ConnectionClosed;

            if (tmp != null)
                FireEvent(t => tmp(), null);
        }

        public event Action<long> CurrentTime;

        void EWrapper.currentTime(long time)
        {
            var tmp = CurrentTime;

            if (tmp != null)
                FireEvent(t => tmp(time), null);
        }

        public event Action<TickPriceMessage> TickPrice;

        void EWrapper.tickPrice(int tickerId, int field, double price, TickAttrib attribs)
        {
            var tmp = TickPrice;

            if (tmp != null)
                FireEvent(t => tmp(new TickPriceMessage(tickerId, field, price, attribs)), null);
        }

        public event Action<TickSizeMessage> TickSize;

        void EWrapper.tickSize(int tickerId, int field, int size)
        {
            var tmp = TickSize;

            if (tmp != null)
                FireEvent(t => tmp(new TickSizeMessage(tickerId, field, size)), null);
        }

        public event Action<int, int, string> TickString;

        void EWrapper.tickString(int tickerId, int tickType, string value)
        {
            var tmp = TickString;

            if (tmp != null)
                FireEvent(t => tmp(tickerId, tickType, value), null);
        }

        public event Action<int, int, double> TickGeneric;

        void EWrapper.tickGeneric(int tickerId, int field, double value)
        {
            var tmp = TickGeneric;

            if (tmp != null)
                FireEvent(t => tmp(tickerId, field, value), null);
        }

        public event Action<int, int, double, string, double, int, string, double, double> TickEFP;

        void EWrapper.tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture, int holdDays, string futureLastTradeDate, double dividendImpact, double dividendsToLastTradeDate)
        {
            var tmp = TickEFP;

            if (tmp != null)
                FireEvent(t => tmp(tickerId, tickType, basisPoints, formattedBasisPoints, impliedFuture, holdDays, futureLastTradeDate, dividendImpact, dividendsToLastTradeDate), null);
        }

        public event Action<int> TickSnapshotEnd;

        void EWrapper.tickSnapshotEnd(int tickerId)
        {
            var tmp = TickSnapshotEnd;

            if (tmp != null)
                FireEvent(t => tmp(tickerId), null);
        }

        public event Action<ConnectionStatusMessage> NextValidId;

        void EWrapper.nextValidId(int orderId)
        {
            NextOrderId = orderId;
            var tmp = NextValidId;

            if (tmp != null)
                FireEvent(t => tmp(new ConnectionStatusMessage(true)), null);
        }

        public event Action<int, DeltaNeutralContract> DeltaNeutralValidation;

        void EWrapper.deltaNeutralValidation(int reqId, DeltaNeutralContract deltaNeutralContract)
        {
            var tmp = DeltaNeutralValidation;

            if (tmp != null)
                FireEvent(t => tmp(reqId, deltaNeutralContract), null);
        }

        public event Action<ManagedAccountsMessage> ManagedAccounts;

        void EWrapper.managedAccounts(string accountsList)
        {
            var tmp = ManagedAccounts;

            if (tmp != null)
                FireEvent(t => tmp(new ManagedAccountsMessage(accountsList)), null);
        }

        public event Action<TickOptionMessage> TickOptionCommunication;

        void EWrapper.tickOptionComputation(int tickerId, int field, int tickAttrib, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
            var tmp = TickOptionCommunication;

            if (tmp != null)
                FireEvent(t => tmp(new TickOptionMessage(tickerId, field, tickAttrib, impliedVolatility, delta, optPrice, pvDividend, gamma, vega, theta, undPrice)), null);
        }
        
        public event Action<AccountSummaryMessage> AccountSummary;

        void EWrapper.accountSummary(int reqId, string account, string tag, string value, string currency)
        {
            var tmp = AccountSummary;

            if (tmp != null)
                FireEvent(t => tmp(new AccountSummaryMessage(reqId, account, tag, value, currency)), null);
        }

        public event Action<AccountSummaryEndMessage> AccountSummaryEnd;

        void EWrapper.accountSummaryEnd(int reqId)
        {
            var tmp = AccountSummaryEnd;

            if (tmp != null)
                FireEvent(t => tmp(new AccountSummaryEndMessage(reqId)), null);
        }

        public event Action<AccountValueMessage> UpdateAccountValue;

        void EWrapper.updateAccountValue(string key, string value, string currency, string accountName)
        {
            var tmp = UpdateAccountValue;

            if (tmp != null)
                FireEvent(t => tmp(new AccountValueMessage(key, value, currency, accountName)), null);
        }

        public event Action<UpdatePortfolioMessage> UpdatePortfolio;

        void EWrapper.updatePortfolio(Contract contract, double position, double marketPrice, double marketValue, double averageCost, double unrealizedPNL, double realizedPNL, string accountName)
        {
            var tmp = UpdatePortfolio;

            if (tmp != null)
                FireEvent(t => tmp(new UpdatePortfolioMessage(contract, position, marketPrice, marketValue, averageCost, unrealizedPNL, realizedPNL, accountName)), null);
        }

        public event Action<UpdateAccountTimeMessage> UpdateAccountTime;

        void EWrapper.updateAccountTime(string timestamp)
        {
            var tmp = UpdateAccountTime;

            if (tmp != null)
                FireEvent(t => tmp(new UpdateAccountTimeMessage(timestamp)), null);
        }

        public event Action<AccountDownloadEndMessage> AccountDownloadEnd;

        void EWrapper.accountDownloadEnd(string account)
        {
            var tmp = AccountDownloadEnd;

            if (tmp != null)
                FireEvent(t => tmp(new AccountDownloadEndMessage(account)), null);
        }

        public event Action<OrderStatusMessage> OrderStatus;

        void EWrapper.orderStatus(int orderId, string status, double filled, double remaining, double avgFillPrice, int permId, int parentId, double lastFillPrice, int clientId, string whyHeld, double mktCapPrice)
        {
            var tmp = OrderStatus;

            if (tmp != null)
                FireEvent(t => tmp(new OrderStatusMessage(orderId, status, filled, remaining, avgFillPrice, permId, parentId, lastFillPrice, clientId, whyHeld, mktCapPrice)), null);
        }

        public event Action<OpenOrderMessage> OpenOrder;

        void EWrapper.openOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            var tmp = OpenOrder;

            if (tmp != null)
                FireEvent(t => tmp(new OpenOrderMessage(orderId, contract, order, orderState)), null);
        }

        public event Action OpenOrderEnd;

        void EWrapper.openOrderEnd()
        {
            var tmp = OpenOrderEnd;

            if (tmp != null)
                FireEvent(t => tmp(), null);
        }

        public event Action<ContractDetailsMessage> ContractDetails;

        void EWrapper.contractDetails(int reqId, ContractDetails contractDetails)
        {
            var tmp = ContractDetails;

            if (tmp != null)
                FireEvent(t => tmp(new ContractDetailsMessage(reqId, contractDetails)), null);
        }

        public event Action<int> ContractDetailsEnd;

        void EWrapper.contractDetailsEnd(int reqId)
        {
            var tmp = ContractDetailsEnd;

            if (tmp != null)
                FireEvent(t => tmp(reqId), null);
        }

        public event Action<ExecutionMessage> ExecDetails;

        void EWrapper.execDetails(int reqId, Contract contract, Execution execution)
        {
            var tmp = ExecDetails;

            if (tmp != null)
                FireEvent(t => tmp(new ExecutionMessage(reqId, contract, execution)), null);
        }

        public event Action<int> ExecDetailsEnd;

        void EWrapper.execDetailsEnd(int reqId)
        {
            var tmp = ExecDetailsEnd;

            if (tmp != null)
                FireEvent(t => tmp(reqId), null);
        }

        public event Action<CommissionReport> CommissionReport;

        void EWrapper.commissionReport(CommissionReport commissionReport)
        {
            var tmp = CommissionReport;

            if (tmp != null)
                FireEvent(t => tmp(commissionReport), null);
        }

        public event Action<FundamentalsMessage> FundamentalData;

        void EWrapper.fundamentalData(int reqId, string data)
        {
            var tmp = FundamentalData;

            if (tmp != null)
                FireEvent(t => tmp(new FundamentalsMessage(data)), null);
        }

        public event Action<HistoricalDataMessage> HistoricalData;

        void EWrapper.historicalData(int reqId, Bar bar)
        {
            var tmp = HistoricalData;

            if (tmp != null)
                FireEvent(t => tmp(new HistoricalDataMessage(reqId, bar)), null);
        }

        public event Action<HistoricalDataEndMessage> HistoricalDataEnd;

        void EWrapper.historicalDataEnd(int reqId, string startDate, string endDate)
        {
            var tmp = HistoricalDataEnd;

            if (tmp != null)
                FireEvent(t => tmp(new HistoricalDataEndMessage(reqId, startDate, endDate)), null);
        }

        public event Action<MarketDataTypeMessage> MarketDataType;

        void EWrapper.marketDataType(int reqId, int marketDataType)
        {
            var tmp = MarketDataType;

            if (tmp != null)
                FireEvent(t => tmp(new MarketDataTypeMessage(reqId, marketDataType)), null);
        }

        public event Action<DeepBookMessage> UpdateMktDepth;

        void EWrapper.updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            var tmp = UpdateMktDepth;

            if (tmp != null)
                FireEvent(t => tmp(new DeepBookMessage(tickerId, position, operation, side, price, size, "", false)), null);
        }

        public event Action<DeepBookMessage> UpdateMktDepthL2;

        void EWrapper.updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size, bool isSmartDepth)
        {
            var tmp = UpdateMktDepthL2;

            if (tmp != null)
                FireEvent(t => tmp(new DeepBookMessage(tickerId, position, operation, side, price, size, marketMaker, isSmartDepth)), null);
        }

        public event Action<int, int, string, string> UpdateNewsBulletin;

        void EWrapper.updateNewsBulletin(int msgId, int msgType, string message, string origExchange)
        {
            var tmp = UpdateNewsBulletin;

            if (tmp != null)
                FireEvent(t => tmp(msgId, msgType, message, origExchange), null);
        }

        public event Action<PositionMessage> Position;

        void EWrapper.position(string account, Contract contract, double pos, double avgCost)
        {
            var tmp = Position;

            if (tmp != null)
                FireEvent(t => tmp(new PositionMessage(account, contract, pos, avgCost)), null);
        }

        public event Action PositionEnd;

        void EWrapper.positionEnd()
        {
            var tmp = PositionEnd;

            if (tmp != null)
                FireEvent(t => tmp(), null);
        }

        public event Action<RealTimeBarMessage> RealtimeBar;

        void EWrapper.realtimeBar(int reqId, long time, double open, double high, double low, double close, long volume, double WAP, int count)
        {
            var tmp = RealtimeBar;

            if (tmp != null)
                FireEvent(t => tmp(new RealTimeBarMessage(reqId, time, open, high, low, close, volume, WAP, count)), null);
        }

        public event Action<string> ScannerParameters;

        void EWrapper.scannerParameters(string xml)
        {
            var tmp = ScannerParameters;

            if (tmp != null)
                FireEvent(t => tmp(xml), null);
        }

        public event Action<ScannerMessage> ScannerData;

        void EWrapper.scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            var tmp = ScannerData;

            if (tmp != null)
                FireEvent(t => tmp(new ScannerMessage(reqId, rank, contractDetails, distance, benchmark, projection, legsStr)), null);
        }

        public event Action<int> ScannerDataEnd;

        void EWrapper.scannerDataEnd(int reqId)
        {
            var tmp = ScannerDataEnd;

            if (tmp != null)
                FireEvent(t => tmp(reqId), null);
        }

        public event Action<AdvisorDataMessage> ReceiveFA;

        void EWrapper.receiveFA(int faDataType, string faXmlData)
        {
            var tmp = ReceiveFA;

            if (tmp != null)
                FireEvent(t => tmp(new AdvisorDataMessage(faDataType, faXmlData)), null);
        }

        public event Action<BondContractDetailsMessage> BondContractDetails;

        void EWrapper.bondContractDetails(int requestId, ContractDetails contractDetails)
        {
            var tmp = BondContractDetails;

            if (tmp != null)
                FireEvent(t => tmp(new BondContractDetailsMessage(requestId, contractDetails)), null);
        }

        public event Action<string> VerifyMessageAPI;

        void EWrapper.verifyMessageAPI(string apiData)
        {
            var tmp = VerifyMessageAPI;

            if (tmp != null)
                FireEvent(t => tmp(apiData), null);
        }

        public event Action<bool, string> VerifyCompleted;

        void EWrapper.verifyCompleted(bool isSuccessful, string errorText)
        {
            var tmp = VerifyCompleted;

            if (tmp != null)
                FireEvent(t => tmp(isSuccessful, errorText), null);
        }

        public event Action<string, string> VerifyAndAuthMessageAPI;

        void EWrapper.verifyAndAuthMessageAPI(string apiData, string xyzChallenge)
        {
            var tmp = VerifyAndAuthMessageAPI;

            if (tmp != null)
                FireEvent(t => tmp(apiData, xyzChallenge), null);
        }

        public event Action<bool, string> VerifyAndAuthCompleted;

        void EWrapper.verifyAndAuthCompleted(bool isSuccessful, string errorText)
        {
            var tmp = VerifyAndAuthCompleted;

            if (tmp != null)
                FireEvent(t => tmp(isSuccessful, errorText), null);
        }

        public event Action<int, string> DisplayGroupList;

        void EWrapper.displayGroupList(int reqId, string groups)
        {
            var tmp = DisplayGroupList;

            if (tmp != null)
                FireEvent(t => tmp(reqId, groups), null);
        }

        public event Action<int, string> DisplayGroupUpdated;

        void EWrapper.displayGroupUpdated(int reqId, string contractInfo)
        {
            var tmp = DisplayGroupUpdated;

            if (tmp != null)
                FireEvent(t => tmp(reqId, contractInfo), null);
        }


        void EWrapper.connectAck()
        {
            if (ClientSocket.AsyncEConnect)
                ClientSocket.startApi();
        }

        public event Action<PositionMultiMessage> PositionMulti;

        void EWrapper.positionMulti(int reqId, string account, string modelCode, Contract contract, double pos, double avgCost)
        {
            var tmp = PositionMulti;

            if (tmp != null)
                FireEvent(t => tmp(new PositionMultiMessage(reqId, account, modelCode, contract, pos, avgCost)), null);
        }

        public event Action<int> PositionMultiEnd;

        void EWrapper.positionMultiEnd(int reqId)
        {
            var tmp = PositionMultiEnd;

            if (tmp != null)
                FireEvent(t => tmp(reqId), null);
        }

        public event Action<AccountUpdateMultiMessage> AccountUpdateMulti;

        void EWrapper.accountUpdateMulti(int reqId, string account, string modelCode, string key, string value, string currency)
        {
            var tmp = AccountUpdateMulti;

            if (tmp != null)
                FireEvent(t => tmp(new AccountUpdateMultiMessage(reqId, account, modelCode, key, value, currency)), null);
        }

        public event Action<int> AccountUpdateMultiEnd;

        void EWrapper.accountUpdateMultiEnd(int reqId)
        {
            var tmp = AccountUpdateMultiEnd;

            if (tmp != null)
                FireEvent(t => tmp(reqId), null);
        }

        public event Action<SecurityDefinitionOptionParameterMessage> SecurityDefinitionOptionParameter;

        void EWrapper.securityDefinitionOptionParameter(int reqId, string exchange, int underlyingConId, string tradingClass, string multiplier, HashSet<string> expirations, HashSet<double> strikes)
        {
            var tmp = SecurityDefinitionOptionParameter;

            if (tmp != null)
                FireEvent(t => tmp(new SecurityDefinitionOptionParameterMessage(reqId, exchange, underlyingConId, tradingClass, multiplier, expirations, strikes)), null);
        }

        public event Action<int> SecurityDefinitionOptionParameterEnd;

        void EWrapper.securityDefinitionOptionParameterEnd(int reqId)
        {
            var tmp = SecurityDefinitionOptionParameterEnd;

            if (tmp != null)
                FireEvent(t => tmp(reqId), null);
        }

        public event Action<SoftDollarTiersMessage> SoftDollarTiers;

        void EWrapper.softDollarTiers(int reqId, SoftDollarTier[] tiers)
        {
            var tmp = SoftDollarTiers;

            if (tmp != null)
                FireEvent(t => tmp(new SoftDollarTiersMessage(reqId, tiers)), null);
        }

        public event Action<FamilyCode[]> FamilyCodes;

        void EWrapper.familyCodes(FamilyCode[] familyCodes)
        {
            var tmp = FamilyCodes;

            if (tmp != null)
                FireEvent(t => tmp(familyCodes), null);
        }

        public event Action<SymbolSamplesMessage> SymbolSamples;

        void EWrapper.symbolSamples(int reqId, ContractDescription[] contractDescriptions)
        {
            var tmp = SymbolSamples;

            if (tmp != null)
                FireEvent(t => tmp(new SymbolSamplesMessage(reqId, contractDescriptions)), null);
        }


        public event Action<DepthMktDataDescription[]> MktDepthExchanges;

        void EWrapper.mktDepthExchanges(DepthMktDataDescription[] depthMktDataDescriptions)
        {
            var tmp = MktDepthExchanges;

            if (tmp != null)
                FireEvent(t => tmp(depthMktDataDescriptions), null);
        }

        public event Action<TickNewsMessage> TickNews;

        void EWrapper.tickNews(int tickerId, long timeStamp, string providerCode, string articleId, string headline, string extraData)
        {
            var tmp = TickNews;

            if (tmp != null)
                FireEvent(t => tmp(new TickNewsMessage(tickerId, timeStamp, providerCode, articleId, headline, extraData)), null);
        }

        public event Action<int, Dictionary<int, KeyValuePair<string, char>>> SmartComponents;

        void EWrapper.smartComponents(int reqId, Dictionary<int, KeyValuePair<string, char>> theMap)
        {
            var tmp = SmartComponents;

            if (tmp != null)
                FireEvent(t => tmp(reqId, theMap), null);
        }

        public event Action<TickReqParamsMessage> TickReqParams;

        void EWrapper.tickReqParams(int tickerId, double minTick, string bboExchange, int snapshotPermissions)
        {
            var tmp = TickReqParams;

            if (tmp != null)
                FireEvent(t => tmp(new TickReqParamsMessage(tickerId, minTick, bboExchange, snapshotPermissions)), null);
        }

        public event Action<NewsProvider[]> NewsProviders;

        void EWrapper.newsProviders(NewsProvider[] newsProviders)
        {
            var tmp = NewsProviders;

            if (tmp != null)
                FireEvent(t => tmp(newsProviders), null);
        }

        public event Action<NewsArticleMessage> NewsArticle;

        void EWrapper.newsArticle(int requestId, int articleType, string articleText)
        {
            var tmp = NewsArticle;

            if (tmp != null)
                FireEvent(t => tmp(new NewsArticleMessage(requestId, articleType, articleText)), null);
        }

        public event Action<HistoricalNewsMessage> HistoricalNews;

        void EWrapper.historicalNews(int requestId, string time, string providerCode, string articleId, string headline)
        {
            var tmp = HistoricalNews;

            if (tmp != null)
                FireEvent(t => tmp(new HistoricalNewsMessage(requestId, time, providerCode, articleId, headline)), null);
        }

        public event Action<HistoricalNewsEndMessage> HistoricalNewsEnd;

        void EWrapper.historicalNewsEnd(int requestId, bool hasMore)
        {
            var tmp = HistoricalNewsEnd;

            if (tmp != null)
                FireEvent(t => tmp(new HistoricalNewsEndMessage(requestId, hasMore)), null);
        }

        public event Action<HeadTimestampMessage> HeadTimestamp;

        void EWrapper.headTimestamp(int reqId, string headTimestamp)
        {
            var tmp = HeadTimestamp;

            if (tmp != null)
                FireEvent(t => tmp(new HeadTimestampMessage(reqId, headTimestamp)), null);
        }

        public event Action<HistogramDataMessage> HistogramData;

        void EWrapper.histogramData(int reqId, HistogramEntry[] data)
        {
            var tmp = HistogramData;

            if (tmp != null)
                FireEvent(t => tmp(new HistogramDataMessage(reqId, data)), null);
        }

        public event Action<HistoricalDataMessage> HistoricalDataUpdate;

        void EWrapper.historicalDataUpdate(int reqId, Bar bar)
        {
            var tmp = HistoricalDataUpdate;

            if (tmp != null)
                FireEvent(t => tmp(new HistoricalDataMessage(reqId, bar)), null);
        }

        public event Action<int, int, string> RerouteMktDataReq;

        void EWrapper.rerouteMktDataReq(int reqId, int conId, string exchange)
        {
            var tmp = RerouteMktDataReq;

            if (tmp != null)
                FireEvent(t => tmp(reqId, conId, exchange), null);
        }

        public event Action<int, int, string> RerouteMktDepthReq;

        void EWrapper.rerouteMktDepthReq(int reqId, int conId, string exchange)
        {
            var tmp = RerouteMktDepthReq;

            if (tmp != null)
                FireEvent(t => tmp(reqId, conId, exchange), null);
        }

        public event Action<MarketRuleMessage> MarketRule;

        void EWrapper.marketRule(int marketRuleId, PriceIncrement[] priceIncrements)
        {
            var tmp = MarketRule;

            if (tmp != null)
                FireEvent(t => tmp(new MarketRuleMessage(marketRuleId, priceIncrements)), null);
        }

        public event Action<PnLMessage> pnl;

        void EWrapper.pnl(int reqId, double dailyPnL, double unrealizedPnL, double realizedPnL)
        {
            var tmp = pnl;

            if (tmp != null)
                FireEvent(t => tmp(new PnLMessage(reqId, dailyPnL, unrealizedPnL, realizedPnL)), null);
        }

        public event Action<PnLSingleMessage> pnlSingle;

        void EWrapper.pnlSingle(int reqId, int pos, double dailyPnL, double unrealizedPnL, double realizedPnL, double value)
        {
            var tmp = pnlSingle;

            if (tmp != null)
                FireEvent(t => tmp(new PnLSingleMessage(reqId, pos, dailyPnL, unrealizedPnL, realizedPnL, value)), null);
        }

        public event Action<HistoricalTickMessage> historicalTick;

        void EWrapper.historicalTicks(int reqId, HistoricalTick[] ticks, bool done)
        {
            var tmp = historicalTick;

            if (tmp != null)
                ticks.ToList().ForEach(tick => FireEvent(t => tmp(new HistoricalTickMessage(reqId, tick.Time, tick.Price, tick.Size)), null));
        }

        public event Action<HistoricalTickBidAskMessage> historicalTickBidAsk;

        void EWrapper.historicalTicksBidAsk(int reqId, HistoricalTickBidAsk[] ticks, bool done)
        {
            var tmp = historicalTickBidAsk;

            if (tmp != null)
                ticks.ToList().ForEach(tick => FireEvent(t =>
                                                           tmp(new HistoricalTickBidAskMessage(reqId, tick.Time, tick.TickAttribBidAsk, tick.PriceBid, tick.PriceAsk, tick.SizeBid, tick.SizeAsk)), null));
        }

        public event Action<HistoricalTickLastMessage> historicalTickLast;

        void EWrapper.historicalTicksLast(int reqId, HistoricalTickLast[] ticks, bool done)
        {
            var tmp = historicalTickLast;

            if (tmp != null)
                ticks.ToList().ForEach(tick => FireEvent(t =>
                                                           tmp(new HistoricalTickLastMessage(reqId, tick.Time, tick.TickAttribLast, tick.Price, tick.Size, tick.Exchange, tick.SpecialConditions)), null));
        }

        public event Action<TickByTickAllLastMessage> tickByTickAllLast;

        void EWrapper.tickByTickAllLast(int reqId, int tickType, long time, double price, int size, TickAttribLast tickAttribLast, string exchange, string specialConditions)
        {
            var tmp = tickByTickAllLast;

            if (tmp != null)
                FireEvent(t => tmp(new TickByTickAllLastMessage(reqId, tickType, time, price, size, tickAttribLast, exchange, specialConditions)), null);
        }

        public event Action<TickByTickBidAskMessage> tickByTickBidAsk;

        void EWrapper.tickByTickBidAsk(int reqId, long time, double bidPrice, double askPrice, int bidSize, int askSize, TickAttribBidAsk tickAttribBidAsk)
        {
            var tmp = tickByTickBidAsk;

            if (tmp != null)
                FireEvent(t => tmp(new TickByTickBidAskMessage(reqId, time, bidPrice, askPrice, bidSize, askSize, tickAttribBidAsk)), null);
        }

        public event Action<TickByTickMidPointMessage> tickByTickMidPoint;

        void EWrapper.tickByTickMidPoint(int reqId, long time, double midPoint)
        {
            var tmp = tickByTickMidPoint;

            if (tmp != null)
                FireEvent(t => tmp(new TickByTickMidPointMessage(reqId, time, midPoint)), null);
        }

        public event Action<OrderBoundMessage> OrderBound;

        void EWrapper.orderBound(long orderId, int apiClientId, int apiOrderId)
        {
            var tmp = OrderBound;

            if (tmp != null)
                FireEvent(t => tmp(new OrderBoundMessage(orderId, apiClientId, apiOrderId)), null);
        }

        public event Action<CompletedOrderMessage> CompletedOrder;

        void EWrapper.completedOrder(Contract contract, Order order, OrderState orderState)
        {
            var tmp = CompletedOrder;

            if (tmp != null)
                FireEvent(t => tmp(new CompletedOrderMessage(contract, order, orderState)), null);
        }

        public event Action CompletedOrdersEnd;

        void EWrapper.completedOrdersEnd()
        {
            var tmp = CompletedOrdersEnd;

            if (tmp != null)
                FireEvent(t => tmp(), null);
        }
        
        public event Action<int, string> ReplaceFAEnd;

        void EWrapper.replaceFAEnd(int reqId, string text)
        {
            var tmp = ReplaceFAEnd;

            if (tmp != null)
                FireEvent(t => tmp(reqId, text), null);
        }
        #endregion
        
        
    }
}
﻿using System;

namespace Krs.Ats.IBNet
{
    [Serializable()]
    public class MarketDataTypeEventArgs : EventArgs
    {
        private int _requestId;
        private MarketDataType _marketDataType;

        public MarketDataTypeEventArgs(int requestId, MarketDataType marketDataType)
        {
            _requestId = requestId;
            _marketDataType = marketDataType;
        }

        public MarketDataType MarketDataType
        {
            get => _marketDataType;
	        set => _marketDataType = value;
        }

        public int RequestId
        {
            get => _requestId;
	        set => _requestId = value;
        }
    }
}
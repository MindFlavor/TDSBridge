using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDSBridge.Common.Header
{
    public enum HeaderType
    {
        SQLBatch = 1,
        PreTD7Login = 2,
        RPC = 3,
        TabularResult = 4,
        
        AttentionSignal = 6,
        BulkLoadData = 7,

        TransactionManagerRequest = 14,

        TDS7Login = 16,
        SSPIMessage = 17,
        PreLoginMessage = 18,
        
        Unknown = 0xFF
    }
}

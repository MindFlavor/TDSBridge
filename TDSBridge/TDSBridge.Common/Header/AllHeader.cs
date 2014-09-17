using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDSBridge.Common.Header
{
    public class AllHeader
    {
        protected byte[] _bPayload = null;

        public UInt32 Length
        {
            get
            {
                return
                    ((UInt32)_bPayload[3]) * 0x01000000 +
                    ((UInt32)_bPayload[2]) * 0x00010000 +
                    ((UInt32)_bPayload[1]) * 0x00000100 +
                    ((UInt32)_bPayload[0]) * 0x00000001;                                          
            }
        }

        public AllHeader(byte[] bPayload)
        {
            this._bPayload = bPayload;
        }
    }
}

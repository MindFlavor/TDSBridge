using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDSBridge.Common.Header
{
    public abstract class StatusBitMask
    {
        public const int NORMAL = 0x00;
        public const int END_OF_MESSAGE = 0x01;
        public const int IGNORE_EVENT = 0x02;

        public const int MULTI_PART_MESSAGE = 0x04;

        public const int RESET_CONNECTION = 0x08;

        public const int RESET_CONNECTION_SKIP_TRAN = 0x10;
    }
}

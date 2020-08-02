using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace VoteControl.EthMessages
{
    [Event("VoteFinished")]
    public class VoteEndedEvent: IEventDTO
    {
        [Parameter("uint256", "index")]
        public BigInteger Idx { get; set; }
    }
}

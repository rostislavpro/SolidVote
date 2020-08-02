using System;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace VoteControl.EthMessages
{
    [Function("membersAddr", "string")]
    public class MemberAddressMessage : FunctionMessage
    {
        [Parameter("uint256", "<input>", 1)]
        public BigInteger Idx { get; set; }
    }
}

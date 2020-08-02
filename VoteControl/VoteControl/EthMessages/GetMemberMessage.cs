using System;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace VoteControl.EthMessages
{
    [Function("getMember", "string")]
    public class GetMemberMessage : FunctionMessage
    {
        [Parameter("address", "_address", 1)]
        public string Address { get; set; }
    }
}

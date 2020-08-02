using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace VoteControl.EthMessages
{
    [Function("getProposalInfo", "string")]
    public class GetProposalInfoMessage : FunctionMessage
    {
        [Parameter("uint256", "_index ", 1)]
        public BigInteger Idx { get; set; }
    }
}

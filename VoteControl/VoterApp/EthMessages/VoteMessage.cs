using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace VoterApp.EthMessages
{
    [Function("vote", "string")]
    public class VoteMessage : FunctionMessage
    {
        [Parameter("uint256", "_index ", 1)]
        public BigInteger Idx { get; set; }

        [Parameter("bool", "_supportsProposal", 2)]
        public bool Yes { get; set; }

        [Parameter("string", "_comment", 3)]
        public string Comment { get; set; }
    }
}

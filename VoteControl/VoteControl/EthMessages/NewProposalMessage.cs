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
    [Function("newProposal", "string")]
    public class NewProposalMessage : FunctionMessage
    {
        [Parameter("uint256", "_endDate", 1)]
        public BigInteger EndDate { get; set; }

        [Parameter("string", "_description", 2)]
        public string Description { get; set; }

        [Parameter("string", "_link", 3)]
        public string Link { get; set; }
    }
}

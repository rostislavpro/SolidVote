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
    [Function("getProposalInfoByAddress")]
    class GetProposalInfoByAddressMessage : FunctionMessage
    {
        [Parameter("uint256", "_index")]
        public BigInteger Index { get; set; }

        [Parameter("address", "_address")]
        public string Address { get; set; }
    }
}

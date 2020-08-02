using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace VoteControl.EthMessages
{
    [FunctionOutput]
    public  class MemberAddressResult : IFunctionOutputDTO
    {
        [Parameter("address", "address", 4)]
        public string Address { get; set; }
    }
}

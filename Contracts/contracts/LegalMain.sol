pragma solidity ^0.5.16;

contract LegalMain {

   address owner;

       struct Member {
        address member;
        bool active;
        bool isMember;
        string  name;
        uint256 memberSince;
        uint index;
        uint nnn;
    }
    struct Vote {
        bool supportsProposal;
        address sender;
        string comment;
    }
  
    modifier onlyMembers {
        require (members[msg.sender].isMember);
        require (members[msg.sender].active);
        _;
    }

    // This is the constructor which registers the
    // creator
    constructor() public {

        owner = msg.sender;
        totalNNN=0;
    }
    
    uint256 public totalNNN;

    mapping(address => Member) members;
        /**
     * @dev Congress members addresses list
     */
    address[] public membersAddr;

    /**
     * @dev Count of members in archive
     */
    function numMembers() public view returns (uint256)
    { return membersAddr.length; }




 mapping(uint256 => Proposal) proposals;
    // ---====== PROPOSALS ======---
    /**
     * @dev List of all proposals 
     */
    uint[] public proposalsIndex;

    /**
     * @dev Count of proposals in list
     */
    function numProposals() public view returns (uint256)
    { return proposalsIndex.length; }

     struct Proposal {

 
        address owner;
        uint256 indexProposal;
        string  description;
        string  link;
        address[] votesAddr;
        mapping(address => bool) voted;
        mapping(address => Vote) voteData;

        uint256 numberOfVotes;
        uint256 currentResult;

        uint256 proposalDate;
        bool proposalRejected;
        bool isFinished;


        uint256 endDate;

        uint256 finishDate;
        string  report;

    }


    function newProposal(

        uint256 _endDate,
        string memory  _description,
        string  memory _link
    )
    public
    onlyMembers
    returns (uint256 id)
    {
        //require(_endDate > block.timestamp);
        uint256 index;

        index = proposalsIndex.length;
        proposalsIndex.push(index) ;

        proposals[index].owner = msg.sender;
        proposals[index].indexProposal =index;
        proposals[index].description = _description;
        proposals[index].link = _link;
        proposals[index].numberOfVotes = 0;
        proposals[index].currentResult = 0;
        proposals[index].proposalDate = now;
        proposals[index].proposalRejected = false;
        proposals[index].isFinished = false;
        
        proposals[index].endDate = _endDate;
        return index;

        //ProposalAdded(msg.sender, _hash, _description, _link);
    }


    /**
     * @dev Get campaign common information
     * @param _index Campaign index key
     */
    function getProposalInfo(uint256 _index) public view returns (
        bool supportsProposal,
        bool isVoted,
        string memory description,
       // string link,
        uint256 numberOfVotes,
        bool isFinished,

        bool proposalRejected,
        uint256 endDate
    ) {
        return (
            proposals[_index].voteData[msg.sender].supportsProposal,
            proposals[_index].voted[msg.sender],
            proposals[_index].description,
            //proposals[_index].link,
            proposals[_index].numberOfVotes,
            proposals[_index].isFinished,
            proposals[_index].proposalRejected,
            proposals[_index].endDate
        );
    }




    function getProposalInfoByAddress(uint256 _index,address _address) public view returns (
        bool supportsProposal,
       bool isVoted,
       
        string memory description,
        uint256 numberOfVotes,
        bool proposalRejected,
  
        bool isFinished
    
    ) {
        return (
          proposals[_index].voteData[_address].supportsProposal,
          proposals[_index].voted[_address],
          proposals[_index].description,
          //proposals[_index].link,
          proposals[_index].numberOfVotes,
          proposals[_index].proposalRejected,
          proposals[_index].isFinished

        );
    }


    /**
     * @dev Get campaign common information
     * @param _index Campaign index key
     */
    function getProposalInfoAdditionalByAddress(uint256 _index) public view returns (
        
         bool proposalRejected


    ) {
        return (

            proposals[_index].proposalRejected

   

        );
    }


    event VoteFinished(uint256 index);
    function vote(
        uint256 _index,
        bool _supportsProposal,
        string memory _comment
    )
    public
    onlyMembers
    returns (bool)
    {
   

        require(proposals[_index].indexProposal>=0);
        require(!proposals[_index].voted[msg.sender]);
        require(!proposals[_index].proposalRejected);

        proposals[_index].voted[msg.sender] = true; // Set this voter as having voted
        proposals[_index].votesAddr.push(msg.sender);

        Vote memory v;
        v.supportsProposal = _supportsProposal;
        v.sender = msg.sender;
        v.comment = _comment;

        proposals[_index].voteData[msg.sender] = v;
        proposals[_index].voted[msg.sender] = true;
   
        proposals[_index].numberOfVotes++; // Increase the number of votes
        if (_supportsProposal) { // If they support the proposal
            proposals[_index].currentResult+=members[msg.sender].nnn; // Increase score
           
        }
        
        if(proposals[_index].currentResult*2>totalNNN && !proposals[_index].isFinished)
        {
            proposals[_index].isFinished = true;
            proposals[_index].proposalRejected = false;
            emit VoteFinished(_index);
        }

        return true;
    }

 function renderHelloWorld () public pure returns ( string memory) {
   return 'helloWorld';
 }

     /* @dev Add new company member
     * @param _targetMember Member account address
     * @param _memberName Member full name
     */
    function addMember(address _targetMember, string memory _memberName, uint _nnn) public
     //onlyOwner 
     {
        require(_targetMember != address(0));
        require(!members[_targetMember].isMember);

        members[_targetMember].index = membersAddr.push(_targetMember) - 1;
        members[_targetMember].active = true;
        members[_targetMember].isMember = true;
        members[_targetMember].memberSince = now;
        members[_targetMember].member = _targetMember;
        members[_targetMember].name = _memberName;
        members[_targetMember].nnn = _nnn;
        totalNNN+=_nnn;
        
       
    }

    /**
     * @dev Get member
     * @param _address Member account address
     */
    function getMember(address _address) public view returns (
        bool active,
        bool isMember,
        string memory name,
        uint256 memberSince,
        uint256 memberShare
    ) {
        return (
            members[_address].active,
            members[_address].isMember,
            members[_address].name,
            members[_address].memberSince,
            members[_address].nnn
        );
    }



}

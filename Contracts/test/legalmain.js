const LegalMain = artifacts.require("LegalMain");

let legalMain = null;


contract('LegalMain', function(accounts) {

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    before('Setup contract for each test', async function() {
        legalMain = await LegalMain.deployed();

    })

    it('Create a new proposal', async function() {
        let val = await legalMain.newProposal(1, "first voting", "http:\\test.com", { from: accounts[0] });
        console.log("Transation hash= " + val.tx);
        assert.equal(1, 1);
    });



    it('Voting', async function() {
        await legalMain.vote(0, false, "Alice Comment", { from: accounts[0] });
        await legalMain.vote(0, true, "Bob Comment", { from: accounts[1] });
        await legalMain.vote(0, true, "Carol Comment", { from: accounts[2] });
        // console.log("Transation hash= " + val.tx);
        assert.equal(1, 1);
    });

    it('Voting Results ', async function() {
        let val = await legalMain.getProposalInfo(0);
        // await legalMain.vote(1, true, "Misha Comment", { from: accounts[1] });
        // await legalMain.vote(2, true, "Andrew Comment", { from: accounts[2] });
        console.log("ProposalInfo (supportsProposal,isVoted,description,numberOfVotes,bool isFinished,proposalRejected,endDate)= " + val);
        assert.equal(1, 1);
    });

    it('Create a second proposal', async function() {
        let val = await legalMain.newProposal(1, "second voting", "http:\\test.com", { from: accounts[0] });
        console.log("Transation hash= " + val.tx);
        assert.equal(1, 1);
    });



    it('Second Voting', async function() {
        await legalMain.vote(1, false, "Alice Comment", { from: accounts[0] });
        await legalMain.vote(1, true, "Bob Comment", { from: accounts[1] });
        await legalMain.vote(1, true, "Carol Comment", { from: accounts[2] });
        // console.log("Transation hash= " + val.tx);
        assert.equal(1, 1);
    });

    it('Second Voting Results ', async function() {
        let val = await legalMain.getProposalInfo(1);
        // await legalMain.vote(1, true, "Misha Comment", { from: accounts[1] });
        // await legalMain.vote(2, true, "Andrew Comment", { from: accounts[2] });
        console.log("ProposalInfo (supportsProposal,isVoted,description,numberOfVotes,isFinished,proposalRejected,endDate)= " + val);
        assert.equal(1, 1);
    });



});
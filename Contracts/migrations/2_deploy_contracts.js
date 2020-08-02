module.exports = (deployer, network, accounts) => {

    const LegalMain = artifacts.require("LegalMain");



    return deployer.deploy(LegalMain).then(async() => {

        const legalMain = await LegalMain.deployed();

        console.log('---------------------------');
        console.log('LegalMain address: ', legalMain.address);
        console.log('---------------------------');
        console.log(accounts[0]);
        console.log(accounts[1]);
        console.log(accounts[2]);
        console.log('---------------------------');

        await legalMain.addMember(0x9E9de8735ACc15eDcd7aB629E5D8eb6FC39144dD, "Alice", 444);
        await legalMain.addMember(0xc7433EA82E5f970e6B56d46aEb9189757869b9f8, "Bobe", 555);
        await legalMain.addMember(0x40B7C305130C5f8EaFf7DF71a4F045d5010D3B4A, "Carol", 555);


    });
};
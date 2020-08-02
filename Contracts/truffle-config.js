/*
 * NB: since truffle-hdwallet-provider 0.0.5 you must wrap HDWallet providers in a 
 * function when declaring them. Failure to do so will cause commands to hang. ex:
 * ```
 * mainnet: {
 *     provider: function() { 
 *       return new HDWalletProvider(mnemonic, 'https://mainnet.infura.io/<infura-key>') 
 *     },
 *     network_id: '1',
 *     gas: 4500000,
 *     gasPrice: 10000000000,
 *   },
 */
var HDWalletProvider = require("truffle-hdwallet-provider");
module.exports = {
    // See <http://truffleframework.com/docs/advanced/configuration>
    // to customize your Truffle configuration!

    networks: {
        develop: {
            host: "127.0.0.1",
            port: 9545,
            network_id: "*", // Match any network id
        }
	    /*,
        kovan: {
            provider: function() {
                return new HDWalletProvider("sound kit refuse nice short knee cave hurt banana narrow pumpkin industry", 'https://kovan.infura.io/v3/3edaea63e8224a81aea128ccda948090')
            },
            host: "https://kovan.infura.io/v3/3edaea63e8224a81aea128ccda948090",
            port: 8545,
            network_id: 42,
            gasPrice: 1000000000
        },
        rinkeby: {
            host: "localhost",
            port: 8545,
            network_id: 4,
            gasPrice: 1000000000
        },
        live: {
            host: "localhost",
            port: 8545,
            network_id: 1,
            gasPrice: 10000000000,
            from: "0x0"
        },
        ganache: {
            host: "127.0.0.1",
            port: 7545,
            network_id: "*" // matching any id
        },
        tr: {
            host: "127.0.0.1",
            port: 9545,
            network_id: "*" // matching any id
        }*/
    }
};

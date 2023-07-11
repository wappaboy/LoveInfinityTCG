const HDWalletProvider = require("@truffle/hdwallet-provider");
const {Web3} = require("web3");
const abi = require("./nftContract.json");
require('dotenv').config()

class NftContract {
  constructor() {
    // const contractAddress = "0x0CB2fF14EF8c7fe3410DB79B97FC02E78Ea14e90"; //MUMBAI
    const contractAddress = "0x753455Fc4131e619E6bf80c7977124085a33cd6B"; //ASTAR

    const privateKey = process.env.PRIVATE_KEY;
    const providerUrl = process.env.ASTAR_PROVIDER_URL;

    const hdWalletProvider = new HDWalletProvider({
      privateKeys: [privateKey],
      providerOrUrl: providerUrl,
    });

    const eoaAddr = hdWalletProvider.getAddress();
    this.eoaAddr = eoaAddr;

    const web3 = new Web3(hdWalletProvider);
    const contract = new web3.eth.Contract(abi, contractAddress);
    this.contract = contract;
  }

  async mint(to, uri) {
    try {
        await this.contract.methods.safeMint(to, uri).send({from: this.eoaAddr});
        console.log('safe mint');
    }catch(err) {
        console.log(err);
    }
  }
}

module.exports = {
    NftContract
}
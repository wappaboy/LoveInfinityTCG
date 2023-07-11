require("@nomicfoundation/hardhat-toolbox");
require("dotenv").config();
require("@nomicfoundation/hardhat-verify");

const {PRIVATE_KEY, ALCHEMY_API_KEY_MUMBAI, ALCHEMY_API_KEY_ASTAR, POLYGON_SCAN_API_KEY} = process.env

/** @type import('hardhat/config').HardhatUserConfig */
module.exports = {
  solidity: "0.8.18",
  networks: {
    mumbai: {
      url: `https://polygon-mumbai.g.alchemy.com/v2/${ALCHEMY_API_KEY_MUMBAI}`,
      accounts: [PRIVATE_KEY]
    },
    astar: {
      url: `https://evm.astar.network`,
      accounts: [PRIVATE_KEY]
    }
  },
  etherscan: {
    // Your API key for PolygonScan
    apiKey: {polygonMumbai: POLYGON_SCAN_API_KEY}
  }
};

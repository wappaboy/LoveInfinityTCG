const { expect } = require("chai");
const {
  loadFixture,
} = require("@nomicfoundation/hardhat-toolbox/network-helpers");

describe("MyTreasure contract", function () {
  async function deployMyTreasureFixture() {
    const [owner, addr1, addr2] = await ethers.getSigners();
    const myTreasure = await ethers.deployContract("MyTreasure");

    await myTreasure.waitForDeployment();

    return { myTreasure, owner, addr1, addr2 };
  }

  describe("Transactions", function () {
    it("Should mint if sender is not owner", async function () {
      const { myTreasure, addr1 } = await loadFixture(deployMyTreasureFixture);
      const tokenURI = 'https://my.token.uri';
      await myTreasure.connect(addr1).safeMint(addr1.address, tokenURI);
      expect(await myTreasure.balanceOf(addr1.address)).to.equal(1);
      expect(await myTreasure.tokenURI(0)).to.equal(tokenURI);
    });

    it("Should mint a new token if sender is owner", async function () {
      const { myTreasure, owner, addr1 } = await loadFixture(deployMyTreasureFixture);
      const tokenURI = 'https://my.token.uri';
      await myTreasure.connect(owner).safeMint(addr1.address, tokenURI);
      expect(await myTreasure.balanceOf(addr1.address)).to.equal(1);
      expect(await myTreasure.tokenURI(0)).to.equal(tokenURI);
    });
  });
});

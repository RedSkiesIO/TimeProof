import Web3 from 'web3';
import Vue from 'vue';

const web3 = new Web3(new Web3.providers.HttpProvider('https://kovan.infura.io/v3/679bbc6759454bf58a924bfaf55576b9'));

Vue.prototype.$web3 = {
  async getTimestamp(block) {
    try {
      const blockInfo = await web3.eth.getBlock(block);
      return blockInfo.timestamp * 1000;
    } catch (e) {
      console.error(e);
    }
    return null;
  },

  async verifyTimestamp(id, hash) {
    try {
      const tx = await web3.eth.getTransaction(id);
      const info = JSON.parse(Web3.utils.hexToUtf8(tx.input));
      if (info.hash === hash) {
        const date = await this.getTimestamp(tx.blockNumber);
        return {
          verified: true,
          signature: info.signature,
          publicKey: info.publicKey,
          timestamp: date,
        };
      }
      return {
        verified: false,
      };
    } catch (error) {
      console.error(error);
      return null;
    }
  },

  async updateTimestamps(timestamps) {
    return timestamps.filter(async (stamp) => {
      const tx = await web3.eth.getTransactionReceipt(stamp.txId);
      if (tx) {
        const { timestamp } = await web3.eth.getBlock(stamp.blockNumber);
        stamp.timestamp = timestamp;
        stamp.blockNumber = tx.blockNumber;
        return true;
      }
      return false;
    });
  },
};

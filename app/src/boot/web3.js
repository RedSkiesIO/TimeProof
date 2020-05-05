import Web3 from 'web3';
import Vue from 'vue';
import axios from 'axios';
import Timestamp from '../store/Timestamp';

const web3 = new Web3(new Web3.providers.HttpProvider(process.env.INFURA));

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
      console.log('TX ENDDD');
      console.log(tx);
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

  async updateTimestamps(user, timestamps) {
    const updates = [];
    const promises = timestamps.map(async (stamp) => {
      const tx = await web3.eth.getTransactionReceipt(stamp.txId);
      if (tx) {
        const { timestamp } = await web3.eth.getBlock(tx.blockNumber);
        updates.push(await Timestamp.update({
          where: stamp.txId,
          data: {
            date: timestamp * 1000,
            blockNumber: tx.blockNumber,
          },
        }));
      }
    });
    await Promise.all(promises);
    if (updates.length > 0) {
      await axios.post(`${process.env.API}/updatetimestamps/${user.accountIdentifier}`, updates);
    }
  },
};

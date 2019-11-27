import Web3 from 'web3';
import Vue from 'vue';

const web3 = new Web3(new Web3.providers.HttpProvider('https://kovan.infura.io/v3/679bbc6759454bf58a924bfaf55576b9'));

async function getTimestamp(block) {
  try {
    console.log(block);
    const blockInfo = await web3.eth.getBlock(block);
    console.log(blockInfo);
    return blockInfo.timestamp;
  } catch (e) {
    console.log(e);
  }
  return null;
}

Vue.prototype.$web3 = getTimestamp;

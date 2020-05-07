import Vue from 'vue';
import Timestamp from '../../../store/Timestamp';
import Server from '../index';

class TimestampServer extends Server {
  // eslint-disable-next-line no-unused-vars
  async updateTimestamps(timestamps) {
    const re = /(?:\.([^.]+))?$/;

    timestamps.forEach(async (timestamp) => {
      const { data, status } = await this.axios.get(`${process.env.API}/timestamp/${timestamp.id}`);
      console.log('UPDATEDDDDDDD');
      console.log(data);
      console.log(status);
      if (status === 200 && data) {
        const updatedTimeStamp = {
          id: data.id,
          txId: data.transactionId,
          status: data.status,
          hash: data.fileHash,
          signature: data.signature,
          pubKey: data.publicKey.toLowerCase(),
          name: data.fileName,
          date: data.timestamp,
          type: re.exec(data.fileName)[1],
          blockNumber: Number(data.blockNumber),
        };

        Timestamp.update({
          data: updatedTimeStamp,
        });
      }
    });
  }

  async createTimestamps(file, publicKey) {
    return this.axios.post(`${process.env.API}/timestamp`, {
      fileName: file.name,
      fileHash: file.hash,
      publicKey,
      signature: file.signature,
    });
  }
}


const timestampServer = new TimestampServer();

Vue.prototype.$timestampServer = timestampServer;

export default timestampServer;

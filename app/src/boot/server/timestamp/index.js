import axios from 'axios';
import Vue from 'vue';
import Timestamp from '../../../store/Timestamp';

Vue.prototype.$timestampServer = {
  // eslint-disable-next-line no-unused-vars
  async updateTimestamps(timestamps) {
    const re = /(?:\.([^.]+))?$/;

    timestamps.forEach(async (timestamp) => {
      const { data, status } = await axios.get(`${process.env.API}/timestamp/${timestamp.id}`);

      if (status === 200 && data) {
        const updatedTimeStamp = {
          id: data.id,
          txId: data.transactionId,
          status: data.status,
          hash: data.fileHash,
          signature: data.signature,
          pubKey: data.publicKey.toLowerCase(),
          name: data.fileName,
          date: Number(data.timestamp),
          type: re.exec(data.fileName)[1],
          blockNumber: Number(data.blockNumber),
        };

        Timestamp.update({
          data: updatedTimeStamp,
        });
      }
    });
  },
};

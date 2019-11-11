// import * as JsPDF from 'jspdf';
import Vue from 'vue';
import createDoc from 'docx-templates';
// import { PDFDocument } from 'pdf-lib';
// import templatePDF from 'pdf-templater';
// import cert from './cert.js';

// const readFileIntoArrayBuffer = fd => new Promise((resolve, reject) => {
//   const reader = new FileReader();
//   reader.onerror = reject;
//   reader.onload = () => {
//     resolve(reader.result);
//   };
//   reader.readAsArrayBuffer(fd);
// });

const certificate = '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';

function base64ToArrayBuffer(base64) {
  const binaryString = window.atob(base64);
  const len = binaryString.length;
  const bytes = new Uint8Array(len);
  for (let i = 0; i < len; i += 1) {
    bytes[i] = binaryString.charCodeAt(i);
  }
  return bytes.buffer;
}

const downloadURL = (data, fileName) => {
  const a = document.createElement('a');
  a.href = data;
  a.download = fileName;
  document.body.appendChild(a);
  a.style = 'display: none';
  a.click();
  a.remove();
};

const saveDataToFile = (data, fileName, mimeType) => {
  const blob = new Blob([data], { type: mimeType });
  const url = window.URL.createObjectURL(blob);
  downloadURL(url, fileName, mimeType);
  setTimeout(() => {
    window.URL.revokeObjectURL(url);
  }, 1000);
};

async function create() {
  const cert = await createDoc({
    template: base64ToArrayBuffer(certificate),
    data: {
      hash: '12345',
      proofId: '6789',
      timestamp: 'today',
      user: 'satoshi',
      pubKey: 'blablabla',

    },
  });

  saveDataToFile(
    cert,
    'cert.docx',
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
  );

  // const doc = await PDFDocument.load(base64ToArrayBuffer(cert));
  // templatePDF(doc,
  //   {
  //     hash: '12345',
  //     proofId: '6789',
  //     timestamp: 'today',
  //     user: 'satoshi',
  //     pubKey: 'blablabla',
  //   });
  // const outputBase64 = doc.saveAsBase64(); // Save the doc already replacement
  // saveDataToFile(outputBase64);
}

Vue.prototype.$pdf = create;

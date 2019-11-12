import Vue from 'vue';
import { PDFDocument } from 'pdf-lib';
import templatePDF from 'pdf-templater';

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
  const response = await fetch('../../statics/certificate.pdf');
  const file = await response.arrayBuffer();
  console.log(file);

  const doc = await PDFDocument.load(file);
  await templatePDF(doc,
    {
      hash: '12345',
      proofId: '6789',
      timestamp: 'today',
      user: 'satoshi',
      pubKey: 'blablabla',
    });
  const output = await doc.save(); // Save the doc already replacement
  saveDataToFile(output, 'cert.pdf', 'application/pdf');
}

Vue.prototype.$pdf = create;

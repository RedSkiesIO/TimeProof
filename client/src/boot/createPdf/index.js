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

async function create(name, proof) {
  const response = await fetch('../../statics/certificate.pdf');
  const file = await response.arrayBuffer();

  const doc = await PDFDocument.load(file);
  await templatePDF(doc,
    {
      name: proof.file,
      hash: proof.hash.one,
      hash2: proof.hash.two,
      proofId: proof.proofId.one,
      proofId2: proof.proofId.two,
      timestamp: proof.timestamp,
      user: proof.user,
      signature: proof.signature.one,
      signature2: proof.signature.two,
    });
  const output = await doc.save(); // Save the doc already replacement
  saveDataToFile(output, name, 'application/pdf');
}

Vue.prototype.$pdf = create;

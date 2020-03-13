/* eslint-disable no-unused-vars */
import Vue from 'vue';
import {
  PDFDocument, rgb, StandardFonts,
} from 'pdf-lib';


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
  console.log(proof);
  const response = await fetch('../../statics/CertificateTemplate.pdf');
  const file = await response.arrayBuffer();
  const doc = await PDFDocument.load(file);
  const helveticaFont = await doc.embedFont(StandardFonts.Helvetica);
  const pages = doc.getPages();
  const firstPage = pages[0];

  // Get the width and height of the first page
  const { width, height } = firstPage.getSize();
  console.log(width, height);

  // Draw a string of text diagonally across the first page

  if (proof.file) {
    try {
      const freqIndex = 66;
      const fileLength = proof.file.length;
      if (fileLength > freqIndex) {
        const freq = fileLength % freqIndex === 0
          ? fileLength / freqIndex
          : fileLength / freqIndex + 1;
        const fileNameArr = new Array(Math.floor(freq)).fill('');
        fileNameArr.every((fileName, index) => {
          if (index === 2) {
            fileName = (proof.file.slice(freqIndex * index, fileLength));
          } else {
            fileName = (proof.file.slice(freqIndex * index, freqIndex * index + freqIndex));
          }
          firstPage.drawText(fileName, {
            x: 178.75,
            y: 505 - index * 10,
            size: 10,
            font: helveticaFont,
            color: rgb(0, 0, 0),
          });

          if (index === 2) {
            return false;
          }
          return true;
        });
      }
    } catch (err) {
      console.log(err);
    }
  }

  firstPage.drawText(proof.timestamp, {
    x: 178.75,
    y: 434.75,
    size: 14,
    font: helveticaFont,
    color: rgb(0, 0, 0),
  });

  firstPage.drawText(proof.proofId.one + proof.proofId.two, {
    x: 178.75,
    y: 554.75,
    size: 10,
    font: helveticaFont,
    color: rgb(0, 0, 0),
  });

  firstPage.drawText(proof.user, {
    x: 178.75,
    y: 374,
    size: 14,
    font: helveticaFont,
    color: rgb(0, 0, 0),
  });

  firstPage.drawText(proof.signature.one, {
    x: 178.75,
    y: 261.25,
    size: 10,
    font: helveticaFont,
    color: rgb(0, 0, 0),
  });

  firstPage.drawText(proof.signature.two, {
    x: 178.75,
    y: 252.25,
    size: 10,
    font: helveticaFont,
    color: rgb(0, 0, 0),
  });

  firstPage.drawText(proof.hash.one, {
    x: 178.75,
    y: 321.25,
    size: 10,
    font: helveticaFont,
    color: rgb(0, 0, 0),
  });

  firstPage.drawText(proof.hash.two, {
    x: 178.75,
    y: 311.25,
    size: 10,
    font: helveticaFont,
    color: rgb(0, 0, 0),
  });

  const output = await doc.save(); // Save the doc already replacement
  saveDataToFile(output, name, 'application/pdf');
}

Vue.prototype.$pdf = create;

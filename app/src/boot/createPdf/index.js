/* eslint-disable no-unused-vars */
import Vue from 'vue';
import {
  PDFDocument, rgb, StandardFonts,
} from 'pdf-lib';
import pixelWidth from 'string-pixel-width';

class PdfUtil {
  downloadURL = (data, fileName) => {
    const a = document.createElement('a');
    a.href = data;
    a.download = fileName;
    document.body.appendChild(a);
    a.style = 'display: none';
    a.click();
    a.remove();
  };

  saveDataToFile = (data, fileName, mimeType) => {
    const blob = new Blob([data], { type: mimeType });
    const url = window.URL.createObjectURL(blob);
    this.downloadURL(url, fileName);
    setTimeout(() => {
      window.URL.revokeObjectURL(url);
    }, 1000);
  };

  create = async (name, proof) => {
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
      const fullLeng = pixelWidth(proof.file, { font: 'helvetica', size: 12 });

      let y = 505;
      if (fullLeng <= 358) {
        y -= 10;
      }
      this.drawTextAccordingToPixels(proof.file, firstPage, helveticaFont, 175, y);
    }

    this.drawTextAccordingToPixels(proof.timestamp, firstPage, helveticaFont, 175, 434.75);

    this.drawTextAccordingToPixels(proof.proofId.one + proof.proofId.two,
      firstPage, helveticaFont, 175, 560);

    this.drawTextAccordingToPixels(proof.user, firstPage, helveticaFont, 175, 374);

    this.drawTextAccordingToPixels(proof.signature.one + proof.signature.two,
      firstPage, helveticaFont, 175, 261.25);

    this.drawTextAccordingToPixels(proof.hash.one + proof.hash.two,
      firstPage, helveticaFont, 175, 321.25);

    const output = await doc.save(); // Save the doc already replacement
    this.saveDataToFile(output, name, 'application/pdf');
  }

  drawTextAccordingToPixels = async (data, firstPage, helveticaFont, x, y) => {
    try {
      if (data) {
        data = data.replace(/\n/g, '');

        let str = '';
        let index = 0;
        for (let i = 0; i < data.length; i += 1) {
          str += data[i];
          const leng = pixelWidth(str, { font: 'helvetica', size: 12 });
          if (leng > 358) {
            firstPage.drawText(str, {
              x,
              y: y - index * 15,
              size: 12,
              font: helveticaFont,
              color: rgb(0, 0, 0),
            });
            str = '';
            index += 1;
          } else if (i === data.length - 1) {
            firstPage.drawText(str, {
              x,
              y: y - index * 15,
              size: 12,
              font: helveticaFont,
              color: rgb(0, 0, 0),
            });
          }
        }
      }
    } catch (err) {
      console.log(err);
    }
  }
}

Vue.prototype.$pdf = new PdfUtil();

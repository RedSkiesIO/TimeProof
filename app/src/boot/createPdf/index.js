/* eslint-disable no-unused-vars */
import Vue from 'vue';
import {
  PDFDocument, rgb,
} from 'pdf-lib';
import fontkit from '@pdf-lib/fontkit';
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
    try {
      const response = await fetch('../../statics/CertificateTemplate.pdf');
      const file = await response.arrayBuffer();
      const doc = await PDFDocument.load(file);
      doc.registerFontkit(fontkit);
      const fontResponse = await fetch('../../statics/Montserrat-Regular.ttf');
      const montserratFont = await doc.embedFont(await fontResponse.arrayBuffer());
      const pages = doc.getPages();
      const firstPage = pages[0];

      // Get the width and height of the first page
      const { width, height } = firstPage.getSize();
      console.log(width, height);

      // Draw a string of text diagonally across the first page
      if (proof.file) {
        const lengWidth = pixelWidth(proof.file, { font: 'Verdana', size: 12 });

        if (lengWidth) {
          let y = 505;
          if (lengWidth <= 358) {
            y -= 10;
          }

          this.drawTextAccordingToPixels(proof.file, firstPage, montserratFont, 175, y);
        } else {
          firstPage.drawText(proof.file, {
            x: 175,
            y: 500,
            size: 12,
            font: montserratFont,
            color: rgb(0, 0, 0),
          });
        }
      }

      this.drawTextAccordingToPixels(proof.timestamp, firstPage, montserratFont, 175, 434.75);

      this.drawTextAccordingToPixels(proof.proofId.one + proof.proofId.two,
        firstPage, montserratFont, 175, 560);

      this.drawTextAccordingToPixels(proof.user, firstPage, montserratFont, 175, 374);

      this.drawTextAccordingToPixels(proof.signature.one + proof.signature.two,
        firstPage, montserratFont, 175, 261.25);

      this.drawTextAccordingToPixels(proof.hash.one + proof.hash.two,
        firstPage, montserratFont, 175, 321.25);

      const output = await doc.save(); // Save the doc already replacement
      this.saveDataToFile(output, name, 'application/pdf');
    } catch (err) {
      console.log(err);
    }
  }

  drawTextAccordingToPixels = async (data, firstPage, montserratFont, x, y) => {
    try {
      if (data) {
        data = data.replace(/\n/g, '');

        let str = '';
        let index = 0;

        let width = pixelWidth(data, { font: 'Verdana', size: 12 });
        if (width) {
          for (let i = 0; i < data.length; i += 1) {
            str += data[i];
            width = pixelWidth(str, { font: 'Verdana', size: 12 });
            if (width > 358) {
              firstPage.drawText(str, {
                x,
                y: y - index * 15,
                size: 12,
                font: montserratFont,
                color: rgb(0, 0, 0),
              });
              str = '';
              index += 1;
            } else if (i === data.length - 1) {
              firstPage.drawText(str, {
                x,
                y: y - index * 15,
                size: 12,
                font: montserratFont,
                color: rgb(0, 0, 0),
              });
            }
          }
        } else {
          firstPage.drawText(data, {
            x,
            y,
            size: 12,
            font: montserratFont,
            color: rgb(0, 0, 0),
          });
        }
      }
    } catch (err) {
      console.log(err);
    }
  }
}

Vue.prototype.$pdf = new PdfUtil();

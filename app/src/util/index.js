const expand = (obj) => {
  Object.keys(obj).forEach((key) => {
    const subKeys = key.split('|');
    const target = obj[key];
    delete obj[key];
    if (subKeys) { subKeys.forEach((subKey) => { obj[subKey] = target; }); }
  });

  return obj;
};

const fileMap = expand({
  'pdf|ps|eps': 'fas fa-file-pdf',
  'zip|7z|arj|deb|pkg|rar|rpm|tar.gz|z': 'fas fa-file-archive',
  'png|gif|jpeg|ai|bmp|ico|jpg|ps|psd|svg|tif|tiff|pcx|rle|dib': 'fas fa-file-image',
  'doc|docx|wpd|rtf': 'fas fa-file-word',
  'xls|xlsx|ods|xlsm': 'fas fa-file-excel',
  'aif|mp3|cda|mid|midi|mpa|ogg|wav|wma|wpl': 'fas fa-file-audio',
  'ppt|pptx|pptm|pptx': 'fas fa-file-powerpoint',
});


export const fileIcon = type => fileMap[type] || 'fas fa-file';

// Format a price (assuming a two-decimal currency like EUR or USD for simplicity).
export const formatPrice = (amount, currency) => {
  const price = (amount / 100).toFixed(2);
  const numberFormat = new Intl.NumberFormat('en-GB', {
    style: 'currency',
    currency,
    currencyDisplay: 'symbol',
  });
  return numberFormat.format(price);
};

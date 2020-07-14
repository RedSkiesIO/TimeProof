using AtlasCity.TimeProof.Abstractions;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Helpers;
using AtlasCity.TimeProof.Abstractions.PaymentServiceObjects;
using AtlasCity.TimeProof.Abstractions.Services;
using Dawn;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AtlasCity.TimeProof.Common.Lib.Helpers
{
    public class InvoiceHelper: IInvoiceHelper
    {
        private readonly ILogger _logger;
        private readonly IEmailTemplateHelper _emailTemplateHelper;
        private readonly IEmailService _emailService;
        private readonly IFileHelper _fileHelper;

        private readonly List<char> _numberLetterList = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        private const string ReceiptTemplateFileName = "Timescribe_Receipt_Template.pdf";
        private const string NewReceiptFileName = "Timescribe_Receipt.pdf";

        public InvoiceHelper(
            ILogger logger,
            IEmailTemplateHelper emailTemplateHelper,
            IEmailService emailService,
            IFileHelper fileHelper
        )
        {
            Guard.Argument(logger, nameof(logger)).NotNull();
            Guard.Argument(emailTemplateHelper, nameof(emailTemplateHelper)).NotNull();
            Guard.Argument(emailService, nameof(emailService)).NotNull();
            Guard.Argument(fileHelper, nameof(fileHelper)).NotNull();

            _logger = logger;
            _emailTemplateHelper = emailTemplateHelper;
            _emailService = emailService;
            _fileHelper = fileHelper;
        }

        public string CalculateInvoiceNumber(long number)
        {
            if (number > Constants.MaxInvoiceNumberCycle)
            {
                _logger.Warning($"Number '{number}' has exceeded maximum cycle of '{Constants.MaxInvoiceNumberCycle}'. Resetting required.");
                number = number % Constants.MaxInvoiceNumberCycle;
            }

            var noOf10K = (int)number / 10000;
            var noOf26In10K = noOf10K / 26;

            var remainingNumber = number - (noOf10K * 10000);

            Console.WriteLine(remainingNumber);

            return $"TS{DateTime.UtcNow:yyMMdd}{_numberLetterList[noOf26In10K]}{_numberLetterList[noOf10K % 26]}{remainingNumber:D4}";
        }

        public async Task SendInvoiceAsEmailAttachment(UserDao user, PaymentIntentDao paymentIntent, string subscriptionName, DateTime subscriptionStartDate, long totalAmount, string invoiceNumber, string rootFolder, CancellationToken cancellationToken)
        {
            Guard.Argument(paymentIntent, nameof(paymentIntent)).NotNull();
            Guard.Argument(subscriptionName, nameof(subscriptionName)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(invoiceNumber, nameof(invoiceNumber)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(rootFolder, nameof(rootFolder)).NotNull().NotEmpty().NotWhiteSpace();

            var emailBody = _emailTemplateHelper.GetInvoiceEmailBody(rootFolder, user.FirstName);
            var emailMessage = new EmailDao
            {
                ToAddress = user.Email,
                ToName = user.FullName,
                FromAddress = Constants.AutomatedEmailFromAddress,
                Subject = Constants.InvoiceEmailSubject,
                HtmlBody = emailBody,
            };

            var address = paymentIntent.Address ?? user.Address;

            var filePath = GenerateInvoice(user.FullName, address, $"{paymentIntent.CarIssuer} ****{paymentIntent.Last4}", invoiceNumber, subscriptionName, subscriptionStartDate, totalAmount, rootFolder);
            _logger.Information($"Generated invoice '{filePath}' for user '{user.Id}'");

            await _emailService.SendEmail(emailMessage, filePath, cancellationToken);
            _logger.Information($"Sent invoice for user '{user.Id}'");

            _fileHelper.DeleteFileDirectory(filePath);
            _logger.Information($"Deleted invoice file '{filePath}'");
        }

        private string GenerateInvoice(string fullName, AddressDao address,  string invoiceMethod, string invoiceNumber, string subscriptionName, DateTime subscriptionStartDate, long totalAmountInPence, string rootFolder)
        {
            Guard.Argument(fullName, nameof(fullName)).NotNull().NotEmpty().NotWhiteSpace();
            
            Guard.Argument(invoiceMethod, nameof(invoiceMethod)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(invoiceNumber, nameof(invoiceNumber)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(subscriptionName, nameof(subscriptionName)).NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(subscriptionStartDate, nameof(subscriptionStartDate)).GreaterThan(DateTime.MinValue);
            Guard.Argument(totalAmountInPence, nameof(totalAmountInPence)).GreaterThan(0);

            var itemDescriptionLine1 = $"{subscriptionName} Subscription (Monthly)";
            var itemDescriptionLine2 = $"From {subscriptionStartDate:dd/MM/yyyy} To {subscriptionStartDate.AddMonths(1):dd/MM/yyyy}";

            var addressLine1 = address?.Line1;
            var addressLine2 = address?.Line2;
            var addressLine3 = $"{address?.City} {address?.State}".TrimEnd(" ".ToCharArray());
            var addressLine4 = address?.Postcode;
            var addressLine5 = address?.Country;

            var fontColour = XBrushes.Gray;
            const string fontName = "Montserrat";
            var billedToNameRectangle = new XRect(55, 240, 50, 0);
            var billedToAddressLine1Rectangle = new XRect(55, 255, 50, 0);
            var billedToAddressLine2Rectangle = new XRect(55, 270, 50, 0);
            var billedToAddressLine3Rectangle = new XRect(55, 285, 50, 0);
            var billedToAddressLine4Rectangle = new XRect(55, 300, 50, 0);
            var billedToAddressLine5Rectangle = new XRect(55, 315, 50, 0);
            var addressLineDictionary = new Dictionary<int, XRect> { { 1, billedToAddressLine1Rectangle }, { 2, billedToAddressLine2Rectangle }, { 3, billedToAddressLine3Rectangle }, { 4, billedToAddressLine4Rectangle }, { 5, billedToAddressLine5Rectangle } };

            var invoiceDateRectangle = new XRect(440, 227, 50, 0);
            var paymentMethodRectangle = new XRect(440, 251, 50, 0);
            var receiptNoRectangle = new XRect(440, 274, 50, 0);

            var itemCountRectangle = new XRect(62, 380, 5, 0);
            var itemDescriptionRectangle = new XRect(82, 380, 50, 0);
            var itemValidityRectangle = new XRect(82, 400, 50, 0);

            var amountWithoutVatRectangle = new XRect(535, 380, 20, 0);
            var subTotalAmountRectangle = new XRect(535, 560, 20, 0);
            var vatAmountRectangle = new XRect(535, 585, 20, 0);
            var totalAmountRectangle = new XRect(535, 610, 20, 0);
            var totalAmountPaidRectangle = new XRect(535, 670, 20, 0);

            var rootDirectoryInfo = new DirectoryInfo(rootFolder);
            var templateFilePath = Path.Combine(rootDirectoryInfo.FullName, Constants.TemplateFolder, ReceiptTemplateFileName);
            if (!File.Exists(templateFilePath))
                throw new FileNotFoundException($"Missing file at {templateFilePath}.");

            var doc = PdfReader.Open(templateFilePath, PdfDocumentOpenMode.Import);
            var newDoc = new PdfDocument();

            var newPage = newDoc.AddPage(doc.Pages[0]);

            var gfx = XGraphics.FromPdfPage(newPage);
            var font = new XFont(fontName, 14);

            gfx.DrawString(fullName, font, fontColour, billedToNameRectangle, XStringFormats.BaseLineLeft);

            var currentAddressLine = 1;

            if (!string.IsNullOrWhiteSpace(addressLine1))
            {
                gfx.DrawString(addressLine1, font, fontColour, addressLineDictionary[currentAddressLine], XStringFormats.BaseLineLeft);
                currentAddressLine++;
            }

            if (!string.IsNullOrWhiteSpace(addressLine2))
            {
                gfx.DrawString(addressLine2, font, fontColour, addressLineDictionary[currentAddressLine], XStringFormats.BaseLineLeft);
                currentAddressLine++;
            }

            if (!string.IsNullOrWhiteSpace(addressLine3))
            {
                gfx.DrawString(addressLine3, font, fontColour, addressLineDictionary[currentAddressLine], XStringFormats.BaseLineLeft);
                currentAddressLine++;
            }

            if (!string.IsNullOrWhiteSpace(addressLine4))
            {
                gfx.DrawString(addressLine4, font, fontColour, addressLineDictionary[currentAddressLine], XStringFormats.BaseLineLeft);
                currentAddressLine++;
            }

            if (!string.IsNullOrWhiteSpace(addressLine5))
            {
                gfx.DrawString(addressLine5, font, fontColour, addressLineDictionary[currentAddressLine], XStringFormats.BaseLineLeft);
            }

            gfx.DrawString(DateTime.UtcNow.ToString("dd/MM/yyyy"), font, fontColour, invoiceDateRectangle, XStringFormats.BaseLineLeft);
            gfx.DrawString(invoiceMethod, font, fontColour, paymentMethodRectangle, XStringFormats.BaseLineLeft);
            gfx.DrawString(invoiceNumber, font, fontColour, receiptNoRectangle, XStringFormats.BaseLineLeft);

            gfx.DrawString("1", font, fontColour, itemCountRectangle, XStringFormats.BaseLineLeft);
            gfx.DrawString(itemDescriptionLine1, font, fontColour, itemDescriptionRectangle, XStringFormats.BaseLineLeft);
            gfx.DrawString(itemDescriptionLine2, font, fontColour, itemValidityRectangle, XStringFormats.BaseLineLeft);


            var amountInGbp = (double) totalAmountInPence / 100;
            var amountWithoutVat = GetAmountWithoutVat(amountInGbp);
            var vatAmount = Math.Round(amountInGbp - amountWithoutVat, 2);

            gfx.DrawString($"£{amountWithoutVat}", font, fontColour, amountWithoutVatRectangle, XStringFormats.BaseLineRight);
            gfx.DrawString($"£{amountWithoutVat}", font, fontColour, subTotalAmountRectangle, XStringFormats.BaseLineRight);
            gfx.DrawString($"£{vatAmount}", font, fontColour, vatAmountRectangle, XStringFormats.BaseLineRight);
            gfx.DrawString($"£{amountInGbp}", font, fontColour, totalAmountRectangle, XStringFormats.BaseLineRight);
            gfx.DrawString($"£{amountInGbp}", new XFont(fontName, 20), fontColour, totalAmountPaidRectangle, XStringFormats.BaseLineRight);

            var newFolder = Path.Combine(rootDirectoryInfo.FullName, Guid.NewGuid().ToString());
            if (!Directory.Exists(newFolder))
                Directory.CreateDirectory(newFolder);

            var newInvoiceFilePath = Path.Combine(newFolder, NewReceiptFileName);
            newDoc.Save(newInvoiceFilePath);

            return newInvoiceFilePath;
        }

        private double GetAmountWithoutVat(double fullAmount)
        {
            return Math.Round(fullAmount / 1.2, 2);
        }
    }
}

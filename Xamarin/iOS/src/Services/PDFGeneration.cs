using System;
using EarthLens.Models;
using CoreGraphics;
using Foundation;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using UIKit;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace EarthLens.iOS.Services
{
    public class PDFGeneration : UIView
    {
        private readonly SKImage _resultImage;
        private readonly ImageEntry _imageEntry;
        private nfloat _yOffset;

        public PDFGeneration(SKImage image, ImageEntry imageEntry)
        {
            _resultImage = image;
            _imageEntry = imageEntry;
            
            // Initialize y offset, this will be used afterwards for y position in PDF content
            _yOffset = 0;
        }

        /// <summary>
        /// Generates the pdf contents.
        /// </summary>
        /// <returns>The pdf in NSMutable data format.</returns>
        public NSMutableData GeneratePDF()
        {
            // data buffer to hold the PDF
            var data = new NSMutableData();

            // create a PDF with empty rectangle, which will configure it for 8.5x11 inches
            UIGraphics.BeginPDFContext(data, CGRect.Empty, null);

            // configure the image size according to page size of PDF file
            DrawImageWithBoundingBox(_resultImage.ToCGImage());

            // draw table table grids for categories in observation
            DrawCategorySummary();

            // draw table table grids for all observations
            DrawObservationTable();

            // complete a PDF page
            UIGraphics.EndPDFContent();

            return data;
        }

        /// <summary>
        /// Draws the observation table grids.
        /// </summary>
        private void DrawObservationTable()
        {
            // start the page with padding
            _yOffset = Constants.PDFPagePadding;

            var currentObservations = _imageEntry.Observations.ToList();

            // get page size in pdf document
            var pageHeight = Constants.PDFPageSize.Height;

            // calculates the maximum rows in a page by considering top and bottom padding of the page
            var rowsInPage =
                (int) Math.Floor((pageHeight - Constants.PDFPagePadding * 2) / Constants.PDFPageTableRowHeight);

            // if the table has more row than maximum rows in a page, start a new page
            for (var i = 0; i < currentObservations.Count; i += rowsInPage)
            {
                // start a PDF page
                UIGraphics.BeginPDFPage();
 
                // set the start coordinate of table and draw table grids
                var tableOrigin = new CGPoint(Constants.PDFPagePadding, _yOffset);
                DrawTableLineAt(tableOrigin, Constants.PDFPageTableRowHeight, Constants.PDFPageTableColumnWidth,
                    rowsInPage, Constants.PDFObservationTableNumberOfColumn);

                // if it is the first page of table, draws table header before generates table content
                if(i == 0)
                {
                    // generates table header
                    Constants.ObservationTableHeader[0].DrawString(
                        new CGPoint(tableOrigin.X + Constants.PDFPageTableTextPadding,
                            tableOrigin.Y + Constants.PDFPageTableTextPadding),
                        UIFont.BoldSystemFontOfSize(Constants.PDFPageTextFontSize));

                    Constants.ObservationTableHeader[1].DrawString(
                        new CGPoint(
                            tableOrigin.X + Constants.PDFPageTableTextPadding + Constants.PDFPageTableColumnWidth,
                            tableOrigin.Y + Constants.PDFPageTableTextPadding),
                        UIFont.BoldSystemFontOfSize(Constants.PDFPageTextFontSize));

                    Constants.ObservationTableHeader[2].DrawString(
                        new CGPoint(
                            tableOrigin.X + Constants.PDFPageTableTextPadding +
                            2 * Constants.PDFPageTableColumnWidth,
                            tableOrigin.Y + Constants.PDFPageTableTextPadding),
                        UIFont.BoldSystemFontOfSize(Constants.PDFPageTextFontSize));

                    _yOffset += Constants.PDFPageTableRowHeight;

                    // re-calculates the y position of current content and generate table contents 
                    tableOrigin = new CGPoint(Constants.PDFPagePadding, _yOffset);

                    GenerateObservationTableContent(tableOrigin, Constants.PDFPageTableRowHeight,
                        Constants.PDFPageTableColumnWidth,
                        rowsInPage > currentObservations.Count
                            ? currentObservations.GetRange(i, currentObservations.Count)
                            : currentObservations.GetRange(i, rowsInPage - 1), i);
                }
                // otherwise, generates table content directly
                else
                {
                    GenerateObservationTableContent(tableOrigin, Constants.PDFPageTableRowHeight,
                    Constants.PDFPageTableColumnWidth, currentObservations.Count - i < rowsInPage
                        ? currentObservations.GetRange(i - 1, currentObservations.Count - i + 1)
                        : currentObservations.GetRange(i - 1, rowsInPage), i - 1);
                }
            }
        }

        /// <summary>
        /// Draws the category summary including image informations and categories count for the image.
        /// </summary>
        private void DrawCategorySummary()
        {
            // start the page with padding
            _yOffset = Constants.PDFPagePadding;

            // start a PDF page
            UIGraphics.BeginPDFPage();

            // draws file name
            var filename = _imageEntry.Name;
            filename.DrawString(new CGPoint(Constants.PDFPagePadding, _yOffset),
                UIFont.BoldSystemFontOfSize(Constants.PDFPageImageTitleFontSize));
            _yOffset += Constants.PDFPageImageTitleFontSize + Constants.PDFPageTextPadding;

            // draws file creation date
            var fileDate = _imageEntry.CreationTime.ToString();
            fileDate.DrawString(new CGPoint(Constants.PDFPagePadding, _yOffset),
                UIFont.SystemFontOfSize(Constants.PDFPageTextFontSize));
            _yOffset += Constants.PDFPageTextFontSize + Constants.PDFPageTextPadding;

            // draws categories text
            var selectedFilter = Constants.CategoriesText;
            selectedFilter.DrawString(new CGPoint(Constants.PDFPagePadding, _yOffset),
                UIFont.BoldSystemFontOfSize(Constants.PDFPageTextFontSize));
            _yOffset += Constants.PDFPageTextFontSize + Constants.PDFPageTextPadding;

            var categories = _imageEntry.Observations.Select(o => o.Category.Label).Distinct().ToList();
            var categoriesText = string.Join(Constants.CategoriesJoinText, categories);
            categoriesText.DrawString(new CGPoint(Constants.PDFPagePadding, _yOffset),
                UIFont.SystemFontOfSize(Constants.PDFPageTextFontSize));
            _yOffset += Constants.PDFPageTextFontSize + Constants.PDFPageTextPadding;

            // draws table grids for category summary
            var tableOrigin = new CGPoint(Constants.PDFPagePadding, _yOffset);
            DrawTableLineAt(tableOrigin, Constants.PDFPageTableRowHeight, Constants.PDFPageTableColumnWidth,
                categories.Count + 1, Constants.PDFCategoryTableNumberOfColumn);
            GenerateCategoryTableContent(tableOrigin, Constants.PDFPageTableRowHeight,
                Constants.PDFPageTableColumnWidth);
        }

        /// <summary>
        /// Generate the image with coloured bounding boxes
        /// </summary>
        /// <param name="image">Image.</param>
        private void DrawImageWithBoundingBox(CGImage image)
        {
            // get page size in pdf document
            var pageRect = Constants.PDFPageSize;

            var imageDimension = CalculateImageDimensionInPage(pageRect, _resultImage.Width, _resultImage.Height);

            var imageWithOffset = new CGRect(imageDimension.X, imageDimension.Y - Constants.PDFPagePadding, imageDimension.Width, imageDimension.Height);

            UIGraphics.BeginPDFPage();
            using (var context = UIGraphics.GetCurrentContext())
            {
                context.SaveState();

                context.TranslateCTM(0,imageDimension.Height + Constants.PDFPagePadding);
                context.ScaleCTM(1, -1);
                context.DrawImage(imageWithOffset, image);

                context.RestoreState();

                _yOffset += imageWithOffset.Height;

                // draws number labels on image
                var observations = _imageEntry.Observations.ToList();

                foreach (var pair in observations.Select((value, index) => new { index, value }))
                {
                    // calculate new x and y position for number labels
                    var labelXPosition = (pair.value.BoundingBox.Right) / (_resultImage.Width / imageDimension.Width) +
                                         Constants.PDFPagePadding;
                    var labelYPosition = pair.value.BoundingBox.Bottom /
                                         (_resultImage.Height / imageDimension.Height) + Constants.PDFPagePadding;

                    // get string size
                    var textToDraw = string.Format(CultureInfo.CurrentCulture, Constants.NumberLabelFormat, pair.index + 1);
                    var sizeOfText =
                        textToDraw.StringSize(UIFont.BoldSystemFontOfSize(Constants.PDFImageTextLabelFontSize));
                    var rect = new CGRect(new CGPoint(labelXPosition - sizeOfText.Width, labelYPosition - sizeOfText.Height), sizeOfText);

                    // draws text label background
                    context.SetFillColor(Constants.TextLabelBackgroundColour);
                    context.FillRect(rect);

                    // draws text
                    context.SetFillColor(Constants.TextLabelTextColour);
                    textToDraw.DrawString(new CGPoint(labelXPosition - sizeOfText.Width, labelYPosition - sizeOfText.Height),
                                            UIFont.BoldSystemFontOfSize(Constants.PDFImageTextLabelFontSize));
                }

                context.Dispose();
            }
        }

        /// <summary>
        /// Calculates the image dimension in page according to ratio.
        /// </summary>
        /// <returns>The image dimension in page.</returns>
        /// <param name="pageRect">The dimension of one pdf page.</param>
        /// <param name="width">Original image width.</param>
        /// <param name="height">Original image Height.</param>
        private CGRect CalculateImageDimensionInPage(CGRect pageRect, int width, int height)
        {
            var sizeRatio = width / pageRect.Width;
            return new CGRect(Constants.PDFPagePadding, Constants.PDFPagePadding,
                width / sizeRatio - 2 * Constants.PDFPagePadding, height / sizeRatio);
        }

        /// <summary>
        /// Generates the content of the category table.
        /// </summary>
        /// <param name="tableOrigin">Table start coordinates.</param>
        /// <param name="rowHeight">Row height.</param>
        /// <param name="columnWidth">Column width.</param>
        private void GenerateCategoryTableContent(CGPoint tableOrigin, int rowHeight, int columnWidth)
        {
            var categories = _imageEntry.Observations.Select(o => o.Category).Distinct().ToList();

            // generate table header
            Constants.CategoryTableHeader[0].DrawString(
                new CGPoint(tableOrigin.X + Constants.PDFPageTableTextPadding,
                    tableOrigin.Y + Constants.PDFPageTableTextPadding),
                UIFont.BoldSystemFontOfSize(Constants.PDFPageTextFontSize));
            
            Constants.CategoryTableHeader[1].DrawString(
                new CGPoint(tableOrigin.X + Constants.PDFPageTableTextPadding + columnWidth,
                    tableOrigin.Y + Constants.PDFPageTableTextPadding),
                UIFont.BoldSystemFontOfSize(Constants.PDFPageTextFontSize));

            tableOrigin.Y += rowHeight;

            for (var i = 0; i < categories.Count; i++)
            {
                // draws category label
                categories[i].Label
                    .DrawString(
                        new CGPoint(tableOrigin.X + Constants.PDFPageTableTextPadding,
                            tableOrigin.Y + i * rowHeight + Constants.PDFPageTableTextPadding),
                        UIFont.SystemFontOfSize(Constants.PDFPageTextFontSize));

                // draw category count
                var quantity =
                    _imageEntry.Observations.Count(observation => observation.Category.Equals(categories[i]));
                quantity.ToString()
                    .DrawString(
                        new CGPoint(tableOrigin.X + columnWidth + Constants.PDFPageTableTextPadding,
                            tableOrigin.Y + i * rowHeight + Constants.PDFPageTableTextPadding),
                        UIFont.SystemFontOfSize(Constants.PDFPageTextFontSize));
            }
        }

        /// <summary>
        /// Generates the text content of the observation summary table.
        /// </summary>
        /// <param name="tableOrigin">Table start coordinates.</param>
        /// <param name="rowHeight">Row height.</param>
        /// <param name="columnWidth">Column width.</param>
        /// <param name="observations">Observations.</param>
        /// <param name="startIndex">Start index.</param>
        private void GenerateObservationTableContent(CGPoint tableOrigin, int rowHeight, int columnWidth, List<Observation> observations, int startIndex)
        {
            for (var i = 0; i < observations.Count; i++)
            {
                (i + startIndex + 1).ToString()
                    .DrawString(
                        new CGPoint(tableOrigin.X + Constants.PDFPageTableTextPadding,
                            tableOrigin.Y + i * rowHeight + Constants.PDFPageTableTextPadding),
                        UIFont.SystemFontOfSize(Constants.PDFPageTextFontSize));
                observations.ElementAt(i).Category.Label.DrawString(
                    new CGPoint(tableOrigin.X + columnWidth + Constants.PDFPageTableTextPadding,
                        tableOrigin.Y + i * rowHeight + Constants.PDFPageTableTextPadding),
                    UIFont.SystemFontOfSize(Constants.PDFPageTextFontSize));
                var confidence = string.Format(CultureInfo.CurrentCulture, Constants.ObservationConfidenceFormat,
                    Math.Round(observations.ElementAt(i).Confidence, 4) * 100);
                
                confidence.DrawString(
                    new CGPoint(tableOrigin.X + 2 * columnWidth + Constants.PDFPageTableTextPadding,
                        tableOrigin.Y + i * rowHeight + Constants.PDFPageTableTextPadding),
                    UIFont.SystemFontOfSize(Constants.PDFPageTextFontSize));
            }
        }

        /// <summary>
        /// Draws the table at origin, rowHeight, columnWidth, rowCount and columnCount.
        /// </summary>
        /// <param name="origin">Table start coordinates.</param>
        /// <param name="rowHeight">Row height.</param>
        /// <param name="columnWidth">Column width.</param>
        /// <param name="rowCount">Row count.</param>
        /// <param name="columnCount">Column count.</param>
        private void DrawTableLineAt(CGPoint origin, int rowHeight, int columnWidth, int rowCount, int columnCount)
        {
            // calculate start and end position of table horizontal lines
            for (var i = 0; i <= rowCount; i++)
            {
                var newOrigin = origin.Y + (rowHeight * i);
                var from = new CGPoint(origin.X, newOrigin);
                var to = new CGPoint(origin.X + (columnCount * columnWidth), newOrigin);
                DrawLineFromPoint(from, to);
            }

            // calculate start and end position of table vertical lines
            for (var i = 0; i <= columnCount; i++)
            {
                var newOrigin = origin.X + (columnWidth * i);
                var from = new CGPoint(newOrigin, origin.Y);
                var to = new CGPoint(newOrigin, origin.Y + (rowCount * rowHeight));
                DrawLineFromPoint(from, to);
            }
        }

        /// <summary>
        /// Draws the table line using start and end coordinates.
        /// </summary>
        /// <param name="from">Start coordinate.</param>
        /// <param name="to">End coordinate.</param>
        private void DrawLineFromPoint(CGPoint from, CGPoint to)
        {
            var context = UIGraphics.GetCurrentContext();
            context.SetLineWidth(Constants.PDFPageTableLineWidth);

            context.SetStrokeColor(Constants.PDFTableStrokeColour);
            context.MoveTo(from.X, from.Y);
            context.AddLineToPoint(to.X, to.Y);

            context.StrokePath();
        }
    }
}

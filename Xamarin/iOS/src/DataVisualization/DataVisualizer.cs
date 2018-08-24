using System;
using System.Collections.Generic;
using System.Linq;
using EarthLens.Models;
using SkiaSharp.Views.iOS;
using EarthLens.iOS.Sidebar;
using UIKit;

namespace EarthLens.iOS.DataVisualization
{
    public abstract class DataVisualizer
    {
        public UIView Legend { get; private set; }
        public UIView Chart { get; protected set; }

        protected Dictionary<Category, UIView> Labels;

        public DataVisualizer(IEnumerable<Category> allCategories)
        {
            Legend = GenerateLegend(allCategories);
        }

        /// <summary>
        /// Updates the specified <see cref="Category"/>s in the visualization.
        /// </summary>
        /// <param name="enabledCategories">The enabled <see cref="Category"/>s.</param>
        /// <param name="disabledCategories">The disabled <see cref="Category"/>s.</param>
        /// <param name="view">The specified <see cref="UIView"/> to change the layout.</param>
        public virtual void Update(IEnumerable<Category> enabledCategories, IEnumerable<Category> disabledCategories,
            UIView view)
        {
            if (enabledCategories == null || disabledCategories == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var category in enabledCategories)
            {
                Labels[category].Hidden = false;
            }

            foreach (var category in disabledCategories)
            {
                Labels[category].Hidden = true;
            }
        }

        protected abstract UIView GenerateChart(IEnumerable<Dictionary<Category, int>> allDataPoints,
            IEnumerable<Category> allCategories);

        /// <summary>
        /// Generates the legend of the data visualization given the specified <see cref="Category"/>s.
        /// </summary>
        /// <param name="categories">The specified <see cref="Category"/>s.</param>
        /// <returns>The generated <see cref="UIView"/> representing the legend.</returns>
        private UIView GenerateLegend(IEnumerable<Category> categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException();
            }

            Labels = new Dictionary<Category, UIView>();

            var categoriesList = categories.ToList();

            var totalLength = (nfloat) 0.0;

            foreach (var category in categoriesList)
            {
                var label = GetLabel(category);
                Labels.Add(category, label);
                totalLength += label.Frame.Width;
            }

            var stackView = DataVisualizationUtils.GenerateStackView(Constants.LegendStackViewAxis,
                Constants.LegendStackViewAlignment,
                Constants.LegendStackViewDistribution,
                Constants.LegendStackViewSpacing,
                Labels.Select(pair => pair.Value));

            totalLength += Labels.Count * Constants.LegendStackViewSpacing;

            return DataVisualizationUtils.WrapStackView(stackView, totalLength, Constants.LegendHeight,
                NSLayoutAttribute.LeadingMargin, NSLayoutAttribute.CenterY);
        }

        /// <summary>
        /// Generates the label of the specified <see cref="Category"/>.
        /// </summary>
        /// <param name="category">The specified <see cref="Category"/>.</param>
        /// <returns>The generated <see cref="UIView"/> representing the label of the <see cref="Category"/>.</returns>
        private static UIView GetLabel(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException();
            }

            var circle = new UIButton();
            circle.WidthAnchor.ConstraintEqualTo(Constants.LabelCircleRadius * 2).Active = true;
            circle.HeightAnchor.ConstraintEqualTo(Constants.LabelCircleRadius * 2).Active = true;
            circle.Layer.CornerRadius = Constants.LabelCircleRadius;
            circle.BackgroundColor = category.Color.ToUIColor();

            var text = new UILabel
            {
                Text = category.Label,
                Font = UIFont.SystemFontOfSize(Constants.LabelFontSize),
                TextColor = Constants.LabelTextColor
            };
            text.SizeToFit();

            var stackView = DataVisualizationUtils.GenerateStackView(Constants.LabelStackViewAxis,
                Constants.LabelStackViewAlignment,
                Constants.LabelStackViewDistribution,
                Constants.LabelStackViewSpacing, 
                new UIView[] {circle, text});

            ConfigureBarChartLegendAccessibilityAttributes(circle, text);

            return DataVisualizationUtils.WrapStackView(stackView,
                   text.Frame.Width + Constants.LabelStackViewSpacing + Constants.LabelCircleRadius * 2,
                   Constants.LegendHeight, NSLayoutAttribute.CenterX, NSLayoutAttribute.CenterY);
        }

        /// <summary>
        /// Disposes the current <see cref="DataVisualization"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DataVisualizer()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (Chart == null) return;
            Legend.Dispose();
            Legend = null;
            Chart.Dispose();
            Chart = null;
        }

        #region
        private static void ConfigureBarChartLegendAccessibilityAttributes(UIButton circle, UILabel text)
        {
            circle.IsAccessibilityElement = true;
            circle.UserInteractionEnabled = true;
            circle.AccessibilityTraits = UIAccessibilityTrait.StaticText;
            var colourNumber = FilterViewController.GetColorVoiceOverEncoding(circle.BackgroundColor.CGColor);
            circle.AccessibilityLabel = string.Format(AccessibilityConstants.TimelineLegendColorButtonAccessibilityLabel, colourNumber);
            text.AccessibilityHint = AccessibilityConstants.BarAccessibilityHint;
            text.IsAccessibilityElement = true;
            text.UserInteractionEnabled = true;
            text.AccessibilityTraits = UIAccessibilityTrait.Button;
            text.AccessibilityLabel = text.Text;
        }
        #endregion
    }
}

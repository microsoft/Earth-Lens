using System;
using System.Collections.Generic;
using System.Linq;
using EarthLens.Models;
using CoreGraphics;
using SkiaSharp.Views.iOS;
using UIKit;
using EarthLens.iOS.Sidebar;

namespace EarthLens.iOS.DataVisualization
{
    public class BarChart : IDisposable
    {
        public UIView Chart { get; private set; }
        public BarChartSeries ParentBarChartSeries { get; set; }
        public readonly Dictionary<Category, (UIButton, NSLayoutConstraint, NSLayoutConstraint, UILabel)> Mapping;

        private readonly Dictionary<Category, int> _input;
        private readonly HashSet<Category> _enabledCategories;

        /// <summary>
        /// Constructs a <see cref="BarChart"/> with the specified data points.
        /// </summary>
        /// <param name="dataPoints">The specified data points.</param>
        /// <param name="allCategories">The list of <see cref="Category"/>s displayed to the user by default.</param>
        public BarChart(Dictionary<Category, int> dataPoints, IEnumerable<Category> allCategories)
        {
            if (allCategories == null)
            {
                throw new ArgumentNullException();
            }

            _input = dataPoints;

            Mapping = new Dictionary<Category, (UIButton, NSLayoutConstraint, NSLayoutConstraint, UILabel)>();

            var categories = allCategories.ToList();
            foreach (var category in categories)
            {
                var tuple = GetBar(category.Color.ToUIColor());
                Mapping[category] = tuple;
                tuple.Item1.TouchUpInside += (sender, e) => CountViewDidChange((UIButton)sender, category);
            }

            var stackView = DataVisualizationUtils.GenerateStackView(Constants.BarChartStackViewAxis,
                Constants.BarChartStackViewAlignment,
                Constants.BarChartStackViewDistribution,
                Constants.BarChartStackViewSpacing,
                categories.Select(category => Mapping[category].Item1));

            Chart = DataVisualizationUtils.WrapStackView(stackView, Constants.BarChartWidth, Constants.BarChartHeight,
                NSLayoutAttribute.CenterX, NSLayoutAttribute.Bottom);

            var countLabels = Mapping.Select(mapping => mapping.Value.Item4).ToArray();
            Chart.AddSubviews(countLabels);

            _enabledCategories = new HashSet<Category>();
            ConfigureChartAccessibilityAttributes();
        }

        /// <summary>
        /// Updates the specified <see cref="Category" />s in the visualization.
        /// </summary>
        /// <param name="enabledCategories">The enabled <see cref="Category" />s.</param>
        /// <param name="disabledCategories">The disabled <see cref="Category" />s.</param>
        /// <param name="view">The specified <see cref="UIView" /> to change the layout.</param>
        /// <param name="maxDataPoint">The maximum data point in the entire bar chart series.</param>
        /// <param name="barWidth">The specified bar width of each bar.</param>
        public void Update(IEnumerable<Category> enabledCategories, IEnumerable<Category> disabledCategories, UIView
            view, int maxDataPoint, nfloat barWidth)
        {
            if (enabledCategories == null || disabledCategories == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var category in enabledCategories)
            {
                if (!_input.ContainsKey(category) || !Mapping[category].Item1.Hidden)
                {
                    continue;
                }

                Mapping[category].Item1.Hidden = false;
                _enabledCategories.Add(category);
            }

            foreach (var category in disabledCategories)
            {
                if (!_input.ContainsKey(category) || Mapping[category].Item1.Hidden)
                {
                    continue;
                }

                Mapping[category].Item1.Hidden = true;
                _enabledCategories.Remove(category);
            }

            UpdateBarShape(view, maxDataPoint, barWidth);
            ConfigureBarsAccessibilityAttributes();

        }

        /// <summary>
        /// Returns a bar in the visualization corresponding to the specified data point and the specified color.
        /// </summary>
        /// <param name="color">The specified <see cref="UIColor"/>.</param>
        /// <returns>The corresponding bar with width/height constraints.</returns>
        private static (UIButton, NSLayoutConstraint, NSLayoutConstraint, UILabel) GetBar(UIColor color)
        {
            var bar = new UIButton
            {
                BackgroundColor = color
            };

            var widthConstraint = bar.WidthAnchor.ConstraintEqualTo(0); // dummy constraint
            widthConstraint.Active = true;

            var heightConstraint = bar.HeightAnchor.ConstraintEqualTo(0); // dummy constraint
            heightConstraint.Active = true;

            bar.Layer.CornerRadius = Constants.BarChartBarCornerRadius;
            bar.Hidden = true;

            var count = new UILabel
            {
                BackgroundColor = Constants.CountLabelBackgroundColor
            };

            count.Layer.Opacity = Constants.CountLabelOpacity;
            count.TextColor = Constants.CountLabelTextColor;
            count.Font = UIFont.SystemFontOfSize(Constants.CountLabelTextFontSize);
            count.ClipsToBounds = true;
            count.Layer.CornerRadius = Constants.CountLabelTextFontSize * Constants.CountLabelCornerRatio;
            count.Hidden = true;

            return (bar, widthConstraint, heightConstraint, count);
        }

        /// <summary>
        /// Should be called when user wants to show/hide counts in bar chart.
        /// </summary>
        /// <param name="label">Category label.</param>
        public void CountViewDidChange(UIButton sender, Category category)
        {
            var displayedLabel = GetLabelPopupBasedOnBar(sender);
            if (displayedLabel.Hidden)
            {
                ChangeAccessibilityVoiceOverFocusToCountPopup(displayedLabel);
            }

            ParentBarChartSeries.ToggleCategoryCounts(category);
        }

        /// <summary>
        /// Updates the bar widths of enabled categories.
        /// </summary>
        /// <param name="view">The <see cref="UIView"/> on which layout changes are applied.</param>
        /// <param name="maxDataPoint">The maximum data point in the entire bar chart series.</param>
        /// <param name="barWidth">The bar width of the bar to update.</param>
        private void UpdateBarShape(UIView view, int maxDataPoint, nfloat barWidth)
        {
            var classCount = _enabledCategories.Count;
            var xOffeset = (Constants.ThumbnailWidth - classCount * (barWidth + Constants.BarChartStackViewSpacing)) / 2;

            foreach (var pair in _enabledCategories.Select((value, index) => new { index, value }))
            {
                var widthConstraint = Mapping[pair.value].Item2;
                var heightConstraint = Mapping[pair.value].Item3;
                var countView = Mapping[pair.value].Item4;
                UIView.Animate(1, () =>
                {
                    widthConstraint.Constant = barWidth;
                    heightConstraint.Constant =
                        Constants.BarChartMaxBarHeight * ((nfloat)_input[pair.value] / maxDataPoint) *
                        Constants.MaxDataPointFactor;
                    view.LayoutIfNeeded();
                });
                countView.Text = _input[pair.value].ToString();
                countView.SizeToFit();

                var yPosition = Constants.BarChartMaxBarHeight - Constants.BarChartMaxBarHeight *
                                ((nfloat)_input[pair.value] / maxDataPoint) *
                                Constants.MaxDataPointFactor;

                if (yPosition + countView.Frame.Height > Constants.BarChartMaxBarHeight)
                {
                    yPosition -= countView.Frame.Height;
                }

                countView.Frame =
                    new CGRect(
                        xOffeset + (barWidth + Constants.BarChartStackViewSpacing) * pair.index + barWidth +
                        Constants.CountLabelXOffest, yPosition, countView.Frame.Width, countView.Frame.Height);

                ConfigureCountPopoverAccessibilityAttributes(countView, countView.Text);
            }
            ConfigureBarsAccessibilityAttributes();
        }

        /// <summary>
        /// Disposes the current <see cref="BarChart"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BarChart()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (Chart == null) return;
            Chart.Dispose();
            Chart = null;
        }

        #region Configure Accessibility Attributes
        private static void ConfigureCountPopoverAccessibilityAttributes(UIView view ,string countText)
        {
            view.IsAccessibilityElement = true;
            view.AccessibilityTraits = UIAccessibilityTrait.StaticText;
            view.AccessibilityLabel = string.Format(AccessibilityConstants.CountPopoverAccessibilityLabel,countText);
        }

        private void ConfigureBarAccessibilityAttributes(UIButton bar)
        {
            var colourNumber = FilterViewController.GetColorVoiceOverEncoding(bar.BackgroundColor.CGColor);
            var className = GetCategoryLabelBaseOnMatchingColorBar(bar.BackgroundColor);
            var countString = GetClassCountBaseOnMatchingColorBar(bar.BackgroundColor);
            bar.AccessibilityTraits = UIAccessibilityTrait.Button;
            bar.AccessibilityLabel = string.Format(AccessibilityConstants.BarAccessibilityLabel, new object[] { colourNumber, className, countString });
            bar.AccessibilityHint = AccessibilityConstants.BarAccessibilityHint;
        }

        private void ConfigureChartAccessibilityAttributes()
        {
            Chart.ShouldGroupAccessibilityChildren = true;
        }

        private string GetCategoryLabelBaseOnMatchingColorBar(UIColor barColor)
        {
            foreach(var pair in Mapping)
            {
                if (pair.Value.Item1.BackgroundColor.Equals(barColor))
                {
                    return pair.Key.Label;
                }
            }

            return null;
        }

        private string GetClassCountBaseOnMatchingColorBar(UIColor barColor)
        {
            foreach (var pair in Mapping)
            {
                if (pair.Value.Item1.BackgroundColor.Equals(barColor))
                {
                    return pair.Value.Item4.Text;
                }
            }
            return null;
        }

        private UILabel GetLabelPopupBasedOnBar(UIButton bar)
        {
            foreach (var pair in Mapping)
            {
                if (pair.Value.Item1 == bar)
                {
                    return pair.Value.Item4;
                }
            }
            return null;
        }

        private void ConfigureBarsAccessibilityAttributes()
        {
            foreach(var pair in Mapping)
            {
                ConfigureBarAccessibilityAttributes(pair.Value.Item1);
            }
        }

        private static void ChangeAccessibilityVoiceOverFocusToCountPopup(UIView viewToFocus)
        {
            UIAccessibility.PostNotification(UIAccessibilityPostNotification.ScreenChanged, viewToFocus);
        }
        #endregion
    }
}

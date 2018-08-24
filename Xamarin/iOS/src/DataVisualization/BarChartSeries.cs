using System;
using System.Collections.Generic;
using System.Linq;
using EarthLens.Models;
using UIKit;

namespace EarthLens.iOS.DataVisualization
{
    public sealed class BarChartSeries : DataVisualizer, IDisposable
    {
        public IList<BarChart> BarCharts;

        private ISet<Category> _enabledCategories;
        private IDictionary<Category, int> _maxDataPoints;
        private SortedSet<int> _maxDataPointSet;
        private IEnumerable<Dictionary<Category, int>> _input;

        /// <summary>
        /// Constructs a <see cref="DataVisualizer" /> with the specified data points and <see cref="Category" />s.
        /// </summary>
        /// <param name="allDataPoints">The specified data points for each image.</param>
        /// <param name="allCategories">The specified <see cref="Category" />s.</param>
        public BarChartSeries(IEnumerable<Dictionary<Category, int>> allDataPoints, IEnumerable<Category> allCategories)
            : base(allCategories)
        {
            Chart = GenerateChart(allDataPoints, allCategories);
            SetupLegendEventHandlers();
        }

        protected override UIView GenerateChart(IEnumerable<Dictionary<Category, int>> allDataPoints,
            IEnumerable<Category> allCategories)
        {
            if (allDataPoints == null || allCategories == null)
            {
                throw new ArgumentNullException();
            }

            var allDataPointsList = allDataPoints.ToList();
            _input = allDataPointsList;
            BarCharts = new List<BarChart>();
            _enabledCategories = new HashSet<Category>();
            _maxDataPoints = new Dictionary<Category, int>();
            _maxDataPointSet = new SortedSet<int>();

            foreach (var dataPoints in allDataPointsList)
            {
                var barChart = new BarChart(dataPoints, allCategories);
                BarCharts.Add(barChart);
                barChart.ParentBarChartSeries = this;

                foreach (var categoryDataPointPair in dataPoints)
                {
                    var category = categoryDataPointPair.Key;
                    var dataPoint = categoryDataPointPair.Value;
                    if (_maxDataPoints.ContainsKey(category))
                    {
                        _maxDataPoints[category] = Math.Max(_maxDataPoints[category], dataPoint);
                    }
                    else
                    {
                        _maxDataPoints[category] = dataPoint;
                    }
                }
            }

            var stackView = DataVisualizationUtils.GenerateStackView(Constants.BarChartSeriesStackViewAxis,
                Constants.BarChartSeriesStackViewAlignment,
                Constants.BarChartSeriesStackViewDistribution,
                Constants.BarChartSeriesStackViewSpacing,
                BarCharts.Select(barChart => barChart.Chart));

            var imageCount = allDataPointsList.Count();
            var width = imageCount * Constants.BarChartWidth +
                        (imageCount - 1) * Constants.BarChartSeriesStackViewSpacing;

            return DataVisualizationUtils.WrapStackView(stackView, width, Constants.BarChartSeriesStackViewHeight,
                NSLayoutAttribute.CenterX, NSLayoutAttribute.CenterY);
        }

        /// <inheritdoc />
        /// <summary>
        /// Updates the specified <see cref="T:EarthLens.Models.Category" />s in the visualization.
        /// </summary>
        /// <param name="enabledCategories">The enabled <see cref="T:EarthLens.Models.Category" />s.</param>
        /// <param name="disabledCategories">The disabled <see cref="T:EarthLens.Models.Category" />s.</param>
        /// <param name="view">The specified <see cref="T:UIKit.UIView" /> to change the layout.</param>
        public override void Update(IEnumerable<Category> enabledCategories, IEnumerable<Category> disabledCategories,
            UIView view)
        {
            if (enabledCategories == null || disabledCategories == null)
            {
                throw new ArgumentNullException();
            }

            base.Update(enabledCategories, disabledCategories, view);

            foreach (var category in enabledCategories)
            {
                if (_enabledCategories.Contains(category))
                {
                    continue;
                }

                _enabledCategories.Add(category);
                _maxDataPointSet.Add(_maxDataPoints[category]);
            }

            foreach (var category in disabledCategories)
            {
                if (!_enabledCategories.Contains(category))
                {
                    continue;
                }

                _enabledCategories.Remove(category);
                _maxDataPointSet.Remove(_maxDataPoints[category]);
            }

            var maxDataPoint = _maxDataPointSet.Count == 0 ? 0 : _maxDataPointSet.Max;

            var maxNumberOfEnabledCategories = 0;
            foreach (var dataPoints in _input)
            {
                var numberOfEnabledCategories = 0;
                foreach (var categoryDataPointPair in dataPoints)
                {
                    if (_enabledCategories.Contains(categoryDataPointPair.Key))
                    {
                        numberOfEnabledCategories++;
                    }
                }

                maxNumberOfEnabledCategories = Math.Max(maxNumberOfEnabledCategories, numberOfEnabledCategories);
            }

            foreach (var barChart in BarCharts)
            {
                barChart.Update(enabledCategories, disabledCategories, view, maxDataPoint,
                    Constants.BarChartBarWidthTotal / maxNumberOfEnabledCategories);
            }
        }

        /// <summary>
        /// Toggles the count labels corresponding to the specified <see cref="Category"/>.
        /// </summary>
        /// <param name="category">The specified <see cref="Category"/>.</param>
        public void ToggleCategoryCounts(Category category)
        {
            foreach (var chart in BarCharts)
            {
                var countLabel = chart.Mapping[category].Item4;
                countLabel.Hidden = !countLabel.Hidden;
            }
        }

        /// <summary>
        /// Setups the event handlers for tapping on legend labels.
        /// </summary>
        private void SetupLegendEventHandlers()
        {
            foreach (var pair in Labels)
            {
                var category = pair.Key;
                var label = pair.Value;
                label.AddGestureRecognizer(new UITapGestureRecognizer(tap =>
                {
                    ToggleCategoryCounts(category);
                }));
            }
        }

        ~BarChartSeries()
        {
            Dispose(false);
        }
    }
}

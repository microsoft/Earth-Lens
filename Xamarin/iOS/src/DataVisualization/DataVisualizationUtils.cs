using System;
using System.Collections.Generic;
using UIKit;

namespace EarthLens.iOS.DataVisualization
{
    public static class DataVisualizationUtils
    {
        /// <summary>
        /// Generates a <see cref="UIStackView"/> given the specified <see cref="UILayoutConstraintAxis"/>,
        /// <see cref="UIStackViewAlignment"/>, <see cref="UIStackViewDistribution"/>, spacing and sub-views.
        /// </summary>
        /// <param name="axis">The specified <see cref="UILayoutConstraintAxis"/>.</param>
        /// <param name="alignment">The specified <see cref="UIStackViewAlignment"/>.</param>
        /// <param name="distribution">The specified <see cref="UIStackViewDistribution"/>.</param>
        /// <param name="spacing">The specified spacing.</param>
        /// <param name="subViews">The specified sub-views.</param>
        /// <returns>The generated <see cref="UIStackView"/>.</returns>
        public static UIStackView GenerateStackView(UILayoutConstraintAxis axis, UIStackViewAlignment alignment,
            UIStackViewDistribution distribution, nfloat spacing, IEnumerable<UIView> subViews)
        {
            if (subViews == null)
            {
                throw new ArgumentNullException();
            }

            var stackView = new UIStackView
            {
                Axis = axis,
                Alignment = alignment,
                Distribution = distribution,
                Spacing = spacing
            };

            foreach (var subView in subViews)
            {
                stackView.AddArrangedSubview(subView);
            }

            stackView.TranslatesAutoresizingMaskIntoConstraints = false;
            stackView.SizeToFit();
            stackView.LayoutIfNeeded();

            ConfigureStackViewAccessibilityAttributes(stackView);
            return stackView;
        }

        /// <summary>
        /// Wraps the specified <see cref="UIStackView"/> and generates a <see cref="UIView"/>.
        /// </summary>
        /// <param name="stackView">The specified <see cref="UIStackView"/>.</param>
        /// <param name="width">The specified, optional width of the target <see cref="UIView"/>.</param>
        /// <param name="height">The specified, optional height of the target <see cref="UIView"/>.</param>
        /// <param name="horizontalConstraint">The constraint attribute on the horizontal axis.</param>
        /// <param name="verticalConstraint">The constraint attribute on the vertical axis.</param>
        /// <returns>The generated <see cref="UIView"/>.</returns>
        public static UIView WrapStackView(UIStackView stackView, nfloat? width, nfloat? height,
            NSLayoutAttribute horizontalConstraint, NSLayoutAttribute verticalConstraint)
        {
            var wrapper = new UIView();

            if (width.HasValue)
            {
                wrapper.WidthAnchor.ConstraintEqualTo(width.Value).Active = true;
            }

            if (height.HasValue)
            {
                wrapper.HeightAnchor.ConstraintEqualTo(height.Value).Active = true;
            }

            var centerXConstraint = NSLayoutConstraint.Create(stackView, horizontalConstraint, NSLayoutRelation.Equal,
                wrapper, horizontalConstraint, 1f, 0f);
            var centerYConstraint = NSLayoutConstraint.Create(stackView, verticalConstraint, NSLayoutRelation.Equal,
                wrapper, verticalConstraint, 1f, 0f);

            wrapper.AddSubview(stackView);
            wrapper.AddConstraints(new[]
            {
                centerXConstraint,
                centerYConstraint
            });

            wrapper.TranslatesAutoresizingMaskIntoConstraints = false;
            wrapper.SizeToFit();
            wrapper.LayoutIfNeeded();

            ConfigureStackViewAccessibilityAttributes(wrapper);
            return wrapper;
        }

        #region
        private static void ConfigureStackViewAccessibilityAttributes(UIView view)
        {
            view.ShouldGroupAccessibilityChildren = true;
        }
        #endregion
    }
}

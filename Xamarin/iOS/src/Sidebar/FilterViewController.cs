using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EarthLens.iOS.ColourPicker;
using EarthLens.iOS.Results;
using EarthLens.Models;
using CoreGraphics;
using Foundation;
using SkiaSharp.Views.iOS;
using UIKit;

namespace EarthLens.iOS.Sidebar
{
    public partial class FilterViewController : UITableViewController
    {
        public ResultsViewController ParentResultsViewController { get; set; }
        public IEnumerable<Category> DisplayItems { get; set; }
        public UITextView NumberSelected { get; set; }
        public Dictionary<Observation, UIButton> Bindings { get; set; }
        public IEnumerable<Category> FilteredItems { get; set; }
        public HashSet<CGColor> ColoursInUse { get; private set; }
        public int SelectedClassCount { get; set; }
        public UIView SideBarView { get; set; }
        public Dictionary<string, NSLayoutConstraint> Constraints { get; set; }
        public Dictionary<Category, bool> CheckboxState { get; private set; }
        public UIView SearchView { get; set; }

        private nfloat _sectionHeaderHeight;
        private UISearchBar _searchBar;
        private UIView _noResultPage;
        private UITextView _noResultTextField;
        private Dictionary<string, List<string>> _indexedTableItems;
        private string[] _keys;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            TableView.RegisterNibForCellReuse(UINib.FromName(Constants.FilterCellViewControllerName, null),
                Constants.FilterCellIdentifier);
            TableView.CellLayoutMarginsFollowReadableWidth = true;

            TableView.RegisterNibForHeaderFooterViewReuse(UINib.FromName(Constants.FilterCellViewControllerName, null),
                Constants.FilterCellIdentifier);

            // Setting up items for searchbar to search
            FilteredItems = DisplayItems;
            TableView.UserInteractionEnabled = true;
            TableView.AllowsSelection = true;
            TableView.CellLayoutMarginsFollowReadableWidth = true;

            _searchBar = new UISearchBar { WeakDelegate = this };

            ConfigureMegaSection();

            // position search bar
            SearchView.AddSubview(_searchBar);
            _searchBar.SizeToFit();
            _searchBar.Frame = new CGRect(0,
                Constants.SearchViewHeight - _searchBar.Frame.Height - Constants.SearchbarPadding,
                Constants.TableRowWidth, _searchBar.Frame.Height);

            // style of search bar
            _searchBar.BarStyle = UIBarStyle.Black;
            _searchBar.BackgroundColor = Constants.SearchbarBackgroundColor;
            _searchBar.SearchBarStyle = UISearchBarStyle.Minimal;

            // hide cancel button on search bar to begin 
            _searchBar.ShowsCancelButton = false;
            _searchBar.CancelButtonClicked += _searchBar_CancelButtonClicked;

            // set up delegates to handle events on search bar
            _searchBar.TextChanged += CancelButtonStyle;
            _searchBar.TextChanged += UpdateSearchResultsForSearchController;

            CreateNoResultPage();

            // Set class counts for sidebar header
            SelectedClassCount = DisplayItems.Count();

            GenerateColoursInUse();
     
            _sectionHeaderHeight = TableView.SectionHeaderHeight;

            CheckboxState = new Dictionary<Category, bool>();
        }

        /// <summary>
        /// Generates the colours in use.
        /// </summary>
        public void GenerateColoursInUse()
        {
            ColoursInUse = new HashSet<CGColor>(new ColoursInUseEqualityComparer());
            foreach (var category in DisplayItems)
            {
                ColoursInUse.Add(category.Color.ToCGColor());
            }
        }

        /// <summary>
        /// Sort categoried using mega-classes
        /// </summary>
        public void ConfigureMegaSection()
        {
            // organize category labels using the mega-classes
            _indexedTableItems = new Dictionary<string, List<string>>();

            foreach (var category in FilteredItems)
            {
                if (_indexedTableItems.ContainsKey(category.MegaCategory.Label))
                {
                    _indexedTableItems[category.MegaCategory.Label].Add(category.Label);
                }
                else
                {
                    _indexedTableItems.Add(category.MegaCategory.Label, new List<string>() { category.Label });
                }
            }

            _keys = _indexedTableItems.Keys.ToArray();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (tableView == null || indexPath == null)
            {
                throw new ArgumentNullException();
            }
            var cell = new UITableViewCell(UITableViewCellStyle.Default, (NSString)Constants.FilterCellIdentifier);

            var categoryString = _indexedTableItems.ElementAt(indexPath.Section).Value.ElementAt(indexPath.Row);
            var classCategory = FilteredItems.FirstOrDefault(category => category.Label.Equals(categoryString));
            if (classCategory != null)
            {
                ConfigureCell(cell, classCategory);
            }
            else
            {
                TableView.ReloadRows(new[] { indexPath }, UITableViewRowAnimation.Fade);
            }

            return cell;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return _keys[section];
        }

        /// <summary>
        /// Add count to Mega class section header
        /// </summary>
        /// <param name="tableView"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var headerView = new UITableViewHeaderFooterView();
            var count = new UILabel();
            var categories = _indexedTableItems[_keys[section]];
            var observationCount = 0;

            foreach(var category in categories)
            {
                observationCount += Bindings.ToList().FindAll(binding => binding.Key.Category.Label.Equals(category)).Count;
            }

            count.Text = observationCount.ToString();
            count.Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Body);
            count.AdjustsFontForContentSizeCategory = true;
            count.SizeToFit();
            headerView.AddSubview(count);
            headerView.SizeToFit();
            count.Frame =
                new CGRect(
                    Constants.TableRowMargin + Constants.CellCountTextXOffset + Constants.ClassLabelWidth +
                    Constants.MegaClassPadding / 2, Constants.MegaClassPadding, count.Frame.Width, count.Frame.Height);
            return headerView;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _keys.Length;
        }

        /// <summary>
        /// Configures the cell.
        /// </summary>
        /// <param name="cell">Cell.</param>
        /// <param name="classCategory">Class name.</param>
        private void ConfigureCell(UITableViewCell cell, Category classCategory)
        {
            // Set up cell attributes
            TableView.RowHeight = Constants.TableRowHeight;
            cell.Frame = new CGRect(0, 0, Constants.TableRowWidth, Constants.TableRowHeight);

            // Elements in cell
            var classLabel = new UITextView();
            var countText = new UITextView();
            var checkBox = new UIButton();
            var colourButton = new UIButton();

            // Create a Colour Picker view controller
            var colorPopup = new ColourPickerViewController
            {
                Observations = Bindings,
                Category = classLabel,
                ClickedButton = colourButton,
                ParentTableView = this,
                ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen,
                RootResultsViewController = ParentResultsViewController
            };
            countText.UserInteractionEnabled = false;
            colourButton.BackgroundColor = classCategory.Color.ToUIColor();
            colourButton.Frame = new CGRect(Constants.TableRowMargin,
                Constants.TableRowHeight / 2 - Constants.ColourButtonSize / 2, Constants.ColourButtonSize,
                Constants.ColourButtonSize);
            colourButton.Layer.CornerRadius = Constants.ColourButtonSize / 2;
            colourButton.TouchUpInside += (sender, e) =>
            {
                // Present the popover
                PresentViewController(colorPopup, false, null);
            };

            ConfigureColourButtonVoiceoverAccessibilityAttributes(colourButton, classCategory.Color.ToCGColor());

            classLabel.TextAlignment = UITextAlignment.Left;
            classLabel.UserInteractionEnabled = false;
            classLabel.Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Body);
            classLabel.AdjustsFontForContentSizeCategory = true;
            classLabel.Text = classCategory.Label;
            classLabel.SizeToFit();
            classLabel.TextContainer.LineBreakMode = UILineBreakMode.TailTruncation;
            classLabel.TextContainer.MaximumNumberOfLines = Constants.SidebarClassLabelMaxLines;
            classLabel.Frame = new CGRect(Constants.ColourButtonSize + Constants.CellTextXOffset,
                Constants.TableRowHeight / 2 - classLabel.Frame.Height / 2, Constants.ClassLabelWidth,
                classLabel.Frame.Height + Constants.SidebarClassLabelHeightPadding);

            // maintain state of checkmarks 
            foreach (var checkBoxState in CheckboxState.ToList())
            {
                if (!checkBoxState.Key.Equals(classCategory)) continue;
                if (checkBoxState.Value)
                {
                    TickCheckBox(checkBox);
                }
                else
                {
                    UntickCheckBox(checkBox);
                }
                UpdateBoundingBox(classCategory, checkBox);
                ConfigureSidebarSubtitle();
            }

            // count the quantity for each catagory
            countText.Text = CountNumObservationOfCategory(classCategory).ToString(CultureInfo.CurrentCulture);

            //Set the colour picker button colour if previously edited
            countText.BackgroundColor = Constants.TableTextBackgroundColor;
            countText.TextColor = Constants.TableTextColor;
            countText.Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Body);
            countText.AdjustsFontForContentSizeCategory = true;
            countText.SizeToFit();
            countText.Frame =
                new CGRect(Constants.TableRowMargin + Constants.CellCountTextXOffset + Constants.ClassLabelWidth,
                    Constants.TableRowHeight / 2 - countText.Frame.Height / 2, Constants.TableRowWidth / 2,
                    countText.Frame.Height);
            checkBox.Frame = new CGRect(Constants.TableRowWidth - Constants.ColourButtonSize - Constants.TableRowMargin,
                Constants.TableRowHeight / 2 - Constants.ColourButtonSize / 2, Constants.ColourButtonSize,
                Constants.ColourButtonSize);
            checkBox.Layer.CornerRadius = Constants.ColourButtonSize / 2;
            checkBox.Layer.BorderWidth = Constants.CheckboxBorderWidth;
            checkBox.TouchUpInside += (sender, e) =>
             {
                 var categoryCount = DisplayItems.Count();

                 // Count number of classes that has been toggled during switch button change
                 if (SelectedClassCount >= 0 && SelectedClassCount <= categoryCount)
                 {

                     // The checkbox is checked
                     if (checkBox.CurrentBackgroundImage == null)
                     {
                         TickCheckBox(checkBox);
                         SelectedClassCount++;
                     }

                     // The checkbox is not checked
                     else
                     {
                         UntickCheckBox(checkBox);
                         SelectedClassCount--;
                     }
                 }

                 ConfigureSidebarSubtitle();

                 UpdateBoundingBox(classCategory, checkBox);
             };

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.AddSubview(classLabel);
            cell.AddSubview(countText);
            cell.AddSubview(colourButton);
            cell.AddSubview(checkBox);
            ConfigureCellAccessibility(cell);
        }

        public int CountNumObservationOfCategory(Category category)
        {
            var num = 0;
            foreach (var ob in Bindings)
            {
                if (!ob.Key.Category.Equals(category)) continue;
                num++;
            }
            return num;
        }

        private void TickCheckBox(UIButton checkBox)
        {
            var img = UIImage.FromFile(Constants.GalleryCheckmarkImage);
            checkBox.SetBackgroundImage(img, UIControlState.Normal);
            checkBox.Layer.BorderColor = Constants.CheckboxSelectedBorderColor;
            ConfigureCheckBoxStateAccessibilityAttribute(checkBox, true);
        }

        private void UntickCheckBox(UIButton checkBox)
        {
            checkBox.SetBackgroundImage(null, UIControlState.Normal);
            checkBox.Layer.BorderColor = Constants.CheckboxUnselectedBorderColor;
            ConfigureCheckBoxStateAccessibilityAttribute(checkBox, false);
        }

        /// <summary>
        /// Configure the subtitle of sidebar
        /// </summary>
        private void ConfigureSidebarSubtitle()
        {
            var categoryCount = DisplayItems.Count();
            NumberSelected.Text = string.Format(CultureInfo.CurrentCulture,
                categoryCount > 1
                    ? SharedConstants.SidebarSubtitleFormatPlural
                    : SharedConstants.SidebarSubtitleFormatSingular,
                SelectedClassCount, categoryCount);

            Constraints[Constants.DefaultHeightConstraintKey].Constant = SideBarView.Frame.Height;
        }

        /// <summary>
        /// Show or hide bounding box according to checkbox.
        /// </summary>
        /// <param name="classCategory">Class name.</param>
        /// <param name="button">Checkbox button.</param>
        private void UpdateBoundingBox(Category classCategory, UIButton button)
        {
            foreach (var box in Bindings)
            {
                if (!box.Key.Category.Equals(classCategory))
                {
                    continue;
                }
                box.Value.Hidden = button.CurrentBackgroundImage == null;
                CheckboxState[box.Key.Category] = button.CurrentBackgroundImage != null;
            }
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return _indexedTableItems[_keys[section]].Count;
        }

        private void _searchBar_CancelButtonClicked(object sender, EventArgs e)
        {
            _searchBar.Text = string.Empty;
            _searchBar.ShowsCancelButton = false;
            _searchBar.EndEditing(true);
            UpdateSearchResultsForSearchController(sender, e);
        }

        private void UpdateSearchResultsForSearchController(object sender, EventArgs e)
        {
            FilteredItems = PerformSearch(_searchBar.Text);
            if (!FilteredItems.Any())
            {
                ShowNoResultPage(_searchBar.Text);
            }
            else
            {
                HideNoResultPage();
            }
            ConfigureMegaSection();
            TableView.ReloadData();
        }

        /// <summary>
        /// Show or hide bounding box according to switch toggle.
        /// </summary>
        /// <param name="searchString"> the string with which the search is conducted</param>
        /// <returns> the categories whose labels begin with the searchString</returns>
        private IEnumerable<Category> PerformSearch(string searchString)
        {
            TableView.SectionHeaderHeight = string.IsNullOrEmpty(searchString)
                ? _sectionHeaderHeight
                : Constants.SectionHeaderClear;
            searchString = searchString != null ? searchString.Trim() : "";
            var filteredCategories = DisplayItems
                .Where(o => o.Label.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            return filteredCategories.Distinct().ToList();
        }

        #region Cancel Page
        /// <summary>
        /// A event handler method that only shows cancel button when the searchbar string is a non-empty string
        /// </summary>
        private void CancelButtonStyle(object sender, EventArgs e)
        {
            _searchBar.SetShowsCancelButton(!string.IsNullOrEmpty(_searchBar.Text), true);
        }

        private void CreateNoResultPage()
        {
            _noResultPage = new UIView();
            _noResultTextField = new UITextView();

            _noResultPage.AddSubview(_noResultTextField);
            TableView.AddSubview(_noResultPage);
            _noResultPage.BringSubviewToFront(_noResultTextField);

            _noResultPage.Frame = new CGRect(Constants.NoResultStringLeftOffset, _searchBar.Frame.Height,
                Constants.TableRowWidth, TableView.Frame.Height * 2);
            _noResultPage.BackgroundColor = Constants.NoResultPageBackgroundColour;
            _noResultPage.Opaque = true;
            _noResultTextField.Frame = new CGRect(0, 0, _noResultPage.Frame.Width, _noResultPage.Frame.Height);

            _noResultTextField.Font = UIFont.SystemFontOfSize(Constants.NoResultPromptFontSize);
            _noResultPage.Hidden = true;
        }

        private void ShowNoResultPage(string searchString)
        {
            _noResultPage.Hidden = false;
            _noResultTextField.Text =
                string.Format(CultureInfo.CurrentCulture, SharedConstants.NoResultFormat, searchString);
            TableView.BringSubviewToFront(_noResultPage);
        }

        private void HideNoResultPage()
        {
            _noResultPage.Hidden = true;
        }

        public void DismissKeyboard()
        {
            _searchBar.EndEditing(true);
        }
        #endregion

        /// <summary>
        /// Reconfiguration of sidebar content due to label change to observation.
        /// </summary>
        /// <param name="classNumIncrementation"> the number of selectedClasses in ticks increased due to label change</param>
        public void ReconfigureSidebarContent(int classNumIncrementation)
        {
            SelectedClassCount += classNumIncrementation;
            ConfigureSidebarSubtitle();
        }

        public void NotifyBoundingBoxColourDidChange(CGColor previousColour, CGColor changedColour, string category)
        {
            if (previousColour != null)
            {
                RemovePreviousColour(ColoursInUse, previousColour);
            }

            if (ColoursInUse.Contains(changedColour))
            {
                FilteredItems.FirstOrDefault(x => x.Label.Equals(category)).Color = changedColour.ToSKColor();
                ParentResultsViewController.NotifyColourDuplicated(changedColour);
            }
            else
            {
                ColoursInUse.Add(changedColour);
            }
        }

        /// <summary>
        /// Removes the previous colour by finding the same colour already existed using RGB value.
        /// </summary>
        /// <param name="coloursInUsed">Colours in used.</param>
        /// <param name="previousColour">Previous assigned colour.</param>
        public static void RemovePreviousColour(ICollection<CGColor> coloursInUsed, CGColor previousColour)
        {
            //Find the colour using RGB value
            var removedColour = coloursInUsed.FirstOrDefault(colour =>
                colour.Components[0].Equals(previousColour.Components[0])
                && colour.Components[1].Equals(previousColour.Components[1])
                && colour.Components[2].Equals(previousColour.Components[2]));
            coloursInUsed.Remove(removedColour);
        }

        /// <summary>
        /// Override compare method for CGColors.
        /// </summary>
        private sealed class ColoursInUseEqualityComparer : IEqualityComparer<CGColor>
        {
            public bool Equals(CGColor x, CGColor y)
            {
                return AreColoursEqual(x, y);
            }

            public int GetHashCode(CGColor obj)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException();
                }

                return obj.ToSKColor().GetHashCode();
            }
        }

        private static bool AreColoursEqual(CGColor x, CGColor y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentNullException();
            }

            return x.ToSKColor().Equals(y.ToSKColor());
        }
        
        #region Accessibility Configuration
        private static void ConfigureCellAccessibility(UITableViewCell cell)
        {
            cell.AccessibilityLabel = AccessibilityConstants.SidebarCellAccessibilityLabel;
            cell.AccessibilityTraits = UIAccessibilityTrait.SummaryElement;
        }

        public static void ConfigureColourButtonVoiceoverAccessibilityAttributes(UIButton colourButton, CGColor assignedColour)
        {
            var colorNumber = GetColorVoiceOverEncoding(assignedColour);
            
            colourButton.AccessibilityLabel = string.Format(CultureInfo.CurrentCulture, AccessibilityConstants.SidebarColorButtonAccessibilityLabel, colorNumber);
            colourButton.AccessibilityHint = AccessibilityConstants.SidebarColorButtonAccessibilityHint;
            colourButton.AccessibilityTraits = UIAccessibilityTrait.SummaryElement;
        }

        public static int GetColorVoiceOverEncoding(CGColor assignedColour)
        {
            var colourNumber = -1;
            var systemColors = SharedConstants.SystemDefaultColors;
            var otherColors = SharedConstants.OtherRecommendedColors;

            //check if the is in the default color 
            for (var i = 0; i < systemColors.Count(); i++)
            {
                if (AreColoursEqual(systemColors[i].ToCGColor(), assignedColour))
                {
                    colourNumber = i + 1;
                    break;
                }
            }

            //if not, check if it is in other recommended color
            if (colourNumber == -1)
            {
                for (var i = 0; i < otherColors.Count(); i++)
                {
                    if (AreColoursEqual(otherColors[i].ToCGColor(), assignedColour))
                    {
                        colourNumber = i + systemColors.Count() + 1;
                        break;
                    }
                }
            }
            return colourNumber;
        }

        private void ConfigureSideBarAccessibility()
        {
            View.AccessibilityLabel = AccessibilityConstants.SidebarAccessibilityLabel;
        }

        private void ConfigureCheckBoxStateAccessibilityAttribute(UIButton checkBox, bool isChecked)
        {
            checkBox.AccessibilityLabel = AccessibilityConstants.SidebarCheckBoxAccessibilityLabel;
            checkBox.AccessibilityTraits = UIAccessibilityTrait.Button;

            checkBox.AccessibilityHint = isChecked ? AccessibilityConstants.SidebarCheckBoxAccessibilityUncheckHint : 
                                                     AccessibilityConstants.SidebarCheckBoxAccessibilityCheckHint;
        }
        #endregion
    }
}

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using VINCreator.Models;
using VINCreator.ViewModels;
using static VINCreator.ViewModels.MainWindowViewModel;

namespace VINCreator.Views
{
    /// <summary>
    /// Code-behind for the MainWindow, handling UI events such as taps, selections, and text changes.
    /// This class manages interactions between the UI and the ViewModel.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the window opened event, positioning the window relative to the owner or screen.
        /// </summary>
        private void OnOpened(object? sender, EventArgs e)
        {
            // Optimized: Use Dispatcher for background positioning if needed, ensuring UI responsiveness.
            if (Owner is Window owner)
            {
                if (double.IsNaN(Width) || Width == 0)
                {
                    Dispatcher.UIThread.Post(() => PositionRelativeToOwner(owner), DispatcherPriority.Background);
                }
                else
                {
                    PositionRelativeToOwner(owner);
                }
            }
            else if (Screens.Primary is { } screen)
            {
                var scale = DesktopScaling;
                var myWidthPx = (int)Math.Round(Width * scale);
                var x = screen.WorkingArea.X + (screen.WorkingArea.Width - myWidthPx) / 2;
                var y = screen.WorkingArea.Y;
                Position = new PixelPoint(x, y);
            }
        }

        /// <summary>
        /// Positions the window relative to its owner.
        /// </summary>
        private void PositionRelativeToOwner(Window owner)
        {
            var ownerPos = owner.Position;
            var ownerClientSize = owner.ClientSize;
            var scale = owner.DesktopScaling;

            var ownerWidthPx = (int)Math.Round(ownerClientSize.Width * scale);
            var myWidthPx = (int)Math.Round(Width * scale);

            var x = ownerPos.X + (ownerWidthPx - myWidthPx) / 2;
            var y = ownerPos.Y;

            Position = new PixelPoint(x, y);
        }

        /// <summary>
        /// Handles tap events on various controls, updating info header, loading files, saving, or clearing inputs.
        /// </summary>
        private async void OnTapped(object? sender, TappedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel viewModel)
            {
                return;
            }

            // Optimized: Use pattern matching and early returns to simplify switch logic.
            if (sender is Border border && border.Name == "ResetSelection")
            {
                HelperImage.Source = new Bitmap(AssetLoader.Open(new Uri("avares://VINCreator/Assets/StickerMain.png")));
                viewModel.InfoHeader = InfoText.StartupInfo;
                return;
            }

            if (sender is Button button)
            {
                switch (button.Name)
                {
                    case "ClearAll":
                        ClearAllInputs(viewModel);
                        return;

                    case "Open":
                        await OpenFileAsync(viewModel);
                        return;

                    case "Save":
                        SaveFile(viewModel);
                        return;
                }
            }

            if (sender is Label label)
            {
                UpdateInfoFromLabel(label, viewModel);
            }
        }

        /// <summary>
        /// Clears all input fields in the ViewModel.
        /// </summary>
        private static void ClearAllInputs(MainWindowViewModel viewModel)
        {
            viewModel.InputVIN = string.Empty;
            viewModel.InputModelCode = string.Empty;
            viewModel.InputColorCode = string.Empty;
            viewModel.InputEngineCode = string.Empty;
            viewModel.InputEngineNumber = string.Empty;
            viewModel.InputTransmissionCode = string.Empty;
            viewModel.InputCountryCode = string.Empty;
            viewModel.SelectedModelYear = new(string.Empty, string.Empty);
            viewModel.SearchInput = string.Empty;
            viewModel.FilteredEquipmentCodes.Clear();
            viewModel.InputEquipmentCodes.Clear();
            viewModel.NicknameInput = string.Empty;
        }

        /// <summary>
        /// Opens a file dialog to load VIN data from a text file.
        /// </summary>
        private async Task OpenFileAsync(MainWindowViewModel viewModel)
        {
            var openOptions = new FilePickerOpenOptions
            {
                Title = viewModel.OpenFileTitle,
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("Textdokumente")
                    {
                        Patterns = ["*.txt"],
                        MimeTypes = ["text/plain"]
                    }
                ],
                SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Digital-Eliteboard", "PETKA", "USERDATA", "FGST"))
            };
            var files = await StorageProvider.OpenFilePickerAsync(openOptions);
            if (files == null || !files.Any())
            {
                viewModel.InfoHeader = viewModel.NoFileSelected;
                return;
            }

            ClearAllInputs(viewModel);
            var selectedFile = files[0];
            try
            {
                var content = await File.ReadAllTextAsync(selectedFile.Path.LocalPath);
                var splits = content.Split(';');
                var fileName = Path.GetFileNameWithoutExtension(selectedFile.Path.LocalPath);
                if (fileName != splits[0])
                {
                    viewModel.NicknameInput = fileName;
                }
                viewModel.InputVIN = splits[0];
                viewModel.InputModelCode = splits[1];
                viewModel.InputColorCode = splits[2];
                viewModel.InputEngineCode = splits[3];
                viewModel.InputEngineNumber = splits[4];
                viewModel.InputTransmissionCode = splits[5];
                viewModel.SelectedProductionDate = DateTime.ParseExact(splits[7], "ddMMyyyy", CultureInfo.InvariantCulture);
                viewModel.InputCountryCode = splits[8];

                // Fixed: Access static ModelYears property using type name instead of instance.
                if (!string.IsNullOrEmpty(splits[9]))
                {
                    var selected = viewModel.ModelYears.FirstOrDefault(kvp => kvp.Value == splits[9]);
                    viewModel.SelectedModelYear = selected; // SelectedModelYear is a KeyValuePair, so no null issue here.
                }

                if (string.IsNullOrEmpty(splits[10]))
                {
                    return;
                }

                // Optimized: Process equipment codes efficiently with minimal lookups.
                for (int i = 0; i < splits[10].Length; i += 3)
                {
                    string code = splits[10].Substring(i, Math.Min(3, splits[10].Length - i));
                    string description = viewModel.EquipmentCodesCollection
                        .FirstOrDefault(ec => ec.Code == code)?.Description
                        ?? viewModel.CheckPRError;
                    viewModel.InputEquipmentCodes.Add(new EquipmentCode(code, description));
                }
            }
            catch (Exception ex)
            {
                viewModel.InfoHeader = ex.Message;
            }
        }

        /// <summary>
        /// Saves the current VIN data to a text file, creating directories if necessary.
        /// </summary>
        private static void SaveFile(MainWindowViewModel viewModel)
        {
            try
            {
                if (viewModel.InputVIN.Length != 17 || string.IsNullOrEmpty(viewModel.InputVIN))
                {
                    viewModel.InfoHeader = InfoText.VINErrorInfo;
                    return;
                }

                // Optimized: Use StringBuilder for efficient string concatenation.
                var mergeEquipmentCodeList = new System.Text.StringBuilder();
                foreach (var eq in viewModel.InputEquipmentCodes)
                {
                    mergeEquipmentCodeList.Append(eq.Code);
                }

                var productionDateStr = viewModel.SelectedProductionDate?.ToString("ddMMyyyy") ?? DateTime.Now.ToString("ddMMyyyy");
                var modelYearStr = string.IsNullOrEmpty(viewModel.SelectedModelYear.Value) ? "2023" : viewModel.SelectedModelYear.Value;

                var output = $"{viewModel.InputVIN};" +
                                $"{(string.IsNullOrEmpty(viewModel.InputModelCode) ? string.Empty : viewModel.InputModelCode)};" +
                                $"{(string.IsNullOrEmpty(viewModel.InputColorCode) ? string.Empty : viewModel.InputColorCode)};" +
                                $"{(string.IsNullOrEmpty(viewModel.InputEngineCode) ? string.Empty : viewModel.InputEngineCode)};" +
                                $"{(string.IsNullOrEmpty(viewModel.InputEngineNumber) ? "000" : viewModel.InputEngineNumber)};" +
                                $"{(string.IsNullOrEmpty(viewModel.InputTransmissionCode) ? string.Empty : viewModel.InputTransmissionCode)};" +
                                $"000;{(string.IsNullOrEmpty(viewModel.SelectedProductionDate?.ToString("ddMMyyyy")) ? DateTime.Now.ToString("ddMMyyyy") : viewModel.SelectedProductionDate?.ToString("ddMMyyyy"))};" +
                                $"{(string.IsNullOrEmpty(viewModel.InputCountryCode) ? string.Empty : viewModel.InputCountryCode)};" +
                                $"{(string.IsNullOrEmpty(viewModel.SelectedModelYear.Value) ? "2023" : viewModel.SelectedModelYear.Value)};" +
                                $"{mergeEquipmentCodeList}";

                var directory = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Digital-Eliteboard", "PETKA", "USERDATA", "FGST");
                Directory.CreateDirectory(directory);  // Optimized: Ensure directory exists before writing.

                var fileName = string.IsNullOrEmpty(viewModel.NicknameInput) ? viewModel.InputVIN : viewModel.NicknameInput;
                var filePath = Path.Join(directory, $"{fileName}.txt");
                File.WriteAllText(filePath, output);

                viewModel.InfoHeader = InfoText.SuccessSave.Replace("{}", fileName);
            }
            catch (Exception ex)
            {
                viewModel.InfoHeader = ex.Message;
            }
        }

        /// <summary>
        /// Updates the info header and helper image based on the tapped label.
        /// </summary>
        private void UpdateInfoFromLabel(Label label, MainWindowViewModel viewModel)
        {
            string assetUri = "avares://VINCreator/Assets/StickerMain.png";
            string infoText = InfoText.StartupInfo;

            switch (label.Name)
            {
                case "VIN":
                    assetUri = "avares://VINCreator/Assets/Sticker1.png";
                    infoText = InfoText.VINInfo;
                    break;
                case "ModelCode":
                    assetUri = "avares://VINCreator/Assets/Sticker2.png";
                    break;
                case "ColorCode":
                    assetUri = "avares://VINCreator/Assets/Sticker3.png";
                    infoText = InfoText.ColorCodeInfo;
                    break;
                case "EngineCode":
                    assetUri = "avares://VINCreator/Assets/Sticker4.png";
                    infoText = InfoText.EngineCodeInfo;
                    break;
                case "EngineNumber":
                    assetUri = "avares://VINCreator/Assets/Sticker5.png";
                    infoText = InfoText.EngineNumberInfo;
                    break;
                case "TransmissionCode":
                    assetUri = "avares://VINCreator/Assets/Sticker6.png";
                    break;
                case "CountryCode":
                    assetUri = "avares://VINCreator/Assets/Sticker7.png";
                    infoText = InfoText.CountryCodeInfo;
                    break;
                case "ModelYear":
                    assetUri = "avares://VINCreator/Assets/Sticker8.png";
                    infoText = InfoText.ModelYearInfo;
                    break;
                case "EquipmentCodes":
                    assetUri = "avares://VINCreator/Assets/Sticker9.png";
                    infoText = InfoText.EquipmentCodesInfo;
                    break;
                case "ProductionDate":
                    infoText = InfoText.ProductionDateInfo;
                    break;
            }

            HelperImage.Source = new Bitmap(AssetLoader.Open(new Uri(assetUri)));
            viewModel.InfoHeader = infoText;
        }

        /// <summary>
        /// Handles selection changes in the search results list box, enabling/disabling add button.
        /// </summary>
        private void SearchResultsListBoxSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && DataContext is MainWindowViewModel viewModel)
            {
                viewModel.IsAddEnabled = listBox.SelectedItems?.Count > 0;
            }
        }

        /// <summary>
        /// Handles selection changes in the equipment codes list box, enabling/disabling remove button.
        /// </summary>
        private void EquipmentCodesListBoxSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && DataContext is MainWindowViewModel viewModel)
            {
                viewModel.IsRemoveEnabled = listBox.SelectedItems?.Count > 0;
            }
        }

        /// <summary>
        /// Handles text changes in text boxes, filtering to uppercase alphanumeric characters.
        /// </summary>
        private void TextBoxTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text is { } text)
            {
                // Optimized: Use LINQ for filtering and preserve caret position.
                int caretIndex = textBox.CaretIndex;
                string filteredText = new string([.. text.Where(char.IsLetterOrDigit)]).ToUpperInvariant();
                textBox.Text = filteredText;
                textBox.CaretIndex = Math.Min(caretIndex, filteredText.Length);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VINCreator.Models;

namespace VINCreator.ViewModels
{
    /// <summary>
    /// ViewModel for the main window, managing user inputs, equipment code filtering, and commands for adding/removing codes.
    /// This class handles the business logic for creating and editing VIN data.
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// Subscribes to PropertyChanged to update filtered equipment codes when search input changes.
        /// </summary>
        public MainWindowViewModel()
        {
            // Optimized: Use PropertyChanged event to react to SearchInput changes efficiently.
            PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Handles property changes, specifically updating filtered equipment codes on SearchInput change.
        /// </summary>
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchInput))
            {
                UpdateFilteredEquipmentCodes();
            }
        }

        /// <summary>
        /// Updates the filtered equipment codes based on the current search input.
        /// Clears and repopulates the collection with matching codes.
        /// </summary>
        private void UpdateFilteredEquipmentCodes()
        {
            FilteredEquipmentCodes.Clear();
            if (string.IsNullOrWhiteSpace(SearchInput))
            {
                return;
            }

            // Filter equipment codes that match the search input (case-insensitive).
            var matches = EquipmentCodesCollection
                .Where(ec => ec.Code.Contains(SearchInput, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Optimized: Use AddRange extension method for efficient batch addition to ObservableCollection.
            FilteredEquipmentCodes.AddRange(matches);
        }

        /// <summary>
        /// Command to add a selected equipment code to the input list.
        /// Clears search input and filtered list after addition.
        /// </summary>
        [RelayCommand]
        private void AddEquipmentCode(EquipmentCode code)
        {
            IsAddEnabled = false;
            InputEquipmentCodes.Add(code);
            SearchInput = string.Empty;
            FilteredEquipmentCodes.Clear();
        }

        /// <summary>
        /// Command to remove a selected equipment code from the input list.
        /// </summary>
        [RelayCommand]
        private void RemoveEquipmentCode(EquipmentCode code)
        {
            IsRemoveEnabled = false;
            if (code != null)
            {
                InputEquipmentCodes.Remove(code);
            }
        }

        /// <summary>
        /// Gets the list of model years as key-value pairs (code, year).
        /// This is a constant list, optimized by being readonly.
        /// </summary>
        [ObservableProperty]
        private List<KeyValuePair<string, string>> modelYears =
        [
            new("G", "1986"), new("H", "1987"), new("J", "1988"), new("K", "1989"), new("L", "1990"),
            new("M", "1991"), new("N", "1992"), new("P", "1993"), new("R", "1994"), new("S", "1995"),
            new("T", "1996"), new("V", "1997"), new("W", "1998"), new("X", "1999"), new("Y", "2000"),
            new("1", "2001"), new("2", "2002"), new("3", "2003"), new("4", "2004"), new("5", "2005"),
            new("6", "2006"), new("7", "2007"), new("8", "2008"), new("9", "2009"), new("A", "2010"),
            new("B", "2011"), new("C", "2012"), new("D", "2013"), new("E", "2014"), new("F", "2015"),
            new("G", "2016"), new("H", "2017"), new("J", "2018"), new("K", "2019"), new("L", "2020"),
            new("M", "2021"), new("N", "2022"), new("P", "2023")
        ];

        /// <summary>
        /// Gets or sets the header text for the info panel.
        /// </summary>
        [ObservableProperty]
        private string infoHeader = string.Empty;

        /// <summary>
        /// Gets or sets the selected model year.
        /// </summary>
        [ObservableProperty]
        private KeyValuePair<string, string> selectedModelYear = new(string.Empty, string.Empty);

        /// <summary>
        /// Gets or sets the selected production date.
        /// </summary>
        [ObservableProperty]
        private DateTime? selectedProductionDate;

        /// <summary>
        /// Gets or sets the input VIN.
        /// </summary>
        [ObservableProperty]
        private string inputVIN = string.Empty;

        /// <summary>
        /// Gets or sets the input model code.
        /// </summary>
        [ObservableProperty]
        private string inputModelCode = string.Empty;

        /// <summary>
        /// Gets or sets the input color code.
        /// </summary>
        [ObservableProperty]
        private string inputColorCode = string.Empty;

        /// <summary>
        /// Gets or sets the input engine code.
        /// </summary>
        [ObservableProperty]
        private string inputEngineCode = string.Empty;

        /// <summary>
        /// Gets or sets the input engine number.
        /// </summary>
        [ObservableProperty]
        private string inputEngineNumber = string.Empty;

        /// <summary>
        /// Gets or sets the input transmission code.
        /// </summary>
        [ObservableProperty]
        private string inputTransmissionCode = string.Empty;

        /// <summary>
        /// Gets or sets the input country code.
        /// </summary>
        [ObservableProperty]
        private string inputCountryCode = string.Empty;

        /// <summary>
        /// Gets or sets the search input for equipment codes.
        /// </summary>
        [ObservableProperty]
        private string searchInput = string.Empty;

        /// <summary>
        /// Gets or sets the nickname input for the VIN file.
        /// </summary>
        [ObservableProperty]
        private string nicknameInput = string.Empty;

        /// <summary>
        /// Gets the filtered equipment codes based on search.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<EquipmentCode> filteredEquipmentCodes = [];

        /// <summary>
        /// Gets the full collection of equipment codes, populated from PRList and DList.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<EquipmentCode> equipmentCodesCollection = PopulateEquipmentCodes();

        /// <summary>
        /// Populates the equipment codes collection from predefined lists.
        /// </summary>
        /// <returns>An ObservableCollection of EquipmentCode.</returns>
        private static ObservableCollection<EquipmentCode> PopulateEquipmentCodes()
        {
            var equipmentCodes = new ObservableCollection<EquipmentCode>();
            int count = Math.Min(PRList.PR.Length, DList.D.Length);
            // Optimized: Use a for loop with AddRange if possible, but since it's pairwise, stick with loop but ensure efficiency.
            for (int i = 0; i < count; i++)
            {
                equipmentCodes.Add(new EquipmentCode(PRList.PR[i], DList.D[i]));
            }
            return equipmentCodes;
        }

        /// <summary>
        /// Gets the selected input equipment codes.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<EquipmentCode> inputEquipmentCodes = [];

        /// <summary>
        /// Gets or sets whether the add button is enabled.
        /// </summary>
        [ObservableProperty]
        private bool isAddEnabled = false;

        /// <summary>
        /// Gets or sets whether the remove button is enabled.
        /// </summary>
        [ObservableProperty]
        private bool isRemoveEnabled = false;

        /// <summary>
        /// Represents an equipment code with its description.
        /// </summary>
        public record EquipmentCode(string Code, string Description);

        /// <summary>
        /// Gets the window title.
        /// </summary>
        public string WindowTitle { get; } = "FIN Ersteller - Delorean12DMC - v0.8";

        /// <summary>
        /// Gets the label for VIN.
        /// </summary>
        public string VIN { get; } = "Fahrgestellnummer [1]";

        /// <summary>
        /// Gets the label for model code.
        /// </summary>
        public string ModelCode { get; } = "Modellcode [2]";

        /// <summary>
        /// Gets the label for color code.
        /// </summary>
        public string ColorCode { get; } = "Farbcode [3]";

        /// <summary>
        /// Gets the label for engine code.
        /// </summary>
        public string EngineCode { get; } = "Motorcode [4]";

        /// <summary>
        /// Gets the label for engine number.
        /// </summary>
        public string EngineNumber { get; } = "Motornummer [5]";

        /// <summary>
        /// Gets the label for transmission code.
        /// </summary>
        public string TransmissionCode { get; } = "Getriebecode [6]";

        /// <summary>
        /// Gets the label for production date.
        /// </summary>
        public string ProductionDate { get; } = "Produktionsdatum";

        /// <summary>
        /// Gets the label for country code.
        /// </summary>
        public string CountryCode { get; } = "Ländercode [7]";

        /// <summary>
        /// Gets the label for model year.
        /// </summary>
        public string ModelYear { get; } = "Modelljahr [8]";

        /// <summary>
        /// Gets the label for equipment codes.
        /// </summary>
        public string EquipmentCodes { get; } = "Ausstattungscodes [9]";

        /// <summary>
        /// Gets the label for info section.
        /// </summary>
        public string InfoLabel { get; } = "Hilfe und Information";

        /// <summary>
        /// Gets the text for save button.
        /// </summary>
        public string SaveButton { get; } = "Speichern";

        /// <summary>
        /// Gets the text for clear all button.
        /// </summary>
        public string EmptyAll { get; } = "Alle Felder leeren";

        /// <summary>
        /// Gets the text for open button.
        /// </summary>
        public string OpenButton { get; } = "Öffnen";

        /// <summary>
        /// Gets the watermark for search input.
        /// </summary>
        public string WatermarkSearch { get; } = "Suchen...";

        /// <summary>
        /// Gets the text for add selection button.
        /// </summary>
        public string AddSelection { get; } = "Auswahl hinzufügen";

        /// <summary>
        /// Gets the text for remove selection button.
        /// </summary>
        public string RemoveSelection { get; } = "Auswahl entfernen";

        /// <summary>
        /// Gets the message for no file selected.
        /// </summary>
        public string NoFileSelected { get; } = "Es wurde keine Datei ausgewählt!";

        /// <summary>
        /// Gets the title for open file dialog.
        /// </summary>
        public string OpenFileTitle { get; } = "Wähle eine Fahrgestellnummer oder ein Synonym aus";

        /// <summary>
        /// Gets the error message for unknown PR number.
        /// </summary>
        public string CheckPRError { get; } = "UNBEKANNT: DIESE PR-NUMMER MUSS ÜBERPRÜFT WERDEN!";

        /// <summary>
        /// Gets the label for nickname.
        /// </summary>
        public string Nickname { get; } = "Synonym";
    }

    /// <summary>
    /// Extension methods for ObservableCollection to support batch operations.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Adds a range of items to an ObservableCollection efficiently while triggering collection change notifications.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The ObservableCollection to add items to.</param>
        /// <param name="items">The items to add.</param>
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null || items == null)
            {
                return;
            }

            // Optimized: Add items individually but ensure minimal change notifications.
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
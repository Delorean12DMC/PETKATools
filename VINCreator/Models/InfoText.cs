namespace VINCreator.Models
{
    /// <summary>
    /// Provides static string constants for informational messages and tooltips used throughout the application.
    /// These strings are used to display help texts, error messages, and success notifications in the UI.
    /// </summary>
    internal class InfoText
    {
        /// <summary>
        /// Information about the startup process and how the offline VIN search works.
        /// </summary>
        public static string StartupInfo = "Damit die Offline FIN Suche funktioniert, muss das Fahrzeug vorher mit allen Details hier angelegt werden. Anhand diesen Spezifikationen filtert PETKA im Anschluss die passenden Ersatzteile heraus und graut nicht passende Ersatzteile aus.";

        /// <summary>
        /// Explanation on how to convert color codes using PETKA.
        /// </summary>
        public static string ColorCodeInfo = "Der Farbcode muss in PETKA über die V-Seiten von der Lacknummer in die Verkaufskennzeichnung umgewandelt werden. Am Beispielbild wird aus der Lacknummer Y9B die Verkaufskennzeichnung A2. Die Innenausstattung (rechts neben dem Schrägstrich) bleibt unverändert. Der richtige Wert für dieses Feld wäre in diesem Beispiel nun \"A2A2LA\".";

        /// <summary>
        /// Note on engine codes for newer vehicles.
        /// </summary>
        public static string EngineCodeInfo = "Kann bei neueren Fahrzeugen auch 4-stellig sein.";

        /// <summary>
        /// Information about the optional engine number.
        /// </summary>
        public static string EngineNumberInfo = "Nicht zwangsläufig vorhanden.";

        /// <summary>
        /// Location of the production date in vehicle documents.
        /// </summary>
        public static string ProductionDateInfo = "Das Datum befindet sich in der Zulassungsbescheinigung Teil II (auch bekannt als Fahrzeugschein) unter Punkt 6.";

        /// <summary>
        /// Format requirement for country codes.
        /// </summary>
        public static string CountryCodeInfo = "Der Ländercode beginnt immer mit X.";

        /// <summary>
        /// How to find the model year in the VIN.
        /// </summary>
        public static string ModelYearInfo = "Das Modelljahr befindet sich an der 10ten Stelle in der Fahrgestellnummer.";

        /// <summary>
        /// Instructions for searching and managing equipment codes.
        /// </summary>
        public static string EquipmentCodesInfo = "Nutze die Suche um PR-Codes aus der Datenbank zu suchen und im Anschluss auszuwählen. Nutze den 'Auswahl hinzufügen' Knopf um den Eintrag der Liste hinzuzufügen. Ein falscher Eintrag kann ausgewählt werden und dann mit dem 'Auswahl entfernen' Knopf wieder gelöscht werden.";

        /// <summary>
        /// Basic information about the VIN structure.
        /// </summary>
        public static string VINInfo = "Die Fahrgestellnummer besteht aus 17 Zeichen.";

        /// <summary>
        /// Error message for invalid VIN input.
        /// </summary>
        public static string VINErrorInfo = "Die Fahrgestellnummer muss zwingend eingetragen werden und muss aus 17 Zeichen bestehen!";

        /// <summary>
        /// Success message template for saving a VIN file.
        /// </summary>
        public static string SuccessSave = "Die FIN-Datei wurde erfolgreich unter dem Namen: '{}' gespeichert.";
    }
}

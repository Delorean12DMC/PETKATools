using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using VINCreator.Models;
using VINCreator.ViewModels;
using VINCreator.Views;

namespace VINCreator
{
    /// <summary>
    /// The main application class, handling initialization and framework setup.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the application by loading XAML.
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Called when framework initialization is completed, setting up the main window and disabling duplicate validation.
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Disable Avalonia's built-in data annotation validation to avoid duplicates with CommunityToolkit.
                DisableAvaloniaDataAnnotationValidation();

                var viewModel = new MainWindowViewModel
                {
                    InfoHeader = InfoText.StartupInfo
                };
                desktop.MainWindow = new MainWindow
                {
                    DataContext = viewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>
        /// Disables Avalonia's data annotation validation plugins.
        /// </summary>
        private static void DisableAvaloniaDataAnnotationValidation()
        {
            var dataValidationPluginsToRemove = BindingPlugins.DataValidators
                .OfType<DataAnnotationsValidationPlugin>()
                .ToArray();

            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}
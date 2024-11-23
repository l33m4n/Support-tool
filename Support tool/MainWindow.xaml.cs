using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Threading;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Documents;
using System.IO;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using System.Diagnostics;
using System.Windows.Input;

namespace Support_tool
{
    public partial class MainWindow : Window
    {
        private System.Timers.Timer clipboardClearTimer;
        private DispatcherTimer feedbackTimer;  //Create a different timer for copyFeedback, to create a visual indicator 
        private DispatcherTimer progressBarTimer;
        private DispatcherTimer buttonLockTimer; //Lock button timer to prevent copying error while clipboard copy task is pending 
        int pcCounter = 0; // Count the amount of TV creds we are storing to quikcly reference back 
        private int progressBarTimeLeft = 15000; // 15 seconds in milliseconds
        private bool isProgressBarRunning = false; // To check if progress bar is already running
        private bool clipboardClearScheduled = false; // To track if clipboard clear is scheduled
        private ThemeManager _themeManager; // Instance of ThemeManager
        private SettingsManager settingsManager; // Load instance of settingsmanager
        private string settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CopyPastaSettings", "CopypastaSettings.txt"); // FIle path of settings file 


        public MainWindow()
        {
            InitializeComponent();

            _themeManager = new ThemeManager(); // Initialize the ThemeManager

            settingsManager = new SettingsManager();
             ApplySettings(); 
             Console.WriteLine($"Selected Theme: {settingsManager.SelectedTheme}");
        }

        //BEGIN CUSTOM TITLEBAR CODEBLOCK HERE
        // Allows dragging the window by clicking on the title bar
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        // Minimize window
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Maximize or restore window
        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        // Close window
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //END CUSTOM TITLEBAR CODEBLOCK HERE

        private void ApplySettings()
        {
            // Apply dark mode
            darkModeCheckbox.IsChecked = settingsManager.IsDarkMode;

            // Apply always on top
            this.Topmost = settingsManager.IsAlwaysOnTop;
            topmostCheckbox.IsChecked = settingsManager.IsAlwaysOnTop;

            // Apply theme
            foreach (var item in themeSelector.Items)
            {
                if (item is ComboBoxItem comboBoxItem && comboBoxItem.Content.ToString() == settingsManager.SelectedTheme)
                {
                    themeSelector.SelectedItem = comboBoxItem;
                    break; // Exit once the correct item is found
                }
            }

            // Optional: if the theme is not found, set a default
            if (themeSelector.SelectedItem == null)
            {
                themeSelector.SelectedItem = themeSelector.Items[0]; // Default to the first item or specify
            }
        }


        private void SaveSettings()
        {
            settingsManager.IsDarkMode = darkModeCheckbox.IsChecked == true;
            settingsManager.IsAlwaysOnTop = topmostCheckbox.IsChecked == true;

            // Check if the user has selected a theme, if not use a default theme
            if (themeSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                settingsManager.SelectedTheme = selectedItem.Content.ToString(); // Get actual theme value
            }
            else
            {
                settingsManager.SelectedTheme = "Purple"; // Default theme if none is selected
            }

            // Write the settings to the file, with variable prefixes
            var settings = $"IsDarkMode={settingsManager.IsDarkMode}\n" +
                           $"IsAlwaysOnTop={settingsManager.IsAlwaysOnTop}\n" +
                           $"SelectedTheme={settingsManager.SelectedTheme}";
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CopyPastaSettings"))) // if settings path does not exist, make it 
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CopyPastaSettings"));
            }
            File.WriteAllText(settingsFilePath, settings);
        }



        protected override void OnClosed(EventArgs e)
        {
            SaveSettings();
            base.OnClosed(e);
        }

        private void DarkModeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SetDarkMode(true);
        }

        private void DarkModeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetDarkMode(false);
        }

        private void SetDarkMode(bool isDarkMode)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(isDarkMode ? BaseTheme.Dark : BaseTheme.Light);
            paletteHelper.SetTheme(theme);
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            settingsCard.Visibility = (settingsCard.Visibility == Visibility.Collapsed)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void lockCredsButton_Click(object sender, RoutedEventArgs e)
        {
            if (dailyPwTextField.Text.Length > 0 && adminPwTextField.Text.Length > 0)
            {
                adminPwTextField.IsReadOnly = true;
                dailyPwTextField.IsReadOnly = true;
                MessageBox.Show("Passwords locked in, please restart to application to change the passwords."); //MessageBox shown when the credential boxes are locked.
            }
            else
            {
                MessageBox.Show("It appears that both textboxes contain a null value. Therefore, I refuse to lock them.", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error); //If the textfields are empty or null, do not allow locking the passwords.
            }
        }

        private void dailyPwButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCopyFeedback();
            CopyPassword(dailyPwTextField.Text);
            LockButtons(); // Lock the buttons for 0.250 seconds
        }

        private void adminPwButton_Click_1(object sender, RoutedEventArgs e)
        {
            ShowCopyFeedback();
            CopyPassword(adminPwTextField.Text);
            LockButtons(); // Lock the buttons for 0.250 seconds
        }

        private void ShowCopyFeedback()
        {
            copyFeedback.Visibility = Visibility.Visible;
            if (feedbackTimer == null)
            {
                feedbackTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(5)
                };
                feedbackTimer.Tick += (s, args) =>
                {
                    copyFeedback.Visibility = Visibility.Collapsed;
                    feedbackTimer.Stop();
                };
            }
            feedbackTimer.Start();
        }

        private void CopyPassword(string password)
        {
            // Copy the password to the clipboard
            CopyToClipboard(password);

            // If the progress bar is already running, we just copy the password
            // but don't reset the progress bar or schedule a new clipboard clear
            if (isProgressBarRunning)
            {
                return; // Don't start another progress bar or timer
            }

            // Start the progress bar and clipboard clear process if not already running
            isProgressBarRunning = true;
            StartProgressBar(); // Start progress bar

            if (!clipboardClearScheduled)
            {
                clipboardClearScheduled = true;
                StartClipboardClearTimer(); // Start the clipboard clear timer
            }
        }

        private void CopyToClipboard(string text)
        {
            try
            {
                Clipboard.SetText(text);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error copying to clipboard: {ex.Message}" + "\n\n  STOP SPAMMING THE BUTTONS. I AM NOT A WIZARD.", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error); //error handling for when clipboard service is innaccessible 
            }
        }

        private void StartProgressBar()
        {
            // Set progress bar to 100% at the start
            progressBar.Value = 100; // Start at 100
            progressBarTimeLeft = 15000; // Reset the timer for 15 seconds
            timeCounter.Content = (progressBarTimeLeft / 1000).ToString(); // Set initial timeCounter value

            // DispatcherTimer to update the progress bar
            progressBarTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150) // Update every 150 ms
            };

            progressBarTimer.Tick += (s, args) =>
            {
                if (progressBarTimeLeft > 0)
                {
                    progressBarTimeLeft -= 150; // Decrement the time left
                    progressBar.Value = 100 * (progressBarTimeLeft / 15000.0); // Decrease progress bar

                    // Update the timeCounter label with remaining seconds
                    timeCounter.Content = (progressBarTimeLeft / 1000).ToString(); // Convert ms to seconds
                }
                else
                {
                    // Reset the label to "0" when progress bar finishes
                    timeCounter.Content = "15";

                    // Clear the clipboard after the progress bar finishes
                    progressBar.Value = 100; // Reset progress bar to 100
                    progressBarTimer.Stop(); // Stop the progress bar timer
                    isProgressBarRunning = false; // Reset progress bar running state
                    clipboardClearScheduled = false; // Allow new clipboard clear on next copy
                }
            };

            progressBarTimer.Start();
        }


        private void StartClipboardClearTimer()
        {
            // Stop and dispose of the previous timer if it exists
            clipboardClearTimer?.Stop();
            clipboardClearTimer?.Dispose();

            clipboardClearTimer = new System.Timers.Timer(15000); // 15 seconds timer
            clipboardClearTimer.Elapsed += (s, args) =>
            {
                try
                {
                    Dispatcher.Invoke(() => Clipboard.SetText("     ")); // Clear clipboard
                }
                catch (System.Exception ex)
                {
                    Dispatcher.Invoke(() => MessageBox.Show($"Failed to clear clipboard: {ex.Message}", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error));
                }
                finally
                {
                    clipboardClearTimer.Stop();
                    clipboardClearTimer.Dispose();
                }
            };
            clipboardClearTimer.Start();
        }

        // Lock the buttons for 0.250 seconds after a copy action
        private void LockButtons()
        {
            // Disable both buttons
            dailyPwButton.IsEnabled = false;
            adminPwButton.IsEnabled = false;

            // Start a timer to re-enable the buttons after 0.250 seconds
            if (buttonLockTimer == null)
            {
                buttonLockTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(250) // 0.250 seconds
                };

                buttonLockTimer.Tick += (s, args) =>
                {
                    dailyPwButton.IsEnabled = true;  // Re-enable both buttons
                    adminPwButton.IsEnabled = true;
                    buttonLockTimer.Stop(); // Stop the timer
                };
            }

            buttonLockTimer.Start();
        }

        private void topmostCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            // Set the window to be topmost
            this.Topmost = true;
        }

        // Event handler for when the checkbox is unchecked
        private void topmostCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Set the window to not be topmost
            this.Topmost = false;
        }
        //Exit button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //Open notepad form button
        private void OpenNotepadButton_Click(object sender, RoutedEventArgs e)
        {
            NotepadWindow notepadWindow = new NotepadWindow();
            notepadWindow.Show();
        }

        private void themeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ensure that the themeSelector is not null
            if (themeSelector == null)
            {
                MessageBox.Show("Theme selector is null.");
                return; // Exit if themeSelector is not initialized
            }

            // Check if any item is selected
            if (themeSelector.SelectedItem == null)
            {
                MessageBox.Show("No item is selected in the theme selector.");
                return; // Exit if no item is selected
            }

            // Attempt to get the selected item
            if (themeSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                // Get the content of the selected ComboBoxItem to determine the selected theme
                string selectedTheme = selectedItem.Content.ToString();

                // Ensure _themeManager is initialized
                if (_themeManager == null)
                {
                    MessageBox.Show("Theme manager is null.");
                    return; // Exit if themeManager is not initialized
                }

                // Call SetTheme using the instance and the dark mode state
                bool isDarkMode = darkModeCheckbox.IsChecked == true; // Check if the dark mode checkbox is checked
                _themeManager.SetTheme(selectedTheme, isDarkMode);
            }
            else
            {
                MessageBox.Show("Selected item is not a ComboBoxItem.");
            }
        }

        private void moveDownTVcredsButton_Click(object sender, RoutedEventArgs e)
        {
            pcCounter++; 
            string tvCredsCounter = ("=======" + pcCounter + "======= \n" + "ID: " + tvIDtextbox.Text + "\n" + "PW: " + tvPWtextbox.Text + "\n");
            tvCredsTextBox.AppendText(tvCredsCounter);
        }


        //Clear tvCredsTextBox contents 
        private void ClearTextBoxContent()
        {
            var newDocument = new FlowDocument();

            // Add a new paragraph with the desired line spacing settings
            var paragraph = new Paragraph();
            paragraph.LineHeight = 1; // Use your desired line height value
            paragraph.Inlines.Add(new Run(string.Empty)); // Empty text but preserves structure

            newDocument.Blocks.Add(paragraph);

            // Replace the old document with the new empty one
            tvCredsTextBox.Document = newDocument;
        }


        private void resetCredsButton_Click(object sender, RoutedEventArgs e)
        {
            tvIDtextbox.Text = string.Empty;
            tvPWtextbox.Text = string.Empty;
            ClearTextBoxContent();
            pcCounter = 0;
        }

        private void placeholderButton1_Click(object sender, RoutedEventArgs e)
        {
            ShowCopyFeedback(); //This is the suggested code for a command button, this will flash a copy feedback indicator to the user to enforce that the data was copied.
        }

        private void placeholderButton2_Click(object sender, RoutedEventArgs e)
        {
            ShowCopyFeedback();
        }

        private void employeeHandbookButton_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://www.google.com"; //Change this URL to match either your employee handbook URL, or a static file. This method can be changed easily to reference a PDF instead of a URL.
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}

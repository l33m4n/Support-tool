using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace Support_tool
{
    public partial class NotepadWindow : Window
    {
        private System.Timers.Timer autosaveTimer;

        public NotepadWindow()
        {
            InitializeComponent();
            InitializeAutosaveTimer(); // Initialize the timer
            LoadContent(); // Load saved content on window initialization
        }

        //BEGIN TITLEBAR CODE BLOCK 
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove(); // Allows the window to be dragged by the title bar
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the window
        }
        // Minimize the window
        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Maximize or Restore the window
        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
        //END TITLEBAR CODEBLOCK


        private void InitializeAutosaveTimer()
        {
            autosaveTimer = new System.Timers.Timer(5000); // Set up the timer for 5 seconds
            autosaveTimer.Elapsed += AutosaveTimer_Elapsed; // Attach the elapsed event handler
            autosaveTimer.AutoReset = true; // Make the timer repeat
            autosaveTimer.Enabled = false; // Initially disable the timer
        }

        private void LoadContent()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Support tool auto saved notepad.txt");

            if (File.Exists(filePath))
            {
                // Load the content from the file
                string content = File.ReadAllText(filePath);

                // Clear existing content in the RichTextBox
                notepadRichTextbox.Document.Blocks.Clear();

                // Create a new Paragraph and add the content
                Paragraph paragraph = new Paragraph(new Run(content));
                notepadRichTextbox.Document.Blocks.Add(paragraph);
            }
        }

        private void AutosaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Use Dispatcher to ensure the checkbox is accessed on the UI thread
            Dispatcher.Invoke(() =>
            {
                if (autosaveCheckbox.IsChecked == true)
                {
                    SaveContent(); // Call the save method
                }
            });
        }

        private void SaveContent()
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Support tool auto saved notepad.txt");

                // Get the content from the RichTextBox
                TextRange textRange = new TextRange(notepadRichTextbox.Document.ContentStart, notepadRichTextbox.Document.ContentEnd);
                string content = textRange.Text; // Use TextRange to get the text

                // Write the content to the file on the UI thread
                Dispatcher.Invoke(() =>
                {
                    File.WriteAllText(filePath, content); // Write the content to the file
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error, show a message box, etc.)
                MessageBox.Show("Error saving the file: " + ex.Message);
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void topmostCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
        }

        private void topmostCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
        }

        private void autosaveCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            autosaveTimer.Start(); // Start the timer when checked
        }

        private void autosaveCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            autosaveTimer.Stop(); // Stop the timer when unchecked
        }

        private void backToMainFormButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(); //Indentify the mainform 
            if (mainWindow == null) // If null, MainWindow is closed or not opened yet
            {
                // Create and show a new instance of MainWindow
                mainWindow = new MainWindow();
                mainWindow.Show();
            } else
            {
                MessageBox.Show("Main form is already open! Go find it.");
            }

        }
    }
}

using System;
using System.Data;
using System.Data.Odbc;
using System.Windows;
using System.Windows.Controls;

namespace MiniERP
{
    /// <summary>
    /// Fenster zur Verwaltung von Lieferanten.
    /// Lieferanten anzeigen, hinzufügen und löschen.
    /// </summary>
    public partial class Lieferanten : Window
    {
        private OdbcConnection odbcConnection;
        private OdbcDataAdapter dataAdapter;
        private OdbcCommand odbcCommand;
        private DataSet dataSet;
        public Lieferanten()
        {
            InitializeComponent();

            // Verbindung initialisieren
            odbcConnection = new OdbcConnection();
            odbcConnection.ConnectionString = "DSN=dbdemo2";

            // Lieferanten und Personen anzeigen
            ShowLieferanten();
            ShowPersonen();
        }

        /// <summary>
        /// Zeigt alle Lieferanten in der ListBox an.
        /// </summary>
        private void ShowLieferanten()
        {
            string sql_Lieferanten = "SELECT * FROM lieferanten;";
            dataSet = new DataSet();
            try
            {
                odbcConnection.Open();
                dataAdapter = new OdbcDataAdapter(sql_Lieferanten, odbcConnection);
                dataAdapter.Fill(dataSet, "lieferantenTable");
                lbLieferanten.ItemsSource = dataSet.Tables["lieferantenTable"].DefaultView;
                lbLieferanten.DisplayMemberPath = "firma";
                lbLieferanten.SelectedValuePath = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType() + "\n\n" + ex.Message);
            }
            finally { odbcConnection.Close(); }
        }

        /// <summary>
        /// Zeigt alle Personen in der ComboBox zum Hinzufügen von Lieferanten an.
        /// </summary>
        private void ShowPersonen()
        {
            string sql_Personen = "SELECT * FROM personen ORDER BY nachname;";
            dataSet = new DataSet();
            try
            {
                odbcConnection.Open();
                dataAdapter = new OdbcDataAdapter(sql_Personen, odbcConnection);
                dataAdapter.Fill(dataSet, "personenTable");
                cbPersonen.ItemsSource = dataSet.Tables["personenTable"].DefaultView;
                cbPersonen.SelectedValuePath = "id";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType() + "\n\n" + ex.Message);
            }
            finally { odbcConnection.Close(); }
        }

        /// <summary>
        /// Fügt einen neuen Lieferanten hinzu.
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string sql_neuer_lieferant = "INSERT INTO lieferanten VALUES (NULL, ?, ?)";
            try
            {
                odbcConnection.Open();
                odbcCommand = new OdbcCommand(sql_neuer_lieferant, odbcConnection);
                odbcCommand.Parameters.AddWithValue("@personID", Int32.Parse(cbPersonen.SelectedValue.ToString()));
                odbcCommand.Parameters.AddWithValue("@firma", tbFirma.Text);
                odbcCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType() + "\n\n" + ex.Message);
            }
            finally
            {
                odbcConnection.Close();
                ShowLieferanten();
            }
        }

        /// <summary>
        /// Aktualisiert die ListBox, wenn eine Person in der ComboBox ausgewählt wird.
        /// </summary>

        private void cbPersonen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowLieferanten();
        }

        /// <summary>
        /// Löscht den ausgewählten Lieferanten aus der Datenbank.
        /// </summary>
        private void btnLoeschen_Click(object sender, RoutedEventArgs e)
        {

            if (lbLieferanten.SelectedValue == null)
            {
                MessageBox.Show("Keinen Eintrag zum Löschen ausgewählt");
            }
            else
            {
                try
                {
                    MessageBoxResult result = MessageBox.Show("Wollen Sie wirklich den Liferanten aus der Datenbank löschen?",
                    "Bitte bestätigen Sie den Löschvorgang", MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.Yes);
                    if (result == MessageBoxResult.Yes)
                    {
                        string deletestr = "DELETE FROM lieferanten WHERE id=?";
                        odbcConnection.Open();
                        odbcCommand = new OdbcCommand(deletestr, odbcConnection);
                        odbcCommand.Parameters.AddWithValue("@id", Int32.Parse(lbLieferanten.SelectedValue.ToString()));
                        odbcCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.GetType() + "\n\n" + ex.Message);
                }
                finally
                {
                    odbcConnection.Close();
                    ShowLieferanten();
                }
            }
        }
        private void btnZurueck_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

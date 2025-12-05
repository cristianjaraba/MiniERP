using System;
using System.Data;
using System.Data.Odbc;
using System.Windows;

namespace MiniERP
{
    /// <summary>
    /// Fenster zur Verwaltung von Artikeln.
    /// Anzeigen, Hinzufügen und Löschen von Artikeln.
    /// </summary>
    public partial class Artikel : Window
    {
        private OdbcConnection odbcConnection;
        private OdbcDataAdapter odbcDataAdapter;
        private OdbcCommand odbcCommand;
        private DataSet dataSet;
        public Artikel()
        {
            InitializeComponent();

            // Verbindung zur Datenbank initialisieren
            odbcConnection = new OdbcConnection();
            odbcConnection.ConnectionString = "DSN=dbdemo2";

            // Artikel in ListBox anzeigen
            ShowArtikel();
        }

        private void btnZurueck_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Ruft alle Artikel aus der Datenbank ab und zeigt sie in der ListBox an.
        /// </summary>
        private void ShowArtikel()
        {
            string sqlArtikel = "SELECT * FROM artikel;";
            dataSet = new DataSet();
            try
            {
                odbcConnection.Open();
                odbcDataAdapter = new OdbcDataAdapter(sqlArtikel, odbcConnection);
                odbcDataAdapter.Fill(dataSet, "artikelTable");
                lbArtikel.ItemsSource = dataSet.Tables["artikelTable"].DefaultView;
                lbArtikel.SelectedValuePath = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType() + "\n" + ex.Message);
            }
            finally { odbcConnection.Close(); }
        }

        /// <summary>
        /// Fügt einen neuen Artikel in die Datenbank ein.
        /// Werte werden aus den TextBoxen gelesen.
        /// </summary>
        private void btnHinzufuegen_Click(object sender, RoutedEventArgs e)
        {
            string sqlHinzufuegen = "INSERT INTO artikel VALUES (NULL, ?, ?, ?, ?, ?, ?)";
            try
            {
                odbcConnection.Open();
                odbcCommand = new OdbcCommand(sqlHinzufuegen, odbcConnection);

                // Parameter setzen
                odbcCommand.Parameters.AddWithValue("@name", tbName.Text);
                odbcCommand.Parameters.AddWithValue("@beschreibung", tbBeschreibung.Text);
                odbcCommand.Parameters.AddWithValue("@groesse", tbGroesse.Text);
                odbcCommand.Parameters.AddWithValue("@farbe", tbFarbe.Text);
                odbcCommand.Parameters.AddWithValue("@menge", Int32.Parse(tbMenge.Text.ToString()));
                odbcCommand.Parameters.AddWithValue("@preis", Int32.Parse(tbPreis.Text.ToString()));

                // SQL ausführen
                odbcCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType() + "\n" + ex.Message);
            }
            finally
            {
                odbcConnection.Close();
                ShowArtikel(); // ListBox aktualisieren
            }
        }

        /// <summary>
        /// Löscht den ausgewählten Artikel aus der Datenbank.
        /// </summary>
        private void btnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            if (lbArtikel.SelectedItem == null)
            {
                MessageBox.Show("\"Keinen Eintrag zum Löschen ausgewählt.");
            }
            else
            {
                try
                {
                    MessageBoxResult result = MessageBox.Show("Möchten Sie den Artikel aus der Datenbank löschen?",
                    "Bitte bestätigen Sie den Löschvorgang", MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.Yes);
                    if (result == MessageBoxResult.Yes)
                    {
                        string sqlLoeschen = "DELETE FROM artikel WHERE id=?";
                        odbcConnection.Open();
                        odbcCommand = new OdbcCommand(sqlLoeschen, odbcConnection);
                        odbcCommand.Parameters.AddWithValue("@id", Int32.Parse(lbArtikel.SelectedValue.ToString()));
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
                    ShowArtikel(); // ListBox aktualisieren
                }
            }
        }
    }
}

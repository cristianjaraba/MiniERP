using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.Odbc;
using System.Data;

namespace MiniERP
{
    /// <summary>
    /// Hauptfenster der Anwendung.
    /// Hier können Personen ausgewählt und deren Bestellungen angezeigt werden.
    /// Außerdem Navigation zu Artikel- und Lieferantenfenster.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Verbindung zur ODBC-Datenbank
        private OdbcConnection odbcConnection;
        private OdbcDataAdapter dataAdapter;
        private DataSet dataSet;
        public MainWindow()
        {
            InitializeComponent();

            // ODBC-Verbindung initialisieren
            odbcConnection = new OdbcConnection();
            odbcConnection.ConnectionString = "DSN=dbdemo2";

            // Alle Personen abrufen und ComboBox füllen
            PersonenAbrufen();

        }

        /// <summary>
        /// Ruft alle Personen ab, die Kunden sind, und zeigt sie in der ComboBox an.
        /// </summary>
        private void PersonenAbrufen()
        {
            string sqlPersonen = "SELECT * FROM personen WHERE personen.id IN (SELECT personID FROM kunden) ORDER BY personen.nachname;";
            dataSet = new DataSet();
            try
            {
                odbcConnection.Open();
                dataAdapter = new OdbcDataAdapter(sqlPersonen, odbcConnection);
                dataAdapter.Fill(dataSet, "personenTable");

                // ComboBox binden
                cbPersonen.ItemsSource = dataSet.Tables["personenTable"].DefaultView;
                cbPersonen.DisplayMemberPath = "nachname";
                cbPersonen.SelectedValuePath = "id";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType() + "\n" + ex.Message);
            }
            finally { odbcConnection.Close(); }

        }

        /// <summary>
        /// Zeigt die Bestellungen der ausgewählten Person in der ListBox an.
        /// </summary>
        private void bestellungenShow()
        {

            string sqlBestellungen = "SELECT k.id, p.vorname, p.nachname, b.datum, bp.menge, a.name, a.beschreibung FROM kunden k, bestellungen b, artikel a, bestellpositionen bp, personen p WHERE p.id= ? AND k.personID = p.id AND k.id = b.kundenID AND b.id = bp.bestellID AND bp.artikelID = a.id ORDER BY b.datum";
            dataSet = new DataSet();

            try
            {
                odbcConnection.Open();
                OdbcCommand odbcCommand = new OdbcCommand(sqlBestellungen, odbcConnection);
                OdbcDataAdapter odbcDataAdapter = new OdbcDataAdapter(odbcCommand);

                // Parameter für die Person setzen
                odbcCommand.Parameters.AddWithValue("@p.id", Int32.Parse(cbPersonen.SelectedValue.ToString()));
                odbcDataAdapter.Fill(dataSet, "tableBestellungen");

                // ListBox binden
                lbBestellungen.ItemsSource = dataSet.Tables["tableBestellungen"].DefaultView;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType() + "\n\n" + ex.Message);
            }
            finally { odbcConnection.Close(); }

        }

        /// <summary>
        /// Event: Wird ausgelöst, wenn die ausgewählte Person geändert wird.
        /// Zeigt die Bestellungen der neuen Person an.
        /// </summary>
        private void selectedItem_Changed(object sender, SelectionChangedEventArgs e)
        {
            bestellungenShow();
        }

        private void btnBeenden_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Öffnet das Lieferanten-Fenster.
        /// </summary>
        private void btnLieferanten_Click(object sender, RoutedEventArgs e)
        {
            Lieferanten lieferanten = new Lieferanten();
            lieferanten.Show();
        }

        /// <summary>
        /// Öffnet das Artikel-Fenster.
        /// </summary>
        private void btnArtikel_Click(object sender, RoutedEventArgs e)
        {
            Artikel artikel = new Artikel();
            artikel.Show();
        }
    }
}

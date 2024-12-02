using System.Data;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Microsoft.Data.SqlClient;

namespace Sovelluskehitys2024
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        string polku = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\k2200423\\Documents\\testitietokanta.mdf;Integrated Security=True;Connect Timeout=30";
        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Kirjastosovellus";
            this.MinHeight = 500;
            this.MinWidth = 800;

            ThemeManager.Current.ChangeTheme(this, "Light.Blue");

            try
            {
                PaivitaDataGrid("SELECT * FROM kirjat","kirjat", kirjalista);
                PaivitaDataGrid("SELECT * FROM asiakkaat", "asiakkaat", asiakaslista);
                PaivitaDataGrid("SELECT * FROM kopiot", "kopiot", kopiolista);
                PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='0'", "lainat", lainalista);
                PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='1'", "lainat", lainattulista);

                PaivitaKirjaComboBox(kirjalista_cb, kirjalista_cb_2);
                PaivitaAsiakasComboBox();
            }
            catch
            {
                tilaviesti.Text = "Ei tietokantayhteyttä";
            }
            
        }

        private void PaivitaDataGrid(string kysely, string taulu, DataGrid grid)
        {
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            SqlCommand komento = yhteys.CreateCommand();
            komento.CommandText = kysely;

            SqlDataAdapter adapteri = new SqlDataAdapter(komento);
            DataTable dt = new DataTable(taulu);
            adapteri.Fill(dt);

            grid.ItemsSource = dt.DefaultView;

            yhteys.Close();
        }
        private void PaivitaKirjaComboBox(ComboBox kombo1, ComboBox kombo2)
        {
            //tuotelista_cb.Items.Clear();

            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            SqlCommand komento = new SqlCommand("SELECT * FROM kirjat", yhteys);
            SqlDataReader lukija = komento.ExecuteReader();

            DataTable taulu = new DataTable();
            taulu.Columns.Add("ID", typeof(string));
            taulu.Columns.Add("NIMI", typeof(string));

            /* tehdään sidokset että comboboxissa näytetää datataulua*/
            kombo1.ItemsSource = taulu.DefaultView;
            kombo1.DisplayMemberPath = "NIMI";
            kombo1.SelectedValuePath = "ID";

            kombo2.ItemsSource = taulu.DefaultView;
            kombo2.DisplayMemberPath = "NIMI";
            kombo2.SelectedValuePath = "ID";

            while (lukija.Read()) // käsitellään kyselytulos rivi riviltä
            {
                int id = lukija.GetInt32(0);
                string nimi = lukija.GetString(1);
                taulu.Rows.Add(id, nimi); // lisätään datatauluun rivi tietoineen
                //tuotelista_cb.Items.Add(lukija.GetString(1));
            }
            lukija.Close();

            yhteys.Close();
        }
        private void PaivitaAsiakasComboBox()
        {
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            SqlCommand komento = new SqlCommand("SELECT * FROM asiakkaat", yhteys);
            SqlDataReader lukija = komento.ExecuteReader();

            DataTable taulu = new DataTable();
            taulu.Columns.Add("ID", typeof(string));
            taulu.Columns.Add("NIMI", typeof(string));

            asiakaslista_cb.ItemsSource = taulu.DefaultView;
            asiakaslista_cb.DisplayMemberPath = "NIMI";
            asiakaslista_cb.SelectedValuePath = "ID";


            while (lukija.Read())
            {
                int id = lukija.GetInt32(0);
                string nimi = lukija.GetString(1);
                taulu.Rows.Add(id, nimi);
                //tuotelista_cb.Items.Add(lukija.GetString(1));

            }
            lukija.Close();

            yhteys.Close();
        }
        private void UusiKirjaButton(object sender, RoutedEventArgs e)
        {
            using (SqlConnection yhteys = new SqlConnection(polku))
            {
                yhteys.Open();

                string kysely = "INSERT INTO kirjat (nimi, vuosi, tekija) OUTPUT INSERTED.id VALUES (@nimi, @vuosi, @tekija)";
                SqlCommand komento = new SqlCommand(kysely, yhteys);
                komento.Parameters.AddWithValue("@nimi", kirjanimi.Text);
                komento.Parameters.AddWithValue("@vuosi", kirjavuosi.Text);
                komento.Parameters.AddWithValue("@tekija", kirjatekija.Text);

                int kirja_id = (int)komento.ExecuteScalar();

                string kysely2 = "INSERT INTO kopiot (kirja_id, maara) VALUES (@kirja_id, 3)";
                SqlCommand komento2 = new SqlCommand(kysely2, yhteys);
                komento2.Parameters.AddWithValue("@kirja_id", kirja_id);
                komento2.ExecuteNonQuery();

                yhteys.Close();

                PaivitaDataGrid("SELECT * FROM kirjat", "kirjat", kirjalista);
                PaivitaDataGrid("SELECT * FROM kopiot", "kopiot", kopiolista);
                PaivitaKirjaComboBox(kirjalista_cb, kirjalista_cb_2);
            }
        }
        private void PoistaKirjaButton(object sender, RoutedEventArgs e)
        {
            using (SqlConnection yhteys = new SqlConnection(polku))
            {
                yhteys.Open();

                string id = kirjalista_cb.SelectedValue.ToString();

                // Delete from kopiot table first
                string kysely1 = "DELETE FROM kopiot WHERE kirja_id=@kirja_id;";
                SqlCommand komento1 = new SqlCommand(kysely1, yhteys);
                komento1.Parameters.AddWithValue("@kirja_id", id);
                komento1.ExecuteNonQuery();

                // Delete from kirjat table
                string kysely2 = "DELETE FROM kirjat WHERE id=@id;";
                SqlCommand komento2 = new SqlCommand(kysely2, yhteys);
                komento2.Parameters.AddWithValue("@id", id);
                komento2.ExecuteNonQuery();

                yhteys.Close();

                PaivitaDataGrid("SELECT * FROM kirjat", "kirjat", kirjalista);
                PaivitaKirjaComboBox(kirjalista_cb, kirjalista_cb_2);
            }
        }

        private void LisaaAsiakasButton(object sender, RoutedEventArgs e)
        {
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            string kysely = "INSERT INTO asiakkaat (nimi, osoite, puhelin) VALUES ('" + asiakasnimi.Text + "','" + asiakasosoite.Text + "','" + asiakaspuhelin.Text + "')";
            SqlCommand komento = new SqlCommand(kysely, yhteys);
            komento.ExecuteNonQuery();
            yhteys.Close();

            PaivitaDataGrid("SELECT * FROM asiakkaat", "asiakkaat", asiakaslista);
            PaivitaAsiakasComboBox();
        }

        private void LisaaLainaButton(object sender, RoutedEventArgs e)
        {
            using (SqlConnection yhteys = new SqlConnection(polku))
            {
                yhteys.Open();

                string asiakasID = asiakaslista_cb.SelectedValue.ToString();
                string kirja_nimi = kirjalista_cb_2.Text; // Use Text to get the displayed name

                // Query to get the kopio_id from the kopiot table using the kirja_nimi
                string kopioQuery = @"
                    SELECT TOP 1 ko.id 
                    FROM kopiot ko
                    INNER JOIN kirjat k ON ko.kirja_id = k.id
                    WHERE k.nimi = @kirja_nimi";
                SqlCommand kopioCommand = new SqlCommand(kopioQuery, yhteys);
                kopioCommand.Parameters.AddWithValue("@kirja_nimi", kirja_nimi);

                object result = kopioCommand.ExecuteScalar();
                if (result != null)
                {
                    int kopioID = (int)result;

                    // Insert into lainat table using the retrieved kopio_id
                    string sql = "INSERT INTO lainat (asiakas_id, kopio_id, haettu) VALUES (@asiakas_id, @kopio_id, 0)";
                    SqlCommand komento = new SqlCommand(sql, yhteys);
                    komento.Parameters.AddWithValue("@asiakas_id", asiakasID);
                    komento.Parameters.AddWithValue("@kopio_id", kopioID);
                    komento.ExecuteNonQuery();

                    yhteys.Close();

                    PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='0'", "lainat", lainalista);
                }
                else
                {
                    MessageBox.Show("No matching record found for the given book name.");
                }
            }
        }
        private void laina_haettu_click(object sender, RoutedEventArgs e)
        {
            DataRowView rivinäkymä = (DataRowView)((Button)e.Source).DataContext;
            String tilaus_id = rivinäkymä[0].ToString();

            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            string sql = "UPDATE lainat SET haettu=1 WHERE id='" + tilaus_id + "';";

            SqlCommand komento = new SqlCommand(sql, yhteys);
            komento.ExecuteNonQuery();

            yhteys.Close();

            PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='0'", "lainat", lainalista);
            PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='1'", "lainat", lainattulista);

        }
        private void laina_palautettu_click(object sender, RoutedEventArgs e)
        {
            DataRowView rivinäkymä = (DataRowView)((Button)e.Source).DataContext;
            String tilaus_id = rivinäkymä[0].ToString();

            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            string sql = "DELETE FROM lainat WHERE id='" + tilaus_id + "';";

            SqlCommand komento = new SqlCommand(sql, yhteys);
            komento.ExecuteNonQuery();

            yhteys.Close();

            PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='0'", "lainat", lainalista);
            PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='1'", "lainat", lainattulista);

        }
    }
}
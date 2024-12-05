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
    public partial class MainWindow : MetroWindow
    {
        string polku = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\k2200423\\Documents\\testitietokanta.mdf;Integrated Security=True;Connect Timeout=30";
        public MainWindow()
        {
            InitializeComponent();

            // Sovelluksen ikkunan nimi ja koko
            this.Title = "Kirjastosovellus";
            this.MinHeight = 500;
            this.MinWidth = 800;

            //Poistetaan ylimääräiset rivit datagridista
            kirjalista.CanUserAddRows = false;
            asiakaslista.CanUserAddRows = false;
            kopiolista.CanUserAddRows = false;
            lainalista.CanUserAddRows = false;
            lainattulista.CanUserAddRows = false;

            //Laitetaan teemaksi Light.Blue
            ThemeManager.Current.ChangeTheme(this, "Light.Blue");

            try
            {
                //Datagridien ja comboboxien päivitys
                PaivitaDataGrid("SELECT * FROM kirjat","kirjat", kirjalista);
                PaivitaDataGrid("SELECT * FROM asiakkaat", "asiakkaat", asiakaslista);
                PaivitaDataGrid(@"SELECT k.*, (SELECT COUNT(*) FROM lainat l WHERE l.kopio_id = k.kirja_id) AS Lainassa FROM kopiot k", "kopiot", kopiolista);
                PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='0'", "lainat", lainalista);
                PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='1'", "lainat", lainattulista);

                PaivitaKirjaComboBox(kirjalista_cb, kirjalista_cb_2);
                PaivitaAsiakasComboBox();
            }
            catch
            {
                //Jos tietokantayhteyttä ei saada, näytetään virheilmoitus
                tilaviesti.Text = "Ei tietokantayhteyttä";
            }
            
        }
        private void Sivunvaihto(object sender, SelectionChangedEventArgs e)
        {
            //Tyhjennetään tilaviesti sivunvaihdolla
            tilaviesti.Text = string.Empty;
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
            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            SqlCommand komento = new SqlCommand("SELECT * FROM kirjat", yhteys);
            SqlDataReader lukija = komento.ExecuteReader();

            DataTable taulu = new DataTable();
            taulu.Columns.Add("ID", typeof(string));
            taulu.Columns.Add("NIMI", typeof(string));

            //Haetaan kirjat comboboxeihin
            kombo1.ItemsSource = taulu.DefaultView;
            kombo1.DisplayMemberPath = "NIMI";
            kombo1.SelectedValuePath = "ID";

            kombo2.ItemsSource = taulu.DefaultView;
            kombo2.DisplayMemberPath = "NIMI";
            kombo2.SelectedValuePath = "ID";

            while (lukija.Read())
            {
                int id = lukija.GetInt32(0);
                string nimi = lukija.GetString(1);
                taulu.Rows.Add(id, nimi);
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
            tilaviesti.Text = "Kirja lisätty";
        }
        private void PoistaKirjaButton(object sender, RoutedEventArgs e)
        {
            using (SqlConnection yhteys = new SqlConnection(polku))
            {
                yhteys.Open();

                string id = kirjalista_cb.SelectedValue.ToString();

                // Tarkistetaan onko kirja lainassa
                string tarkistus = "SELECT COUNT(*) FROM lainat WHERE kopio_id IN (SELECT id FROM kopiot WHERE kirja_id=@kirja_id)";
                SqlCommand tarkistuskomento = new SqlCommand(tarkistus, yhteys);
                tarkistuskomento.Parameters.AddWithValue("@kirja_id", id);

                int lainassa = (int)tarkistuskomento.ExecuteScalar();

                if (lainassa > 0)
                {
                    // Kirja on lainassa, ei voida poistaa
                    tilaviesti.Text = "Ei voi poistaa, kirja lainassa";
                }
                else
                {
                    // Poistetaan kirja ja sen kopiot
                    string kysely1 = "DELETE FROM kopiot WHERE kirja_id=@kirja_id;";
                    SqlCommand komento1 = new SqlCommand(kysely1, yhteys);
                    komento1.Parameters.AddWithValue("@kirja_id", id);
                    komento1.ExecuteNonQuery();

                    string kysely2 = "DELETE FROM kirjat WHERE id=@kirja_id;";
                    SqlCommand komento2 = new SqlCommand(kysely2, yhteys);
                    komento2.Parameters.AddWithValue("@kirja_id", id);
                    komento2.ExecuteNonQuery();

                    PaivitaDataGrid("SELECT * FROM kirjat", "kirjat", kirjalista);
                    PaivitaDataGrid("SELECT * FROM kopiot", "kopiot", kopiolista);
                    PaivitaKirjaComboBox(kirjalista_cb, kirjalista_cb_2);
                    tilaviesti.Text = "Kirja poistettu";
                }
                yhteys.Close();
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
            tilaviesti.Text = "Asiakas lisätty";
        }

        private void LisaaLainaButton(object sender, RoutedEventArgs e)
        {
            SqlConnection yhteys = new SqlConnection(polku);
        
            yhteys.Open();

            string asiakasID = asiakaslista_cb.SelectedValue.ToString();
            string kirja_nimi = kirjalista_cb_2.Text; // Kirjan nimi saadaan te

            // Haetaan kirjan id kirjan nimen perusteella
            string kirjaQuery = "SELECT id FROM kirjat WHERE nimi = @kirja_nimi";
            SqlCommand kirjaCommand = new SqlCommand(kirjaQuery, yhteys);
            kirjaCommand.Parameters.AddWithValue("@kirja_nimi", kirja_nimi);

            object kirjaResult = kirjaCommand.ExecuteScalar();
            if (kirjaResult != null)
            {
                int kirjaID = (int)kirjaResult;

                // Haetaan kopioiden määrä kirja_id:n perusteella
                string maaraQuery = "SELECT maara FROM kopiot WHERE kirja_id = @kirja_id";
                SqlCommand maaraCommand = new SqlCommand(maaraQuery, yhteys);
                maaraCommand.Parameters.AddWithValue("@kirja_id", kirjaID);

                int maara = (int)maaraCommand.ExecuteScalar();

                // Lasketaan kuinka monta kirjan kopiota on lainassa
                string countQuery = "SELECT COUNT(*) FROM lainat l INNER JOIN kopiot k ON l.kopio_id = k.id WHERE k.kirja_id = @kirja_id";
                SqlCommand countCommand = new SqlCommand(countQuery, yhteys);
                countCommand.Parameters.AddWithValue("@kirja_id", kirjaID);

                int currentLoans = (int)countCommand.ExecuteScalar();

                if (currentLoans < maara)
                {
                    // Haetaan kopio_id kirja_id:n perusteella
                    string kopioQuery = "SELECT TOP 1 id FROM kopiot WHERE kirja_id = @kirja_id";
                    SqlCommand kopioCommand = new SqlCommand(kopioQuery, yhteys);
                    kopioCommand.Parameters.AddWithValue("@kirja_id", kirjaID);

                    object kopioResult = kopioCommand.ExecuteScalar();
                    int kopioID = (int)kopioResult;

                    // Lisätään laina tietokantaan
                    string sql = "INSERT INTO lainat (asiakas_id, kopio_id, haettu) VALUES (@asiakas_id, @kopio_id, 0)";
                    SqlCommand komento = new SqlCommand(sql, yhteys);
                    komento.Parameters.AddWithValue("@asiakas_id", asiakasID);
                    komento.Parameters.AddWithValue("@kopio_id", kopioID);
                    komento.ExecuteNonQuery();

                    yhteys.Close();

                    PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='0'", "lainat", lainalista);
                    PaivitaDataGrid(@"SELECT k.*, (SELECT COUNT(*) FROM lainat l WHERE l.kopio_id = k.kirja_id) AS Lainassa FROM kopiot k", "kopiot", kopiolista);
                    tilaviesti.Text = "Laina hyväksytty";
                }
                else
                {
                    tilaviesti.Text = "Kaikki kirjan kopiot lainassa";
                }
            }
        }

        private void laina_haettu_click(object sender, RoutedEventArgs e)
        {
            DataRowView rivinäkymä = (DataRowView)((Button)e.Source).DataContext;
            String tilaus_id = rivinäkymä[0].ToString();

            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            //Merkitään laina haetuksi
            string sql = "UPDATE lainat SET haettu=1 WHERE id='" + tilaus_id + "';";

            SqlCommand komento = new SqlCommand(sql, yhteys);
            komento.ExecuteNonQuery();

            yhteys.Close();

            PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='0'", "lainat", lainalista);
            PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='1'", "lainat", lainattulista);
            PaivitaDataGrid(@"SELECT k.*, (SELECT COUNT(*) FROM lainat l WHERE l.kopio_id = k.kirja_id) AS Lainassa FROM kopiot k", "kopiot", kopiolista);
            tilaviesti.Text = "Laina haettu";
        }
        private void laina_palautettu_click(object sender, RoutedEventArgs e)
        {
            DataRowView rivinäkymä = (DataRowView)((Button)e.Source).DataContext;
            String tilaus_id = rivinäkymä[0].ToString();

            SqlConnection yhteys = new SqlConnection(polku);
            yhteys.Open();

            //Merkitään laina palautetuksi ja poistetaan laina tietokannasta
            string sql = "DELETE FROM lainat WHERE id='" + tilaus_id + "';";

            SqlCommand komento = new SqlCommand(sql, yhteys);
            komento.ExecuteNonQuery();

            yhteys.Close();

            PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='0'", "lainat", lainalista);
            PaivitaDataGrid("SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='1'", "lainat", lainattulista);
            PaivitaDataGrid(@"SELECT k.*, (SELECT COUNT(*) FROM lainat l WHERE l.kopio_id = k.kirja_id) AS Lainassa FROM kopiot k", "kopiot", kopiolista);
            tilaviesti.Text = "Laina palautettu";
        }
    }
}
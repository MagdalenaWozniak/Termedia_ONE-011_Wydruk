using cdn_api;
using Hydra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Wydruk.Formaty;

namespace Wydruk
{
    public partial class WyborWydruku : Form
    {
        private int XLWersjaAPI = 20202;
        private int XLTrybWsadowy = 1;
        private int XLWinieta = -1;
        private int XLUtworzWlasnaSesje = 1;
        private int sesja = -1;
        private string XLSQLServer = "192.168.4.24";
        private string XLSQLDatabase = "ERPXL_Termedia_Test";//"ERPXL_PoloMarket"
        private string XLUser = "ADMIN"; //
        private string XLPass = "NE!w0rk12";//"Dupa2003!";
        private string connectionString = @"Data Source=192.168.4.24;Initial Catalog=ERPXL_Termedia_Test; User ID=onesupport;Password=a956!2A@";
        FS faktura;
        List<FS> FSy=new List<FS>();
        List<Formaty> formaty2 = new List<Formaty>();
        public WyborWydruku(List<FS> listaFS)
        {
            FSy = listaFS;
            faktura = listaFS[0];
            //faktura = fakturaFS;
            string typ = "";
            InitializeComponent();
            // MessageBox.Show("Faktura GIDTyp: "+faktura.GIDTyp, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            switch (faktura.GIDTyp)
            {
                case 2033:
                    {
                        typ = "FS-";
                        break;
                    }
                case 2037:
                    {
                        typ = "FSE-";
                        break;
                    }
                case 1603:
                    {
                        typ = "MMW";
                        break;
                    }
                default:
                    {
                        MessageBox.Show("Dla wybranego typu dokumentu nie istnieją formaty wydruku", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    }
            }
            if (typ != "")
            {
                if (Runtime.ConfigurationDictionary.Baza.ToString().Substring(0, 19) == "ERPXL_Termedia_TEST")
                {
                    //MessageBox.Show("Jestem tutaj", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    XLSQLDatabase = "Termedia TEST";
                    connectionString = @"Data Source=192.168.4.24;Initial Catalog=ERPXL_Termedia_Test; User ID=onesupport;Password=a956!2A@";
                }
                else if (Runtime.ConfigurationDictionary.Baza.ToString().Substring(0, 14) == "ERPXL_Termedia")
                {
                    XLSQLDatabase = "Termedia";
                    connectionString = @"Data Source=192.168.4.24;Initial Catalog=ERPXL_Termedia; User ID=onesupport;Password=a956!2A@";
                }
                else
                {
                    MessageBox.Show("Dodatek nie posiada funkcjonalności dla tej bazy.", "Komunikat",
                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Dispose(true);
                    return;

                }
                string formatyQuery = "SELECT [RpF2_RpWId] ,[RpF2_Id] ,[RpF2_Nazwa] FROM [CDN].[RptFormaty2]";
                formaty2 = SelectListRowColumn(formatyQuery, "RptFormaty2", typ);
                string[] stringRodzaje = new string[formaty2.Count];
                int i = 0;
                int selectedIndex = 0;
                foreach (Formaty format in formaty2)
                {
                    stringRodzaje[i] = format.Nazwa;
                    if (format.Nazwa.Contains("domyślny"))
                        selectedIndex = i;
                    i++;

                }
                formaty.Items.AddRange(stringRodzaje);
                if (i > 0)
                    formaty.SelectedIndex = selectedIndex;
            }
            else
            {
                Dispose(true);
                return;
            }

        }

        private void button_Click(object sender, EventArgs e)
        {
            string numerFaktury = "";
            try
            {
                if (Convert.ToString(formaty.SelectedItem) == "")
                {
                    MessageBox.Show("Nie wybrano formatu wydruku", "Komunikat",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //Application.Exit();
                    return;
                }

                //ADODB._Recordset recordset = Runtime.WindowController.GetQueueMarked((int)ProcId, listID, GetCallbackThread());

                // MessageBox.Show(Runtime.ConfigurationDictionary.Baza.ToString().Substring(0, 18));
                

                #region logowanie
                XLLoginInfo_20202 nowyUzytkownik = new XLLoginInfo_20202();
                nowyUzytkownik.Wersja = XLWersjaAPI;
                nowyUzytkownik.Winieta = XLWinieta;
                nowyUzytkownik.TrybWsadowy = XLTrybWsadowy;
                nowyUzytkownik.UtworzWlasnaSesje = XLUtworzWlasnaSesje;
                nowyUzytkownik.SerwerKlucza = XLSQLServer;
                nowyUzytkownik.Baza = XLSQLDatabase;
                nowyUzytkownik.OpeIdent = XLUser;
                nowyUzytkownik.OpeHaslo = XLPass;
                nowyUzytkownik.PortHTTPSerweraKlucza = 1;
                nowyUzytkownik.RodzajSerweraKlucza = 0;
                nowyUzytkownik.SerwerKlucza = "localhost::5000185870";
                nowyUzytkownik.ProgramID = "ONE-011 Wydruk";

                int blad = cdn_api.cdn_api.XLLogin(nowyUzytkownik, ref sesja);

                if (blad != 0)
                {
                    MessageBox.Show("Błąd logowania: " + blad, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                #endregion


                foreach (FS fakturaFS in FSy)
                {


                    numerFaktury = fakturaFS.numerFs(connectionString);

                    XLWydrukInfo_20202 wydruk = new XLWydrukInfo_20202();
                    wydruk.Wersja = XLWersjaAPI;
                    wydruk.Zrodlo = 1;
                    wydruk.Wydruk = formaty2[formaty.SelectedIndex].Wydruk;
                    wydruk.Format = formaty2[formaty.SelectedIndex].Format;
                    wydruk.FiltrSQL = "(TrN_GIDTyp=" + fakturaFS.GIDTyp + " AND TrN_GIDNumer=" + fakturaFS.GIDNumer + ")";
                    wydruk.Urzadzenie = 3;
                    wydruk.DrukujDoPliku = 1;
                    wydruk.PlikDocelowy = "C:\\wydruki\\" + numerFaktury.Replace("\\", "_").Replace("/", "_") + ".pdf";
                    int blad_wydruk = cdn_api.cdn_api.XLWykonajPodanyWydruk(wydruk);

                    if (blad_wydruk != 0)
                    {
                        MessageBox.Show("Błąd generowanie wydruku API: " + blad_wydruk, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }




                cdn_api.cdn_api.XLLogout(sesja);

                //MessageBox.Show("Zakończono wydruk faktury " + numerFaktury, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception en)
            {
                MessageBox.Show("Błąd generowanie wydruku: " + en.Message, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return false;
            }

            MessageBox.Show("Zakończono wydruk faktur", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //Application.Exit();
            //ActiveForm.Close();
            Dispose(true);
        }

        public List<Formaty> SelectListRowColumn(string query, string srcTableName, string typ)
        {
            List<string> str = new List<string>();
            List<Formaty> myList = new List<Formaty>();
            string sttr = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, srcTableName);
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        foreach (DataColumn column in (InternalDataCollectionBase)dataSet.Tables[0].Columns)
                        {
                            str.Add(row[column].ToString());

                            //MessageBox.Show(column + ": " + row[column].ToString());
                        }
                        if(Convert.ToString(str[2]).Contains(typ))
                            myList.Add(new Formaty(Convert.ToString(str[2]), Convert.ToInt32(str[1]), Convert.ToInt32(str[0])));
                        str.Clear();

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(sttr = sttr + "Bład SQL:" + ex.ToString() + "\n");
            }
            return myList;
        }
    }
}

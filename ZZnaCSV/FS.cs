using cdn_api;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wydruk
{
    public class FS
    {
        public int GIDNumer { get; set; }
        public int GIDTyp { get; set; }
        public int GIDFirma {get; set; }
        public short GIDLp { get; set; }

        public FS(int GIDNumer, int GIDTyp, int GIDFirma, short GIDLp)
        {
            this.GIDNumer = GIDNumer;
            this.GIDTyp = GIDTyp;
            this.GIDFirma = GIDFirma;
            this.GIDLp = GIDLp;
        }
        

       

        public string numerFs(string connection_string)
        {
            string wynikQuery = "";
            string nrQuery = "SELECT CDN.NumerDokumentu(TrN_GIDTyp, TrN_SpiTyp, TrN_TrNTyp, TrN_TrNNumer,TrN_TrNRok, TrN_TrNSeria, TrN_TrNMiesiac) FROM CDN.TraNag WHERE TrN_GIDTyp =" + GIDTyp + " AND TrN_GIDNumer = " + GIDNumer + " and TrN_GIDLp = " + GIDLp + " and TrN_GIDFirma = " + GIDFirma;
            wynikQuery = ConnectToSQL(connection_string, nrQuery);
            return wynikQuery;
        }


        public string ConnectToSQL(string connection_string, string query)
        {
            string str = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "Query");
                    if (dataSet.Tables.Count == 0)
                    {
                        return "";
                    }
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        str = row[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                str = str + "Bład SQL:" + ex.ToString() + "\n";
            }
            return str;
        }


    }
}

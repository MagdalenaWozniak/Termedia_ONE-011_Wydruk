using Hydra;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Wydruk;
using System.Collections.Generic;
using static Wydruk.FS;
using System.Text;
using System;
using System.Data.SqlClient;
using System.Data;
using cdn_api;

[assembly: CallbackAssemblyDescription(Name: "ONE-011 Wydruk",
                                           Description: "Wygenerowanie wydruku",
                                           Author: "OneSupport",
                                           Version: "1.0",
                                           SystemVersion: "2021.0.2",
                                           Created: "11-04-2021")]


namespace Wydruk
{
    [SubscribeProcedure(Procedures.ListaDokumentow, "Wygenerowanie wydruku")]
    public class Wydruk_Class : Callback
    {
        
        private ClaWindow RapButton;
        private ClaWindow zakladkaTabWewnetrzne;
        private ClaWindow filtrBtn;
        //private List<ListaZS> ZSy = new List<ListaZS>();
        int lista;
        private bool bSpacja = false;
        long iNumer;
        long iTyp;
        long iFirma;
        long iLp;
        private ClaWindow listaPoz;
        private List<FS> listaFS = new List<FS>();
        public override void Init()
        {

            AddSubscription(true, 0, Events.JustAfterWindowOpening, new TakeEventDelegate(OnOpenWindow));
            AddSubscription(false, 0, Events.ResizeWindow, new TakeEventDelegate(Resize));
        }



        private bool OnOpenWindow(Procedures ProcId, int ControlId, Events Event)
        {

            ClaWindow Parent = GetWindow();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Rectangle WndBnd = GetWindow().Bounds;
            zakladkaTabWewnetrzne = GetWindow().AllChildren["?TabSprzedaz"];
            listaPoz = GetWindow().AllChildren["?TabHan"];
            lista = GetWindow().AllChildren["?Handlowe"].Id;
            RapButton = Parent.Children.Add(ControlTypes.button);
            RapButton.Visible = true;
            RapButton.TextRaw = "Druk";
            RapButton.ToolTipRaw = "Wygenerowanie wydruku";
            //RapButton.IconRaw = "C:\\Onesupport\\PROnaFS\\proforma.ico";

            filtrBtn = GetWindow().AllChildren["?BRW1::TotalWorking"];                            //pobiera współrzędne przycisku zapisz
            RapButton.Bounds = new Rectangle(filtrBtn.Bounds.X - 75, filtrBtn.Bounds.Y, 25, 15);
          //tu sprawdzic

            RapButton.OnAfterMouseDown += new TakeEventDelegate(BtnOnMouseDown);
            RapButton.OnBeforeAccepted += new TakeEventDelegate(zaczytaj_OnAfterAccepted);
            

            //pobrane z pola w zakładce Raporty
            return true;
        }

        public bool Resize(Procedures ProcId, int ControlId, Events Event)
        {
            filtrBtn = GetWindow().AllChildren["?BRW1::TotalWorking"];                            //pobiera współrzędne przycisku zapisz
            RapButton.Bounds = new Rectangle(filtrBtn.Bounds.X - 75, filtrBtn.Bounds.Y, 25, 15);

            return true;
        }

        bool zaczytaj_OnAfterAccepted(Procedures ProcedureId, int ControlId, Events Event)
        {
            bSpacja = true;
            Runtime.WindowController.PostEvent(listaPoz.Id, Events.NewSelection);
            iNumer = TraNag.TrN_GIDNumer;
            iFirma = TraNag.TrN_GIDFirma;
            iLp = TraNag.TrN_GIDLp;
            iTyp = TraNag.TrN_GIDTyp;
            

            return true;
        }

        public bool BtnOnMouseDown(Procedures ProcId, int ControlId, Events Event)
        {
            try
            {
                ADODB._Recordset recordset = Runtime.WindowController.GetQueueMarked((int)ProcId, lista, GetCallbackThread());

                 bool zaznaczone =FSy(recordset);

                //FS fakturaZazn = new FS(Convert.ToInt32(iNumer), Convert.ToInt32(iTyp), Convert.ToInt32(iFirma), (short)iLp);
                if(!zaznaczone)
                    listaFS.Add(new FS(Convert.ToInt32(iNumer), Convert.ToInt32(iTyp), Convert.ToInt32(iFirma), (short)iLp));
                //Application.Run(new WyborWydruku(listaFS));

                WyborWydruku wydruk = new WyborWydruku(listaFS);
                wydruk.ShowDialog();

                //MessageBox.Show("Zakończono dodawanie faktury", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception en)
            {
                //MessageBox.Show("Błąd w BtnOnMouseDown: "+en.Message, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return false;
            }
            listaFS.Clear();
            return true;
        }

        bool FSy(ADODB._Recordset recordset)
        {

            try
            {
                StringBuilder dokumenty = new StringBuilder();
                if (recordset != null)
                {
                    int count = recordset.RecordCount;

                    if (count != 0)
                    {
                        recordset.MoveFirst();

                        while (recordset.EOF == false)
                        {
                            ADODB.Fields fields = recordset.Fields;
                            //MessageBox.Show("Numer: "+Convert.ToInt32(fields[2].Value), "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                            listaFS.Add(new FS(Convert.ToInt32(fields[2].Value), Convert.ToInt32(fields[0].Value), Convert.ToInt32(fields[1].Value), Convert.ToInt16(fields[3].Value)));
                            
                            recordset.MoveNext();
                        }
                    }
                    else
                    {
                       // MessageBox.Show("Jestem w tym else", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                else
                {
                   // MessageBox.Show("Jestem w drugim else", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd pobierania zaznaczonych towarów --> " + ex.Message, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
        }


        public override void Cleanup()
        {

        }

        

     
    }
}

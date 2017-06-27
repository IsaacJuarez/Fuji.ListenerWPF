using Fuji.ListenerWPF.DataAccess;
using Fuji.ListenerWPF.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuji.ListenerWPF
{
    public class NapoleonDataAccess
    {
        public static NAPOLEONEntities NapoleonDA;

        public static bool actualizaEstatus(string AccNum)
        {
            bool valido = false;
            try
            {
                using (NapoleonDA = new NAPOLEONEntities())
                {
                    if (NapoleonDA.tbl_MST_Estudio.Any(z => z.vchAccessionNumber.Trim().ToUpper() == AccNum.Trim().ToUpper()))
                    {
                        tbl_MST_Estudio estudio = NapoleonDA.tbl_MST_Estudio.First(z => z.vchAccessionNumber.Trim().ToUpper() == AccNum.Trim().ToUpper());
                        if (estudio != null)
                        {
                            if (NapoleonDA.tbl_DET_Estudio.Any(z => z.intEstudioID == estudio.intEstudioID))
                            {

                                var lista = NapoleonDA.tbl_DET_Estudio.Where(z => z.intEstudioID == estudio.intEstudioID).ToList();
                                lista.ForEach(a => a.intEstatusID = 4);
                                NapoleonDA.SaveChanges();
                                //List<tbl_DET_Estudio> lst = new List<tbl_DET_Estudio>();
                                //lst = NapoleonDA.tbl_DET_Estudio.Where(z => z.intEstudioID == estudio.intEstudioID).ToList();
                                //if (lst != null)
                                //{
                                //    foreach (tbl_DET_Estudio detalle in lst)
                                //    {
                                //        using (NapoleonDA = new NAPOLEONEntities())
                                //        {
                                //            detalle.intEstatusID = 4;
                                //            NapoleonDA.SaveChanges();
                                //        }
                                //    }
                                valido = true;
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception eAE)
            {
                valido = false;
                Log.EscribeLog("Existe un error en NapoleonDataAccess.actualizaEstatus: " + eAE.Message);
            }
            return valido;
        }
    }
}

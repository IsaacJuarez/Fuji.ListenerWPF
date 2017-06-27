using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fuji.ListenerWPF.Extensions
{
    public class XMLConfigurator
    {
        public static string path = ConfigurationManager.AppSettings["ConfigDirectory"] != null ? ConfigurationManager.AppSettings["ConfigDirectory"].ToString() : "";

        public static string getXMLfile(string fullpath)
        {
            string _config = "";
            try
            {
                //quitarAcentos(fullpath);                
                XmlDocument estudio = new XmlDocument();
                StreamReader sr = new StreamReader(
                     File.OpenRead(fullpath),
                     Encoding.ASCII);
                estudio.Load(sr);
                sr.Close();

                XmlNode node = estudio.DocumentElement.SelectSingleNode("/bloqueo");
                //Accession Number
                _config = node["AN"]?.InnerText != "" ? node["AN"]?.InnerText : "";
                
            }
            catch (Exception eXMLC)
            {
                _config = "";
                Log.EscribeLog("Existe un error al obtener los valores de configuración: " + eXMLC.Message);
            }
            return _config;

        }

        //private static void quitarAcentos(string fullpath)
        //{
        //    try
        //    {
        //        string text = File.ReadAllText(fullpath);
        //        text = text.Replace("Ó", "O");
        //        File.WriteAllText(fullpath, text);
        //    }
        //    catch(Exception eqA)
        //    {
        //        Log.EscribeLog("Existe un error al quitar acentos: " + eqA.Message);
        //    }
        //}
    }
}

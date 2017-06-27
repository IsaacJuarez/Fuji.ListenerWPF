using Fuji.ListenerWPF.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Fuji.ListenerWPF
{
    partial class ListenerWPFService : ServiceBase
    {
        private static Timer SyncTimer = new Timer();
        private static string RepositoryDicrectory = "";
        private static string RepositoryMove = "";
        private static string UserName = "";
        private static string Password = "";
        public ListenerWPFService()
        {
            cargarServicio();
        }

        private void cargarServicio()
        {
            try
            {
                Log.EscribeLog("Se carga el servicio ListenerWPFService. " + " [" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "]");
                decimal segundosPoleo;
                try
                {
                    segundosPoleo = ConfigurationManager.AppSettings["segundosPoleo"] != null ? Convert.ToDecimal(ConfigurationManager.AppSettings["segundosPoleo"].ToString()) : 1;
                }
                catch (Exception eGPOLeo)
                {
                    Log.EscribeLog("Existe un error al obtener el tiempo para el poleo del servicio: " + eGPOLeo.Message + " " + eGPOLeo.StackTrace + " " + eGPOLeo.InnerException);
                    segundosPoleo = 60;
                }
                //Log.EscribeLog("Ruta para segundosPoleo:" + segundosPoleo);
                try
                {
                    RepositoryDicrectory = ConfigurationManager.AppSettings["RepositoryDicrectory"] != null ? ConfigurationManager.AppSettings["RepositoryDicrectory"].ToString() : "";
                }
                catch (Exception eDirec)
                {
                    Log.EscribeLog("No fue posible obtener la carpeta: " + eDirec.Message);
                }
                //Log.EscribeLog("Ruta para RepositoryDicrectory:" + RepositoryDicrectory);
                try
                {
                    RepositoryMove = ConfigurationManager.AppSettings["RepositoryMove"] != null ? ConfigurationManager.AppSettings["RepositoryMove"].ToString() : "";
                }
                catch (Exception eDirec)
                {
                    Log.EscribeLog("No fue posible obtener la carpeta para mover los archivos: " + eDirec.Message);
                }
                try
                {
                    UserName = ConfigurationManager.AppSettings["User"] != null ? ConfigurationManager.AppSettings["User"].ToString() : "";
                }
                catch (Exception eDirec)
                {
                    Log.EscribeLog("No fue posible obtener el usuario: " + eDirec.Message);
                }
                try
                {
                    Password = ConfigurationManager.AppSettings["Pass"] != null ? ConfigurationManager.AppSettings["Pass"].ToString() : "";
                }
                catch (Exception eDirec)
                {
                    Log.EscribeLog("No fue posible obtener el password: " + eDirec.Message);
                }
                //Log.EscribeLog("Ruta para mover:" + RepositoryMove);
                //Console.ReadKey();
                int minutos = (int)(1000 * segundosPoleo);
                SyncTimer.Elapsed += new System.Timers.ElapsedEventHandler(SyncTimer_Elapsed);
                SyncTimer.Interval = minutos;
                SyncTimer.Enabled = true;
                SyncTimer.Start();
                Log.EscribeLog("Esperando");
                //Console.ReadKey();
            }
            catch (Exception eCs)
            {
                Log.EscribeLog("Existe un error al cargar el servicio." + eCs.Message);
                Log.EscribeLog("Existe un error al cargar el servicio." + eCs.Message);
            }
        }

        private void SyncTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Log.EscribeLog("[" + DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString() + "] Leyendo estudios marcar como dictados.");
                //Log.EscribeLog("[" + DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString() + "] Leyendo estudios marcar como dictados.");
                //Log.EscribeLog("Ruta: " + RepositoryDicrectory);
                ExternalResourceConnection.PinvokeWindowsNetworking.ConnectToRemote(RepositoryDicrectory, UserName, Password);
                if (Directory.Exists(RepositoryDicrectory))
                {
                    DirectoryInfo di = new DirectoryInfo(RepositoryDicrectory);
                    FileInfo[] listFiles = di.GetFiles("*.xml", SearchOption.AllDirectories);
                    Log.EscribeLog("Archivos encontrados: " + listFiles.Count().ToString());
                    if (listFiles.Count() > 0)
                    {
                        foreach (FileInfo _file in listFiles)
                        {
                            Log.EscribeLog("Leyendo: " + _file.Name.ToString());
                            //Log.EscribeLog("Leyendo: " + _file.Name.ToString());
                            string accNum = leerArchivo(_file.FullName);
                            if (accNum != "")
                            {
                                Log.EscribeLog("AccNum : " + accNum);
                                bool valido = actualizaEstatus(accNum);
                                moverarchivo(valido, _file.FullName);
                            }
                        }
                    }
                }
                else
                {
                    Log.EscribeLog("Repositorio no encontrado. " + RepositoryDicrectory);
                }
                ExternalResourceConnection.PinvokeWindowsNetworking.DisconnectRemote(RepositoryDicrectory);
            }
            catch (Exception eListener)
            {
                Log.EscribeLog("Existe un error en SyncTimer_Elapsed: " + eListener.Message);
            }
        }

        private void moverarchivo(bool valido, string ruta)
        {
            try
            {
                string filehtml = "";
                string filexml = "";
                string pathhtml = "";
                filexml = Path.GetFileName(ruta);
                filehtml = filexml.Replace(".xml", ".html");
                pathhtml = ruta.Replace(filexml, filehtml);
                ExternalResourceConnection.PinvokeWindowsNetworking.ConnectToRemote(RepositoryDicrectory, UserName, Password);
                if (valido)
                {
                    Log.EscribeLog("Archivo " + Path.GetFileName(ruta) + " Correcto");
                    //Log.EscribeLog("de: " + ruta);
                    //Log.EscribeLog("para: " + RepositoryMove + @"Correcto\" + Path.GetFileName(ruta));
                    File.Move(pathhtml, RepositoryMove + @"Correcto\" + Path.GetFileName(pathhtml));
                    File.Move(ruta, RepositoryMove + @"Correcto\" + Path.GetFileName(ruta));
                }
                else
                {
                    Log.EscribeLog("Archivo " + Path.GetFileName(ruta) + " con errores.");
                    //Log.EscribeLog("de: " + ruta);
                    //Log.EscribeLog("para: " + RepositoryMove + @"Correcto\" + Path.GetFileName(ruta));
                    File.Move(pathhtml, RepositoryMove + @"Error\" + Path.GetFileName(pathhtml));
                    File.Move(ruta, RepositoryMove + @"Error\" + Path.GetFileName(ruta));
                }
                ExternalResourceConnection.PinvokeWindowsNetworking.DisconnectRemote(RepositoryDicrectory);
            }
            catch (Exception eMA)
            {
                Log.EscribeLog("Existe un erro al mover el archivo " + ruta + ": " + eMA.Message);
            }
        }

        private string leerArchivo(string fullName)
        {
            string accNum = "";
            try
            {
                accNum = XMLConfigurator.getXMLfile(fullName);
            }
            catch(Exception eLA)
            {
                Log.EscribeLog("Existe un error al leer el archivo " + Path.GetFileName(fullName) + " :" + eLA.Message);
            }
            return accNum;
        }

        private bool actualizaEstatus(string AccNum)
        {
            bool valido = false;
            try
            {
                valido = NapoleonDataAccess.actualizaEstatus(AccNum);
            }
            catch(Exception eaE)
            {
                valido = false;
                Log.EscribeLog("Existe un error en actualizaEstatus: " + eaE.Message);
            }
            return valido;
        }

        protected override void OnStart(string[] args)
        {
            // TODO: agregar código aquí para iniciar el servicio.
        }

        protected override void OnStop()
        {
            // TODO: agregar código aquí para realizar cualquier anulación necesaria para detener el servicio.
        }
    }
}

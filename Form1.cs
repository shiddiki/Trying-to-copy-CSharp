using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace copeytry_direct
{
    public partial class Form1 : Form
    {
        public static string[] fromnames;
        public static string tonames;
        public static string names11, names21 = "",names12,names22="",fromcopy1,fromcopy2;
        public static int sent = 0,failedno = 0;
        public static string[] failed;
        static Thread tocpy1,tocpy2;
        public Form1()
        {
            InitializeComponent();
        }
        string a;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog from = new OpenFileDialog();
            from.Multiselect = true;
            if (from.ShowDialog() == DialogResult.OK)
            {
                fromnames= from.FileNames;
                //MessageBox.Show(fromnames);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog to = new FolderBrowserDialog();
            //to.m = true;
            if (to.ShowDialog() == DialogResult.OK)
            {
                tonames = to.SelectedPath;
                //MessageBox.Show(fromnames);
               // copy();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
               foreach (string fim in fromnames)
               {

                   names11 = "";
                   names21 = "";
                   for (int i = fim.Length - 1; i >= 0; i--)
                   {
                       if (fim[i] == '\\')
                           break;
                       names11 += fim[i];
                   }
                   for (int i = names11.Length - 1; i >= 0; i--)
                       names21 += names11[i];

                   using (FileStream stream = File.OpenRead(fim))
                Copystream(stream,tonames+"\\"+names21);
               
               }
               stopwatch.Stop();
               MessageBox.Show("Time elapsed: " + stopwatch.Elapsed);
                
            }
            Environment.Exit(0);
        }

        

        public static void Copystream(System.IO.Stream inStream, string outputFilePath)
        {
            int bufferSize = 1024 * 1024;

          /*  using (inStream)
            {
                using (BinaryReader r = new BinaryReader(inStream))
                {
                    using (FileStream fs = new FileStream(outputFilePath, FileMode.OpenOrCreate))
                    {
                        using (BinaryWriter w = new BinaryWriter(fs))
                        {
                            w.Write(r.ReadBytes((int)inStream.Length - 1));
                        }
                    }
                }
            }*/
            

            using (FileStream fileStream = new FileStream(outputFilePath, FileMode.OpenOrCreate,FileAccess.Write))
            {
                fileStream.SetLength(inStream.Length);
                int bytesRead = -1;
                byte[] bytes = new byte[bufferSize];

                while ((bytesRead = inStream.Read(bytes, 0, bufferSize)) > 0)
                {
                    fileStream.Write(bytes, 0, bytesRead);
                    fileStream.Flush();
                }
            }
        }

        private void copy()
        {
            Process[] processes = Process.GetProcesses();
            

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            
            int tot = 0,faul=0;
            
            string msg="";
            
                foreach (string frompath in fromnames)
                {
                    
                    
                        tot++;
                        
                        if (tot!=0)
                        {

                            names11 = "";
                            names21 = "";
                            for (int i = frompath.Length - 1; i >= 0; i--)
                            {
                                if (frompath[i] == '\\')
                                    break;
                                names11 += frompath[i];
                            }
                            for (int i = names11.Length - 1; i >= 0; i--)
                                names21 += names11[i];



                            fromcopy1 = "";
                            fromcopy1 = frompath;

                            tocpy1 = new Thread(thredcopy1);
                            tocpy1.Start();
                            
                        }
                        else
                        {
                            names12 = "";
                            names22 = "";
                            for (int i = frompath.Length - 1; i >= 0; i--)
                            {
                                if (frompath[i] == '\\')
                                    break;
                                names12 += frompath[i];
                            }
                            for (int i = names12.Length - 1; i >= 0; i--)
                                names22 += names12[i];



                            fromcopy2 = "";
                            fromcopy2 = frompath;
                            tocpy2 = new Thread(thredcopy2);
                            tocpy2.Start();
                        }

                       // if (tot  == 0)
                        {
                            while (tot != sent)
                            {
                                faul++;
                                if (faul == 1000000)
                                    faul = 0;
                            }
                        
                        }
                
                
                }

                while (tot != sent)
                {
                    faul++;
                    if (faul == 1000000)
                        faul = 0;
                }
            stopwatch.Stop();


            if (failedno >= 1)
            {
                for (int i = 0; i < failedno; i++)
                    msg += failed[i] + "\\n";

                MessageBox.Show(msg);
            }
            else
            {
                MessageBox.Show("Time elapsed: " + stopwatch.Elapsed);
            
            }
        
        }
        public void thredcopy1()
        {
            string fm1 = fromcopy1;

           // MessageBox.Show(fm1);
            try
            {
                File.Copy(fm1, tonames + "\\" + names21);

            }
            catch(Exception es)
            {
               // MessageBox.Show(es.ToString());
                failed[failedno] = "To copy file Form " + fm1 + " To" + tonames;
                failedno++;
                
            }

            sent++;
            tocpy1.Abort();
        }
        public void thredcopy2()
        {
            string fm2 = fromcopy2;
           // MessageBox.Show(fm2);
            try
            {
                File.Copy(fm2, tonames + "\\" + names22);

            }
            catch (Exception es)
            {
               // MessageBox.Show(es.ToString());
                failed[failedno] = "To copy file Form " + fm2 + " To" + tonames;
                failedno++;

            }

            sent++;
            tocpy2.Abort();
        }
    }
}

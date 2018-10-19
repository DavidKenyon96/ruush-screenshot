//*************************************************************************************************************************************************
//*** David Kenyon's Screenshot Project
//*** Ruush: A streamlined screenshot capturing software
//*** Class: Controller.cs
//*** Created: April 2 2018
//*** Last Updated: August 7 2018
//*************************************************************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using RestSharp.Authenticators;
using RestSharp;
using System.IO;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RuushApplication
{
    class Controller
    {
        readonly string path = @"c:\\ruush\\";

        protected internal DirectoryInfo CreateDir()
        {
            try
            {
                //If the directory does not exist, create it
                if (!Directory.Exists(path))
                {
                    DirectoryInfo ruushDir = Directory.CreateDirectory(path);
                    Console.WriteLine("Created new " + path + " directory ");
                    Console.WriteLine();
                    return ruushDir;
                }
               else
                {
                    Console.WriteLine(path + " exists already.");
                    Console.WriteLine();
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("CreateDir() failed: {0}", e.ToString());
                throw;
            }
        }

        protected internal Bitmap TakeScreenshot()
        {
            Console.WriteLine("Initializing the variables...");
            Console.WriteLine();

            //Collect resolution values of user's screen
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            Bitmap  memoryImage = new Bitmap(resolution.Width, resolution.Height);
            Size s = new Size(memoryImage.Width, memoryImage.Height);
            string str = "";
            try
            {
                //Create graphics 
                Console.WriteLine("Creating Graphics...");
                Console.WriteLine();
                Graphics memoryGraphics = Graphics.FromImage(memoryImage);

                //Capture screen
                Console.WriteLine("Copying data from screen...");
                Console.WriteLine();
                memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
                //Create screenshot save path to ruush directory as a .png with current timestamp
                str = path + @"\\Screenshot-" + DateTime.Now.ToString("MM-dd-yyyy-hh-mm-ss") + ".png";

                //Save image 
                Console.WriteLine("Saving the image...");
                memoryImage.Save(str);
                Console.WriteLine("Picture has been saved...");
                return memoryImage;
            }
            catch (Exception e)
            {
                Console.WriteLine("TakeScreenshot() process failed: {0}", e.ToString());
                throw;
            }
        }
        protected internal FileInfo FindRecentScreenshot() 
        {
            try
            {
                //Find the ruush directory and select *most recent* image
                var directory = new DirectoryInfo(path);
                var recentScreenshot = (from f in directory.GetFiles()
                                        orderby f.LastWriteTime descending
                                        select f).First();
                return recentScreenshot;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("FindRecentScreenshot() process failed: {0}", e.ToString());
                throw;
            }
        }
        protected internal void PaintAndUpload()
        {
            try
            { 
            //string representation of recentScreenshot for this method
            var currentImage = FindRecentScreenshot().FullName;

            //Open MSpaint with myFile, which is the most recent screenshot captured
            Process p = Process.Start("mspaint", currentImage);
            p.EnableRaisingEvents = true;
            p.Exited += (sender1, e1) =>
                {
                    DialogResult dialogResultUpload = MessageBox.Show("Would you like to upload your image to imgur?", "Upload", MessageBoxButtons.YesNo);
                    if (dialogResultUpload == DialogResult.Yes)
                    {
                        ImgurUpload(currentImage);
                    }
                    else if (dialogResultUpload == DialogResult.No)
                    {
                        //display dialog box
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("PaintAndUpload() process failed: {0}", e.ToString());
                throw;
            }
        }

        protected internal void ImgurUpload(string currentImage)
        {
            try
            {
                //Find most recent image in ruush directory which is edited version of myFile
                using (Image image = Image.FromFile(currentImage))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] imageBytes to Base64 String
                        string base64String = Convert.ToBase64String(imageBytes);

                        //Postman generated code for uploading and authentication
                        var client = new RestClient("https://api.imgur.com/3/image");
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Postman-Token", "0eb3efcc-c67c-a091-b566-23769a7e92d2");
                        request.AddHeader("Cache-Control", "no-cache");
                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        request.AddHeader("Authorization", "Bearer ac2edbb80eb73303e0902873a52d9d71a61db3b9");
                        request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
                        request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"image\"\r\n\r\n" + base64String + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
                        IRestResponse response = client.Execute(request);
                        Console.WriteLine("Upload to Imgur success");
                        //URLToClipboard();
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ImgurUpload() process failed: {0}", e.ToString());
                throw;
            }
        }
        /*unimplemented method that is supposed to capture url of recently uploaded image*/
        protected internal void URLToClipboard()
        {
            //var client1 = new RestClient("https://api.imgur.com/3/account/me/images");
            //var request1 = new RestRequest(Method.GET);
            //request.AddHeader("Postman-Token", "33145957-f0d5-dd7e-ca14-9383d804c198");
            //request.AddHeader("Cache-Control", "no-cache");
            //request.AddHeader("Authorization", "Bearer {{accessToken}}");
            //IRestResponse response1 = client.Execute(request1);
            //Clipboard.SetText(request1);
            //Console.WriteLine("Save to clipboard success");
        }     
    }
}

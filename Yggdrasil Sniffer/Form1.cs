using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web.UI;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;


namespace Yggdrasil_Sniffer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            outputbox.Text = "";
            //---Connect to the Ygdrassil Authentication servers
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://authserver.mojang.com/authenticate");
            httpWebRequest.ContentType = "application/json"; //---We MUST send "application/json" or the POST request will fail
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                //---Using the Newtonsoft.Json.Linq library
                //---Full path is Newtonsoft.Json.Linq.JObject AND
                //----------------Newtonsoft.Json.Linq.JProperty
                JObject json = new JObject(
                    new JProperty("agent", new JObject(new JProperty("name", "Minecraft"), new JProperty("version", "1"))),
                    new JProperty("username", this.username.Text),
                    new JProperty("password", this.password.Text));

                streamWriter.Write(json.ToString()); //---This writes the JSON as it should be.
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse httpResponse;

            try
            {
                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string json;
                
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    outputbox.Text = "SUCCESS" + Environment.NewLine;
                    json = streamReader.ReadToEnd(); //---Can't get the nested JSON, read the entire response minus the 200 code.
                }
                //deserialize the json
                string[] data = json.Split(new char[] {',', '\"', ':'}); //---Split up the response into strings. Many will be blank. See below

                //---These are not arbitrary numbers. This is just how the JSON response split up.
                Globals._accesstoken = data[4]; //---THIS HAS CHANGED!---//
                Globals._clienttoken = data[10]; //---See last new lines of code in Progam.cs---//
                Globals._UUID = data[19];
                Globals._username = data[25];


                outputbox.Text += "ACCESS TOKEN: " + Globals._accesstoken + Environment.NewLine; //---Print out data.
                outputbox.Text += "CLIENT TOKEN: " + Globals._clienttoken + Environment.NewLine;
                outputbox.Text += "UUID:         " + Globals._UUID + Environment.NewLine;
                outputbox.Text += "USERNAME:     " + Globals._username + Environment.NewLine;
            }
            catch (WebException)
            {
                outputbox.Text = "FAILURE"; //---OOPS...probably a bad password or servers are down or something.
                Globals._accesstoken = "";
                Globals._clienttoken = "";
                Globals._UUID = "";
                Globals._username = "";
            } 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.Show();
        }
    }


    #region Othercode

    public class jsondata
    {
        public string accesstoken { get; set; }
        public string clienttoken { get; set; }
        public string UUID { get; set; }
        public string username { get; set; }
    }

    #endregion
}

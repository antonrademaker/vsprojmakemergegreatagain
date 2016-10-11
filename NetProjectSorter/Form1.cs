using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace NetProjectSorter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            inputText.Text = @"Input your .*proj file here";
            sortedOutput.Text = @"Output";
            Name = "Make Merge Great Again For Visual Studio .proj files";
        }

        private static readonly XNamespace nsMsBuild = "http://schemas.microsoft.com/developer/msbuild/2003";

        private void button1_Click(Object sender, EventArgs e)
        {
            XDocument xmlDoc;

            if (!TryReadXml(inputText.Text, out xmlDoc))
            {
                return;
            }

            var nodesToSort = new[] { "Reference", "Compile", "Content", "None"};
             

            var itemGroups = xmlDoc.Root.Elements(nsMsBuild+"ItemGroup");

            foreach (var nodeName in nodesToSort)
            {
                foreach (var itemGroup in itemGroups)
                {
                    var contents = itemGroup.Descendants(nsMsBuild + nodeName).Where(j => j.Attributes().Any(i => i.Name == "Include")).OrderBy(i => i.Attribute("Include").Value).ToArray();
                    itemGroup.Descendants(nsMsBuild + nodeName).Where(j => j.Attributes().Any(i => i.Name == "Include")).Remove();
                    itemGroup.Add(contents);
                }
            }

            using (TextWriter writer = new Utf8StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(writer))
                {
                    xmlDoc.Save(writer);
                }

                sortedOutput.Text = writer.ToString();
            }
        }

        private bool TryReadXml(string input, out XDocument xmlDoc)
        {
            try
            {
                xmlDoc = XDocument.Parse(input);
                return true;
            } catch (Exception e)
            {
                sortedOutput.Text = e.Message + "\r\n" + e.StackTrace;
                xmlDoc = null;
                return false;
            }
             
        }

        private void sortedOutput_KeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A) sortedOutput.SelectAll();
        }

        private void inputText_KeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A) inputText.SelectAll();

        }
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.SyntaxFiles;
using tccdotnet.Controls;

namespace tccdotnet
{
    public partial class Form1 : Form
    {
        // manages docking and floating of panels
        private DockExtender DockExtender;
        // used to make the output pane floating/draggable
        IFloaty outputFloaty;
        private static Boolean showMessage = true;

        public Form1()
        {
            InitializeComponent();
            syntaxDocument.SyntaxFile = SyntaxLanguage.CPP.ToString();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            headerMessage.Activate(tabOutput);
            headerMessage.CloseClick += new EventHandler(headerMessage_CloseClick);

            DockExtender = new DockExtender(this);
            outputFloaty = DockExtender.Attach(panelOutput, headerMessage, splitterRight);
            outputFloaty.Docking += new EventHandler(outputFloaty_Docking);
            outputFloaty.Hide();
        }

        private void headerMessage_CloseClick(object sender, EventArgs e) {
            outputFloaty.Hide();
        }
        private void outputFloaty_Docking(object sender, EventArgs e) { }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            FileStream strm;
            OpenFileDialog ofn = new OpenFileDialog();
            ofn.Filter = "C Files (*.c)|*.c|Text Files (*.txt)|*.txt";
            ofn.Title = "Type File";

            if (ofn.ShowDialog() == DialogResult.OK)
            {


                strm = new FileStream(ofn.FileName, FileMode.Open, FileAccess.Read);
                StreamReader rdr = new StreamReader(strm);
                codeEditorControl1.Document.Text = rdr.ReadToEnd();
                rdr.Close();
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            outputFloaty.Show();
            showMessage = !showMessage;

            tvParseTree.Nodes.Clear();
            Scanner scanner = new Scanner();
            //Parser parser = new Parser(scanner, new GrammarTree());
            Parser parser = new Parser(scanner);

            ParseTree tree = parser.Parse(codeEditorControl1.Document.Text);
            //object treeinstance = parsertype.InvokeMember("Parse", BindingFlags.InvokeMethod, null, parser, new object[] { input });
            //IParseTree itree = treeinstance as IParseTree;
            //int a = 0;
            //foreach (ParseError err in tree.Errors)
            //{

            //    statusError.Text = String.Format("Line: {0,3}, Column: {1,3} : {2}", err.Line, err.Column, err.Message);
            //    a++;
            //    break;
            //}
            //if (a == 0)
            //    statusError.Text = string.Format("No Errors Found.");

            //CompilerResult result = new CompilerResult();
            if (tree.Errors.Count > 0)
            {
                foreach (ParseError err in tree.Errors)
                {

                    statusError.Text = String.Format("Line: {0,3}, Column: {1,3} : {2}", err.Line, err.Column, err.Message);
                    break;
                }
            }
            else
                statusError.Text = string.Format("No Errors Found.");

            
            ParseTreeViewer.Populate(tvParseTree, tree);

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (showMessage)
            {
                outputFloaty.Show();
                tabOutput.SelectedIndex = 0;
                toolStripButton3.Text = "Hide Messages";
            }
            else
            {
                outputFloaty.Hide();
                toolStripButton3.Text = "Show Messages";
            }
            showMessage = !showMessage;
        }

    }
}

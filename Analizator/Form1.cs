using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LanguageLibrary;

namespace Analizator
{
    public partial class Form1 : Form
    {
        public Lexer LexerProcess;
        public LogError ErrorProcess;
        public Syntax SyntaxProcess;
        public Execute Compiller;

        public Form1()
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.Columns.Add("Строка", listView1.Width / 22);
            listView1.Columns.Add("Символ", listView1.Width / 22);
            listView1.Columns.Add("Анализатор", listView1.Width / 12);
            listView1.Columns.Add("Сообщение", (listView1.Width / 3)*2);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Count() > 0)
            {
                toolStripProgressBar1.Visible = true;
                toolStripStatusLabel1.Visible = true;
                toolStripStatusLabel1.Text = " Лексический анализ ....";
                LexerProcess = new Lexer(richTextBox1.Text);
                toolStripProgressBar1.Value = 34;
                ErrorProcess = new LogError();
                ErrorProcess.ParseLexemError(LexerProcess.ErrorLexems.ToList());
                SyntaxProcess = new Syntax(LexerProcess.Lexems, ErrorProcess);
                toolStripStatusLabel1.Text = " Синтаксический анализ ....";
                toolStripProgressBar1.Value = 65;
                LoadError(ErrorProcess);
                foreach(string st in SyntaxProcess.Variable)
                {
                    SyntaxProcess.resultat.result=SyntaxProcess.resultat.result.Replace(st, "1");
                }
                if (ErrorProcess.ListError.Count == 0)
                {
                    toolStripStatusLabel1.Text = " Компиляция ....";
                    Compiller = new Execute(listView1);
                    Compiller.execute(SyntaxProcess.resultat, SyntaxProcess.varname);
                    toolStripProgressBar1.Value = 100;
                    toolStripProgressBar1.Visible = false;
                    toolStripStatusLabel1.Text = " Компиляция успешно выполнена!";
                }
            }
        }


        private void LoadError(LogError err)
        {
            string temp2;
            richTextBox1.SelectAll();
            richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, FontStyle.Regular);
            richTextBox1.SelectionColor = System.Drawing.SystemColors.WindowText;
            listView1.Items.Clear();
            for (int i = 0; i < err.ListError.Count; i++)
            {
                richTextBox1.Select(err.ListError[i].ErrorLexem.Offset-err.ListError[i].ErrorLexem.Length, err.ListError[i].ErrorLexem.Length);
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, FontStyle.Underline);
                richTextBox1.SelectionColor = Color.Red;
                string temp1 = err.SourceType[err.ListError[i].ErrorSource].ToString();
                
                if(err.ListError[i].ErrorCode==1)
                    temp2 = err.ErrorsMessage[err.ListError[i].ErrorCode].ToString()+" '"+err.ListError[i].ErrorLexem.Value+"'";
                else
                    temp2 = err.ErrorsMessage[err.ListError[i].ErrorCode].ToString();
                
                listView1.Items.Add((err.ListError[i].ErrorLexem.Row+1).ToString());
                listView1.Items[i].SubItems.Add((err.ListError[i].ErrorLexem.Offset - err.ListError[i].ErrorLexem.Length).ToString());
                listView1.Items[i].SubItems.Add(temp1);
                listView1.Items[i].SubItems.Add(temp2);
                listView1.Items[i].SubItems.Add(err.ListError[i].ErrorLexem.Length.ToString());
            }
            richTextBox1.Select(0, 0);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.Select(ErrorProcess.ListError[listView1.FocusedItem.Index].ErrorLexem.Offset
                -ErrorProcess.ListError[listView1.FocusedItem.Index].ErrorLexem.Length,
                ErrorProcess.ListError[listView1.FocusedItem.Index].ErrorLexem.Length);
            richTextBox1.Focus();   
            
        }

    }
}

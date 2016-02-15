using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ExpertSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            typeof(TableLayoutPanel)
           .GetProperty("DoubleBuffered",
              System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
           .SetValue(tableLayoutPanel1, true, null);
            buildTable(15);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private int AddTableRow(List<Control> objs, int n)
        {
            int rowIndex = this.tableLayoutPanel1.RowCount++;
            
            //int percent = 100 / (n + 1);
         
            for (int i = 0; i < objs.Count; i++)
            {
                tableLayoutPanel1.Controls.Add(objs[i], i, rowIndex);
            }

            
            return rowIndex;
        }

        private void addColumns(int count)
        {
            //TODO: percents
            tableLayoutPanel1.ColumnCount = count;
        }


        /*
         * n - size of matrix
         * 
         */
        private void buildTable(int n)
        {
            tableLayoutPanel1.Visible = false;
            RowStyle style = new RowStyle(SizeType.AutoSize);
            tableLayoutPanel1.RowStyles.Add(style);
            //tableLayoutPanel1.VerticalScroll.Maximum = 200;
            const int NUMBER_OF_ADDITIONAL_COLS = 3;
            addColumns(n + NUMBER_OF_ADDITIONAL_COLS);

            // build head
            Label localCriteria = new Label();
            localCriteria.Text = "Локальный критерий";

            Label[] criterias = new Label[n];
            for (int i = 0; i < criterias.Length; i++)
            {
                criterias[i] = new Label();
                SetDoubleBuffered(criterias[i]);
                criterias[i].Text = "K" + (i + 1);
            }

            Label coef = new Label();
            coef.Text = "C";

            Label alpha = new Label();
            alpha.Text = "alpha";

            List<Control> head = new List<Control>();
            head.Add(localCriteria);
            head.AddRange(criterias);
            head.Add(coef);
            head.Add(alpha);

            AddTableRow(head, n);

            // build input rows:

            for(int i = 0; i < n; i++) 
            {
                List<Control> row = new List<Control>();
                Label name = new Label();
                name.Text = "K" + (i + 1);
                row.Add(name);

                for (int j = 0; j < n; j++)
                {
                    TextBox txtbox = new TextBox();
                    SetDoubleBuffered(txtbox);
                    row.Add(txtbox);
                }

                TextBox coefTextBox = new TextBox();
                coefTextBox.Enabled = false;
                row.Add(coefTextBox);
                TextBox alphaTextBox = new TextBox();
                alphaTextBox.Enabled = false;
                row.Add(alphaTextBox);

                AddTableRow(row, n);
            }

            tableLayoutPanel1.Visible = true;
        }

        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }
    }
}

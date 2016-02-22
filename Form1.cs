using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;

namespace ExpertSystem
{
    public partial class Form1 : Form
    {
        private int currentDim;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            typeof(TableLayoutPanel)
           .GetProperty("DoubleBuffered",
              System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
           .SetValue(tableLayoutPanel1, true, null);
            buildTable(5);
            dimension.SelectedValue = 5;
            currentDim = 5;
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
            currentDim = n;
            tableLayoutPanel1.Visible = false;
            RowStyle style = new RowStyle(SizeType.AutoSize);
            tableLayoutPanel1.RowStyles.Add(style);
            tableLayoutPanel1.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, 200);
            //tableLayoutPanel1.VerticalScroll.Maximum = 200;
            const int NUMBER_OF_ADDITIONAL_COLS = 3;
            addColumns(n + NUMBER_OF_ADDITIONAL_COLS);

            // build head
            Label localCriteria = new Label();
            localCriteria.Text = "Локальный критерий";
            localCriteria.Width = 200;

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

            int k = 0;

            for(int i = 0; i < n; i++) 
            {
                List<Control> row = new List<Control>();
                Label name = new Label();
                name.Text = "K" + (i + 1);
                row.Add(name);

                for (int j = 0; j < n; j++)
                {
                    TextBox txtbox = new TextBox();
                    Pair<int, int> cell = getCell(k);
                    if(cell.First < cell.Second)
                    {
                        txtbox.TabStop = false;
                        txtbox.ReadOnly = true;
                    }
                    txtbox.Name = Convert.ToString(k++);
                    txtbox.TextChanged += new System.EventHandler(this.handleTextChanged);
                    SetDoubleBuffered(txtbox);
                    row.Add(txtbox);
                }

                TextBox coefTextBox = new TextBox();
                coefTextBox.ReadOnly = true;
                coefTextBox.TabStop = false;
                row.Add(coefTextBox);
                TextBox alphaTextBox = new TextBox();
                alphaTextBox.ReadOnly = true;
                alphaTextBox.TabStop = false;
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

        private void dimension_SelectedValueChanged(object sender, EventArgs e)
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.ColumnCount = 1;
            int dim = Convert.ToInt32(dimension.SelectedItem);
            buildTable(dim);
        }

        private void handleTextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            Pair<int, int> cell = getCell(Convert.ToInt32(txtBox.Name));
            if (cell.First > cell.Second)
            {
                try
                {
                    double val = 1 / Convert.ToDouble(txtBox.Text);
                    setValue(cell.First, cell.Second, Convert.ToString(val));
                }
                catch (FormatException)
                {
                    MessageBox.Show("Неверный формат");
                }
            }
        }

        private void setValue(int row, int col, string str)
        {
            TextBox textBox = (TextBox) this.Controls.Find(Convert.ToString(row * currentDim + col), true)[0];
            textBox.Text = str;
        }

        private Pair<int, int> getCell(int num)
        {
            int col = num % currentDim;
            int row = num / currentDim;
            return new Pair<int, int>(col, row);
        }
    }

    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    }
}

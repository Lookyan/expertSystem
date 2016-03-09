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
        private int currentDim;
        private IDictionary<int, double> consistIndex = new Dictionary<int, double>();
        private int roundVal = 5;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            typeof(TableLayoutPanel)
           .GetProperty("DoubleBuffered",
              System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
           .SetValue(tableLayoutPanel1, true, null);
            const int startSize = 5;
            buildTable(startSize);
            dimension.SelectedValue = startSize;
            currentDim = startSize;
            consistIndex[3] = 0.58;
            consistIndex[4] = 0.9;
            consistIndex[5] = 1.12;
            consistIndex[6] = 1.24;
            consistIndex[7] = 1.32;
            consistIndex[8] = 1.41;
            consistIndex[9] = 1.45;
            consistIndex[10] = 1.49;
            consistIndex[11] = 1.51;
            consistIndex[12] = 1.54;
            consistIndex[13] = 1.56;
            consistIndex[14] = 1.57;
            consistIndex[15] = 1.58;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // coef determing
            double[] coefs = new double[currentDim];
            double[] alphas = new double[currentDim];
            double[] revs = new double[currentDim];
            double[] lastRevs = new double[currentDim];
            double[] sums = new double[currentDim];
            double fullSum = 0;

            double coefSum = 0;
            for (int i = 0; i < currentDim; i++)
            {
                coefs[i] = 1;
                for (int j = 0; j < currentDim; j++)
                {
                    try
                    {
                        coefs[i] *= getValue(i, j);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Матрица содержит недопустимые значения");
                        return;
                    }
                }
                coefs[i] = Math.Pow(coefs[i], 1.0 / currentDim);
                coefSum += coefs[i];
            }

            for (int i = 0; i < currentDim; i++)
            {
                alphas[i] = coefs[i] / coefSum;
                setAdditionalValue("coef" + i, coefs[i]);
                setAdditionalValue("alpha" + i, alphas[i]);
            }


            double lambdaMax = 0;
            // check consistency
            for (int i = 0; i < currentDim; i++)
            {
                double currentLambda = 0;
                sums[i] = 0;
                for (int j = 0; j < currentDim; j++)
                {
                    currentLambda += getValue(j, i);
                    sums[i] += getValue(i, j);
                }
                fullSum += sums[i];

                revs[i] = 1 / currentLambda;
                setAdditionalValue("rev" + i, revs[i]);

                setAdditionalValue("sum" + i, sums[i]);

                // if last column
                if(i == currentDim - 1)
                {
                    for (int l = 0; l < currentDim; l++)
                    {
                        lastRevs[l] = getValue(l, currentDim - 1) / currentLambda;
                        setAdditionalValue("lastrev" + l, lastRevs[l]);
                    }
                }
                currentLambda *= alphas[i];
                lambdaMax += currentLambda;
            }

            for (int i = 0; i < currentDim; i++)
            {
                setAdditionalValue("avg" + i, (alphas[i] + revs[i] + lastRevs[i]) / 3);
            }

            for (int i = 0; i < currentDim; i++)
            {
                setAdditionalValue("sumDivAll" + i, sums[i] / fullSum);
            }

            double consistValue = (lambdaMax - currentDim) / ((currentDim - 1) * consistIndex[currentDim]);
            cons.Text = Convert.ToString(consistValue);
            if (consistValue > 0.1)
            {
                MessageBox.Show("Не согласована. Отношение согласованности: " + Convert.ToString(consistValue));
            }
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
            const int NUMBER_OF_ADDITIONAL_COLS = 8;
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

            Label rev = new Label();
            rev.Text = "alpha*";

            Label lastRev = new Label();
            lastRev.Text = "alpha**";

            Label avg = new Label();
            avg.Text = "alpha (average)";

            Label sum = new Label();
            sum.Text = "Row sum";

            Label sumDivAll = new Label();
            sumDivAll.Text = "Row sum / whole sum";

            List<Control> head = new List<Control>();
            head.Add(localCriteria);
            head.AddRange(criterias);
            head.Add(coef);
            head.Add(alpha);
            head.Add(rev);
            head.Add(lastRev);
            head.Add(avg);
            head.Add(sum);
            head.Add(sumDivAll);

            AddTableRow(head, n);

            // build input rows:

            int k = 0;
            int additionalColCount = 0;

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
                    if (i == j)
                    {
                        txtbox.Text = "1";
                    }
                    SetDoubleBuffered(txtbox);
                    row.Add(txtbox);
                }

                TextBox coefTextBox = new TextBox();
                coefTextBox.ReadOnly = true;
                coefTextBox.TabStop = false;
                coefTextBox.Name = "coef" + additionalColCount;
                row.Add(coefTextBox);
                TextBox alphaTextBox = new TextBox();
                alphaTextBox.ReadOnly = true;
                alphaTextBox.TabStop = false;
                alphaTextBox.Name = "alpha" + additionalColCount;
                row.Add(alphaTextBox);

                TextBox revTextBox = new TextBox();
                revTextBox.ReadOnly = true;
                revTextBox.TabStop = false;
                revTextBox.Name = "rev" + additionalColCount;
                row.Add(revTextBox);

                TextBox lastRevTextBox = new TextBox();
                lastRevTextBox.ReadOnly = true;
                lastRevTextBox.TabStop = false;
                lastRevTextBox.Name = "lastrev" + additionalColCount;
                row.Add(lastRevTextBox);

                TextBox avgTextBox = new TextBox();
                avgTextBox.ReadOnly = true;
                avgTextBox.TabStop = false;
                avgTextBox.Name = "avg" + additionalColCount;
                row.Add(avgTextBox);

                TextBox sumTextBox = new TextBox();
                sumTextBox.ReadOnly = true;
                sumTextBox.TabStop = false;
                sumTextBox.Name = "sum" + additionalColCount;
                row.Add(sumTextBox);

                TextBox sumDivAllTextBox = new TextBox();
                sumDivAllTextBox.ReadOnly = true;
                sumDivAllTextBox.TabStop = false;
                sumDivAllTextBox.Name = "sumDivAll" + additionalColCount;
                row.Add(sumDivAllTextBox);

                AddTableRow(row, n);
                additionalColCount++;
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
            if (txtBox.Text == "")
            {
                return;
            }
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
            textBox.Text = Convert.ToString(Math.Round(Convert.ToDouble(str), roundVal));
        }

        private void setAdditionalValue(string name, double value)
        {
            TextBox textBox = (TextBox) this.Controls.Find(name, true)[0];
            value = Math.Round(value, roundVal);
            textBox.Text = Convert.ToString(value);
        }

        private double getValue(int row, int col)
        {
            TextBox textBox = (TextBox)this.Controls.Find(Convert.ToString(row * currentDim + col), true)[0];
            return Convert.ToDouble(textBox.Text);
        }

        private Pair<int, int> getCell(int num)
        {
            int col = num % currentDim;
            int row = num / currentDim;
            return new Pair<int, int>(col, row);
        }

        private void roundNum_ValueChanged(object sender, EventArgs e)
        {
            roundVal = Convert.ToInt32(roundNum.Value);
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Автор: Лукьянченко Александр");
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

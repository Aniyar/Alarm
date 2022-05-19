using ALARm.Core;
using ALARm.Core.Report;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ALARm_Report.controls
{
    public partial class FilterForm : Form
    {
        public ReportPeriod ReportPeriod { get; set; }
        public string ReportClasssName { get; set; }
        public List<Filter> Filters { get; set; }
        private string previousValue { get; set; } = "";

        public FilterForm()
        {
            InitializeComponent();
        }


        public void SetDataSource(List<Filter> filter)
        {
            Filters = filter;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = Filters;
            dataGridView1.Columns.Add("Name", "Имя");
            dataGridView1.Columns.Add("Value", "Значение");
            dataGridView1.Columns[0].DataPropertyName = "Name";
            dataGridView1.Columns[0].Width = 250;
            dataGridView1.Columns[0].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].DataPropertyName = "Value";
            dataGridView1.Columns[0].Resizable =  DataGridViewTriState.False;
            dataGridView1.Columns[1].Resizable = DataGridViewTriState.False;


        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DateTime parsed;
            if (!DateTime.TryParse(e.FormattedValue.ToString(), out parsed))
            {
                this.dataGridView1.CancelEdit();
            }
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
           
            var filters = (List<Filter>)dataGridView1.DataSource;
            foreach (var f in filters) {
                if (f.Value == null) {
                    MessageBox.Show("Укажите все входные параметры для формирования отчета");
                    return;
                }
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void FilterForm_Shown(object sender, EventArgs e)
        {
            var filters = (List<Filter>)dataGridView1.DataSource;
            foreach (var f in filters)
            {
                if (f.GetType() == typeof(DateFilter))
                {

                  
                    var ds = new List<string>();
                    var dt = DateTime.Parse(((DateFilter)f).Value);
                    for (int i=1; i<=DateTime.DaysInMonth(dt.Year,dt.Month); i++)
                    {
                        ds.Add($"{(i < 10 ? "0" : "")}{i}.{(dt.Month < 10 ? "0" : "")}{dt.Month}.{dt.Year}");
                    }
                    dataGridView1.Rows[filters.IndexOf(f)].Cells[1].Value = null;
                    dataGridView1.Rows[filters.IndexOf(f)].Cells[1] = new DataGridViewComboBoxCell()
                    {
                        DataSource = ds,
                        Value = ds[0],
                    };


                }
                if (f.GetType() == typeof(TripTypeFilter))
                {
                    var ds = new List<string>();
                    ds.Add("");
                    ds.Add("контрольная");
                    ds.Add("рабочая");
                    ds.Add("дополнительная");
                    dataGridView1.Rows[filters.IndexOf(f)].Cells[1].Value = null;
                    dataGridView1.Rows[filters.IndexOf(f)].Cells[1] = new DataGridViewComboBoxCell()

                    {
                        DataSource = ds,
                        Value = ds[0],
                    };
                    
                }
                if (f.GetType() == typeof(PU32TypeFilter))
                {
                    var ds = new List<string>();
                    ds.Add("");
                    ds.Add("по ПЧ");
                    ds.Add("по направлению");
                    ds.Add("cравнительная");
                    dataGridView1.Rows[filters.IndexOf(f)].Cells[1].Value = null;
                    dataGridView1.Rows[filters.IndexOf(f)].Cells[1] = new DataGridViewComboBoxCell()
                    {
                        DataSource = ds,
                        Value = ds[0],
                        

                    };

                    
                   
                   // dataGridView1. += DataGridView_CellValueChanged;


                }
                if (f.GetType() == typeof(PeriodFilter))
                {
                    var currentYear = DateTime.Now.Year;
                    var ds = new List<String>();
                    for (int y = currentYear-1; y<= currentYear+1 ; y++)
                    for (int m=1; m<=12; m++)
                    {
                            var p = new ReportPeriod() { PeriodMonth = m, PeriodYear = y };
                            ds.Add(p.ToString());
                    }
                    dataGridView1.Rows[filters.IndexOf(f)].Cells[1].Value = null;
                    dataGridView1.Rows[filters.IndexOf(f)].Cells[1] = new DataGridViewComboBoxCell()
                    {
                        DataSource = ds,
                    };
                }

            }
        }

       

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 1 && e.Control is ComboBox && dataGridView1.CurrentCell.RowIndex == 3)
            {
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.SelectedIndexChanged -= LastColumnComboSelectionChanged;
                comboBox.SelectedIndexChanged += LastColumnComboSelectionChanged;
                
            }
        }
        private void LastColumnComboSelectionChanged(object sender, EventArgs e)
        {

            var value = (sender as ComboBox).SelectedValue;
            var rowIndex = dataGridView1.CurrentRow.Index;
            if (rowIndex != 3)
                return;
            if (value != null && !value.ToString().Equals(""))
            {
                var tripType = dataGridView1.Rows[2].Cells[1].Value;
                if (ReportClasssName.Equals("PU32") && rowIndex == 3 && !value.ToString().Equals("cравнительная") && !previousValue.Equals(value)) {

                    //var filters = (List<Filter>)dataGridView1.DataSource;
                   
                    if (Filters.Count > 4)
                    {
                        Filters.RemoveRange(4, Filters.Count-4);
                    }
                    Filters.Add(new DateFilter() { Name = "Дата проверки с:", Value = ReportPeriod.StartDate.ToString("dd.MM.yyyy") });
                    Filters.Add(new DateFilter() { Name = "                         по:", Value = ReportPeriod.FinishDate.ToString("dd.MM.yyyy") });
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = Filters;
                    FilterForm_Shown(sender, e);
                    
                    (dataGridView1.Rows[3].Cells[1] as DataGridViewComboBoxCell).Value = value;
                    dataGridView1.Rows[2].Cells[1].Value = tripType;
                    previousValue = value.ToString();
                }
                if (ReportClasssName.Equals("PU32") && rowIndex == 3 && value.ToString().Equals("cравнительная") && !previousValue.Equals(value))
                {
                    if (Filters.Count > 4)
                    {
                        Filters.RemoveRange(4, Filters.Count - 4);
                    }
                    
                    Filters.Add(new PeriodFilter() { Name = "Сравниваемый период" });
                    Filters.Add(new TripTypeFilter() { Name = "Тип сравниваемой поездки" });
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = Filters;
                    FilterForm_Shown(sender, e);

                    (dataGridView1.Rows[3].Cells[1] as DataGridViewComboBoxCell).Value = value;
                    dataGridView1.Rows[2].Cells[1].Value = tripType;
                    previousValue = value.ToString();

                }
            }
                
        }
    }
}


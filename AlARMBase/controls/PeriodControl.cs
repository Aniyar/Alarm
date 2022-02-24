using System;
using System.ComponentModel;
using System.Windows.Forms;
using ALARm.Core;
using ALARm.Services;
using MetroFramework;

namespace ALARm.controls
{
    public partial class PeriodControl : UserControl
    {
        private int mtoType = -1;
        public Int64 CurrentId = -1;
        public Int64 TrackId = -1;
        // The Category attribute tells the designer to display  
        // it in the Flash grouping.   
        // The Description attribute provides a description of  
        // the property.   
        [
        Category("Flash"),
        Description("from 0 to 4")
        ]
        public int MtoType
        {
            get => mtoType;
            set
            {
                mtoType = value;

                Invalidate();
            }
        }
        public PeriodControl()
        {
            InitializeComponent();
        }
        public void DataSourceClear()
        {
            TrackId = -1;
            mgPeriod.Rows.Clear();
        }
        public event EventHandler PeriodSelectionChanged;

        private void mgPeriod_SelectionChanged(object sender, EventArgs e)
        {
            if (periodBindingSource.Count <= 0)
                return;
            if (((Period)periodBindingSource.Current).Id == 0)
                return;
            CurrentId = ((Period)periodBindingSource.Current).Id;
            PeriodSelectionChanged?.Invoke(this, e);
        }
        public void SetDataSource(Object bs)
        {
            periodBindingSource.DataSource = bs;
            if (periodBindingSource.Count > 0)
                CurrentId = ((Period)periodBindingSource.Current).Id;
        }
        public BindingSource GetDataSource()
        {
            return periodBindingSource;
        }

        internal void Build(int mtotype)
        {
            mtoType = mtotype;
            MtoType = mtotype;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (TrackId < 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_track, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var periodForm = new PeriodForm())
            {
                periodForm.ShowDialog();
                var result = periodForm.Result;
                if (result == DialogResult.Cancel)
                    return;
                var period = periodForm.Period;
                period.Track_Id = TrackId;
                period.Mto_Type = MtoType;
                
                period = MainTrackStructureService.InsertPeriod(period);

                if (period.Id == -1)
                {
                    MessageHelper.ShowError(this, alerts.insert_error);
                    return;
                }
                try
                {
                    periodBindingSource.ResetBindings(true);
                    periodBindingSource.Add(period);
                    periodBindingSource.MoveLast();
                }
                catch
                {

                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (periodBindingSource.Count > 0)
            {
                if (MetroMessageBox.Show(this, alerts.remove_ask, alerts.Delete, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        var obj = periodBindingSource.Current as Period;
                        if (obj != null)
                        {
                            if (MainTrackStructureService.DeletePeriod(obj.Id))
                            {
                                periodBindingSource.RemoveCurrent();
                                PeriodSelectionChanged?.Invoke(sender, e);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MetroMessageBox.Show(this, ex.Message, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MetroMessageBox.Show(this, alerts.empty_table, alerts.Delete, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (periodBindingSource.Count > 0)
            {
                try
                {
                    var obj = periodBindingSource.Current as Period;
                    if (obj != null)
                    {
                        using (var periodForm = new PeriodForm(obj))
                        {
                            periodForm.ShowDialog();
                            var result = periodForm.Result;
                            if (result == DialogResult.Cancel)
                                return;
                            DateTime start_date = obj.Start_Date, final_date = obj.Final_Date;
                            obj.Start_Date = periodForm.Period.Start_Date;
                            obj.Final_Date = periodForm.Period.Final_Date;

                            if (MainTrackStructureService.UpdatePeriod(obj))
                            {
                                periodBindingSource.EndEdit();
                                periodBindingSource.ResetCurrentItem();
                            }
                            else
                            {
                                obj.Start_Date = start_date;
                                obj.Final_Date = final_date;
                                periodBindingSource.EndEdit();
                                periodBindingSource.ResetCurrentItem();
                                MetroMessageBox.Show(this, alerts.edit_error + ". " + alerts.check_fields_filling, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MetroMessageBox.Show(this, ex.Message, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MetroMessageBox.Show(this, alerts.empty_table, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tsSectionPeriod_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ALARm.Core;
using AutoMapper;
using ALARm.Services;
using MetroFramework;

namespace ALARm.controls
{
    public partial class StationControl : MetroFramework.Controls.MetroUserControl
    {
        private int admLevel = -1;
        public Int64 currentUnitId = -1;
        public Int64 parentId = -1;
        [
        Category("Flash"),
        Description("from 0 to 4")
        ]
        public int AdmLevel
        {
            get
            {
                return admLevel;
            }
            set
            {
                admLevel = value;

                Invalidate();
            }
        }
        [
        Category("ShowedColumns"),
        Description("Label")
        ]
        public string Title
        {
            get
            {
                return UnitName.Text;
            }
            set
            {
                UnitName.Text = value;

            }
        }
        [
        Category("Columns"),
        Description("Show code column in metrogrid")
        ]
        public Boolean ShowCode
        {
            get
            {
                return metrogrid.Columns["codeDataGridViewTextBoxColumn"].Visible;
            }
            set
            {
                metrogrid.Columns["codeDataGridViewTextBoxColumn"].Visible = value;
            }
        }
        [
        Category("Columns"),
        Description("Show title column in metrogrid")
        ]
        public Boolean ShowTitle
        {
            get
            {
                return metrogrid.Columns[1].Visible;
            }
            set
            {
                metrogrid.Columns[1].Visible = value;
            }
        }
        [
        Category("Columns"),
        Description("Show length column in metrogrid")
        ]
        public Boolean ShowLength
        {
            get
            {
                return metrogrid.Columns[3].Visible;
            }
            set
            {
                metrogrid.Columns[3].Visible = value;
            }
        }
        [
        Category("Flash"),
        Description("Show filter")
        ]
        public Boolean ShowFilter {
            get {
                return tbSearch.Visible;
            }
            set {
                tbSearch.Visible = value;
                toolStripSeparator2.Visible = value;
            }
        }
        public StationControl()
        {
            InitializeComponent();
            metrogrid.AutoGenerateColumns = false;
        }
        public void DataSourceClear()
        {
            parentId = -1;
            metrogrid.Rows.Clear();
        }
        public void ClearSelection()
        {
            metrogrid.SelectionChanged -= metroGrid1_SelectionChanged;
            metrogrid.ClearSelection();
            metrogrid.SelectionChanged += metroGrid1_SelectionChanged;
        }
        public void SetDataSource(Object bs)
        {
            metrogrid.SelectionChanged -= metroGrid1_SelectionChanged;
            bindingSource.DataSource = bs;
            stationBindingSource.DataSource = bs;
            metrogrid.ClearSelection();
            metrogrid.SelectionChanged += metroGrid1_SelectionChanged;
            /*if (stationBindingSource.Count > 0)
                currentUnitId = ((StationObject)stationBindingSource.Current).Id;*/
        }
        public BindingSource GetDataSource()
        {
            return bindingSource;
        }

        internal void Build(string caption, int admlevel)
        {
            UnitName.Text = caption;
            AdmLevel = admlevel;
            if (admlevel == 9 || admlevel == 10)
            {
                metrogrid.Columns["codeDataGridViewTextBoxColumn"].Visible = false;
            }
            if (admlevel == 11 || admlevel == 12)
            {
                metrogrid.Columns["strack_cloumn"].Visible = true;
            }
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if (parentId < 0)
            {
                MessageHelper.ShowWarning(this, alerts.before_insert);
                return;
            }
            using (var admUnitFrm = new AdmUniForm())
            {
                if ((admLevel >= AdmStructureConst.AdmStation) && ((admLevel <= AdmStructureConst.AdmParkTrack)))
                {
                    admUnitFrm.SetStationObjectStatus(admLevel);
                }
                admUnitFrm.ShowDialog();
                var result = admUnitFrm.Result;
                if (result == DialogResult.Cancel)
                    return;
                var station = admUnitFrm.Admunit;
               
                (station as StationObject).Parent_Id = this.parentId;

                if (admLevel == AdmStructureConst.AdmStationTrack)
                {
                    (station as StationTrack).Adm_station_id = this.parentId;
                }
                else if (admLevel == AdmStructureConst.AdmParkTrack)
                {
                    (station as StationTrack).Stw_park_id = this.parentId;
                }

                station = AdmStructureService.Insert(station);
                if ((station as StationObject).Id == -1)
                {
                    MessageHelper.ShowError(this, alerts.insert_error);
                    return;
                }
                bindingSource.Add(station);
                stationBindingSource.DataSource = bindingSource.DataSource;
                stationBindingSource.ResetBindings(false);
                stationBindingSource.MoveLast();
            }
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (MetroMessageBox.Show(this, alerts.remove_ask, "Удаление...", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var obj = stationBindingSource.Current as AdmUnit;
                    if (obj != null)
                    {
                        if (AdmStructureService.Delete(obj.Id, admLevel))
                        {
                            bindingSource.RemoveAt(bindingSource.IndexOf(stationBindingSource.Current));
                            stationBindingSource.DataSource = bindingSource.DataSource;
                            stationBindingSource.ResetBindings(false);
                        }
                        //UnitSelectionChanged(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    MetroMessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EditBtn_Click(object sender, EventArgs e)
        {
            if (stationBindingSource.Count > 0)
            {
                try
                {
                    if (bindingSource[bindingSource.IndexOf(stationBindingSource.Current)] is object obj)
                    {
                        using (var admUnitFrm = new AdmUniForm((AdmUnit)obj))
                        {
                            if ((admLevel >= AdmStructureConst.AdmStation) && ((admLevel <= AdmStructureConst.AdmStationTrack)))
                            {
                                admUnitFrm.SetStationObjectStatus(admLevel);
                            }
                            admUnitFrm.ShowDialog();
                            var result = admUnitFrm.Result;
                            if (result == DialogResult.Cancel)
                                return;
                            string code = ((StationObject)obj).Code, name = ((StationObject)obj).Name, objecttype = ((StationObject)obj).Object_type;
                            int type_id = ((StationObject)obj).Type_id;
                            ((StationObject)obj).Code = ((StationObject)admUnitFrm.Admunit).Code;
                            ((StationObject)obj).Name = ((StationObject)admUnitFrm.Admunit).Name;
                            ((StationObject)obj).Object_type = ((StationObject)admUnitFrm.Admunit).Object_type;
                            ((StationObject)obj).Type_id = ((StationObject)admUnitFrm.Admunit).Type_id;
                            if (AdmStructureService.Update(obj, admLevel))
                            {
                                bindingSource.EndEdit();
                                stationBindingSource.DataSource = bindingSource.DataSource;
                                stationBindingSource.ResetBindings(false);
                            }
                            else
                            {
                                ((StationObject)obj).Code = code;
                                ((StationObject)obj).Name = name;
                                ((StationObject)obj).Type_id = type_id;
                                ((StationObject)obj).Object_type = objecttype;
                                bindingSource.EndEdit();
                                stationBindingSource.DataSource = bindingSource.DataSource;
                                stationBindingSource.ResetBindings(false);
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

        private void metroGrid1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (stationBindingSource.Count <= 0)
                    return;
                if (((AdmUnit)stationBindingSource.Current).Id == 0)
                    return;
                currentUnitId = ((AdmUnit)stationBindingSource.Current).Id;
                UnitSelectionChanged?.Invoke(this, e);
            }
            catch
            {
                return;
            }
        }
        public event EventHandler UnitSelectionChanged;

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            if (tbSearch.Visible)
            {
                string filterText = tbSearch.Text;

                if (String.IsNullOrEmpty(filterText))
                {
                    stationBindingSource.DataSource = bindingSource.DataSource;
                }
                else
                {
                    if (admLevel == AdmStructureConst.AdmStation)
                    {
                        stationBindingSource.DataSource = (bindingSource.DataSource as List<Station>).Where(s => s.Name.Contains(filterText)).ToList();
                    }
                }

                stationBindingSource.ResetBindings(false);
            }
        }
    }
}

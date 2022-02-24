using ALARm.Core;
using ALARm.Services;
using AutoMapper;
using MetroFramework;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using MetroFramework.Controls;
using System.Drawing;

namespace ALARm.controls
{
    public partial class AdmUnitControls : MetroFramework.Controls.MetroUserControl
    {
        private int admLevel = -1;
        public Int64 CurrentUnitId = -1;
        public Int64 ParentId = -1;

        // The Category attribute tells the designer to display  
        // it in the Flash grouping.   
        // The Description attribute provides a description of  
        // the property.   
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
            Category("Flash"),
            Description("Hide buttons")
        ]
        public Boolean HideButtons {
            get {
                if (AddBtn.Visible)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set {
                if (value)
                {
                    AddBtn.Visible = false;
                    EditBtn.Visible = false;
                    DeleteBtn.Visible = false;
                    AddBtn.Click -= AddBtn_Click;
                    EditBtn.Click -= EditBtn_Click;
                    DeleteBtn.Click -= DeleteBtn_Click;
                }
            }
        }

        public AdmUnitControls()
        {
            InitializeComponent();
        }

        public void DataSourceClear()
        {
            ParentId = -1;
            metrogrid.Rows.Clear();
        }


        public void SetDataSource(Object bs)
        {
            metrogrid.SelectionChanged -= MetroGrid1_SelectionChanged;
            admUnitBindingSource.DataSource = bs;
            metrogrid.ClearSelection();
            metrogrid.SelectionChanged += MetroGrid1_SelectionChanged;
            /*if (admUnitBindingSource.Count > 0)
                CurrentUnitId = ((AdmUnit)admUnitBindingSource.Current).Id;*/
        }

        public BindingSource GetDataSource()
        {
            return admUnitBindingSource;
        }

        internal void Build(string caption, int admlevel)
        {
            UnitName.Text = caption;
            AdmLevel = admlevel;
            if ((admLevel >= AdmStructureConst.AdmPchu) && (admLevel <= AdmStructureConst.AdmPdb))
            {
                metrogrid.Columns[1].DataPropertyName = "Chief_fullname";
                metrogrid.Columns[1].HeaderText = alerts.chief_full_name;
            }
            if (admLevel == AdmStructureConst.AdmTrack)
            {
                metrogrid.Columns.RemoveAt(1);
                metrogrid.Columns["TrackBorder"].Visible = true;
            }
            if (admlevel == AdmStructureConst.AdmRoad)
            {
                metrogrid.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText="Сокр", DataPropertyName="Abbr"});
                metrogrid.Columns[metrogrid.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                metrogrid.Columns[metrogrid.Columns.Count - 1].Width = 40;


            }
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if ((ParentId < 0) && !(admLevel == AdmStructureConst.AdmRoad))
            {
                MetroMessageBox.Show(this, alerts.before_insert, "Добавление...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var admUnitFrm = new AdmUniForm())
            {
                if ((admLevel >= AdmStructureConst.AdmPchu) && (admLevel <= AdmStructureConst.AdmPdb))
                    admUnitFrm.ChangeMetroLabelText(alerts.chief_full_name);
                if (admLevel == AdmStructureConst.AdmTrack)
                    admUnitFrm.HideNameControl();
                if (admLevel == AdmStructureConst.AdmDirection)
                    admUnitFrm.SetDirectionMode();
                if (admLevel == AdmStructureConst.AdmRoad)
                {
                    var mp = new MetroPanel() { Location = new System.Drawing.Point() { X = 20, Y = 134 },  Size = new Size() { Width=330, Height=45}  };
                    mp.Controls.Add(new Label() { Name = "lbAbbr", Text = "Сокращенно", Location = new System.Drawing.Point() { X = 3, Y = 3 }, Size = new System.Drawing.Size() { Width = 102, Height = 31 } });
                    mp.Controls.Add(new MetroTextBox() { Name = "tbAbbr", Text = "", Location = new System.Drawing.Point() { X = 105, Y = 3 }, Size = new System.Drawing.Size() { Width = 222, Height = 31 } });
                    admUnitFrm.Controls.Add(mp);

                }
                admUnitFrm.ShowDialog();
                var result = admUnitFrm.Result;
                if (result == DialogResult.Cancel)
                    return;
                object road = new AdmUnit();
                Mapper.Reset();
                switch (AdmLevel)
                {
                    case AdmStructureConst.AdmRoad:
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<AdmUnit, AdmRoad>();
                        });
                        road = Mapper.Map<AdmRoad>(admUnitFrm.Admunit);
                        break;
                    case AdmStructureConst.AdmNod:
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<AdmUnit, AdmNod>();
                        });
                        road = Mapper.Map<AdmNod>(admUnitFrm.Admunit);
                        break;
                    case AdmStructureConst.AdmDistance:
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<AdmUnit, AdmDistance>();
                        });
                        road = Mapper.Map<AdmDistance>(admUnitFrm.Admunit);
                        break;
                    case AdmStructureConst.AdmPchu:
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<AdmUnit, AdmPchu>();
                        });
                        road = Mapper.Map<AdmPchu>(admUnitFrm.Admunit);
                        break;
                    case AdmStructureConst.AdmPd:
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<AdmUnit, AdmPd>();
                        });
                        road = Mapper.Map<AdmPd>(admUnitFrm.Admunit);
                        break;
                    case AdmStructureConst.AdmPdb:
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<AdmUnit, AdmPdb>();
                        });
                        road = Mapper.Map<AdmPdb>(admUnitFrm.Admunit);
                        break;
                    case AdmStructureConst.AdmDirection:
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<AdmUnit, AdmDirection>();
                        });
                        road = Mapper.Map<AdmDirection>(admUnitFrm.Admunit);
                        break;
                    case AdmStructureConst.AdmTrack:
                        Mapper.Initialize(cfg =>
                        {
                            cfg.CreateMap<AdmUnit, AdmTrack>();
                        });
                        road = Mapper.Map<AdmTrack>(admUnitFrm.Admunit);
                        break;
                }
               
                ((AdmUnit) road).Parent_Id = ParentId;
                road = AdmStructureService.Insert(road);
                if (((AdmUnit) road).Id == -1)
                {
                    MetroMessageBox.Show(this, alerts.insert_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                admUnitBindingSource.Add(road);
                admUnitBindingSource.MoveLast();
            }
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (admUnitBindingSource.Count > 0)
            {
                if (MetroMessageBox.Show(this, alerts.remove_ask, alerts.Delete, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        if (admUnitBindingSource.Current is AdmUnit obj)
                        {
                            if (admLevel == AdmStructureConst.AdmDirection)
                            {
                                List<AdmRoad> roads = AdmStructureService.GetDirectionRoads(obj.Id);
                                if (roads.Count > 1)
                                {
                                    string roadString = "Текущее направление (" + obj.Name + " " + obj.Code + ") встречается на следующих дорогах:";
                                    foreach (var r in roads)
                                        roadString += " " + r.Name + " (" + r.Code + ")";
                                    roadString += "\nУдалить текущее направление со всех дорог?\nНажмите \"Нет\" для удаления с текущей дороги";
                                    DialogResult result = MetroMessageBox.Show(this, roadString, "Внимание", MessageBoxButtons.YesNoCancel);
                                    
                                    if (result == DialogResult.Yes)
                                    {
                                        if (AdmStructureService.DeleteDirection(obj.Id, ParentId, true))
                                            admUnitBindingSource.RemoveCurrent();
                                        UnitSelectionChanged?.Invoke(sender, e);
                                    }
                                    else if (result == DialogResult.No)
                                    {
                                        if (AdmStructureService.DeleteDirection(obj.Id, ParentId, false))
                                            admUnitBindingSource.RemoveCurrent();
                                        UnitSelectionChanged?.Invoke(sender, e);
                                    }
                                }
                                else if (roads.Count == 1)
                                {
                                    if (AdmStructureService.DeleteDirection(obj.Id, ParentId, true))
                                        admUnitBindingSource.RemoveCurrent();
                                    UnitSelectionChanged?.Invoke(sender, e);
                                }
                            }
                            else
                            {
                                if (AdmStructureService.Delete(obj.Id, admLevel))
                                {
                                    admUnitBindingSource.RemoveCurrent();
                                }
                                UnitSelectionChanged?.Invoke(sender, e);
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

        private void EditBtn_Click(object sender, EventArgs e) {
            if (admUnitBindingSource.Count > 0)
            {
                try
                {
                    if (admUnitBindingSource.Current is AdmUnit obj)
                    {
                        using (var admUnitFrm = new AdmUniForm(obj))
                        {
                            if ((admLevel >= AdmStructureConst.AdmPchu) && (admLevel <= AdmStructureConst.AdmPdb))
                                admUnitFrm.ChangeMetroLabelText(alerts.chief_full_name);
                            if (admLevel == AdmStructureConst.AdmTrack)
                            {
                                admUnitFrm.HideNameControl();
                            }
                            if (admLevel == AdmStructureConst.AdmRoad)
                            {
                                var mp = new MetroPanel() { Location = new System.Drawing.Point() { X = 20, Y = 134 }, Size = new Size() { Width = 330, Height = 45 } };
                                mp.Controls.Add(new Label() { Name = "lbAbbr", Text = "Сокращенно", Location = new System.Drawing.Point() { X = 3, Y = 3 }, Size = new System.Drawing.Size() { Width = 102, Height = 31 } });
                                var tbAbbr = new MetroTextBox() { Name = "tbAbbr", Text = "", Location = new System.Drawing.Point() { X = 105, Y = 3 }, Size = new System.Drawing.Size() { Width = 222, Height = 31 } };
                                tbAbbr.Text = obj.Abbr;
                                mp.Controls.Add(tbAbbr);
                                admUnitFrm.Controls.Add(mp);

                            }


                            admUnitFrm.ShowDialog();
                            var result = admUnitFrm.Result;
                            if (result == DialogResult.Cancel)
                                return;
                            string code = obj.Code, chief_fullname = obj.Chief_fullname, name = obj.Name;
                            obj.Code = ((AdmUnit)admUnitFrm.Admunit).Code;
                            obj.Chief_fullname = ((AdmUnit)admUnitFrm.Admunit).Chief_fullname;
                            obj.Name = ((AdmUnit)admUnitFrm.Admunit).Name;
                            obj.Abbr = ((AdmUnit)admUnitFrm.Admunit).Abbr;
                            if (AdmStructureService.Update(obj, admLevel))
                            {
                                admUnitBindingSource.EndEdit();
                                admUnitBindingSource.ResetCurrentItem();
                            }
                            else
                            {
                                obj.Code = code;
                                obj.Chief_fullname = chief_fullname;
                                obj.Name = name;
                                admUnitBindingSource.EndEdit();
                                admUnitBindingSource.ResetCurrentItem();
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

        private void MetroGrid1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (admUnitBindingSource.Count <= 0)
                    return;
                if (((AdmUnit)admUnitBindingSource.Current).Id==0)
                    return;
                CurrentUnitId = ((AdmUnit)admUnitBindingSource.Current).Id;
                UnitSelectionChanged?.Invoke(this, e);
            }
            catch
            {
                return;
            }

        }

        public string GetDirectionName()
        {
            return ((AdmUnit)admUnitBindingSource.Current).Name;
        }

        public List<Int64> GetTrackID()
        {
            if (admLevel == AdmStructureConst.AdmTrack)
            {
                List<Int64> trackID = new List<Int64>();
                foreach (AdmUnit obj in admUnitBindingSource.List)
                {
                    trackID.Add(obj.Id);
                }
                trackID.Sort();
                return trackID;
            }
            else
            {
                return null;
            }
        }

        public event EventHandler UnitSelectionChanged;
    }
}

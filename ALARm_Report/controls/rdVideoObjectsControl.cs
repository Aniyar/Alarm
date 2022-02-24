using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ALARm.Core;
using ALARm.Services;

namespace ALARm.controls
{
    public partial class rdVideoObjectsControl : MetroFramework.Controls.MetroUserControl
    {
        public int rdLvl = -1;
        public long currentId = -1;
        public int maxClassId = -1;
        public string filterTripsFiles = String.Empty; 
        public string filterObjects = String.Empty;
        public string filterKm = String.Empty;

        public rdVideoObjectsControl()
        {
            InitializeComponent();
            dataObjects.AutoGenerateColumns = false;
        }

        internal void Build(string caption)
        {
            rdLvl = 2;
            var rdc = RdStructureService.GetRdClasses();
            //listObjects.DataSource = ;
           // listObjects.DisplayMember = "description";
            //listObjects.ValueMember = "class_id";
            //maxClassId = RdStructureService.GetRdClasses().Last().Class_id;
            //RdStructureService.updCheck(rdLvl);
        }

        private void listObjects_filter(object sender, ItemCheckEventArgs e)
        {
            if (checkObjects.Checked == true)
            {
                this.BeginInvoke((MethodInvoker)delegate {
                    filterObjects = String.Empty;
                    if (listObjects.CheckedItems.Count == 1)
                    {
                        filterObjects = "oid=";
                        foreach (RdClasses rdClasses in listObjects.CheckedItems)
                        {
                            filterObjects += rdClasses.Class_id.ToString();
                        }
                    }
                    else
                    {
                        foreach (RdClasses rdClasses in listObjects.CheckedItems)
                        {
                            if (filterObjects != String.Empty)
                                filterObjects += " OR ";
                            filterObjects += "oid=" + rdClasses.Class_id.ToString();
                        }
                    }
                    filter_Object();
                });
            }
            else filterObjects = String.Empty;
        }

        private void startFinalKm_filter(object sender, EventArgs e)
        {
            if (checkKm.Checked == true)
            {
                filterKm = String.Empty;
                if (startKm.Text == String.Empty)
                {
                    filterKm = String.Empty;
                }
                else if (finalKm.Text == String.Empty)
                {
                    if (startKm.Text.All(char.IsDigit))
                    {
                        filterKm = "km=" + startKm.Text.ToString();
                    }
                    else
                        startKm.Text = String.Empty;
                }
                else
                {
                    if (startKm.Text.All(char.IsDigit) && finalKm.Text.All(char.IsDigit))
                    {
                        if (int.Parse(startKm.Text) < int.Parse(finalKm.Text))
                        {
                            filterKm = "km BETWEEN " + startKm.Text.ToString() + " AND " + finalKm.Text.ToString();
                        }
                        else
                        {
                            startKm.Text = String.Empty;
                            finalKm.Text = String.Empty;
                            filterKm = String.Empty;
                        }
                    }
                    else
                    {
                        startKm.Text = String.Empty;
                        finalKm.Text = String.Empty;
                    }
                }
                filter_Km();
            }
            else filterKm = String.Empty;
        }

        private void filter_Object()
        {
            if (checkKm.Checked == true && checkObjects.Checked == true)
            {
                if (filterKm != String.Empty && filterObjects != String.Empty && filterTripsFiles != String.Empty)
                {
                    bsRdVideoObjects.DataSource = RdStructureService.GetRdObjectKm(filterObjects, filterKm, filterTripsFiles);
                }
                else
                {
                    List<VideoObject> rdObjects = new List<VideoObject>();
                    bsRdVideoObjects.DataSource = rdObjects;
                }
            }
            else if (checkObjects.Checked == true)
            {
                if (filterObjects != String.Empty && filterTripsFiles != String.Empty)
                {
                    bsRdVideoObjects.DataSource = RdStructureService.GetRdObject(filterObjects, filterTripsFiles);
                }
                else
                {
                    List<VideoObject> rdObjects = new List<VideoObject>();
                    bsRdVideoObjects.DataSource = rdObjects;
                }
            }
        }

        private void filter_Km()
        {
            if (checkKm.Checked == true && checkObjects.Checked == true)
            {
                if (filterKm != String.Empty && filterObjects != String.Empty && filterTripsFiles != String.Empty)
                {
                    bsRdVideoObjects.DataSource = RdStructureService.GetRdObjectKm(filterObjects, filterKm, filterTripsFiles);
                }
                else
                {
                    List<VideoObject> rdObjects = new List<VideoObject>();
                    bsRdVideoObjects.DataSource = rdObjects;
                }
            }
            else if (checkKm.Checked == true)
            {
                if (filterKm != String.Empty && filterTripsFiles != String.Empty)
                {
                    bsRdVideoObjects.DataSource = RdStructureService.GetRdObjectKm(filterKm, filterTripsFiles);
                }
                else
                {
                    List<VideoObject> rdObjects = new List<VideoObject>();
                    bsRdVideoObjects.DataSource = rdObjects;
                }
            }
        }
    }
}

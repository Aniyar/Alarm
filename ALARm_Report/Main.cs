using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ALARm.Core;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using ALARm.Services;
using System.IO;
using ALARm_Report;
using ALARm.Core.Report;
using ALARm_Report.Forms;
using ALARm_Report.controls;
using MetroFramework;
using System.Xml.Linq;
using MetroFramework.Forms;

namespace ALARm
{
    public partial class Main : MetroFramework.Forms.MetroForm
    {
        public Main()
        {
            InitializeComponent();
            Build();
        }

        private void Build()
        {
            admRoadBindingSource.DataSource = AdmStructureService.GetUnits(AdmStructureConst.AdmRoad, -1);
            conRoadRd.SelectedIndex = -1;
            conDistanceRd.Build("Дистанции", AdmStructureConst.AdmDistance);
            conTrips.Build("Поездки");
            conTripsFiles.Build("Файлы поездки");
            conRdVideoObjects.Build("Объекты");
            cbReport.SetDataSource(RdStructureService.GetCatalog(RdStructureConst.ReportCatalog));
        }

        private void conRoadRd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (conRoadRd.SelectedIndex > -1)
            {
                conDistanceRd.dataSourceClear();
                conTrips.dataSourceClear();
                conTripsFiles.dataSourceClear();
                conDistanceRd.parentId = ((AdmUnit)conRoadRd.SelectedItem).Id;
                conDistanceRd.setDataList(conDistanceRd.parentId);
            }
        }

        private void conDirectionRd_UnitSelectionChanged(object sender, EventArgs e)
        {
            conTrips.dataSourceClear();
            conTripsFiles.dataSourceClear();
			cbPeriod.Clear();
            if (conDistanceRd.getDataCount() > 0)
            {
                conTrips.parentIDs = RdStructureService.GetAdmDirectionIDs(conDistanceRd.currentUnitId);
                conTripsFiles.parentIDs = RdStructureService.GetAdmDirectionIDs(conDistanceRd.currentUnitId);
                if (conTrips.parentIDs.Count > 0)
                {
                    conTrips.setDataPeriod(conTrips.parentIDs);
                    conTripsFiles.setDataPeriod(conTripsFiles.parentIDs);
                    conTrips_UnitSelectionChanged(sender, e);
                }
				cbPeriod.SetDataSource(RdStructureService.GetReportPeriods(conDistanceRd.currentUnitId));
            }
        }

        private void conTrips_UnitSelectionChanged(object sender, EventArgs e)
        {
            if (conTrips.getDataCount() > 0)
            {
                conTripsFiles.parentId = conTrips.currentId;
                conTripsFiles.setDataPeriod(conTripsFiles.parentId);
            }
        }

        private void conTripsFiles_UnitSelectionChanged(object sender, EventArgs e)
        {
            string filter = String.Empty;
            foreach (int id in conTripsFiles.GetCheckedItemsIDs())
            {
                if (filter != String.Empty)
                    filter += " OR ";
                filter += "fileid=" + id.ToString();
            }
            conRdVideoObjects.filterTripsFiles = filter;
        }

        private void RdSaveButton_Click(object sender, EventArgs e)
        {
            
            if (conDistanceRd.currentUnitId == -1)
            {
                MetroMessageBox.Show(this, ALARm_Report.Properties.Resources.distanceSelectionError, this.Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            int km = -1, oid = -1;
            bool bKm = false, bOid = false;
            XmlTextWriter writer = null;
            XmlTextWriter myWriter = null;
            XPathDocument myXPathDoc = null;
            XslCompiledTransform myXslTrans = new XslCompiledTransform();
            List<long> tripIDs = ((List<TripFiles>)conTripsFiles.GetDataSource()).Where(tmp => tmp.Checked_Status == true).Select(tmp => tmp.Trip_id).ToList<long>();
            List<Trips> tripsList = ((List<Trips>)conTrips.GetDataSource()).Where(tmp => tripIDs.Contains(tmp.Id)).ToList();
            List<TripFiles> tripFilesList = ((List<TripFiles>)conTripsFiles.GetDataSource()).Where(tmp => tmp.Checked_Status == true).ToList();

            if (tripFilesList.Count < 1)
            {
                MetroMessageBox.Show(this, ALARm_Report.Properties.Resources.paramDataMissing, this.Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (rbAll.Checked == true && rbSvod.Checked == false && rbDet.Checked == false)
            {
                oid = -1;

                string dirName = System.IO.Path.GetTempPath() + "report";
                DirectoryInfo dirInfo = new DirectoryInfo(dirName);

                if (!dirInfo.Exists)
                    dirInfo.Create();

                try
                {
                    XDocument xdFullDocument = new XDocument();
                    XElement xeRdVideoObjects = new XElement("rd_video_objects");

                    foreach (AdmDirection admDirection in ((List<AdmDirection>)RdStructureService.GetAdmDirection(RdStructureService.GetAdmDirectionIDs(conDistanceRd.currentUnitId))))
                    {
                        XElement xeDirection = new XElement("Direction",
                            new XAttribute("id", admDirection.Id.ToString()),
                            new XAttribute("code", admDirection.Code),
                            new XAttribute("name", admDirection.Name));
                        
                        foreach (Trips trips in tripsList)
                        {
                            if (trips.Direction_id == admDirection.Id)
                            {
                                XElement xeTrips = new XElement("Trips",
                                    new XAttribute("id", trips.Id.ToString()),
                                    new XAttribute("tripDate", trips.Trip_date.ToShortDateString()));

                                progressBar_set(RdStructureService.GetRdObjectCount(tripFilesList.Select(tmp => tmp.Id).ToList<long>()).Count * (conRdVideoObjects.maxClassId + 1));

                                foreach (VideoObjectCount videoObjectCount in RdStructureService.GetRdObjectCount(tripFilesList.Where(tmp => tmp.Trip_id == trips.Id).Select(tmp => tmp.Id).ToList<long>()))
                                {
                                    XElement xeKM = new XElement("KM",
                                        new XAttribute("km", videoObjectCount.Km),
                                        new XAttribute("amount", videoObjectCount.Count));

                                    oid = 0;
                                    
                                    while (oid <= conRdVideoObjects.maxClassId)
                                    {
                                        string str_oid = "oid = " + oid.ToString();
                                        string str_km = "km = " + videoObjectCount.Km.ToString();
                                        string str_tripfiles = "fileid in (";
                                        foreach (long fileID in tripFilesList.Select(tmp => tmp.Id).ToList<long>())
                                            str_tripfiles += fileID.ToString() + ", ";
                                        str_tripfiles = str_tripfiles.TrimEnd(new char[] { ',', ' ' });
                                        str_tripfiles += ")";

                                        List<VideoObject> videoObjects = RdStructureService.GetRdObjectKm(str_oid, str_km, str_tripfiles);

                                        XElement xeOID = new XElement("OID",
                                            new XAttribute("oid", oid.ToString()),
                                            new XAttribute("detail", admDirection.Id.ToString() + "_" + trips.Id.ToString() + "_" + videoObjectCount.Km.ToString() + "_" + oid.ToString() + ".html"),
                                            new XAttribute("name", RdStructureService.GetRdClasses(oid).Description),
                                            new XAttribute("count", videoObjects.Count.ToString()));

                                        string xmlFile = Path.GetTempPath() + "report\\" + admDirection.Id.ToString() + "_" + trips.Id.ToString() + "_" + videoObjectCount.Km.ToString() + "_" + oid.ToString() + ".xml";

                                        XDocument _xdDetailFullDocument = new XDocument();

                                        XElement _xeRdVideoObjects = new XElement("rd_video_objects");
                                        XElement _xeDirection = new XElement("Direction",
                                            new XAttribute("id", admDirection.Id.ToString()),
                                            new XAttribute("code", admDirection.Code),
                                            new XAttribute("name", admDirection.Name));
                                        XElement _xeTrips = new XElement("Trips",
                                            new XAttribute("id", trips.Id.ToString()),
                                            new XAttribute("tripDate", trips.Trip_date.ToShortDateString()));
                                        XElement _xeKM = new XElement("KM",
                                            new XAttribute("km", videoObjectCount.Km),
                                            new XAttribute("amount", videoObjectCount.Count));
                                        XElement _xeOID = new XElement("OID",
                                            new XAttribute("oid", oid.ToString()),
                                            new XAttribute("name", RdStructureService.GetRdClasses(oid).Description));

                                        foreach (VideoObject vidobj in videoObjects.OrderBy(tmp => tmp.Pt).ThenBy(tmp => tmp.Mtr))
                                        {
                                            XElement _xeObject = new XElement("Object",
                                                new XAttribute("mtr", (vidobj.Mtr + (vidobj.Pt - 1) * 100).ToString()));
                                            _xeOID.Add(_xeObject);
                                        }

                                        _xeKM.Add(_xeOID);
                                        _xeTrips.Add(_xeKM);
                                        _xeDirection.Add(_xeTrips);
                                        _xeRdVideoObjects.Add(_xeDirection);
                                        _xeRdVideoObjects.Save(xmlFile);

                                        try
                                        {
                                            myWriter = new XmlTextWriter(xmlFile.Replace(".xml", ".html"), Encoding.UTF8);
                                            myXPathDoc = new XPathDocument(xmlFile);
                                            myXslTrans.Load(xmlAndXslFiles.detailFullXSLfile);
                                            myXslTrans.Transform(myXPathDoc, null, myWriter);
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Ошибка: " + ex.Message);
                                        }
                                        finally
                                        {
                                            myWriter.Close();
                                            File.Delete(xmlFile);
                                        }

                                        xeKM.Add(xeOID);
                                        oid++;
                                        progressBar.PerformStep();
                                    }

                                    xeTrips.Add(xeKM);
                                }

                                xeDirection.Add(xeTrips);
                            }
                        }

                        xeRdVideoObjects.Add(xeDirection);
                    }
                    xdFullDocument.Add(xeRdVideoObjects);
                    xdFullDocument.Save(xmlAndXslFiles.fullXMLfile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }

                try
                {
                    myWriter = new XmlTextWriter(System.IO.Path.GetTempPath() + "report\\result_full_vo.html", Encoding.UTF8);
                    myXPathDoc = new XPathDocument(xmlAndXslFiles.fullXMLfile);
                    myXslTrans.Load(xmlAndXslFiles.fullXSLfile);
                    myXslTrans.Transform(myXPathDoc, null, myWriter);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
                finally
                {
                    myWriter.Close();
                    System.Diagnostics.Process.Start(System.IO.Path.GetTempPath() + "report\\result_full_vo.html");
                    progressBar.Value = 0;
                }
            }
            else if (rbAll.Checked == false && rbSvod.Checked == true && rbDet.Checked == false)
            {
                km = -1; oid = -1;
                bKm = false; bOid = true;

                try
                {
                    writer = new XmlTextWriter(xmlAndXslFiles.summaryXMLfile, Encoding.UTF8);

                    writer.WriteStartDocument();
                    writer.WriteStartElement("rd_video_objects");

                    foreach (AdmDirection admDirection in ((List<AdmDirection>)RdStructureService.GetAdmDirection(RdStructureService.GetAdmDirectionIDs(conDistanceRd.currentUnitId))))
                    {
                        writer.WriteStartElement("Direction");
                        writer.WriteAttributeString("id", admDirection.Id.ToString());
                        writer.WriteAttributeString("code", admDirection.Code);
                        writer.WriteAttributeString("name", admDirection.Name);

                        foreach (Trips trips in tripsList)
                        {
                            if (trips.Direction_id == admDirection.Id)
                            {
                                writer.WriteStartElement("Trips");
                                writer.WriteAttributeString("id", trips.Id.ToString());
                                writer.WriteAttributeString("tripDate", trips.Trip_date.ToShortDateString());

                                foreach (TripFiles tripFiles in tripFilesList)
                                {
                                    if (tripFiles.Trip_id == trips.Id)
                                    {
                                        foreach (VideoObjectCount rdObjectCount in ((List<VideoObjectCount>)RdStructureService.GetRdObjectCount(tripFiles.Id)))
                                        {
                                            writer.WriteStartElement("KM");
                                            writer.WriteAttributeString("km", rdObjectCount.Km.ToString());
                                            writer.WriteAttributeString("amount", rdObjectCount.Count.ToString());
                                            oid = 0;

                                            while (oid <= conRdVideoObjects.maxClassId)
                                            {
                                                writer.WriteStartElement("OID");
                                                writer.WriteAttributeString("oid", oid.ToString());
                                                writer.WriteAttributeString("name", RdStructureService.GetRdClasses(oid).Description);
                                                writer.WriteAttributeString("count", ((List<VideoObject>)RdStructureService.GetRdObject(oid, rdObjectCount.Km, tripFiles.Id)).Count.ToString());
                                                writer.WriteEndElement();
                                                oid++;
                                            }

                                            writer.WriteEndElement();
                                        }
                                    }
                                }

                                writer.WriteEndElement();
                            }
                        }

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
                try
                {
                    myWriter = new XmlTextWriter(System.IO.Path.GetTempPath() + "result_summary_vo.html", Encoding.UTF8);
                    myXPathDoc = new XPathDocument(xmlAndXslFiles.summaryXMLfile);
                    myXslTrans.Load(xmlAndXslFiles.summaryXSLfile);
                    myXslTrans.Transform(myXPathDoc, null, myWriter);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
                finally
                {
                    myWriter.Close();
                    System.Diagnostics.Process.Start(System.IO.Path.GetTempPath() + "result_summary_vo.html");
                }
            }
            else if (rbAll.Checked == false && rbSvod.Checked == false && rbDet.Checked == true)
            {
                if (conRdVideoObjects.filterTripsFiles != String.Empty)
                {
                    if (conRdVideoObjects.filterObjects != String.Empty || conRdVideoObjects.filterKm != String.Empty)
                    {
                        km = -1; oid = -1;
                        bKm = false; bOid = false;

                        if (conRdVideoObjects.filterObjects != String.Empty && conRdVideoObjects.filterKm != String.Empty)
                        {
                            try
                            {
                                writer = new XmlTextWriter(xmlAndXslFiles.detailXMLfile, Encoding.UTF8);

                                writer.WriteStartDocument();
                                writer.WriteStartElement("rd_video_objects");

                                foreach (AdmDirection admDirection in ((List<AdmDirection>)RdStructureService.GetAdmDirection(RdStructureService.GetAdmDirectionIDs(conDistanceRd.currentUnitId))))
                                {
                                    writer.WriteStartElement("Direction");
                                    writer.WriteAttributeString("id", admDirection.Id.ToString());
                                    writer.WriteAttributeString("code", admDirection.Code);
                                    writer.WriteAttributeString("name", admDirection.Name);

                                    foreach (Trips trips in tripsList)
                                    {
                                        if (trips.Direction_id == admDirection.Id)
                                        {
                                            writer.WriteStartElement("Trips");
                                            writer.WriteAttributeString("id", trips.Id.ToString());
                                            writer.WriteAttributeString("tripDate", trips.Trip_date.ToShortDateString());

                                            foreach (TripFiles tripFiles in tripFilesList)
                                            {
                                                if (tripFiles.Trip_id == trips.Id)
                                                {
                                                    foreach (VideoObject rdObject in ((List<VideoObject>)RdStructureService.GetRdObjectKm(conRdVideoObjects.filterObjects, conRdVideoObjects.filterKm, conRdVideoObjects.filterTripsFiles)).OrderBy(u => u.Km).ThenBy(u => u.Oid).ThenBy(u => u.Pt).ThenBy(u => u.Mtr))
                                                    {
                                                        if (rdObject.Fileid == tripFiles.Id)
                                                        {
                                                            if (rdObject.Km != km)
                                                            {
                                                                if (bKm)
                                                                {
                                                                    writer.WriteEndElement();
                                                                    writer.WriteEndElement();
                                                                    bKm = false;
                                                                    bOid = false;
                                                                }
                                                                writer.WriteStartElement("KM");
                                                                writer.WriteAttributeString("km", rdObject.Km.ToString());
                                                                km = rdObject.Km;
                                                                oid = -1;
                                                                bKm = true;
                                                            }

                                                            if (rdObject.Oid != oid)
                                                            {
                                                                if (bOid)
                                                                {
                                                                    writer.WriteEndElement();
                                                                    bOid = false;
                                                                }
                                                                writer.WriteStartElement("OID");
                                                                writer.WriteAttributeString("oid", rdObject.Oid.ToString());
                                                                oid = rdObject.Oid;
                                                                writer.WriteAttributeString("name", RdStructureService.GetRdClasses(oid).Description);
                                                                bOid = true;
                                                            }

                                                            writer.WriteStartElement("Object");
                                                            writer.WriteAttributeString("mtr", (rdObject.Mtr + (rdObject.Pt - 1) * 100).ToString());
                                                            writer.WriteEndElement();
                                                        }
                                                    }
                                                    km = -1; oid = -1;
                                                    bKm = false; bOid = false;
                                                    writer.WriteEndElement();
                                                    writer.WriteEndElement();
                                                }
                                            }

                                            writer.WriteEndElement();
                                        }
                                    }

                                    writer.WriteEndElement();
                                }

                                writer.WriteEndElement();
                                writer.WriteEndDocument();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ошибка: " + ex.Message);
                            }
                            finally
                            {
                                if (writer != null)
                                    writer.Close();
                            }
                        }
                        else if (conRdVideoObjects.filterObjects != String.Empty)
                        {
                            try
                            {
                                writer = new XmlTextWriter(xmlAndXslFiles.detailXMLfile, Encoding.UTF8);

                                writer.WriteStartDocument();
                                writer.WriteStartElement("rd_video_objects");

                                foreach (AdmDirection admDirection in ((List<AdmDirection>)RdStructureService.GetAdmDirection(RdStructureService.GetAdmDirectionIDs(conDistanceRd.currentUnitId))))
                                {
                                    writer.WriteStartElement("Direction");
                                    writer.WriteAttributeString("id", admDirection.Id.ToString());
                                    writer.WriteAttributeString("code", admDirection.Code);
                                    writer.WriteAttributeString("name", admDirection.Name);

                                    foreach (Trips trips in tripsList)
                                    {
                                        if (trips.Direction_id == admDirection.Id)
                                        {
                                            writer.WriteStartElement("Trips");
                                            writer.WriteAttributeString("id", trips.Id.ToString());
                                            writer.WriteAttributeString("tripDate", trips.Trip_date.ToShortDateString());
                                            
                                            foreach (VideoObject rdObject in ((List<VideoObject>)RdStructureService.GetRdObject(conRdVideoObjects.filterObjects, conRdVideoObjects.filterTripsFiles)).OrderBy(u => u.Km).ThenBy(u => u.Oid).ThenBy(u => u.Pt).ThenBy(u => u.Mtr))
                                            {
                                                if (rdObject.Km != km)
                                                {
                                                    if (bKm)
                                                    {
                                                        writer.WriteEndElement();
                                                        writer.WriteEndElement();
                                                        bKm = false;
                                                        bOid = false;
                                                    }
                                                    writer.WriteStartElement("KM");
                                                    writer.WriteAttributeString("km", rdObject.Km.ToString());
                                                    km = rdObject.Km;
                                                    oid = -1;
                                                    bKm = true;
                                                }

                                                if (rdObject.Oid != oid)
                                                {
                                                    if (bOid)
                                                    {
                                                        writer.WriteEndElement();
                                                        bOid = false;
                                                    }
                                                    writer.WriteStartElement("OID");
                                                    writer.WriteAttributeString("oid", rdObject.Oid.ToString());
                                                    oid = rdObject.Oid;
                                                    writer.WriteAttributeString("name", RdStructureService.GetRdClasses(oid).Description);
                                                    bOid = true;
                                                }

                                                writer.WriteStartElement("Object");
                                                writer.WriteAttributeString("mtr", (rdObject.Mtr + (rdObject.Pt - 1) * 100).ToString());
                                                writer.WriteEndElement();
                                            }
                                            km = -1; oid = -1;
                                            bKm = false; bOid = false;
                                            writer.WriteEndElement();
                                            writer.WriteEndElement();
                                            writer.WriteEndElement();
                                        }
                                    }

                                    writer.WriteEndElement();
                                }

                                writer.WriteEndElement();
                                writer.WriteEndDocument();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ошибка: " + ex.Message);
                            }
                            finally
                            {
                                if (writer != null)
                                    writer.Close();
                            }
                        }
                        else
                        {
                            try
                            {
                                writer = new XmlTextWriter(xmlAndXslFiles.detailXMLfile, Encoding.UTF8);

                                writer.WriteStartDocument();
                                writer.WriteStartElement("rd_video_objects");

                                foreach (AdmDirection admDirection in ((List<AdmDirection>)RdStructureService.GetAdmDirection(RdStructureService.GetAdmDirectionIDs(conDistanceRd.currentUnitId))))
                                {
                                    writer.WriteStartElement("Direction");
                                    writer.WriteAttributeString("id", admDirection.Id.ToString());
                                    writer.WriteAttributeString("code", admDirection.Code);
                                    writer.WriteAttributeString("name", admDirection.Name);

                                    foreach (Trips trips in tripsList)
                                    {
                                        if (trips.Direction_id == admDirection.Id)
                                        {
                                            writer.WriteStartElement("Trips");
                                            writer.WriteAttributeString("id", trips.Id.ToString());
                                            writer.WriteAttributeString("tripDate", trips.Trip_date.ToShortDateString());

                                            foreach (TripFiles tripFiles in tripFilesList)
                                            {
                                                if (tripFiles.Trip_id == trips.Id)
                                                {
                                                    foreach (VideoObject rdObject in ((List<VideoObject>)RdStructureService.GetRdObjectKm(conRdVideoObjects.filterKm, conRdVideoObjects.filterTripsFiles)).OrderBy(u => u.Km).ThenBy(u => u.Oid).ThenBy(u => u.Pt).ThenBy(u => u.Mtr))
                                                    {
                                                        if (rdObject.Fileid == tripFiles.Id)
                                                        {
                                                            if (rdObject.Km != km)
                                                            {
                                                                if (bKm)
                                                                {
                                                                    writer.WriteEndElement();
                                                                    writer.WriteEndElement();
                                                                    bKm = false;
                                                                    bOid = false;
                                                                }
                                                                writer.WriteStartElement("KM");
                                                                writer.WriteAttributeString("km", rdObject.Km.ToString());
                                                                km = rdObject.Km;
                                                                oid = -1;
                                                                bKm = true;
                                                            }

                                                            if (rdObject.Oid != oid)
                                                            {
                                                                if (bOid)
                                                                {
                                                                    writer.WriteEndElement();
                                                                    bOid = false;
                                                                }
                                                                writer.WriteStartElement("OID");
                                                                writer.WriteAttributeString("oid", rdObject.Oid.ToString());
                                                                oid = rdObject.Oid;
                                                                writer.WriteAttributeString("name", RdStructureService.GetRdClasses(oid).Description);
                                                                bOid = true;
                                                            }

                                                            writer.WriteStartElement("Object");
                                                            writer.WriteAttributeString("mtr", (rdObject.Mtr + (rdObject.Pt - 1) * 100).ToString());
                                                            writer.WriteEndElement();
                                                        }
                                                    }
                                                    km = -1; oid = -1;
                                                    bKm = false; bOid = false;
                                                    writer.WriteEndElement();
                                                    writer.WriteEndElement();
                                                }
                                            }

                                            writer.WriteEndElement();
                                        }
                                    }

                                    writer.WriteEndElement();
                                }

                                writer.WriteEndElement();
                                writer.WriteEndDocument();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ошибка: " + ex.Message);
                            }
                            finally
                            {
                                if (writer != null)
                                    writer.Close();
                            }
                        }

                        try
                        {
                            myWriter = new XmlTextWriter(System.IO.Path.GetTempPath() + "result_detail_vo.html", Encoding.UTF8);
                            myXPathDoc = new XPathDocument(xmlAndXslFiles.detailXMLfile);
                            myXslTrans.Load(xmlAndXslFiles.detailXSLfile);
                            myXslTrans.Transform(myXPathDoc, null, myWriter);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка: " + ex.Message);
                        }
                        finally
                        {
                            myWriter.Close();
                            System.Diagnostics.Process.Start(System.IO.Path.GetTempPath() + "result_detail_vo.html");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Фильтры не выбраны");
                    }
                }
                else
                {
                    MessageBox.Show("Фильтры не выбраны");
                }
            }
        }

        private void rbAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAll.Checked == true)
            {
                rbSvod.Checked = false;
                rbDet.Checked = false;
            }
        }

        private void rbSvod_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSvod.Checked == true)
            {
                rbAll.Checked = false;
                rbDet.Checked = false;
            }
        }

        private void rbDet_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDet.Checked == true)
            {
                rbAll.Checked = false;
                rbSvod.Checked = false;
            }
        }

        private void progressBar_set(int amount)
        {
            progressBar.Value = 0;
            progressBar.Maximum = amount;
            progressBar.Step = 1;
        }

       

        private void cbReport_SelectionChanged(object sender, EventArgs e)
        {
            lbReportTemplates.SetDataSource(RdStructureService.GetReportTemplates(cbReport.CurrentId));
        }

        private void btnReportProcess_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("ene bol busgui chinii zurag");
            if (cbReport.CurrentId == 6)
            {
                string objectToInstantiate = "ALARm_Report.Forms." + lbReportTemplates.CurrentValue.ClassName + ", ALARm_Report";
                var objectType = Type.GetType(objectToInstantiate);
                var report = Activator.CreateInstance(objectType) as Report;
                //report.Process(conDistanceRd.currentUnitId,lbReportTemplates.CurrentValue, metroProgressBar1);
            }
            else
            {
                if (conDistanceRd.currentUnitId == -1)
                {
                    MetroMessageBox.Show(this, ALARm_Report.Properties.Resources.distanceSelectionError, this.Text, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (cbPeriod.Current is null)
                {
                    MetroMessageBox.Show(this, ALARm_Report.Properties.Resources.periodSelectionError, this.Text, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (lbReportTemplates.CurrentValue is null)
                {
                    MetroMessageBox.Show(this, ALARm_Report.Properties.Resources.templateSelectionError, this.Text, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                string objectToInstantiate = "ALARm_Report.Forms." + lbReportTemplates.CurrentValue.ClassName + ", ALARm_Report";
                
                var objectType = Type.GetType(objectToInstantiate);
                var report = Activator.CreateInstance(objectType) as Report;
                report.Process(conDistanceRd.currentUnitId, lbReportTemplates.CurrentValue, cbPeriod.Current, metroProgressBar1);
            }
        }

        private void metroPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbReport_Load(object sender, EventArgs e)
        {

        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void metroPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbPeriod_Load(object sender, EventArgs e)
        {

        }
    }
}

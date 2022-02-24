using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;
using ALARm.Core;
using ALARm.Core.Report;
using ALARm.DataAccess;
using ALARm.Services;

namespace ALARm_Report.Forms
{
    public class GraphicDiagrams : Report
    {
        public int widthInPixel = 622;
        public float widthImMM = 155;
        public int xBegin = 145;
        public int picketX = -735;
        public string copyright = "ЦНТФИ";
        public string softVersion = "ALARm 1.0 (№436-р)";
        public string systemName = "ALARmDK";
        public string diagramName = "-";

        public string righstSideXslt()
        {
            return @"
        <marker id=""marker-arrow"" refX=""2"" refY=""4"" markerUnits=""strokeWidth"" orient=""auto-start-reverse"" markerWidth=""8"" markerHeight=""8"">
                <polyline id = ""markerPoly1"" points=""0,0 8,4 0,8 2,4 0,0"" fill=""blue""></polyline>
        </marker>
        <marker id=""b-circle"" viewBox=""0 0 4 4"" refX=""2"" refY=""2"" orient=""auto"">
            <circle fill=""black"" cx=""2"" cy=""2"" r=""2"" />
        </marker>
            <xsl:for-each select=""rside"">
                <xsl:for-each select=""strights"">
                    <xsl:for-each select=""stright"">
                        <line stroke-width=""1"" stroke=""red""  fill=""none"" stroke-dasharray=""0.5,1.5"">
		                    <xsl:attribute name=""x1""><xsl:value-of select=""@x1"" /></xsl:attribute>
		                    <xsl:attribute name=""x2""><xsl:value-of select=""@x2"" /></xsl:attribute>
		                    <xsl:attribute name=""y1""><xsl:value-of select=""@y1"" /></xsl:attribute>
		                    <xsl:attribute name=""y2""><xsl:value-of select=""@y2"" /></xsl:attribute>
	                </line>
                    </xsl:for-each>
                </xsl:for-each>
                <xsl:for-each select=""crosstie"">
                    <line stroke-width=""5.78"" fill=""none"">
		                <xsl:attribute name=""stroke""><xsl:value-of select=""@st"" /></xsl:attribute>
		                <xsl:attribute name=""stroke-dasharray""><xsl:value-of select=""@sw"" /></xsl:attribute>
		                <xsl:attribute name=""x1""><xsl:value-of select=""../@x8"" /></xsl:attribute>
		                <xsl:attribute name=""x2""><xsl:value-of select=""../@x8"" /></xsl:attribute>
		                <xsl:attribute name=""y1""><xsl:value-of select=""@y1"" /></xsl:attribute>
		                <xsl:attribute name=""y2""><xsl:value-of select=""@y2"" /></xsl:attribute>
	                </line>
                </xsl:for-each>
                <xsl:for-each select=""longRails"">
                    <line stroke-width=""1"" fill=""none"" stroke=""green"">
		                <xsl:attribute name=""y1""><xsl:value-of select=""@y1"" /></xsl:attribute>
		                <xsl:attribute name=""y2""><xsl:value-of select=""@y2"" /></xsl:attribute>
                        <xsl:attribute name=""x1""><xsl:value-of select=""../@lrail"" /></xsl:attribute>
		                <xsl:attribute name=""x2""><xsl:value-of select=""../@lrail"" /></xsl:attribute>
	                </line>
                </xsl:for-each>
                
                
                <xsl:for-each select=""Isojoints"">
                    <polyline  style=""fill:none;stroke:blue;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.5"">
                    <xsl:attribute name=""points"">
                            <xsl:value-of select="" @points1"" />
                        </xsl:attribute>
                 
                    </polyline>
                      <polyline  style=""fill:none;stroke:blue;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.5"">
                    <xsl:attribute name=""points"">
                            <xsl:value-of select="" @points2"" />
                        </xsl:attribute>
                 
                    </polyline>
                      <polyline  style=""fill:none;stroke:blue;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.5"">
                    <xsl:attribute name=""points"">
                            <xsl:value-of select="" @points3"" />
                        </xsl:attribute>
                 
                    </polyline>

                      <polyline  style=""fill:none;stroke:blue;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.5"">
                    <xsl:attribute name=""points"">
                            <xsl:value-of select="" @points4"" />
                        </xsl:attribute>
                 
                    </polyline>



                </xsl:for-each>


                <xsl:for-each select=""switch"">
                    <polyline marker-end=""url(#marker-arrow)"" style = ""fill:none;stroke:blue;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.5"" >
                        <xsl:attribute name = ""points"" ><xsl:value-of select = ""@points"" /></xsl:attribute>
                    </polyline>
                    <polyline style = ""fill:none;stroke:blue;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.5"" >
                        <xsl:attribute name = ""points"" ><xsl:value-of select = ""@center"" /></xsl:attribute>
                    </polyline>
                   <text font-size=""7px"" fill = ""blue"" transform=""rotate(90)"">
                        <xsl:attribute name = ""y""><xsl:value-of select = ""@y""/></xsl:attribute>
						<xsl:attribute name = ""x""><xsl:value-of select = ""@x""/></xsl:attribute>
						<xsl:value-of select = ""@num""/>
				</text>
                </xsl:for-each>
                
                <xsl:for-each select=""artcons/line"">
                    <polyline style = ""fill:none;stroke:black;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3"" >
                        <xsl:attribute name = ""points"" ><xsl:value-of select = ""."" /></xsl:attribute>
                    </polyline>
                </xsl:for-each>
                <xsl:for-each select = ""pickets/p"">
                    <text y=""" + picketX + @""" transform="" rotate(90)"">
                        <xsl:attribute name = ""x"" ><xsl:value-of select = ""@x"" /></xsl:attribute>
                        <xsl:attribute name = ""font-size""><xsl:value-of select = ""@fs"" /></xsl:attribute> 
                        <xsl:value-of select = ""."" />
                    </text>
                </xsl:for-each>
                <line stroke-width=""0.3"" stroke=""black"" fill=""none"">
                    <xsl:attribute name=""x1""><xsl:value-of select=""@x5"" /></xsl:attribute>
                    <xsl:attribute name=""x2""><xsl:value-of select=""@x5"" /></xsl:attribute>
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@minY"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                </line>
                <line stroke-width=""0.3"" stroke=""black"" fill=""none"">
                    <xsl:attribute name=""x1""><xsl:value-of select=""@x6"" /></xsl:attribute>
                    <xsl:attribute name=""x2""><xsl:value-of select=""@x6"" /></xsl:attribute>
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@minY"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                </line>
                <line stroke-width=""0.3"" stroke=""black"" fill=""none"">
                    <xsl:attribute name=""x1""><xsl:value-of select=""@x7"" /></xsl:attribute>
                    <xsl:attribute name=""x2""><xsl:value-of select=""@x7"" /></xsl:attribute>
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@minY"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                </line>
                <line stroke-dasharray=""0.3,99.7"" stroke-width=""6"" stroke=""black"" fill=""none"">
                    <xsl:attribute name=""x1""><xsl:value-of select=""@picket"" /></xsl:attribute>
                    <xsl:attribute name=""x2""><xsl:value-of select=""@picket"" /></xsl:attribute>
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@minY"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                </line>
                <line stroke-dasharray=""0.3,9.7"" stroke-width=""3"" stroke=""black"" fill=""none"">
                    <xsl:attribute name=""x1""><xsl:value-of select=""@ticks"" /></xsl:attribute>
                    <xsl:attribute name=""x2""><xsl:value-of select=""@ticks"" /></xsl:attribute>
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@minY"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                </line>
                <line stroke-width=""0.5"" stroke=""black"" fill=""none"">
                    <xsl:attribute name=""x1""><xsl:value-of select=""@x4"" /></xsl:attribute>
                    <xsl:attribute name=""x2""><xsl:value-of select=""@x4"" /></xsl:attribute>
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@minY"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                </line>
                <line stroke-width=""0.5"" stroke=""black"" fill=""none"" x1=""-20"">
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@minY"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@minY"" /></xsl:attribute>
                    <xsl:attribute name=""x2""><xsl:value-of select=""@x7"" /></xsl:attribute>
                </line>
                <line stroke-width=""0.5"" stroke=""black"" fill=""none"" x1=""-20"">
                    
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                    <xsl:attribute name=""x2""><xsl:value-of select=""@x7"" /></xsl:attribute>
                </line>
                
            </xsl:for-each>
        
        ";

        }
        public float ArtificalEntrance = 151.75f;
        public float ArtificialHeadWidth = 1.5f;

        public float MMToPixelChart(float mm)
        {
            return widthInPixel / widthImMM * mm + xBegin;
        }
        public string MMToPixelChartString(float mm)
        {
            return (widthInPixel / widthImMM * mm + xBegin).ToString().Replace(",", ".");
        }
        public string MMToPixelChartString(double mm)
        {
            return (widthInPixel / widthImMM * mm + xBegin).ToString().Replace(",", ".");
        }

        public string MMToPixelChartWidthString(float mm)
        {
            return (widthInPixel / widthImMM * mm).ToString().Replace(",", ".");
        }

        public XElement RightSideChart(DateTime travelDate, int kilometer, Direction tripDirection, long direction_id, string trackNumber, float[] xGrid)
        {
            int y1 = 1;
            int y2 = 1000;
            var result = new XElement("rside",
                new XAttribute("ticks", MMToPixelChart(xGrid[1]) - widthInPixel / widthImMM / 4),
                new XAttribute("x4", MMToPixelChart(xGrid[1])),
                new XAttribute("x5", MMToPixelChart(xGrid[0])), //151
                new XAttribute("picket",
                    MMToPixelChart(xGrid[1]) + (MMToPixelChart(xGrid[2]) - MMToPixelChart(xGrid[0])) / 2), //146 152.5f 151
                new XAttribute("x6", MMToPixelChart(xGrid[2])),
                new XAttribute("x7", MMToPixelChart(xGrid[3])),
                new XAttribute("x8",
                    MMToPixelChart(xGrid[0] + 0.75f)),// + (MMToPixelChart(152.5f) - MMToPixelChart(151f)) / 2),);
                new XAttribute("lrail",
                    MMToPixelChart(xGrid[0] - 1.75f)));// + (MMToPixelChart(152.5f) - MMToPixelChart(151f)) / 2),);
            var curves = MainTrackStructureService.GetMtoObjectsByCoord(travelDate, kilometer,
                MainTrackStructureConst.MtoCurve, direction_id, trackNumber) as List<Curve>;
            var straighteningThreads = MainTrackStructureService.GetMtoObjectsByCoord(travelDate, kilometer,
                MainTrackStructureConst.MtoStraighteningThread, direction_id, trackNumber) as List<StraighteningThread>;
            var сrossTies = MainTrackStructureService.GetMtoObjectsByCoord(travelDate, kilometer,
                MainTrackStructureConst.MtoCrossTie, direction_id, trackNumber) as List<CrossTie>;
            var nonstandardKm = MainTrackStructureService.GetMtoObjectsByCoord(travelDate, kilometer, MainTrackStructureConst.MtoNonStandard, direction_id, trackNumber) as List<NonstandardKm>;
            int kmLength = nonstandardKm.Count > 0 ? nonstandardKm[0].Len : 1000;
            //рисуем рихтовочную нить
            var strights = new XElement("strights");

            foreach (var strightThred in straighteningThreads)
            {
                int start = strightThred.Start_Km < kilometer ? 0 : strightThred.Start_M;
                int final = strightThred.Final_Km > kilometer ? kmLength : strightThred.Final_M;
                if (tripDirection == Direction.Reverse)
                {
                    start = kmLength - start;
                    final = kmLength - final;
                }
                float x = strightThred.Side_Id == (int)Side.Left ? xGrid[2] + 0.2f : xGrid[0] - 0.2f;
                strights.Add(new XElement("stright",
                    new XAttribute("x1", MMToPixelChart(x)),
                    new XAttribute("x2", MMToPixelChart(x)),
                    new XAttribute("y1", start),
                    new XAttribute("y2", final)
                    ));
            }

            foreach (var curve in curves)
            {
                int start = curve.Start_Km < kilometer ? 0 : curve.Start_M;
                int final = curve.Final_Km > kilometer ? kmLength : curve.Final_M;
                if (tripDirection == Direction.Reverse)
                {
                    start = kmLength - start;
                    final = kmLength - final;
                }
                float x = curve.Side_id == (int)Side.Left ? xGrid[2] + 0.2f : xGrid[0] - 0.2f;
                strights.Add(new XElement("stright",
                    new XAttribute("x1", MMToPixelChart(x)),
                    new XAttribute("x2", MMToPixelChart(x)),
                    new XAttribute("y1", start),
                    new XAttribute("y2", final)
                    ));
            }

            result.Add(strights);
            //рисуем шпалы
            foreach (var crossTie in сrossTies)
            {
                int start = crossTie.Start_Km == kilometer ? crossTie.Start_M : y1;
                int final = crossTie.Final_Km == kilometer ? crossTie.Final_M : y2;
                string ctype = "1,8";
                string color = "black";
                switch (crossTie.Crosstie_type_id)
                {
                    case (int)CrosTieType.Before96:
                        ctype = "1,8,1,2,1,2";
                        break;
                    case (int)CrosTieType.Concrete:
                        ctype = "1,8,1,2";
                        break;
                }
                result.Add(new XElement("crosstie",
                    new XAttribute("sw", ctype),
                    new XAttribute("st", color),
                    new XAttribute("y1", tripDirection == Direction.Reverse ? start : 1000 - start),
                    new XAttribute("y2", tripDirection == Direction.Reverse ? final : 1000 - final)
                    ));
            }
            var longRailses = MainTrackStructureService.GetMtoObjectsByCoord(travelDate, kilometer,
                MainTrackStructureConst.MtoLongRails, direction_id, trackNumber) as List<LongRails>;
            //рисуем бесстыковые пути
            foreach (var longRails in longRailses)
            {
                int start = longRails.Start_Km == kilometer ? longRails.Start_M : y1;
                int final = longRails.Final_Km == kilometer ? longRails.Final_M : y2;
                result.Add(new XElement("longRails",
                    new XAttribute("y1", tripDirection == Direction.Reverse ? start : 1000 - start),
                    new XAttribute("y2", tripDirection == Direction.Reverse ? final : 1000 - final)
                    ));
            }

            ////рисуем Изостыки
            //var Isojoints = MainTrackStructureRepository.GetMtoObjectsByCoord(travelDate, kilometer.Number,
            //    MainTrackStructureConst.MtoProfileObject, trackId) as List<ProfileObject>;

            //foreach (var Is in Isojoints)
            //{

            //    if (Is.Km != kilometer.Number && Is.Meter != kilometer.Meter)
            //        continue;

            //    int Iso = kilometer.Number == Is.Km ? Is.Meter : 0;
            //    int Isoend = kilometer.Number == Is.Km ? Is.Meter : 0;

            //    string center = (kilometer.Rep_type_cni == true ? "754" : MMToPixelChartString(xGrid[0] + 0.75f)).Replace(",", ".") + "," + -(Iso) + " " +
            //                    (kilometer.Rep_type_cni == true ? "754" : MMToPixelChartString(xGrid[0] + 0.75f)).Replace(",", ".") + "," + -(Isoend) + " ";

            //    int y = Iso;
            //    string points1 = (kilometer.Rep_type_cni == true ? "743.5" : MMToPixelChartString(xGrid[0])).Replace(",", ".") + "," + -(y + 1) + " " +
            //                  (kilometer.Rep_type_cni == true ? "746.5" : MMToPixelChartString(xGrid[0])).Replace(",", ".") + "," + -(y + 5) + " " +
            //                  (kilometer.Rep_type_cni == true ? "763" : MMToPixelChartString(xGrid[0] + 3f)).Replace(",", ".") + "," + -(y + 5) + "";

            //    string points2 = (kilometer.Rep_type_cni == true ? "743.5" : MMToPixelChartString(xGrid[0])).Replace(",", ".") + "," + -(y + 1) + " " +
            //               (kilometer.Rep_type_cni == true ? "746.5" : MMToPixelChartString(xGrid[0])).Replace(",", ".") + "," + -(y + 10) + " " +
            //               (kilometer.Rep_type_cni == true ? "763" : MMToPixelChartString(xGrid[0] + 3f)).Replace(",", ".") + "," + -(y) + "";

            //    string points3 = (kilometer.Rep_type_cni == true ? "743.5" : MMToPixelChartString(xGrid[0])).Replace(",", ".") + "," + -(y + 10) + " " +
            //               (kilometer.Rep_type_cni == true ? "746.5" : MMToPixelChartString(xGrid[0])).Replace(",", ".") + 10 + "," + -(y + 10) + " " +
            //               (kilometer.Rep_type_cni == true ? "763" : MMToPixelChartString(xGrid[0] + 3f)).Replace(",", ".") + "," + -(y + 10) + "";

            //    string points4 = (kilometer.Rep_type_cni == true ? "743.5" : MMToPixelChartString(xGrid[0])).Replace(",", ".") + "," + -(y + 10) + " " +
            //               (kilometer.Rep_type_cni == true ? "746.5" : MMToPixelChartString(xGrid[0])).Replace(",", ".") + "," + -(y + 10) + " " +
            //               (kilometer.Rep_type_cni == true ? "763" : MMToPixelChartString(xGrid[0] + 3f)).Replace(",", ".") + "," + -(y + 10) + "";

            //    result.Add(new XElement("Isojoints",
            //        new XAttribute("start", Iso - 2),
            //        new XAttribute("height", 1),
            //        new XAttribute("end", Iso + 2),
            //        //new XAttribute("points", points),
            //        new XAttribute("points1", points1),
            //        new XAttribute("points2", points2),
            //        new XAttribute("points3", points3),
            //        new XAttribute("points4", points4),
            //        new XAttribute("center", 2),
            //        new XAttribute("y", -760),
            //        new XAttribute("x", -(Isoend - 1))
            //        ));

            //}

            //рисуем стрелочные переводы
            var switches = MainTrackStructureService.GetMtoObjectsByCoord(travelDate, kilometer,
                MainTrackStructureConst.MtoSwitch, direction_id, trackNumber) as List<Switch>;

            foreach (var switchh in switches)
            {
                string points = string.Empty;
                int ostryak = switchh.Meter + (int)switchh.Dir_Id * switchh.Length;
                if (ostryak < 0)
                    ostryak = 0;

                string center = MMToPixelChartString(xGrid[0] + 0.75f).Replace(",", ".") + "," +
                                (tripDirection == Direction.Reverse ? ostryak : 1000 - ostryak) + " " +
                                MMToPixelChartString(xGrid[0] + 0.75f).Replace(",", ".") + "," +
                                (tripDirection == Direction.Reverse ? switchh.Meter : 1000 - switchh.Meter) + " ";
                int y = tripDirection == Direction.Reverse ? ostryak : 1000 - ostryak;
                string left = MMToPixelChartString(xGrid[0] - 1.5f).Replace(",", ".") + "," + (y - 2 * (int)switchh.Dir_Id) + " " +
                              MMToPixelChartString(xGrid[0] - 0.5f).Replace(",", ".") + "," + (y) + " " +
                              MMToPixelChartString(xGrid[0] + 2f).Replace(",", ".") + "," + (y);
                string right = MMToPixelChartString(xGrid[0] + 3f).Replace(",", ".") + "," + (y + 2 * (int)switchh.Dir_Id) + " " +
                               MMToPixelChartString(xGrid[0] + 2f).Replace(",", ".") + "," + (y) + " " +
                               MMToPixelChartString(xGrid[0] - 0.5f).Replace(",", ".") + "," + (y) + " ";
                if (switchh.Side_Id == Side.Right)
                {
                    points = tripDirection == Direction.Reverse ? left : right;
                }
                else
                {
                    points = tripDirection == Direction.Direct ? left : right;
                }

                var len = tripDirection == Direction.Direct ? -1 : -switchh.Length;
                int x = (switchh.Dir_Id == SwitchDirection.Direct ? switchh.Meter : ostryak) + len;

                result.Add(new XElement("switch",
                    new XAttribute("points", points),
                    new XAttribute("center", center),
                    new XAttribute("num", switchh.Num),
                    new XAttribute("y", xGrid[4]),
                    new XAttribute("x", tripDirection == Direction.Reverse ? x : 1000 - x)
                    ));
            }
            //рисуем искуственные сооружения
            var artificialConstructions = MainTrackStructureService.GetMtoObjectsByCoord(travelDate, kilometer,
                MainTrackStructureConst.MtoArtificialConstruction, direction_id, trackNumber) as List<ArtificialConstruction>;
            var artificialConstructionLines = new XElement("artcons");
            foreach (var artificialConstruction in artificialConstructions)
            {
                var start = artificialConstruction.Start_Km == kilometer ? artificialConstruction.Start_M : 0;
                var final = artificialConstruction.Final_Km == kilometer ? artificialConstruction.Final_M : 1000;
                if (tripDirection == Direction.Direct)
                {
                    start = 1000 - start;
                    final = 1000 - final;
                    int temp = start;
                    start = final;
                    final = temp;
                }
                string artConLineLeft = MMToPixelChartString(ArtificalEntrance - ArtificialHeadWidth * 1.5f).Replace(",", ".") + "," + (start - 5) + " ";
                string artConLineRight = MMToPixelChartString(ArtificalEntrance + ArtificialHeadWidth * 1.5f).Replace(",", ".") + "," + (start - 5) + " ";
                artConLineLeft += MMToPixelChartString(ArtificalEntrance - ArtificialHeadWidth).Replace(",", ".") + "," + start + " ";
                artConLineRight += MMToPixelChartString(ArtificalEntrance + ArtificialHeadWidth).Replace(",", ".") + "," + start + " ";
                artConLineLeft += MMToPixelChartString(ArtificalEntrance - ArtificialHeadWidth).Replace(",", ".") + "," + final + " ";
                artConLineRight += MMToPixelChartString(ArtificalEntrance + ArtificialHeadWidth).Replace(",", ".") + "," + final + " ";
                artConLineLeft += MMToPixelChartString(ArtificalEntrance - ArtificialHeadWidth * 1.5f).Replace(",", ".") + "," + (final + 5) + " ";
                artConLineRight += MMToPixelChartString(ArtificalEntrance + ArtificialHeadWidth * 1.5f).Replace(",", ".") + "," + (final + 5) + " ";
                artificialConstructionLines.Add(new XElement("line", artConLineLeft));
                artificialConstructionLines.Add(new XElement("line", artConLineRight));
            }
            result.Add(artificialConstructionLines);
            var pickets = new XElement("pickets");
            if (tripDirection == Direction.Direct)
            {
                for (int picket = 10; picket > 1; picket--)
                {
                    pickets.Add(new XElement("p", picket, new XAttribute("x", 1000 - ((picket - 1) * 100 + 10)), new XAttribute("fs", "8px")));
                }
                pickets.Add(new XElement("p", kilometer, new XAttribute("x", 980), new XAttribute("fs", "10px")));
            }
            else
            {
                for (int picket = 2; picket <= 10; picket++)
                {
                    pickets.Add(new XElement("p", picket, new XAttribute("x", (picket - 1) * 100 + 2), new XAttribute("fs", "8px")));
                }
                pickets.Add(new XElement("p", kilometer, new XAttribute("x", 0), new XAttribute("fs", "10px")));
            }

            result.Add(pickets);
            return result;
        }
        public string[] GetPolylines(Direction direction, List<int> meters, List<float>[] yData, float[] positions, float[] koefs)
        {
            string[] points = new string[yData.Length];
            foreach (var meter in meters)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] +=
                        MMToPixelChartString(positions[i] + koefs[i] * yData[i][meters.IndexOf(meter)]).Replace(",", ".") + "," + (direction == Direction.Direct ? meter : 1000 - meter) + " ";
                    //points[i] +=
                    //    MMToPixelChartString(positions[i] + yData[i][meters.IndexOf(meter)]).Replace(",", ".") + "," + (direction == Direction.Reverse ? meter : 1000 - meter) + " ";
                }
            }
            return points;
        }
    }
}

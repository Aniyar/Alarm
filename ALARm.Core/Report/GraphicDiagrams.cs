using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace ALARm.Core.Report
{
    public class GraphicDiagrams : Report
    {
        public int widthInPixel = 622;
        public float widthImMM = 155;
        public int xBegin = 145;
        public int picketX = -735;
        public string copyright = "ЦНТФИ";
        public string systemName = "ALARmDK";
        public string diagramName = "-";

        public double koefUrob = 1; // 0.5; //Коефицент для уровня
        public double kfPro = 0.9; //
        public double kfShab = 1.0; // 1.002;
        public double kfUrb = 1.0;
        public double kfRih = 2;

        public IAdmStructureRepository AdmStructureRepository;
        public IMainTrackStructureRepository MainTrackStructureRepository;
        public IRdStructureRepository RdStructureRepository;
        public List<DigressionMark> DigressionMarks;
        public string RighstSideXslt()
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
               
                <xsl:for-each select=""Isojointsline"">              
                   <line stroke=""blue"" stroke-width=""0.5""   >
                        <xsl:attribute name = ""x1"" ><xsl:value-of select = ""@x1"" /></xsl:attribute>
                        <xsl:attribute name = ""x2"" ><xsl:value-of select = ""@x2"" /></xsl:attribute>
                        <xsl:attribute name = ""y1"" ><xsl:value-of select = ""@y1"" /></xsl:attribute>
                        <xsl:attribute name = ""y2"" ><xsl:value-of select = ""@y1"" /></xsl:attribute>
                    </line>
                </xsl:for-each>
                    <xsl:for-each select=""Isojointsline""> 
                  <line stroke=""blue"" stroke-width=""0.5""  >
                        <xsl:attribute name = ""x1"" ><xsl:value-of select = ""@x3"" /></xsl:attribute>
                        <xsl:attribute name = ""x2"" ><xsl:value-of select = ""@x4"" /></xsl:attribute>
                        <xsl:attribute name = ""y1"" ><xsl:value-of select = ""@y3"" /></xsl:attribute>
                        <xsl:attribute name = ""y2"" ><xsl:value-of select = ""@y4"" /></xsl:attribute>
                   </line>
                    </xsl:for-each>
                    <xsl:for-each select=""Isojointsline""> 
                  <line stroke=""blue"" stroke-width=""0.5""   >
                        <xsl:attribute name = ""x1"" ><xsl:value-of select = ""@x5"" /></xsl:attribute>
                        <xsl:attribute name = ""x2"" ><xsl:value-of select = ""@x6"" /></xsl:attribute>
                        <xsl:attribute name = ""y1"" ><xsl:value-of select = ""@y5"" /></xsl:attribute>
                        <xsl:attribute name = ""y2"" ><xsl:value-of select = ""@y6"" /></xsl:attribute>
                   </line>
                    </xsl:for-each>
                    <xsl:for-each select=""Isojointsline""> 
                  <line stroke=""blue"" stroke-width=""0.5""    >
                        <xsl:attribute name = ""x1"" ><xsl:value-of select = ""@x7"" /></xsl:attribute>
                        <xsl:attribute name = ""x2"" ><xsl:value-of select = ""@x8"" /></xsl:attribute>
                        <xsl:attribute name = ""y1"" ><xsl:value-of select = ""@y7"" /></xsl:attribute>
                        <xsl:attribute name = ""y2"" ><xsl:value-of select = ""@y8"" /></xsl:attribute>
                   </line>
                    </xsl:for-each>
  

                

                <xsl:for-each select=""switch"">
                    <polyline marker-end=""url(#marker-arrow)""  style = ""fill:none;stroke:blue;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.5"" >
                        <xsl:attribute name = ""points"" ><xsl:value-of select = ""@points"" /></xsl:attribute>
                    </polyline>

                    <polyline style = ""fill:none;stroke:blue;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.5"" >
                        <xsl:attribute name = ""points"" ><xsl:value-of select = ""@center"" /></xsl:attribute>
                    </polyline>
                    <text font-size=""7px"" fill = ""blue"" transform=""rotate(90)"">
                        <xsl:attribute name = ""y""><xsl:value-of select = ""@y""/></xsl:attribute>
						<xsl:attribute name = ""x""><xsl:value-of select = ""@txtX""/></xsl:attribute>
						<xsl:value-of select = ""@num""/>
                    </text>
                    <rect width=""110"" fill=""gray"" fill-opacity=""0.3"">
                        <xsl:attribute name = ""x""><xsl:value-of select = ""@xs""/></xsl:attribute>
                        <xsl:attribute name = ""y""><xsl:value-of select = ""@start""/></xsl:attribute>
                        <xsl:attribute name = ""height""><xsl:value-of select = ""@height""/></xsl:attribute>
                    </rect>
                    <line stroke-width=""1"" fill = ""none"" stroke =""gray"" stroke-dasharray=""3,1"" x2=""750"" stroke-opacity=""0.3"">
                        <xsl:attribute name = ""x1""><xsl:value-of select = ""@linex1""/></xsl:attribute>
                        <xsl:attribute name = ""y1""><xsl:value-of select = ""@start""/></xsl:attribute>
                        <xsl:attribute name = ""y2""><xsl:value-of select = ""@start""/></xsl:attribute>
                    </line>
                    <line stroke-width=""1"" fill = ""none"" stroke =""gray"" stroke-dasharray=""3,1"" x2=""750"" stroke-opacity=""0.3"">
                        <xsl:attribute name = ""x1""><xsl:value-of select = ""@linex1""/></xsl:attribute>
                        <xsl:attribute name = ""y1""><xsl:value-of select = ""@end""/></xsl:attribute>
                        <xsl:attribute name = ""y2""><xsl:value-of select = ""@end""/></xsl:attribute>
                    </line>
                  
                </xsl:for-each>
                 <xsl:for-each select=""switchesline"">
                    <line marker-end=""url(#marker-arrow)"" marker-start=""url(#marker-arrow)""  stroke = ""blue"" stroke-width=""0.5""  >
                        <xsl:attribute name = ""x1"" ><xsl:value-of select = ""@x1"" /></xsl:attribute>
                        <xsl:attribute name = ""x2"" ><xsl:value-of select = ""@x2"" /></xsl:attribute>
                        <xsl:attribute name = ""y1"" ><xsl:value-of select = ""@y1"" /></xsl:attribute>
                        <xsl:attribute name = ""y2"" ><xsl:value-of select = ""@y1"" /></xsl:attribute>
                   </line>
                </xsl:for-each>
             
                <xsl:for-each select=""artcons/entrance"">
                    <line stroke=""grey"" stroke-width=""0.5""  fill=""none"" stroke-dasharray=""0.5,0.5"">
                        <xsl:attribute name = ""x1"" ><xsl:value-of select = ""@x2"" /></xsl:attribute>
                        <xsl:attribute name = ""x2"" ><xsl:value-of select = ""@x2"" /></xsl:attribute>
                        <xsl:attribute name = ""y1"" ><xsl:value-of select = ""@y1"" /></xsl:attribute>
                        <xsl:attribute name = ""y2"" ><xsl:value-of select = ""@y2"" /></xsl:attribute>
                    </line>
                    <line stroke=""grey"" stroke-width=""0.5""  fill=""none"" stroke-dasharray=""0.5,0.5"">
                        <xsl:attribute name = ""x1"" ><xsl:value-of select = ""@x1"" /></xsl:attribute>
                        <xsl:attribute name = ""x2"" ><xsl:value-of select = ""@x1"" /></xsl:attribute>
                        <xsl:attribute name = ""y1"" ><xsl:value-of select = ""@y1"" /></xsl:attribute>
                        <xsl:attribute name = ""y2"" ><xsl:value-of select = ""@y2"" /></xsl:attribute>
                    </line>
                  
                </xsl:for-each>
                <xsl:for-each select=""artcons/line"">
                    <polyline style = ""fill:none;stroke:black;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3"" >
                        <xsl:attribute name = ""points"" ><xsl:value-of select = ""."" /></xsl:attribute>
                    </polyline>
                    <line stroke=""black"" stroke-width=""1""  marker-start=""url(#b-circle)""  marker-end=""url(#b-circle)"">
                        <xsl:attribute name = ""x1"" ><xsl:value-of select = ""@x1"" /></xsl:attribute>
                        <xsl:attribute name = ""x2"" ><xsl:value-of select = ""@x2"" /></xsl:attribute>
                        <xsl:attribute name = ""y1"" ><xsl:value-of select = ""@y1"" /></xsl:attribute>
                        <xsl:attribute name = ""y2"" ><xsl:value-of select = ""@y1"" /></xsl:attribute>
                    </line>
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
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@minYround"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                </line>
                <line stroke-dasharray=""0.3,9.7"" stroke-width=""3"" stroke=""black"" fill=""none"">
                    <xsl:attribute name=""x1""><xsl:value-of select=""@ticks"" /></xsl:attribute>
                    <xsl:attribute name=""x2""><xsl:value-of select=""@ticks"" /></xsl:attribute>
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@minYround"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                </line>
                <line stroke-width=""0.5"" stroke=""black"" fill=""none"">
                    <xsl:attribute name=""x1""><xsl:value-of select=""@x4"" /></xsl:attribute>
                    <xsl:attribute name=""x2""><xsl:value-of select=""@x4"" /></xsl:attribute>
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@minY"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                </line>
                <line stroke-width=""0.5"" stroke=""black"" fill=""none"" x1=""217"">
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@minY"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@minY"" /></xsl:attribute>
                    <xsl:attribute name=""x2""><xsl:value-of select=""@x7"" /></xsl:attribute>
                </line>
                <line stroke-width=""0.5"" stroke=""black"" fill=""none"" x1=""217"">
                    
                    <xsl:attribute name=""y1""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                    <xsl:attribute name=""y2""><xsl:value-of select=""../@maxY"" /></xsl:attribute>
                    <xsl:attribute name=""x2""><xsl:value-of select=""@x7"" /></xsl:attribute>
                </line>
                
            </xsl:for-each>
        
        ";

        }
        //     <xsl:for-each select = ""switchesnone"">
        //              <polyline marker-end=""url(#marker-arrow)"" marker-start=""url(#marker-arrow)""   style = ""fill:none;stroke:blue;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.5"" >
        //                  <xsl:attribute name = ""points"" ><xsl:value-of select = ""@points"" /></xsl:attribute>
        //              </polyline>

        //              <polyline style = ""fill:none; stroke:blue;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.5"" >
        //                  <xsl:attribute name = ""points"" ><xsl:value-of select = ""@center"" /></xsl:attribute>
        //              </polyline>
        //              <text font-size=""7px"" fill = ""blue"" transform=""rotate(90)"">
        //                  <xsl:attribute name = ""y""><xsl:value-of select = ""@y""/></xsl:attribute>
        //<xsl:attribute name = ""x""><xsl:value-of select = ""@txtX""/></xsl:attribute>
        //<xsl:value-of select = ""@num""/>
        //              </text>
        //              <rect width = ""110"" fill=""gray"" fill-opacity=""0.3"">
        //                  <xsl:attribute name = ""x""><xsl:value-of select = ""@xs""/></xsl:attribute>
        //                  <xsl:attribute name = ""y""><xsl:value-of select = ""@start""/></xsl:attribute>
        //                  <xsl:attribute name = ""height""><xsl:value-of select = ""@height""/></xsl:attribute>
        //              </rect>
        //              <line stroke-width=""1"" fill = ""none"" stroke =""gray"" stroke-dasharray=""3,1"" x2=""750"" stroke-opacity=""0.3"">
        //                  <xsl:attribute name = ""x1""><xsl:value-of select = ""@linex1""/></xsl:attribute>
        //                  <xsl:attribute name = ""y1""><xsl:value-of select = ""@start""/></xsl:attribute>
        //                  <xsl:attribute name = ""y2""><xsl:value-of select = ""@start""/></xsl:attribute>
        //              </line>
        //              <line stroke-width=""1"" fill = ""none"" stroke =""gray"" stroke-dasharray=""3,1"" x2=""750"" stroke-opacity=""0.3"">
        //                  <xsl:attribute name = ""x1""><xsl:value-of select = ""@linex1""/></xsl:attribute>
        //                  <xsl:attribute name = ""y1""><xsl:value-of select = ""@end""/></xsl:attribute>
        //                  <xsl:attribute name = ""y2""><xsl:value-of select = ""@end""/></xsl:attribute>
        //              </line>
        //          </xsl:for-each>
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

        public XElement RightSideChart(DateTime travelDate, Kilometer kilometer, long trackId, float[] xGrid)
        {
            var GetBedomost = ((List<Bedemost>)RdStructureRepository.GetBedemost(kilometer.Trip.Id)).Where(o => o.Km == kilometer.Number).ToList();
            int y1 = kilometer.Start_m;
            int y2 = kilometer.Final_m; ;
            var result = new XElement("rside",
                new XAttribute("ticks", MMToPixelChart(xGrid[1]) - widthInPixel / widthImMM / 4),
                new XAttribute("x4", MMToPixelChart(xGrid[1])),
                new XAttribute("x5", kilometer.Rep_type_cni == true ? 751 :
                    MMToPixelChart(xGrid[0])), //151
                new XAttribute("picket",
                    MMToPixelChart(xGrid[1]) + (MMToPixelChart(xGrid[2]) - MMToPixelChart(xGrid[0])) / 2), //146 152.5f 151
                new XAttribute("x6", MMToPixelChart(xGrid[2])),
                new XAttribute("x7", MMToPixelChart(xGrid[3])),
                new XAttribute("x8", kilometer.Rep_type_cni == true ? 754 :
                    MMToPixelChart(xGrid[0] + 0.75f)),// + (MMToPixelChart(152.5f) - MMToPixelChart(151f)) / 2),);
                new XAttribute("lrail", kilometer.Rep_type_cni == true ? 745 :
                    MMToPixelChart(xGrid[0] - 1.75f)));// + (MMToPixelChart(152.5f) - MMToPixelChart(151f)) / 2),);


            //рисуем рихтовочную нить
            var strightsnode = new XElement("strights");
            {
                var st = kilometer.signTrapez[0];
                int start = kilometer.meter[0];
                int final = kilometer.meter[1];
                for (int i = 1; i < kilometer.signTrapez.Count; i++)
                {
                    if (kilometer.signTrapez[i] != st)
                    {
                        float x = st + 2 == (int)Side.Right ? xGrid[0] - 0.2f : xGrid[2] + 0.2f;
                        strightsnode.Add(
                            new XElement("stright",
                                new XAttribute("x1", kilometer.Rep_type_cni == true ? (st + 2 == (int)Side.Right ? 750 : 757.7703) : MMToPixelChart(x)),
                                new XAttribute("x2", kilometer.Rep_type_cni == true ? (st + 2 == (int)Side.Right ? 750 : 757.7703) : MMToPixelChart(x)),
                                new XAttribute("y1", -start),
                                new XAttribute("y2", -final)
                                )
                            );
                        start = final;
                        st = kilometer.signTrapez[i];
                    }
                    final = kilometer.meter[i];
                }
                if (start != final)
                {
                    float x = st + 2 == (int)Side.Right ? xGrid[0] - 0.2f : xGrid[2] + 0.2f;
                    strightsnode.Add(
                        new XElement("stright",
                            new XAttribute("x1", kilometer.Rep_type_cni == true ? (st + 2 == (int)Side.Right ? 750 : 757.7703) : MMToPixelChart(x)),
                            new XAttribute("x2", kilometer.Rep_type_cni == true ? (st + 2 == (int)Side.Right ? 750 : 757.7703) : MMToPixelChart(x)),
                            new XAttribute("y1", -start),
                            new XAttribute("y2", -final)
                            )
                        );
                }
            }

            result.Add(strightsnode);
            //рисуем шпалы
            foreach (var crossTie in kilometer.CrossTies)
            {
                int start = crossTie.Start_Km == kilometer.Number ? crossTie.Start_M : y1;
                int final = crossTie.Final_Km == kilometer.Number ? crossTie.Final_M : y2;

                string ctype = "1,8";
                string color = "black";
                switch (crossTie.Crosstie_type_id)
                {
                    case (int)CrosTieType.Before96:
                        //ctype = "1";
                        ctype = "1,8,1,2,1,2";
                        break;
                    case (int)CrosTieType.After96:
                        // ctype = "2";
                        ctype = "1,8,1,2";
                        break;
                    case (int)CrosTieType.Woody:
                        // ctype = "2";
                        ctype = "1,8";
                        break;

                }
                result.Add(new XElement("crosstie",
                    new XAttribute("sw", ctype),
                    new XAttribute("st", color),
                    new XAttribute("y1", -start),
                    new XAttribute("y2", -final)
                    ));
            }
            var longRailses = MainTrackStructureRepository.GetMtoObjectsByCoord(travelDate, kilometer.Number, MainTrackStructureConst.MtoLongRails, trackId) as List<LongRails>;
            //рисуем бесстыковые пути
            foreach (var longRails in longRailses)
            {

                int start = longRails.Start_Km == kilometer.Number ? longRails.Start_M : y1;
                int final = longRails.Final_Km == kilometer.Number ? longRails.Final_M : y2;

                result.Add(
                    new XElement("longRails",
                    new XAttribute("y1", -start),
                    new XAttribute("y2", -final))
                    );
            }
            //рисуем Изостыки
            var Isojoints = MainTrackStructureRepository.GetMtoObjectsByCoord(travelDate, kilometer.Number,
                MainTrackStructureConst.MtoProfileObject, trackId) as List<ProfileObject>;

            foreach (var Is in Isojoints)
            {
                if (Is.Km != kilometer.Number && Is.Meter != kilometer.Meter)
                    continue;
                int Iso = kilometer.Number == Is.Km ? Is.Meter : 0;

                int y = Iso;

                var xleft = MMToPixelChartString(ArtificalEntrance - ArtificialHeadWidth * 1.5f + 4).Replace(",", ".");
                var xright = MMToPixelChartString(ArtificalEntrance + ArtificialHeadWidth * 1.5f + 0.5).Replace(",", ".");
                var xleft1 = MMToPixelChartString(ArtificalEntrance - ArtificialHeadWidth * 1.5f + 3).Replace(",", ".");
                var xright1 = MMToPixelChartString(ArtificalEntrance + ArtificialHeadWidth * 1.5f).Replace(",", ".");
                var xright2 = MMToPixelChartString(ArtificalEntrance + ArtificialHeadWidth * 1.5f).Replace(",", ".");
                result.Add(new XElement("Isojointsline",
                    new XAttribute("start", Iso - 2),

                    new XAttribute("x1", xleft),
                    new XAttribute("x2", xright),
                    new XAttribute("y1", -(y + 5.5)),
                    new XAttribute("y2", -(y + 5.5)),

                    new XAttribute("x3", xleft),
                    new XAttribute("x4", xright),
                    new XAttribute("y3", -(y - 1)),
                    new XAttribute("y4", -(y - 1)),

                    new XAttribute("x5", xleft1),
                    new XAttribute("x6", xright1),
                    new XAttribute("y5", -(y + 2.3)),
                    new XAttribute("y6", -(y + 2.3)),


                    new XAttribute("x7", xright2),
                    new XAttribute("x8", xright2),
                    new XAttribute("y7", -(y - 0.6)),
                    new XAttribute("y8", -(y + 5.6)),

                    new XAttribute("height", 1),
                    new XAttribute("end", Iso + 2),
                    new XAttribute("center", 2),
                    new XAttribute("y", -760),
                    new XAttribute("x", -(Iso - 1))
                    ));



            }


            //рисуем стрелочные переводы
            var switches = MainTrackStructureRepository.GetMtoObjectsByCoord(travelDate, kilometer.Number,
                MainTrackStructureConst.MtoSwitch, trackId) as List<Switch>;

            foreach (var sw in switches)
            {
                // if (kilometer.Number.ToDoubleCoordinate(Math.Max(kilometer.Start_m, kilometer.Final_m)) < Math.Max(sw.RealStartCoordinate, sw.RealFinalCoordinate))
                //     continue;

                if (sw.Km == 711)
                {

                }
                //if (sw.Start_Km != kilometer.Number && sw.Final_Km != kilometer.Number)
                //    continue;

                if (sw.Start_Km != kilometer.Number && sw.Final_Km != kilometer.Number)
                    continue;
                //if (kilometer.Number.ToDoubleCoordinate(Math.Max(kilometer.Start_m, kilometer.Final_m)) < Math.Max(sw.RealStartCoordinate, sw.RealFinalCoordinate))
                //    continue;

                if (sw.Start_M > kilometer.Final_m)
                    continue;
                if (sw.Start_M + 10 < kilometer.Start_m)
                    continue;
                var txtX = -sw.Length / 2;
                int ostryak = kilometer.Number == sw.Start_Km ? sw.Start_M : 0;
                int end = kilometer.Number == sw.Final_Km ? sw.Final_M : sw.Start_M + sw.Length;



                //Стрелка жогарыдан сызу
                if (sw.Dir_Id == SwitchDirection.Reverse)
                //if (sw.Dir_Id == SwitchDirection.Direct)
                {
                    var temp = ostryak;
                    ostryak = end;
                    end = temp;
                    txtX = -1 * txtX;
                }

                string center = (kilometer.Rep_type_cni == true ? "754" : MMToPixelChartString(xGrid[0] + 0.75f)).Replace(",", ".") + "," + -(ostryak) + " " +
                                (kilometer.Rep_type_cni == true ? "754" : MMToPixelChartString(xGrid[0] + 0.75f)).Replace(",", ".") + "," + -(end) + " ";

                int y = ostryak;

                string rightPrSH = (kilometer.Rep_type_cni == true ? "743.5" : MMToPixelChartString(xGrid[0] - 1.5f)).Replace(",", ".") + "," + -(y + 2) + " " +
                                   (kilometer.Rep_type_cni == true ? "746.5" : MMToPixelChartString(xGrid[0] - 0.5f)).Replace(",", ".") + "," + -(y) + " " +
                                   (kilometer.Rep_type_cni == true ? "763" : MMToPixelChartString(xGrid[0] + 2f)).Replace(",", ".") + "," + -(y);


                string leftPSH = (kilometer.Rep_type_cni == true ? "763.5" : MMToPixelChartString(xGrid[0] + 3f)).Replace(",", ".") + "," + -(y - 2) + " " +
                                 (kilometer.Rep_type_cni == true ? "746.5" : MMToPixelChartString(xGrid[0] + 2f)).Replace(",", ".") + "," + -(y) + " " +
                                 (kilometer.Rep_type_cni == true ? "743.5" : MMToPixelChartString(xGrid[0] - 0.5f)).Replace(",", ".") + "," + -(y) + " ";

                string leftPrSH = (kilometer.Rep_type_cni == true ? "763" : MMToPixelChartString(xGrid[0] + 3f)).Replace(",", ".") + "," + -(y + 2) + " " +
                                  (kilometer.Rep_type_cni == true ? "761" : MMToPixelChartString(xGrid[0] + 2f)).Replace(",", ".") + "," + -(y) + " " +
                                  (kilometer.Rep_type_cni == true ? "743.5" : MMToPixelChartString(xGrid[0] - 0.5f)).Replace(",", ".") + "," + -(y) + " ";

                string rightPSH = (kilometer.Rep_type_cni == true ? "763" : MMToPixelChartString(xGrid[0] - 1.5f)).Replace(",", ".") + "," + -(y - 2) + " " +
                                  (kilometer.Rep_type_cni == true ? "761" : MMToPixelChartString(xGrid[0] - 0.5f)).Replace(",", ".") + "," + -(y) + " " +
                                  (kilometer.Rep_type_cni == true ? "743.5" : MMToPixelChartString(xGrid[0] + 2f)).Replace(",", ".") + "," + -(y);

                string notedefind = (kilometer.Rep_type_cni == true ? "763" : MMToPixelChartString(xGrid[0] - 1.5f)).Replace(",", ".") + "," + -(y - 2) + " " +
                                  (kilometer.Rep_type_cni == true ? "761" : MMToPixelChartString(xGrid[0] - 0.5f)).Replace(",", ".") + "," + -(y) + " " +
                                  (kilometer.Rep_type_cni == true ? "743.5" : MMToPixelChartString(xGrid[0] + 1.5f)).Replace(",", ".") + "," + -(y);
                //string notedefind = (kilometer.Rep_type_cni == true ? "763" : MMToPixelChartString(xGrid[0] - 1.5f)).Replace(",", ".") + "," + -(y - 2) + " " +
                //         (kilometer.Rep_type_cni == true ? "761" : MMToPixelChartString(xGrid[0] - 0.5f)).Replace(",", ".") + "," + -(y) + " " +
                //   (kilometer.Rep_type_cni == true ? "743.5" : MMToPixelChartString(xGrid[0] + 1.5f)).Replace(",", ".") + "," + -(y);

                string points = null;


                //Стрелка онга карап туру

                if (sw.Side_Id == Side.Right && sw.Dir_Id == SwitchDirection.Reverse)
                {
                    points = rightPrSH;
                }
                if (sw.Side_Id == Side.Right && sw.Dir_Id == SwitchDirection.Direct)
                {
                    points = rightPSH;
                }
                if (sw.Side_Id == Side.Left && sw.Dir_Id == SwitchDirection.Reverse)
                {
                    points = leftPrSH;
                }
                if (sw.Side_Id == Side.Left && sw.Dir_Id == SwitchDirection.Direct)
                {
                    points = leftPSH;
                }

                if (sw.Side_Id == Side.NotDefined)
                {
                    points = notedefind;

                };

                //var stylenone = "fill:none;stroke:dimgray;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3;marker - end - url(#marker-arrow);marker - start - url(#marker-arrow)";
                //var style = "fill: none; stroke: blue; vector - effect:non - scaling - stroke; stroke - linejoin:round; stroke - width:0.5; marker - end - url(#marker-arrow)";
                if (sw.Side_Id != Side.NotDefined)
                {
                    //   result.Add(new XElement("switch",
                    //new XAttribute("start", -(ostryak > end ? ostryak : end)),
                    //new XAttribute("linex1", (sw.Side_Id == Side.Right && sw.Side_Id != Side.NotDefined ? 300 : 415) + 110),
                    //new XAttribute("height", Math.Abs(ostryak - end)),
                    //new XAttribute("xs", sw.Side_Id == Side.Right && sw.Side_Id != Side.NotDefined ? 300 : 415),
                    //new XAttribute("end", -(ostryak < end ? ostryak : end)),
                    //new XAttribute("points", leftPSH),
                    //new XAttribute("center", center),
                    //new XAttribute("num", sw.Num),
                    //new XAttribute("y", -760),
                    //new XAttribute("x", -(end - 1)),
                    //new XAttribute("txtX", -(end + txtX))
                    //));

                    result.Add(new XElement("switch",
             new XAttribute("start", -(ostryak > end ? ostryak : end)),
             new XAttribute("linex1", (sw.Side_Id == Side.Right ? 300 : 415) + 110),
             new XAttribute("height", Math.Abs(ostryak - end)),
             new XAttribute("xs", sw.Side_Id == Side.Right ? 300 : 415),
             new XAttribute("end", -(ostryak < end ? ostryak : end)),
             new XAttribute("points", points),
             new XAttribute("center", center),
             new XAttribute("num", sw.Num),
             new XAttribute("y", -760),
             new XAttribute("x", -(end - 1)),
             new XAttribute("txtX", -(end + txtX))
             ));

                }
                if (sw.Side_Id == Side.NotDefined)
                {

                    result.Add(new XElement("switch",

                         new XAttribute("start", -(ostryak > end ? ostryak : end)),
                         new XAttribute("linex1", (sw.Side_Id == Side.NotDefined ? 300 : 415) + 110),
                         new XAttribute("height", Math.Abs(ostryak - end)),
                         new XAttribute("xs", sw.Side_Id == Side.NotDefined ? 300 : 415),
                         new XAttribute("end", -(ostryak < end ? ostryak : end)),
                           new XAttribute("center", center),
                         new XAttribute("num", sw.Num),
                         new XAttribute("y", -760),
                         new XAttribute("x", -(end - 1)),
                         new XAttribute("txtX", -(end + txtX))
                 ));
                    result.Add(new XElement("switchesline",
                       new XAttribute("x1", 749),
                       new XAttribute("x2", 760),
                       new XAttribute("y1", -(y)),
                       new XAttribute("y2", -(y))
                       ));
                    result.Add(new XElement("switch",
                                     new XAttribute("start", -(ostryak > end ? ostryak : end)),
                                     new XAttribute("linex1", (sw.Side_Id == Side.NotDefined ? 415 : 415) + 110),
                                     new XAttribute("height", Math.Abs(ostryak - end)),
                                     new XAttribute("xs", sw.Side_Id == Side.NotDefined ? 415 : 415),
                                     new XAttribute("end", -(ostryak < end ? ostryak : end)),
                                     new XAttribute("center", center),
                                     new XAttribute("num", sw.Num),
                                     new XAttribute("y", -760),
                                     new XAttribute("x", -(end - 1)),
                                     new XAttribute("txtX", -(end + txtX))
                                     ));




                }


            }
            //рисуем стрелочные переводы
            //var switchesnone = MainTrackStructureRepository.GetMtoObjectsByCoord(travelDate, kilometer.Number,
            //    MainTrackStructureConst.MtoSwitch, trackId) as List<Switch>;

            //foreach (var sw in switchesnone)
            //{
            //    if (sw.Start_Km != kilometer.Number && sw.Final_Km != kilometer.Number)
            //        continue;
            //    if (kilometer.Number.ToDoubleCoordinate(Math.Max(kilometer.Start_m, kilometer.Final_m)) < Math.Max(sw.RealStartCoordinate, sw.RealFinalCoordinate))
            //        continue;



            //    int ostryak = kilometer.Number == sw.Start_Km ? sw.Start_M : 0;
            //    int end = kilometer.Number == sw.Final_Km ? sw.Final_M : kilometer.Final_m;

            //    var txtX = -sw.Length / 2;

            //    //Стрелка жогарыдан сызу
            //    if (sw.Dir_Id == SwitchDirection.Reverse)
            //    {
            //        var temp = ostryak;
            //        ostryak = end;
            //        end = temp;
            //        txtX = -1 * txtX;
            //    }

            //    string center = (kilometer.Rep_type_cni == true ? "754" : MMToPixelChartString(xGrid[0] + 0.75f)).Replace(",", ".") + "," + -(ostryak) + " " +
            //                    (kilometer.Rep_type_cni == true ? "754" : MMToPixelChartString(xGrid[0] + 0.75f)).Replace(",", ".") + "," + -(end) + " ";

            //    int y = ostryak;



            //    string notedefind = (kilometer.Rep_type_cni == true ? "763" : MMToPixelChartString(xGrid[0] - 1.5f)).Replace(",", ".") + "," + -(y - 2) + " " +
            //                      (kilometer.Rep_type_cni == true ? "761" : MMToPixelChartString(xGrid[0] - 0.5f)).Replace(",", ".") + "," + -(y) + " " +
            //                      (kilometer.Rep_type_cni == true ? "743.5" : MMToPixelChartString(xGrid[0] + 1.5f)).Replace(",", ".") + "," + -(y);

            //    string points = null;

            //    if (sw.Km == 709 && sw.Meter == 592)
            //    {

            //    }
            //    //Стрелка онга карап туру



            //    if (sw.Side_Id == Side.NotDefined)
            //    {
            //        points = notedefind;

            //    }
            //    else
            //        continue;

            //    var stylenone = "fill:none;stroke:dimgray;vector-effect:non-scaling-stroke;stroke-linejoin:round;stroke-width:0.3;marker - end - url(#marker-arrow);marker - start - url(#marker-arrow)";
            //    var style = "fill: none; stroke: blue; vector - effect:non - scaling - stroke; stroke - linejoin:round; stroke - width:0.5; marker - end - url(#marker-arrow)";
            //    result.Add(new XElement("switchesnone",
            //     new XAttribute("start", -(ostryak > end ? ostryak : end)),
            //     new XAttribute("linex1", (sw.Side_Id == Side.Right ? 300 : 415) + 110),
            //     new XAttribute("height", Math.Abs(ostryak - end)),
            //     new XAttribute("xs", sw.Side_Id == Side.Right ? 300 : 415),
            //     new XAttribute("end", -(ostryak < end ? ostryak : end)),
            //     new XAttribute("points", points),
            //     new XAttribute("center", center),
            //     new XAttribute("num", sw.Num),
            //     new XAttribute("y", -760),
            //     new XAttribute("x", -(end - 1)),
            //     new XAttribute("txtX", -(end + txtX))
            //     ));
            //}








            //рисуем искуственные сооружения
            var artificialConstructions = MainTrackStructureRepository.GetMtoObjectsByCoord(travelDate, kilometer.Number,
                MainTrackStructureConst.MtoArtificialConstruction, trackId) as List<ArtificialConstruction>;
            var artificialConstructionLines = new XElement("artcons");
            foreach (var artificialConstruction in artificialConstructions)
            {
                var start = artificialConstruction.Start_Km == kilometer.Number ? artificialConstruction.Start_M : 0;
                var final = artificialConstruction.Final_Km == kilometer.Number ? artificialConstruction.Final_M : kilometer.Final_m;
                var xleft = MMToPixelChartString(ArtificalEntrance - ArtificialHeadWidth * 1.5f).Replace(",", ".");
                var xright = MMToPixelChartString(ArtificalEntrance + ArtificialHeadWidth * 1.5f).Replace(",", ".");
                var x1entrance = MMToPixelChartString(ArtificalEntrance - ArtificialHeadWidth * 1.5f + 0.5);
                var x2entrance = MMToPixelChartString(ArtificalEntrance + ArtificialHeadWidth * 1.5f - 0.5);
                if ((artificialConstruction.Start_Km == kilometer.Number) || (artificialConstruction.Final_Km == kilometer.Number))
                {
                    string artConLineLeft = xleft + "," + (-(start - 5)) + " ";
                    string artConLineRight = xright + "," + (-(start - 5)) + " ";
                    artConLineLeft += MMToPixelChartString(ArtificalEntrance - ArtificialHeadWidth).Replace(",", ".") + "," + -start + " ";
                    artConLineRight += MMToPixelChartString(ArtificalEntrance + ArtificialHeadWidth).Replace(",", ".") + "," + -start + " ";
                    artConLineLeft += MMToPixelChartString(ArtificalEntrance - ArtificialHeadWidth).Replace(",", ".") + "," + -final + " ";
                    artConLineRight += MMToPixelChartString(ArtificalEntrance + ArtificialHeadWidth).Replace(",", ".") + "," + -final + " ";
                    artConLineLeft += xleft + "," + -(final + 5) + " ";
                    artConLineRight += xright + "," + -(final + 5) + " ";
                    artificialConstructionLines.Add(new XElement("line", artConLineLeft, new XAttribute("x1", xleft), new XAttribute("x2", xright), new XAttribute("y1", -(start - 10))));
                    artificialConstructionLines.Add(new XElement("line", artConLineRight, new XAttribute("x1", xleft), new XAttribute("x2", xright), new XAttribute("y1", -(final + 10))));
                    artificialConstructionLines.Add(new XElement("entrance", new XAttribute("x1", x1entrance), new XAttribute("x2", x2entrance), new XAttribute("y1", -(start - artificialConstruction.EntranceLength > kilometer.Start_m ? start - artificialConstruction.EntranceLength : kilometer.Start_m)), new XAttribute("y2", -start)));
                    artificialConstructionLines.Add(new XElement("entrance", new XAttribute("x1", x1entrance), new XAttribute("x2", x2entrance), new XAttribute("y1", -(final + artificialConstruction.EntranceLength < kilometer.Final_m ? final + artificialConstruction.EntranceLength : kilometer.Final_m)), new XAttribute("y2", -final)));
                }

                if ((kilometer.Number == artificialConstruction.Entrance_Start_km) && (kilometer.Number != artificialConstruction.Start_Km))
                    artificialConstructionLines.Add(
                        new XElement("entrance",
                        new XAttribute("x1", x1entrance),
                        new XAttribute("x2", x2entrance),
                        new XAttribute("y1", -(artificialConstruction.Entrance_Start_m + artificialConstruction.EntranceLength < kilometer.Final_m ? artificialConstruction.Entrance_Start_m + artificialConstruction.EntranceLength : kilometer.Final_m)),
                        new XAttribute("y2", -artificialConstruction.Entrance_Start_m)));
                if ((kilometer.Number == artificialConstruction.Entrance_Final_km) && (kilometer.Number != artificialConstruction.Final_Km))
                    artificialConstructionLines.Add(
                        new XElement("entrance",
                        new XAttribute("x1", x1entrance),
                        new XAttribute("x2", x2entrance),
                        new XAttribute("y1", -(artificialConstruction.Entrance_Final_m - artificialConstruction.EntranceLength > kilometer.Start_m ? artificialConstruction.Entrance_Final_m - artificialConstruction.EntranceLength : kilometer.Start_m)),
                        new XAttribute("y2", -artificialConstruction.Entrance_Final_m)));
            }
            result.Add(artificialConstructionLines);

            var pickets = new XElement("pickets");

            for (int picket = kilometer.Start_m / 100 + 1; kilometer.Final_m / 100 >= picket; picket++)
            {
                pickets.Add(new XElement("p", picket + 1, new XAttribute("x", -((picket * 100) + 10)), new XAttribute("fs", "8px")));
            }
            pickets.Add(new XElement("p", kilometer.Number, new XAttribute("x", -(kilometer.Start_m + 20)), new XAttribute("fs", "10px")));


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
                    points[i] += MMToPixelChartString(positions[i] + yData[i][meters.IndexOf(meter)] * koefs[i]).Replace(",", ".") + "," + (direction == Direction.Reverse ? meter : 1000 - meter) + " ";
                }
            }
            return points;
        }
    }
    public class Picket
    {
        public List<int> usedTops = new List<int>();
        public int Start { private get; set; }
        public int Number { get; set; }
        public bool IsFilled
        {
            get
            {
                int count = 0;
                foreach (var note in Digression)
                {
                    if (note.Note().Contains("Уст") || (note.NotMoveAlert))
                        count += 2;
                    else
                        count++;
                }
                return count > 9;
            }
        }

        public override string ToString()
        {
            return $"n={Number} filled={IsFilled} count={Digression.Count()}";
        }
        public void WriteNotesToReport(
            Kilometer kilometer,
            List<int> speedmetres,
            XElement addParam,
            XElement digElements,
            float prosRightPosition,
            float prosLeftPosition,
            float straighRighttPosition,
            float strightLeftPosition,
            float gaugePosition,
            float levelPosition,
            GraphicDiagrams mainParameters,
            ref int fourStepOgrCoun,
            ref int otherfourStepOgrCoun)
        {
            if (kilometer.StationSection.Any())
            {

            }
            Digression = Digression.OrderBy(o => o.Meter).ToList();

            foreach (var note in Digression)
            {
                ////если занчения Speedline выходят за границу линиий начала и конца
                var startDiagram = 0;
                var finalDiagram = kilometer.Meters.Count;

                if (Math.Max(kilometer.Start_m, kilometer.Final_m) < note.Meter)
                {
                    continue;
                }
                //-----------------------------------------------------

                try
                {
                    if (note.DigName == DigressionName.PatternRetraction.Name)
                    {
                        note.DigName = note.DigName;
                    }

                    int meter = note.Meter.RoundTo10();
                    if (!((meter >= Start.RoundTo10()) && (meter < Number * 100)))
                        meter = Start + 30;
                    if (kilometer.StationSection.Any())
                        meter = Start + 20;
                    if (meter <= Start.RoundTo10() + 10)
                        meter += 10;

                    if (usedTops.Contains(meter))
                        meter = usedTops.GetNextTop(Start, meter, Number);
                    if (note.NotMoveAlert)
                    {
                        var speedline = new XElement("speedline",
                            new XAttribute("y1", -(meter + 10)),
                            new XAttribute("y2", -(meter)),
                            new XAttribute("y3", -(meter + 20)),
                            new XAttribute("Meter", note.Meter),
                            new XAttribute("points", $"188,-{ meter + 10} 195,-{ meter + 10} 195,-{note.Meter} 730,-{note.Meter}"),
                            new XAttribute("note1", $"{note.Note().Split(';')[0]}"),
                            new XAttribute("note2", note.Note().Split(';')[1]));
                        if (note.NoSquare == true)
                        {
                            XAttribute att = speedline.Attribute("y3");
                            att.Remove();
                        }
                        addParam.Add(speedline);
                        usedTops.Add(meter + 10);
                        usedTops.Add(meter);
                    }

                    //if (note.DigName.Contains("гр"))
                    //{


                    //    digElements.Add(new XElement("ogrsk",
                    //                                 new XAttribute("top", -meter),
                    //                                 new XAttribute("x", 145),
                    //                                 new XAttribute("note", note.primech),
                    //                                new XAttribute("Meter", note.Meter),
                    //                                 new XAttribute("fw", note.FontStyle())));

                    //    usedTops.Add(meter);

                    //    continue;
                    //}


                    if (note.Note().Contains("Уст.ск:"))
                    {
                        speedmetres.Add(-meter);
                        addParam.Add(new XElement("speedline",
                            new XAttribute("y1", -(meter + 10)),
                            new XAttribute("y2", -(meter)),
                                                    new XAttribute("Meter", note.Meter),
                            new XAttribute("y3", -(meter + 20)),
                            new XAttribute("points", $"188,-{ meter + 10} 195,-{ meter + 10} 195,-{note.Meter} 730,-{note.Meter}"),
                            new XAttribute("note1", $"{note.Meter} {note.Note().Split(' ')[1]}"),
                            new XAttribute("note2", "       " + note.Note().Split(' ')[2])));
                        usedTops.Add(meter.RoundTo10() + 10);
                        usedTops.Add(meter.RoundTo10());
                        continue;

                    }
                    if (note.Note().Contains("Ш:"))
                    {
                        digElements.Add(new XElement("rect",
                                                    new XAttribute("top", -meter - 9),
                                                    new XAttribute("Meter", note.Meter),
                                                    new XAttribute("x", 0)));

                        digElements.Add(new XElement("R",
                                                     new XAttribute("top", -meter),
                                                     new XAttribute("x", 1),
                                                    new XAttribute("Meter", note.Meter),
                                                     new XAttribute("note", note.Alert),
                                                     new XAttribute("fw", note.FontStyle())));

                        usedTops.Add(meter);
                        continue;
                    }
                    if (note.Note().Contains("H"))
                    {
                        //digElements.Add(new XElement("rect",
                        //                            new XAttribute("top", -meter - 9),
                        //                            new XAttribute("Meter", note.Meter),
                        //                            new XAttribute("x", 0)));

                        digElements.Add(new XElement("R",
                                                     new XAttribute("top", -meter),
                                                     new XAttribute("x", 1),
                                                    new XAttribute("Meter", note.Meter),
                                                     new XAttribute("note", note.Alert),
                                                     new XAttribute("fw", note.FontStyle())));

                        usedTops.Add(meter);
                        //  usedTops.Add(meter);
                        continue;
                    }
                    //if (note.Note().Contains("Паспортная кривая"))
                    //{
                    //    digElements.Add(new XElement("rect",
                    //                             new XAttribute("top", -meter - 9),
                    //                             new XAttribute("Meter", note.Meter),
                    //                             new XAttribute("x", 0)));

                    //    digElements.Add(new XElement("R",
                    //                                 new XAttribute("top", -meter),
                    //                                 new XAttribute("x", 1),
                    //                                new XAttribute("Meter", note.Meter),
                    //                                 new XAttribute("note", note.Note()),
                    //                                 new XAttribute("fw", note.FontStyle())));

                    //    usedTops.Add(meter);
                    //    //  usedTops.Add(meter);
                    //    continue;
                    //}

                    if (note.Note().Contains("КУ:"))
                    {
                        List<String> data = new List<String>(note.Note().Split("_".ToCharArray()));

                        if (data.Any())
                        {
                            addParam.Add(new XElement("speedline",
                                new XAttribute("y1", -(meter + 10)),
                                new XAttribute("y2", -(meter)),
                                                    new XAttribute("Meter", note.Meter),
                                new XAttribute("y3", -(meter + 20)),
                                new XAttribute("note1", $"{note.Meter} {data[0]}"),
                                new XAttribute("note2", (!note.Note().Contains("смещения") ? "       " : "") + data[1]),
                                new XAttribute("fw", note.Note().Contains("смещения") ? "bold" : "normal")));

                            usedTops.Add(meter.RoundTo10() + 10);
                            usedTops.Add(meter.RoundTo10());
                        }
                        continue;
                    }
                    if (note.Note().Contains("Стрелка") && note.Meter + 10 > kilometer.Start_m)
                    {
                        digElements.Add(new XElement("rect",
                                                    new XAttribute("top", -meter - 9),
                                                    new XAttribute("Meter", note.Meter),
                                                    new XAttribute("x", 0)));

                        digElements.Add(new XElement("R",
                                                     new XAttribute("top", -meter),
                                                    new XAttribute("Meter", note.Meter),
                                                     new XAttribute("x", 1),
                                                     new XAttribute("note", note.Note()),
                                                     new XAttribute("fw", note.FontStyle())));

                        usedTops.Add(meter);
                        continue;

                    }
                    if (note.DigName == null)
                    {
                        continue;
                    }
                    if (note.DigName.Any())
                    {
                        if (note.DigName.Contains("Рнрст"))
                        {
                            continue;
                        }
                    }


                    if (note.DigName.Contains("ПрУ"))
                    {
                        digElements.Add(new XElement("m",
                                                     new XAttribute("top", -meter),
                                                     new XAttribute("x", 1),
                                                     new XAttribute("note", note.Meter),
                                                     new XAttribute("Meter", note.Meter),
                                                     new XAttribute("fw", "normal")));

                        digElements.Add(new XElement("otst",
                                                     new XAttribute("top", -meter),
                                                     new XAttribute("x", 23),
                                                     new XAttribute("note", note.DigName),
                                                     new XAttribute("Meter", note.Meter),
                                                     new XAttribute("fw", "normal")));
                        digElements.Add(new XElement("otkl",
                                                    new XAttribute("top", -meter),
                                                    new XAttribute("x", 73),
                                                    new XAttribute("note", note.Value),
                                                    new XAttribute("Meter", note.Meter),
                                                    new XAttribute("fw", "normal")));

                        digElements.Add(new XElement("len",
                                                     new XAttribute("top", -meter),
                                                     new XAttribute("x", 102),
                                                     new XAttribute("note", note.Length),
                                                     new XAttribute("Meter", note.Meter),
                                                     new XAttribute("fw", "normal")));

                        digElements.Add(new XElement("ogrsk",
                                                     new XAttribute("top", -meter),
                                                     //new XAttribute("x", 122),
                                                     new XAttribute("x", 170),
                                                     new XAttribute("note", note.Count),
                                                     new XAttribute("Meter", note.Meter),
                                                     new XAttribute("fw", "normal")));

                        usedTops.Add(meter);
                        continue;
                    }
                    if (note.Meter == 327 && note.Km == 712)
                    {
                        note.Km = note.Km;
                    }
                    if (note.DigName.Contains("Р+") || note.DigName.Contains("Рнр+"))
                    {
                        digElements.Add(new XElement("m",
                                                     new XAttribute("top", -meter),
                                                     new XAttribute("x", 1),
                                                     new XAttribute("note", note.Meter),
                                                     new XAttribute("Meter", note.Meter),
                                                     new XAttribute("fw", note.FontStyle())));

                        digElements.Add(new XElement("otst",
                                                     new XAttribute("top", -meter),
                                                     new XAttribute("x", 23),
                                                     new XAttribute("note", note.DigName),
                                                     new XAttribute("Meter", note.Meter),
                                                     new XAttribute("fw", note.FontStyle())));

                        digElements.Add(new XElement("len",
                                                     new XAttribute("top", -meter),
                                                     new XAttribute("x", 102),
                                                     new XAttribute("note", note.Length),
                                                     new XAttribute("Meter", note.Meter),
                                                     new XAttribute("fw", note.FontStyle())));

                        digElements.Add(new XElement("ogrsk",
                                                     new XAttribute("top", -meter),
                                                     //new XAttribute("x", 145),
                                                     new XAttribute("x", 170),
                                                     new XAttribute("note", note.LimitSpeedToString()),
                                                     new XAttribute("Meter", note.Meter),
                                                     new XAttribute("fw", note.FontStyle())));

                        usedTops.Add(meter);

                        if ((note.LimitSpeedToString() != "-/-") && (note.LimitSpeedToString() != ""))
                        {
                            otherfourStepOgrCoun += 1;
                        }

                        continue;
                    }


                    if ((new[] { DigressionName.SpeedUp, DigressionName.SpeedUpNear, DigressionName.Ramp, DigressionName.RampNear, DigressionName.Psi, DigressionName.PsiNear }).Contains(note.Digression))
                    {
                        if ((note.DigName.Contains("Пси") || note.DigName.Contains("?Пси")) && note.LimitSpeedToString() == "-/-")
                            continue;

                        //if(note.DigName.Contains("Укл") || note.DigName.Contains("?Укл") || note.DigName.Contains("Пси") || note.DigName.Contains("?Пси") || note.DigName.Contains("Анп") || note.DigName.Contains("?Анп") )
                        //{
                        //    note.Value = (float)(note.Value / 100.0);
                        //}

                        var primech2 = note.Comment.Any() ? note.Comment : "";
                        var primech3 = GetMarkByNoteType("Анп");
                        if (note.Km == 706 && note.Meter > 170 && note.Meter < 200)
                        {
                            primech3 = GetMarkByNoteType("Анп");
                        }
                        digElements.Add(new XElement("m",
                                            new XAttribute("top", -(meter + 10)),
                                            new XAttribute("x", 1),
                                            new XAttribute("note", note.Meter),
                                                    new XAttribute("Meter", note.Meter),
                                            new XAttribute("fw", note.FontStyle())));

                        digElements.Add(new XElement("otst",
                                            new XAttribute("top", -(meter + 10)),
                                            new XAttribute("x", 23),
                                            new XAttribute("note", note.DigName),
                                                    new XAttribute("Meter", note.Meter),
                                            new XAttribute("fw", note.FontStyle())));
                        digElements.Add(new XElement("otst",
                                          new XAttribute("top", -(meter + 10)),
                                          new XAttribute("x", 220),
                                          new XAttribute("note", primech3),
                                                  new XAttribute("Meter", note.Meter),
                                          new XAttribute("fw", note.FontStyle())));

                        digElements.Add(new XElement("otst",
                                         new XAttribute("top", -(meter + 10)),
                                         new XAttribute("x", 60),
                                         new XAttribute("note", primech2),
                                                 new XAttribute("Meter", note.Meter),
                                         new XAttribute("fw", note.FontStyle())));

                        digElements.Add(new XElement("len",
                                            new XAttribute("top", -(meter + 10)),
                                            new XAttribute("x", 102),
                                            new XAttribute("note", (note.DigName.Contains("Пси") || note.DigName.Contains("?Пси") ? "" : note.Length.ToString())),
                                                    new XAttribute("Meter", note.Meter),
                                            new XAttribute("fw", note.FontStyle())));

                        digElements.Add(new XElement("ogrsk",
                                            new XAttribute("top", -(meter + 10)),
                                            //new XAttribute("x", 145),
                                            new XAttribute("x", 170),
                                            new XAttribute("note", DigressionName.SpeedUpNear == note.Digression ? "" : note.LimitSpeedToString()),
                                                    new XAttribute("Meter", note.Meter),
                                            new XAttribute("fw", note.FontStyle())));



                        if ((note.LimitSpeedToString() != "-/-") && (note.LimitSpeedToString() != "") || note.DigName.Contains("гр"))
                        {
                            otherfourStepOgrCoun += 1;
                        }


                        continue;
                    }


                    if (IsFilled)
                    {
                        meter = Start;
                        if (usedTops.Contains(meter))
                            meter = usedTops.GetNextTopFromBottom(Start);
                    }

                    usedTops.Add(meter);
                    var noteTypes = note.Note().Split(' ');

                    if (noteTypes.Length > 3)
                    {
                        bool isMarkNote = true;
                        float markPosition = 0;

                        if (note.CNI == "Level")
                        {
                            switch (note.Digression.Name)
                            {
                                case string name when name == DigressionName.Level.Name || name == DigressionName.Sag.Name ||
                                                      name == DigressionName.Ramp.Name || name == DigressionName.RampNear.Name:
                                    markPosition = levelPosition;
                                    break;

                                default:
                                    markPosition = 170;
                                    //isMarkNote = false;
                                    break;
                            }
                        }
                        else if (note.CNI == "STR")
                        {
                            switch (note.Digression.Name)
                            {
                                case string name when name == DigressionName.Strightening.Name || name == DigressionName.NoneStrigtSide.Name:
                                    markPosition = levelPosition;
                                    break;

                                default:
                                    markPosition = 170;
                                    //isMarkNote = false;
                                    break;
                            }
                        }
                        else if (note.Diagram_type == "KN-1")
                        {
                            switch (note.Digression.Name)
                            {
                                //long
                                case string name when name == DigressionName.LongWaveLeft.Name:
                                    markPosition = 179.1097f + 7;
                                    isMarkNote = false;
                                    break;
                                case string name when name == DigressionName.LongWaveRight.Name:
                                    markPosition = 254.5523f + 7;
                                    isMarkNote = false;
                                    break;
                                //medium
                                case string name when name == DigressionName.MiddleWaveLeft.Name:
                                    markPosition = 326.4836f + 7;
                                    isMarkNote = false;
                                    break;
                                case string name when name == DigressionName.MiddleWaveRight.Name:
                                    markPosition = 401.9261f + 7;
                                    isMarkNote = false;
                                    break;
                                //short
                                case string name when name == DigressionName.ShortWaveLeft.Name:
                                    markPosition = 471.5165f + 7;
                                    isMarkNote = false;
                                    break;
                                case string name when name == DigressionName.ShortWaveRight.Name:
                                    markPosition = 546.9591f + 7;
                                    isMarkNote = false;
                                    break;

                                default:
                                    isMarkNote = false;
                                    break;
                            }
                        }
                        else if (note.Diagram_type == "GD_PR")
                        {
                            switch (note.Digression.Name)
                            {
                               // слепой стык 
                                case string name when name == DigressionName.TreadTiltRight.Name:
                                    markPosition = 605.7f;
                                    isMarkNote = false;
                                    break;
                            //нпк
                                case string name when name == DigressionName.TreadTiltRight.Name:
                                    markPosition = 605.7f;
                                    isMarkNote = false;
                                    break;
                                case string name when name == DigressionName.TreadTiltLeft.Name:
                                    markPosition = 460.2f;
                                    isMarkNote = false;
                                    break;
                                //пу
                                case string name when name == DigressionName.DownhillRight.Name:
                                    markPosition = 314.5f;
                                    isMarkNote = false;
                                    break;
                                case string name when name == DigressionName.DownhillLeft.Name:
                                    markPosition = 168.6f;
                                    isMarkNote = false;
                                    break;

                                default:
                                    isMarkNote = false;
                                    break;
                            }
                        }
                        else if (note.Diagram_type == "Iznos_relsov")
                        {
                            switch (note.Digression.Name)
                            {
                                //Бок из
                                case string name when name == DigressionName.SideWearLeft.Name:
                                    markPosition = 173.090317f;
                                    isMarkNote = false;
                                    break;
                                case string name when name == DigressionName.SideWearRight.Name:
                                    markPosition = 241.27356f;
                                    isMarkNote = false;
                                    break;
                                //Верт из
                                case string name when name == DigressionName.VertIznosL.Name:
                                    markPosition = 319.119873f;
                                    isMarkNote = false;
                                    break;
                                case string name when name == DigressionName.VertIznosR.Name:
                                    markPosition = 389.104919f;
                                    isMarkNote = false;
                                    break;
                                //Из прив из
                                case string name when name == DigressionName.ReducedWearLeft.Name:
                                    markPosition = 445.044769f;
                                    isMarkNote = false;
                                    break;
                                case string name when name == DigressionName.ReducedWearRight.Name:
                                    markPosition = 519.9657f;
                                    isMarkNote = false;
                                    break;
                                //ИЗ 45 из
                                case string name when name == DigressionName.HeadWearLeft.Name:
                                    markPosition = 595.0649f;
                                    isMarkNote = false;
                                    break;
                                case string name when name == DigressionName.HeadWearRight.Name:
                                    markPosition = 670.0643f;
                                    isMarkNote = false;
                                    break;
                                default:
                                    isMarkNote = false;
                                    break;
                            }
                        }
                        else
                        {
                            switch (note.Digression.Name)
                            {
                                //Бок из
                                case string name when name == DigressionName.SideWearRight.Name:
                                    markPosition = 41.9999988f;
                                    isMarkNote = false;
                                    break;
                                case string name when name == DigressionName.SideWearLeft.Name:
                                    markPosition = 28.0000024f;
                                    isMarkNote = false;
                                    break;
                                //нпк
                                case string name when name == DigressionName.TreadTiltRight.Name:
                                    markPosition = prosRightPosition;
                                    isMarkNote = false;
                                    break;
                                case string name when name == DigressionName.TreadTiltLeft.Name:
                                    markPosition = prosLeftPosition;
                                    isMarkNote = false;
                                    break;
                                //пу
                                case string name when name == DigressionName.DownhillRight.Name:
                                    markPosition = straighRighttPosition;
                                    isMarkNote = false;
                                    break;
                                case string name when name == DigressionName.DownhillLeft.Name:
                                    markPosition = strightLeftPosition;
                                    isMarkNote = false;
                                    break;

                                case string name when name == DigressionName.DrawdownRight.Name:
                                    markPosition = prosRightPosition;
                                    break;
                                case string name when name == DigressionName.DrawdownLeft.Name:
                                    markPosition = prosLeftPosition;
                                    break;
                                case string name when name == DigressionName.Strightening.Name
                                    || name == DigressionName.NoneStrightening.Name || name == DigressionName.NoneStrighteningST.Name || name == DigressionName.StrighteningOnSwitch.Name:
                                    markPosition = strightLeftPosition;
                                    break;
                                case string name when name == DigressionName.Level.Name
                                    || name == DigressionName.Sag.Name:
                                    markPosition = levelPosition;
                                    break;
                                case string name when name == DigressionName.Broadening.Name
                                    || name == DigressionName.Constriction.Name:
                                    markPosition = gaugePosition;
                                    break;
                                default:
                                    isMarkNote = false;
                                    break;
                            }
                        }

                        if (isMarkNote)
                        {
                            int defCoord = note.Meter;
                            int length = note.Length / 2;
                            double dlength = note.Length / 2.0;

                            float prevPosition = markPosition;
                            int index;


                            if (note.Digression.Name == DigressionName.NoneStrightening.Name)
                            {
                                markPosition = markPosition == strightLeftPosition ? straighRighttPosition : strightLeftPosition;
                            }

                            if (markPosition == strightLeftPosition)
                            {
                                var firstMarkSign = 0.0f;

                                for (index = defCoord - length / 2; index <= defCoord + length / 2; index++)
                                {
                                    if ((markPosition == strightLeftPosition) || (markPosition == straighRighttPosition))
                                    {
                                        prevPosition = markPosition;

                                        var mindex = kilometer.meter.IndexOf(index);
                                        if (index <= kilometer.Final_m)
                                        {
                                            if (firstMarkSign == 0.0)
                                            {
                                                firstMarkSign = kilometer.signTrapez[mindex > -1 ? mindex : mindex + 1] < 0 ? straighRighttPosition : strightLeftPosition;
                                            }

                                            if (note.Digression != DigressionName.NoneStrightening)
                                            {
                                                markPosition = firstMarkSign;
                                            }
                                            else
                                            {
                                                markPosition = kilometer.signTrapez[mindex > -1 ? mindex : mindex + 1] < 0 ? straighRighttPosition : strightLeftPosition;
                                            }

                                            if (note.Digression == DigressionName.NoneStrightening)
                                            {
                                                markPosition = markPosition == straighRighttPosition ? strightLeftPosition : straighRighttPosition;
                                            }

                                            digElements.Add(new XElement("line",
                                                new XAttribute("y1", -index),
                                                new XAttribute("y2", -index - 1),
                                                new XAttribute("x", mainParameters.MMToPixelChartString(markPosition)),
                                                new XAttribute("w", mainParameters.MMToPixelChartWidthString(note.Degree == 4 ? 4 : note.Degree == 3 ? 2 : 1))
                                            ));

                                        }
                                    }
                                }
                            }
                            else if (markPosition == straighRighttPosition)
                            {
                                var firstMarkSign = 0.0f;

                                for (index = defCoord - length / 2; index <= defCoord + length / 2; index++)
                                {
                                    if ((markPosition == strightLeftPosition) || (markPosition == straighRighttPosition))
                                    {

                                        //List<TrackObject> localStrights = (from stright in strights where Math.Abs(stright.Meter - index) < 4 select stright).ToList();
                                        prevPosition = markPosition;

                                        var mindex = kilometer.meter.IndexOf(index);
                                        if (index <= kilometer.Final_m)
                                        {
                                            if (firstMarkSign == 0.0)
                                            {
                                                firstMarkSign = markPosition = kilometer.signTrapez[mindex > -1 ? mindex : mindex + 1] < 0 ? straighRighttPosition : strightLeftPosition;
                                            }

                                            if (note.Digression != DigressionName.NoneStrightening)
                                            {
                                                markPosition = firstMarkSign;
                                            }
                                            else
                                            {
                                                markPosition = kilometer.signTrapez[mindex > -1 ? mindex : mindex + 1] < 0 ? straighRighttPosition : strightLeftPosition;
                                            }

                                            if (note.Digression == DigressionName.NoneStrightening)
                                            {
                                                markPosition = markPosition == straighRighttPosition ? strightLeftPosition : straighRighttPosition;
                                            }

                                            digElements.Add(new XElement("line",
                                                new XAttribute("y1", -index),
                                                new XAttribute("y2", -index - 1),
                                                new XAttribute("x", mainParameters.MMToPixelChartString(markPosition)),
                                                new XAttribute("w", mainParameters.MMToPixelChartWidthString(note.Degree == 4 ? 4 : note.Degree == 3 ? 2 : 1))
                                            ));
                                        }
                                    }
                                }
                            }
                            else
                            {

                                digElements.Add(new XElement("line",
                                        new XAttribute("y1", -(defCoord - dlength)),
                                        new XAttribute("y2", -(defCoord + dlength)),
                                        new XAttribute("x", mainParameters.MMToPixelChartString(markPosition)),
                                        new XAttribute("w", mainParameters.MMToPixelChartWidthString(note.Degree == 4 ? 4 : note.Degree == 3 ? 2 : 1))
                                ));
                            }
                        }
                        //доп. параметры
                        else
                        {
                            if ((new[] { DigressionName.FusingGapL, DigressionName.FusingGapR, DigressionName.AnomalisticGap, DigressionName.Gap, DigressionName.GapSimbol }).Contains(note.Digression))
                            {

                            }
                            else
                            {
                                if (note.DigName != DigressionName.NoneStrighteningST.Name)
                                    if (note.Length >= 100)
                                    {
                                        //test
                                    }
                                    digElements.Add(new XElement("line",
                                                    new XAttribute("y1", -note.Meter),
                                                    new XAttribute("y2", -note.finish_meter),
                                                    new XAttribute("x", note.Diagram_type == "GD_PR" || note.Diagram_type == "KN-1" || note.Diagram_type == "Iznos_relsov" ? markPosition.ToString() : mainParameters.MMToPixelChartString(markPosition).ToString()),
                                                    new XAttribute("w", mainParameters.MMToPixelChartWidthString(note.Degree == 4 ? 4 : note.Degree == 3 ? 2 : 1))
                                                ));
                            }
                        }
                    }
                    if (note.Km == 715 && note.Meter > 371 && note.Meter < 390)
                    {

                    }

                    string ogrSk = note.LimitSpeedToString();
                    //if (note.Degree >0 &&  note.Alert.Contains("обкатка"))
                    //  /if note.Alert.Any())
                    //    if (note.Degree > 0 )
                    //    {
                    //    note.Km = note.Km;
                        
                    //}
                    if ((note.LimitSpeedToString() != "-/-") && (note.LimitSpeedToString() != ""))
                    {
                        if ((note.Degree == 4))
                        {
                            fourStepOgrCoun += 1;
                        }
                        else
                        {
                            otherfourStepOgrCoun += 1;
                        }

                    }

                    int count = note.GetCount();
                    if (count > 0)
                    {

                        switch (note.Degree)
                        {
                            //case 1: kilometer.FirstDegreeCountDrawdown += count; break;
                            case 2: kilometer.SecondDegreeCountDrawdown += count; break;
                            case 3: kilometer.ThirdDegreeCountDrawdown += count; break;
                            case 4: kilometer.FourthDegreeCountDrawdown += count; break;
                            default: break;
                        }
                    }

                    var prim = note.Comment;

                    if (prim.Contains("гр"))
                    {
                        otherfourStepOgrCoun += 1;
                    }
                    //if (note.DigName.Contains("Рнрст"))
                    //{
                    //    note.DigName = "Рнр";
                    //}

                    else
                    {
                        if (prim.Contains("/") && note.Note().Split(' ').Count() > 6)
                        {
                            prim = note.Note().Split(' ')[6].Replace(';', ' ');
                        }
                        if (prim.Contains("/"))
                        {
                            prim = "";
                        }
                        prim = prim.Replace("рн", "");
                        prim = prim.Replace(";", " ");
                        prim = prim.Replace("3м", "");
                        prim = Regex.Replace(prim, "  ", " ");
                    }

                    if ((note.Digression == DigressionName.FusingGapL) || (note.Digression == DigressionName.FusingGapR) || (note.Digression == DigressionName.AnomalisticGap) || (note.Digression == DigressionName.Gap) || note.Digression == DigressionName.GapSimbol)
                    {
                        digElements.Add(new XElement("m",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 1),
                                             new XAttribute("note", note.Meter),
                                                    new XAttribute("Meter", note.Meter),
                                             new XAttribute("fw", (note.Digression == DigressionName.Gap) ? "bold" : "normal")
                            ));
                        digElements.Add(new XElement("otst",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 23),
                                             new XAttribute("note", note.DigName),
                                                    new XAttribute("Meter", note.Meter),
                                             new XAttribute("fw", (note.Digression == DigressionName.Gap) ? "bold" : "normal")
                            ));
                        digElements.Add(new XElement("otkl",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 73),
                                             new XAttribute("note", note.Value),
                                                    new XAttribute("Meter", note.Meter),
                                             new XAttribute("fw", (note.Digression == DigressionName.Gap) ? "bold" : "normal")
                            ));
                        digElements.Add(new XElement("bal",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 145),
                                             new XAttribute("note", "" + (note.Digression == DigressionName.GapSimbol ? "20" : note.Digression == DigressionName.Gap ? "50" : "")),
                                                    new XAttribute("Meter", note.Meter),
                                             new XAttribute("fw", (note.Digression == DigressionName.Gap) ? "bold" : "normal")
                            ));
                        digElements.Add(new XElement("ogrsk",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", note.Diagram_type == "ГД сводной доп.параметров NEW" ? 145 : 170),
                                             new XAttribute("note", "" + (note.Digression == DigressionName.AnomalisticGap || note.Digression == DigressionName.GapSimbol ? "" : note.AllowSpeed)),
                                             new XAttribute("Meter", note.Meter),
                                             new XAttribute("fw", (note.Digression == DigressionName.Gap) ? "bold" : "normal")
                            ));
                    }
                    else if ((
                        note.Digression == DigressionName.ImpulsLeft ||
                        note.Digression == DigressionName.ImpulsRight ||

                        note.Digression == DigressionName.LongWaveLeft ||
                        note.Digression == DigressionName.LongWaveRight ||
                        note.Digression == DigressionName.MiddleWaveLeft ||
                        note.Digression == DigressionName.MiddleWaveRight ||
                        note.Digression == DigressionName.ShortWaveLeft ||
                        note.Digression == DigressionName.ShortWaveRight) &&

                        note.Diagram_type == "KN-1")
                    {
                        digElements.Add(new XElement("m",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 1),
                                             new XAttribute("note", note.Meter),
                                                    new XAttribute("Meter", note.Meter),
                                             new XAttribute("fw", note.FontStyle())
                            ));
                        digElements.Add(new XElement("otst",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 23),
                                             new XAttribute("note", note.DigName),
                                                    new XAttribute("Meter", note.Meter),
                                             new XAttribute("fw", note.FontStyle())
                            ));
                        digElements.Add(new XElement("step",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 60),
                                             new XAttribute("fw", note.FontStyle()),
                                                    new XAttribute("Meter", note.Meter),
                                             new XAttribute("note", note.Value.ToString("0.00"))

                            ));
                        digElements.Add(new XElement("len",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 102),
                                             new XAttribute("note", note.Dlina.ToString("0.00")),
                                                    new XAttribute("Meter", note.Meter),
                                             new XAttribute("fw", note.FontStyle())

                            ));

                    }
                    else
                    {
                        digElements.Add(new XElement("m",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 1),
                                             new XAttribute("note", note.Meter),
                                                    new XAttribute("Meter", note.Meter),
                                             new XAttribute("fw", note.FontStyle())
                            ));
                        digElements.Add(new XElement("otst",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 23),
                                             new XAttribute("xl", 27),
                                             new XAttribute("note", note.Comment.Contains("3м") ? note.DigName + "3м" : note.DigName),
                                                    new XAttribute("Meter", note.Meter),
                                             new XAttribute("lvValue", note.DigName.Equals("У") && note.Value > 150 ? "150" : (note.DigName.Equals("У") && note.Value > 75 && note.Value < 150 && note.OnSwitch ? "75" : "")),
                                             new XAttribute("fw", note.FontStyle())
                            ));
                        digElements.Add(new XElement("step",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 60),
                                             new XAttribute("fw", note.Is2to3 || note.IsEqualTo3 ? "bold" : note.FontStyle()),
                                                    new XAttribute("Meter", note.Meter),
                                             new XAttribute("note", note.DigName == DigressionName.PatternRetraction.Name ? "" : (note.Degree + ""))

                            ));
                        digElements.Add(new XElement("otkl",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 73),
                                             new XAttribute("note", note.DigName.Contains("Нпк.л") || note.DigName.Contains("Нпк.п") || note.DigName.Contains("Пу.л") || note.DigName.Contains("Пу.п") ?
                                                                        (note.Value > 0 ? $"1/ {(1.0 / note.Value).ToString("0")}" : "0") :
                                                                            (note.Digression == DigressionName.SideWearLeft || note.Digression == DigressionName.SideWearRight ? note.Value.ToString("0.0") :
                                                                                (note.Value % 1 > 0 ? note.Value.ToString("0.00") : note.Value.ToString("0")))),
                                                    new XAttribute("Meter", note.Meter),
                                             new XAttribute("fw",
                                             (note.DigName.Contains("Нпк.л") || note.DigName.Contains("Нпк.п") || note.DigName.Contains("Пу.л") || note.DigName.Contains("Пу.п"))
                                              && note.Degree == 4 ? "bold" : "normal")));


                        digElements.Add(new XElement("len",
                                                 new XAttribute("top", -meter),
                                                 new XAttribute("x", 102),
                                                 new XAttribute("note", note.Length),
                                                    new XAttribute("Meter", note.Meter),
                                                 new XAttribute("fw",
                                                 (note.DigName.Contains("Нпк.л") || note.DigName.Contains("Нпк.п") || note.DigName.Contains("Пу.л") || note.DigName.Contains("Пу.п"))
                                                  && note.Degree == 4 ? "bold" : "normal")));
                        digElements.Add(new XElement("count",
                                             new XAttribute("top", -meter),
                                             new XAttribute("x", 122),
                                                    new XAttribute("Meter", note.Meter),
                                             //new XAttribute("note", (count <= 1 ? "    " : count.ToString() + " ") + prim),
                                             new XAttribute("note",
                                                 (note.DigName.Contains("Нпк.л") || note.DigName.Contains("Нпк.п") || note.DigName.Contains("Пу.л") || note.DigName.Contains("Пу.п")
                                                 || note.Digression == DigressionName.SideWearRight || note.Digression == DigressionName.SideWearLeft) ?
                                                 note.Count.ToString() : (note.GetPoint(kilometer).ToString() + " ")),
                                             new XAttribute("fw",
                                             (note.DigName.Contains("Нпк.л") || note.DigName.Contains("Нпк.п") || note.DigName.Contains("Пу.л") || note.DigName.Contains("Пу.п"))
                                              && note.Degree == 4 ? "bold" : "normal")

                            ));
                        if (note.Km == 727)
                        {

                        }
                        if (!prim.Contains("ис") && !prim.Contains("м"))
                        {
                            digElements.Add(
                               new XElement("ogrsk",
                                            new XAttribute("top", -meter),
                                            new XAttribute("x", note.Diagram_type == "ГД сводной доп.параметров NEW" || note.Comment.Contains("гр") ? 145 : 170), //"для того чтобы сдвинуть ГР влево вставляю 145"

                                            new XAttribute("note", prim),
                                            //"
                                            //new XAttribute("note", prim = prim.Contains("гр") ? "гр" : prim),
                                            new XAttribute("Meter", note.Meter),
                                            new XAttribute("fw", (note.Digression == DigressionName.Gap) ? "bold" : "normal")
                           ));


                        }

                        digElements.Add(new XElement("islong",
                                                     new XAttribute("top", -meter),
                                                     //new XAttribute("x", 540),
                                                     new XAttribute("x", 145),//выравниваем "Гр,ис,м,*" в одну линнию
                                                     new XAttribute("note", note.IsLong ? "*" : "")

                                                     ));


                        if (prim.Contains("ис") && prim.Contains("?"))
                        {
                            digElements.Add(new XElement("islong",
                                                     new XAttribute("top", -meter),
                                                     //new XAttribute("x", 140),
                                                     new XAttribute("x", 145), //выравниваем "Гр,ис,м,*" в одну линнию
                                                     new XAttribute("note", "ис?")
                                                     ));
                        }
                        if (prim.Contains("ис") && !prim.Contains("?"))
                        {
                            digElements.Add(new XElement("islong",
                                                     new XAttribute("top", -meter),
                                                     //new XAttribute("x", 140),
                                                     new XAttribute("x", 145), //выравниваем "Гр,ис,м,*" в одну линнию
                                                     new XAttribute("note", "ис")
                                                     ));
                        }
                        if (prim.Contains("м"))
                        {
                            digElements.Add(new XElement("islong",
                                                     new XAttribute("top", -meter),
                                                     //new XAttribute("x", 140),
                                                     new XAttribute("x", 145), //выравниваем "Гр,ис,м,*" в одну линнию
                                                     new XAttribute("note", "м")
                                                     ));
                        }

                        //if (note.Comment == "ис;ис;")
                        //{
                        //    digElements.Add(new XElement("islong",
                        //                             new XAttribute("top", -meter),
                        //                             //new XAttribute("x", 140),
                        //                             new XAttribute("x", 170), //выравниваем "Гр,ис,м,*" в одну линнию
                        //                             //new XAttribute("note", note.Comment == "ис;ис;"? "ис?":"")
                        //                                  new XAttribute("note", "")
                        //                             ));
                        //}
                        if (ogrSk.Equals("-/-/-") || ogrSk.Equals("-/-"))
                        {

                        }

                        else
                        {
                            digElements.Add(new XElement("ogrsk",
                                                 new XAttribute("top", -meter),
                                                 new XAttribute("x", 170),
                                                 //new XAttribute("x", 145),
                                                 new XAttribute("Meter", note.Meter),
                                                 new XAttribute("note", note.Degree == 1 ? "" : ogrSk),
                                                 new XAttribute("fw", note.FontStyle())
                                ));
                        }
                        if (prim.Contains("гр"))
                        {
                            digElements.Add(new XElement("ogrsk",
                                                new XAttribute("top", -meter),
                                                new XAttribute("x", 170),
                                                //new XAttribute("x", 145),
                                                new XAttribute("Meter", note.Meter),
                                                new XAttribute("note", "  " + "-/60"),
                                                new XAttribute("fw", note.FontStyle())
                               ));

                        }
                    }

                }
                catch (Exception e)
                {
                    System.Console.Error.WriteLine("Ошибка при записи координаты отступления: " + e.Message);
                }
            }
        }




        private string GetMarkByNoteType(string note)
        {
            if (note.Contains("Укл") || note.Contains("?Уkл"))
                return "˅";
            if (note.Contains("Анп"))
                return "▼";
            if (note.Contains("Пси"))
                return "*";
            return "";
        }
        public List<DigressionMark> Digression { get; set; } = new List<DigressionMark>();
        public List<Bedemost> Bedemosts { get; set; } = new List<Bedemost>();
    }
}
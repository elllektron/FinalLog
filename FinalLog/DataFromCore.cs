﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;


namespace FinalLog
{
    public class DataFromCore
    {
        public DataFromCore(string wellName, List<string> runs, XmlDocument coreFile,
            string wellType, string mudType, string company)
        {

            _wellName = wellName;
            _runs = runs;
            _coreFile = coreFile;
            _wellType = wellType;
            _mudType = mudType;
            _company = company;
            RunFillingHeaders();
        }

        const double feetToMeter = 3.28084;
        const double inchToMillimeter = 25.4;
        const double feetToMillimeter = 304.8;
        const double ppgToGcc = 8.34;

        private readonly string _wellName;
        private readonly List<string> _runs;
        private readonly XmlDocument _coreFile;
        private readonly string _wellType;
        private readonly string _mudType;
        private readonly string _company;

        private Dictionary<string, double> _nozzlessSquare { get; set; } = new Dictionary<string, double>();
        private Dictionary<string, string> _bitType { get; set; } = new Dictionary<string, string>();

        private List<string> _toolsList = new()
        {
            "IDS",
            "Pulser (AES)",
            "Driver Insert",
            "MFR Collar",
            "Azimuthal MFR (GWAR)",
            "HAGR",
            "Am-241Be Source",
            "Neutron Insert",
            "Cs-137 Source",
            "Density Insert"
        };




        //Свойства из конструктора
        public string WellName { get; set; }
        public List<string> RunNumbers { get; set; }
        public string WellType { get; set; }
        public string MudType { get; set; }
        public string Company { get; set; }

        //свойства полученные из LWDJob
        public string JobNumber { get; set; }
        public string FieldName { get; set; }
        public string PadName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string RigType { get; set; }
        public string StartDate { get; set; }


        public string Declination { get; set; }
        public string MagneticDip { get; set; }
        public double SSTVD { get; set; }

        public double StartMD { get; set; }
        public double EndMD { get; set; }
        public double HoleSize { get; set; }
        public string StartDateHeader { get; set; }
        public string EndDateHeader { get; set; }
        public int RunCount { get; set; }
        public Dictionary<double, double> CasingShoeSize { get; set; } = new Dictionary<double, double>()
        { 
            {393.7, 324 },
            {295.3, 245 },
            {220.7, 178 },  
        };
        public Dictionary<double, double> CasingShoeDepthDict { get; set; } = new Dictionary<double, double>();
        public List<double> CasingShoeDepth { get; set; } = new List<double>();
        public Dictionary<string, double> StartDepthRuns { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> EndDepthRuns { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, string> StartDateRuns { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> EndDateRuns { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> StartTimeRuns { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> EndTimeRuns { get; set; } = new Dictionary<string, string>();

        //Данные по раствору
        public Dictionary<string, string> Solid { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Chlorides { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Sand { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Rm { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Rmf { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Rmc { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Temp { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> RmMaxTemp { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, double> FunelViscosity { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> Oil { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> PV { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> YP { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> PH { get; set; } = new Dictionary<string, double>();


        public Dictionary<string, string> BHAType { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Circulation { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DrillingHours { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> Engeneer { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, double> FlowRateRuns { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> MaxIncRuns { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> MinIncRuns { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> MaxMudRuns { get; set; } = new Dictionary<string, double>();
        public List<double> NozzlessSquare { get; set; } = new List<double>();
        public List<string> BitType { get; set; } = new List<string>();

        public Dictionary<string, double> HoleSizeRuns { get; set; } = new Dictionary<string, double>();
        public Dictionary<double, List<double>> StartDepthOfHoleSize { get; set; } = new Dictionary<double, List<double>>();
        public Dictionary<double, List<double>> EndDepthOfHoleSize { get; set; } = new Dictionary<double, List<double>>();
        public Dictionary<double, List<double>> MaxIncOfHoleSize { get; set; } = new Dictionary<double, List<double>>();
        public Dictionary<double, List<double>> MinIncOfHoleSize { get; set; } = new Dictionary<double, List<double>>();
        public Dictionary<double, List<double>> MaxMudOfHoleSize { get; set; } = new Dictionary<double, List<double>>();

        //Данные по инсертам
        public Dictionary<string, string> IDS { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, Dictionary<string, string>> Tools { get; set; } = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, Dictionary<string, string>> Offsets { get; set; } = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, double> LengthBHA { get; set; } = new Dictionary<string, double>();

        public Dictionary<string, SortedDictionary<double, List<string>>> DailyMudSum { get; set; } = new Dictionary<string, SortedDictionary<double, List<string>>>();

        //Ошибки в заполнении файла Core
        public Dictionary<string, List<string>> ErrorsInRunsDict { get; set; } = new Dictionary<string, List<string>>();

        public Dictionary<string, Dictionary<string, string>> ItemsDict { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        private void RunFillingHeaders()
        {
            WellName = _wellName;
            RunNumbers = _runs;
            WellType = _wellType;
            MudType = _mudType;
            Company = _company;
            RunCount = _runs.Count;

            GetDataFromLWDJob();
            GetDataFromAllRuns();

            GetMagneticDip(_runs[0]);
            GetDeclination(_runs[0]);
            GetSSTVD();
            GetStartDepthHeader();
            GetEndDepthHeader();
            GetHoleSize();
            GetStartDateHeader();
            GetEndDateHeader();
            GetHolesizeofDepth();
            GetDataFromCurrentRuns();
            CasingShoeDepthCompare();
            ErrorsInRuns();
        }

        private void GetDataFromLWDJob()
        {
            var lwdJob = _coreFile.GetElementsByTagName("LWDJob").Item(0).ChildNodes;

            for (int i = 0; i < lwdJob.Count; i++)
            {
                string item = lwdJob.Item(i).InnerText;
                switch (lwdJob.Item(i).Name)
                {
                    case "Field":
                        FieldName = TranslateString(item.ToLower());
                        break;

                    case "Rig":
                        var result = "";
                        for (int j = 0; j < item.Length; j++)
                        {
                            if (char.IsDigit(item[j]))
                                result += item[j];
                        }
                        PadName = result;
                        break;

                    case "Rig_Type":
                        RigType = item.Replace("BU", "БУ");
                        break;

                    case "Latitude":
                        Latitude = item.Replace('N', 'С');
                        break;

                    case "Longitude":
                        Longitude = item.Replace("E", "В");
                        break;

                    case "Job_Number":
                        JobNumber = item;
                        break;
                    case "Start_Date":
                        StartDate = item;
                        break;
                    default:
                        break;
                }
            }
        }


        private void GetDataFromAllRuns()
        {
            var runs = _coreFile.GetElementsByTagName("Run");

            /*List<string> itemsEmpytLsit = new();*/
            for (int i = 0; i < runs.Count; i++)
            {
                var runItems = runs.Item(i).ChildNodes;
                bool flag = true;
                string runNumber = "";
                List<string> itemsEmpytLsit = new();

                for (int j = 0; j < runItems.Count; j++)
                {
                    var item = runItems.Item(j).InnerText;
                    
                    switch (runItems.Item(j).Name)
                    {
                        case "Run_Number":
                            if (item.Length < 3)
                            {
                                runNumber = item;
                                
                                flag = true;
                            }
                            else
                            {
                                flag = false;
                            }
                            break;

                        case "In_Hole_Date":
                            StartDateRuns.Add(runNumber, item);
                            break;

                        case "In_Hole_MD":
                            StartDepthRuns.Add(runNumber, double.Parse(item) / feetToMeter);
                            break;
                        case "Out_Hole_Date":
                            //EndDateRuns.Add(runNumber, item);
                            break;
                        case "Out_Hole_MD":
                            EndDepthRuns.Add(runNumber, double.Parse(item, CultureInfo.InvariantCulture) / feetToMeter);
                            break;
                        case "Circ_Hrs":
                            if (string.IsNullOrEmpty(item))
                                itemsEmpytLsit.Add("Circ_Hrs");
                            else
                                Circulation.Add(runNumber, item);  
                            break;
                        case "Drill_Hrs":
                            if (string.IsNullOrEmpty(item))
                                itemsEmpytLsit.Add("Drill_Hrs");
                            else
                                DrillingHours.Add(runNumber, item);
                            break;
                        case "Engineers_On_Duty":
                            var engeneerData = runItems.Item(j).ChildNodes;
                            for (int k = 0; k < engeneerData.Count; k++)
                            {
                                string name = "";
                                for (int l = 0; l < engeneerData.Item(k).ChildNodes.Count; l++)
                                {
                                    name = name + " " + engeneerData.Item(k).ChildNodes.Item(l).InnerText;
                                }
                                name = GetEngeneerName(name);
                                if (!string.IsNullOrEmpty(name))
                                {
                                    Engeneer.Add(runNumber, TranslateString(name) + ".");
                                    break;
                                } 
                            }
                            if (!Engeneer.ContainsKey(runNumber))
                                Engeneer.Add(runNumber, "WTF");
                            break;
                        case "Bit_Data":
                            var bit = runItems.Item(j).ChildNodes;
                            double holeSize;
                            for (int k = 0; k < bit.Count; k++)
                            {
                                if (bit.Item(k).Name == "Size")
                                {
                                    if (string.IsNullOrEmpty(bit.Item(k).InnerText))
                                        itemsEmpytLsit.Add("HoleSize");
                                    else
                                    {
                                        holeSize = double.Parse(bit.Item(k).InnerText);
                                        HoleSizeRuns.Add(runNumber, Math.Round(holeSize * feetToMillimeter, 1));
                                    }
                                    
                                }

                                if (bit.Item(k).Name == "TFA")
                                {
                                    if (!string.IsNullOrEmpty(bit.Item(k).InnerText))
                                        _nozzlessSquare.Add(runNumber, Math.Pow(Math.Sqrt(double.Parse(bit.Item(k).InnerText)) * inchToMillimeter, 2));
                                    else
                                    {
                                        _nozzlessSquare.Add(runNumber, 0);
                                        itemsEmpytLsit.Add("TFA");
                                    }
                                        
                                }

                                if (bit.Item(k).Name == "Type")
                                {
                                    if (string.IsNullOrEmpty(bit.Item(k).InnerText))
                                        itemsEmpytLsit.Add("Bit_Type");
                                    else
                                        _bitType.Add(runNumber, bit.Item(k).InnerText);
                                }
                            }
                            break;

                        case "Survey_Data":
                            var survItem = runItems.Item(j).ChildNodes;
                            for (int k = 0; k < survItem.Count; k++)
                            {
                                switch (survItem.Item(k).Name)
                                {
                                    case "Casing_Shoe":
                                        if (string.IsNullOrEmpty(survItem.Item(k).InnerText))
                                            itemsEmpytLsit.Add("Casing_Shoe");

                                        else if (survItem.Item(k).InnerText != "" && survItem.Item(k).InnerText != "0")
                                        {
                                            if (!CasingShoeDepth.Contains(double.Parse(survItem.Item(k).InnerText) / feetToMeter))
                                                CasingShoeDepth.Add(double.Parse(survItem.Item(k).InnerText) / feetToMeter);
                                        }
                                        
                                        break;

                                    case "Max_Inc":
                                        if (survItem.Item(k).InnerText != "")
                                        {
                                            MaxIncRuns.Add(runNumber, double.Parse(survItem.Item(k).InnerText));
                                        }
                                        break;
                                    case "Min_Inc":
                                        if (survItem.Item(k).InnerText != "")
                                        {
                                            MinIncRuns.Add(runNumber, double.Parse(survItem.Item(k).InnerText));
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;

                        case "Mud_Data":
                            var mudItem = runItems.Item(j).ChildNodes;
                            double funnelViscosityStart = 0;
                            double funnelViscosityEnd = 0;
                            double oilStart = 0;
                            double oilEnd = 0;
                            double pvStart = 0;
                            double pvEnd = 0;
                            double ypStart = 0;
                            double ypEnd = 0;
                            double phStart = 0;
                            double phEnd = 0;
                            for (int k = 0; k < mudItem.Count; k++)
                            {
                                if (mudItem.Item(k).InnerText == "")
                                {
                                    mudItem.Item(k).InnerText = "0";
                                }
                                switch (mudItem.Item(k).Name)
                                {
                                    case "Density_Max":
                                        MaxMudRuns.Add(runNumber, double.Parse(mudItem.Item(k).InnerText));
                                        break;
                                    case "Funnel_Viscosity_Start":
                                        funnelViscosityStart = double.Parse(mudItem.Item(k).InnerText);
                                        break;
                                    case "Funnel_Viscosity_End":
                                        funnelViscosityEnd = double.Parse(mudItem.Item(k).InnerText);
                                        break;
                                    case "Oil_Percentage_Start":
                                        oilStart = double.Parse(mudItem.Item(k).InnerText);
                                        break;
                                    case "Oil_Percentage_End":
                                        oilEnd = double.Parse(mudItem.Item(k).InnerText);
                                        break;
                                    case "Sand_Percentage_Max":
                                        Sand.Add(runNumber, mudItem.Item(k).InnerText);
                                        break;
                                    case "Solid_Percentage_Max":
                                        Solid.Add(runNumber, mudItem.Item(k).InnerText);
                                        break;
                                    case "PV_Start":
                                        pvStart = double.Parse(mudItem.Item(k).InnerText);
                                        break;
                                    case "PV_End":
                                        pvEnd = double.Parse(mudItem.Item(k).InnerText);
                                        break;
                                    case "YP_Start":
                                        ypStart = double.Parse(mudItem.Item(k).InnerText);
                                        break;
                                    case "YP_End":
                                        ypEnd = double.Parse(mudItem.Item(k).InnerText);
                                        break;
                                    case "pH_Start":
                                        phStart = double.Parse(mudItem.Item(k).InnerText);
                                        break;
                                    case "pH_End":
                                        phEnd = double.Parse(mudItem.Item(k).InnerText);
                                        break;
                                    case "Total_Chlorides_Max":
                                        Chlorides.Add(runNumber, mudItem.Item(k).InnerText);
                                        break;
                                    case "Bore_Hole_Temp_Max":
                                        Temp.Add(runNumber, mudItem.Item(k).InnerText);
                                        break;
                                    case "Rm_At_Temp_Max":
                                        Rm.Add(runNumber, CalculateRmfc(mudItem.Item(k).InnerText));
                                        Rmf.Add(runNumber, CalculateRmfc(mudItem.Item(k).InnerText, "rmf"));
                                        Rmc.Add(runNumber, CalculateRmfc(mudItem.Item(k).InnerText, "rmc"));
                                        RmMaxTemp.Add(runNumber, CalculateRmfc(mudItem.Item(k).InnerText, "rmMaxTemp", runNumber));
                                        break;
                                }
                            }
                            FunelViscosity.Add(runNumber, Math.Max(funnelViscosityStart, funnelViscosityEnd));
                            Oil.Add(runNumber, Math.Max(oilStart, oilEnd));
                            PV.Add(runNumber, Math.Max(pvStart, pvEnd));
                            YP.Add(runNumber, Math.Max(ypStart, ypEnd));
                            PH.Add(runNumber, Math.Max(phStart, phEnd));
                            break;
                        case "BHA":
                            double totalLengh = 0;
                            var bha = runItems.Item(j).ChildNodes;
                            for (int k = 0; k < bha.Count; k++)
                            {
                                if (bha.Item(k).Name == "Type")
                                {
                                    if (string.IsNullOrEmpty(bha.Item(k).InnerText))
                                        itemsEmpytLsit.Add("BHA_Type");
                                    else if (bha.Item(k).InnerText == "Steerable")
                                    {
                                        if (!BHAType.ContainsKey(runNumber))
                                            BHAType.Add(runNumber, "ННБ");
                                    }
                                    else if (bha.Item(k).InnerText == "Rotary Steerable")
                                    {
                                        if (!BHAType.ContainsKey(runNumber))
                                            BHAType.Add(runNumber, "РСС");
                                    }

                                }
                                //Элементы КНБК
                                if (bha.Item(k).Name == "BHA_Part")
                                {
                                    var bhaPart = bha.Item(k).ChildNodes;
                                    for (int l = 0; l < bhaPart.Count; l++)
                                    {
                                        if (bhaPart.Item(l).Name == "Length")
                                        {
                                            if (string.IsNullOrEmpty(bhaPart.Item(l).InnerText))
                                                itemsEmpytLsit.Add("BHA_Length");
                                            else
                                                totalLengh += double.Parse(bhaPart.Item(l).InnerText) / feetToMeter;
                                        }
                                    }
                                }
                            }
                            LengthBHA.Add(runNumber, totalLengh);
                            break;

                        case "Run_Tool_Strings":
                            var tempDict = new Dictionary<string, string>();

                            var toolStrings = runItems.Item(j).ChildNodes;

                            for (int k = 0; k < toolStrings.Count; k++)
                            {
                                var tool = toolStrings.Item(k).ChildNodes;
                                string tempKey = "";
                                for (int l = 0; l < tool.Count; l++)
                                {
                                    for (int m = 0; m < tool.Item(l).ChildNodes.Count; m++)
                                    {
                                        var toolParts = tool.Item(l).ChildNodes;
                                        if (toolParts.Item(m).Name == "Serial_Number")
                                        {
                                            tempKey = toolParts.Item(m).InnerText;
                                        }

                                        if (toolParts.Item(m).Name == "Component")
                                        {
                                            if (_toolsList.Contains(toolParts.Item(m).InnerText))
                                            {
                                                if (toolParts.Item(m).InnerXml == "IDS")
                                                {
                                                    if (tempDict.ContainsValue("IDS"))
                                                        toolParts.Item(m).InnerText = "IDS2";
                                                }

                                                if (!tempDict.ContainsKey(tempKey))
                                                    tempDict.Add(tempKey, toolParts.Item(m).InnerText);

                                            }
                                        }
                                    }
                                }
                            }

                            Tools.Add(runNumber, tempDict);
                            break;
                        case "SensorToBitOffsets":
                            var tempDictOffsets = new Dictionary<string, string>();

                            var sensorToBitOffsets = runItems.Item(j).ChildNodes;
                            for (int k = 0; k < sensorToBitOffsets.Count; k++)
                            {
                                if (sensorToBitOffsets.Item(k).Name == "SensorToBitOffset")
                                {
                                    var sensorsOffsets = sensorToBitOffsets.Item(k).ChildNodes;
                                    string tempVaue = "";
                                    string tempKey = "";
                                    for (int l = 0; l < sensorsOffsets.Count; l++)
                                    {
                                        if (sensorsOffsets.Item(l).Name == "SerialNumber")
                                        {
                                            if (string.IsNullOrEmpty(sensorsOffsets.Item(l).InnerText))
                                                itemsEmpytLsit.Add("SerialNumber_Tool");
                                            else
                                                tempKey = sensorsOffsets.Item(l).InnerText;
                                        }
                                        if (sensorsOffsets.Item(l).Name == "Offset")
                                        {
                                            if (string.IsNullOrEmpty(sensorsOffsets.Item(l).InnerText))
                                                itemsEmpytLsit.Add("Offset");
                                            else
                                                tempVaue = (double.Parse(sensorsOffsets.Item(l).InnerText) * feetToMillimeter).ToString("0.00");
                                        }
                                        if (sensorsOffsets.Item(l).Name == "Rate")
                                        {
                                            if (string.IsNullOrEmpty(sensorsOffsets.Item(l).InnerText))
                                                itemsEmpytLsit.Add("Rate");
                                            else
                                                tempVaue = tempVaue + " / " + sensorsOffsets.Item(l).InnerText;
                                        }
                                    }
                                    tempDictOffsets.Add(tempKey, tempVaue);
                                }
                            }
                            Offsets.Add(runNumber, tempDictOffsets);
                            break;
                        case "Hydraulics_Data":
                            var hydraulics = runItems.Item(j).ChildNodes;
                            for (int k = 0; k < hydraulics.Count; k++)
                            {
                                if (hydraulics.Item(k).Name == "Flow_Rate")
                                {
                                    if(hydraulics.Item(k).InnerText == "")
                                        FlowRateRuns.Add(runNumber, 0);
                                    else
                                        FlowRateRuns.Add(runNumber, double.Parse(hydraulics.Item(k).InnerText) * 3.7854);
                                }
                            }
                            break;
                        case "Dailies":
                            var dailies = runItems.Item(j).ChildNodes;
                            var tempDailyDepth = new SortedDictionary<double, List<string>>();
                            for (int k = 0; k < dailies.Count; k++)
                            {
                                var tempList = new List<string>();
                                double tempDepth = 0;
                                for (int l = 0; l < dailies.Item(k).ChildNodes.Count; l++)
                                {
                                    if (dailies.Item(k).ChildNodes.Item(l).Name == "End_Date")
                                    {
                                        string tempDate = dailies.Item(k).ChildNodes.Item(l).InnerText;
                                        tempList.Add(tempDate);
                                        
                                        if(k == dailies.Count - 2 || dailies.Count - 2 < 0)
                                        {
                                            EndDateRuns.Add(runNumber, tempDate);
                                        }
                                    }
                                    if (dailies.Item(k).ChildNodes.Item(l).Name == "End_MD")
                                    {
                                        tempDepth = double.Parse(dailies.Item(k).ChildNodes.Item(l).InnerText) / feetToMeter;
                                    }
                                    if (dailies.Item(k).ChildNodes.Item(l).Name == "Daily_Mud_Data")
                                    {
                                        var mudDataDailys = dailies.Item(k).ChildNodes.Item(l).ChildNodes;

                                        for (int day = 0; day < mudDataDailys.Count; day++)
                                        {
                                            if (mudDataDailys.Item(day).Name == "Density_Start")
                                            {
                                                string dailyMudWeight = (double.Parse(mudDataDailys.Item(day).InnerText) / ppgToGcc).ToString("0.00");
                                                tempList.Add(dailyMudWeight);
                                            }
                                            if (mudDataDailys.Item(day).Name == "Bore_Hole_Temp_Start")
                                            {
                                                if (string.IsNullOrEmpty(mudDataDailys.Item(day).InnerText))
                                                    itemsEmpytLsit.Add("Bore_Hole_Temp_Start");
                                                else
                                                {
                                                    string dailyTemp = FarenheitToCelsius(mudDataDailys.Item(day).InnerText);
                                                    tempList.Add(dailyTemp);
                                                }
                                                
                                            }
                                        }
                                    }
                                }
                                if (!tempDailyDepth.ContainsKey(tempDepth))
                                    tempDailyDepth.Add(tempDepth, tempList);

                                if (k == 0)
                                {
                                    for (int l = 0; l < dailies.Item(k).ChildNodes.Count; l++)
                                    {
                                        if (dailies.Item(k).ChildNodes.Item(l).Name == "Oper_Hrs")
                                        {
                                            StartTimeRuns.Add(runNumber, TimeConverter(dailies.Item(k).ChildNodes.Item(l).InnerText, true));
                                            if (dailies.Count - 1 == 0)
                                                EndTimeRuns.Add(runNumber, TimeConverter(dailies.Item(k).ChildNodes.Item(l).InnerText, false));
                                        }

                                    }
                                }
                                else if (k == dailies.Count - 1)
                                {
                                    for (int l = 0; l < dailies.Item(k).ChildNodes.Count; l++)
                                    {
                                        if (dailies.Item(k).ChildNodes.Item(l).Name == "Oper_Hrs")
                                        {
                                            EndTimeRuns.Add(runNumber, TimeConverter(dailies.Item(k).ChildNodes.Item(l).InnerText, false));
                                        }
                                    }
                                }
                            }
                            DailyMudSum.Add(runNumber, tempDailyDepth);
                            
                            break;
                        default:
                            break;
                    }
                    
                    if (!flag) break;

                }
                if (itemsEmpytLsit.Count > 0)
                {
                    ErrorsInRunsDict.Add(runNumber, itemsEmpytLsit);

                }
            }
        }


        private void GetHolesizeofDepth()
        {
            foreach (var item in HoleSizeRuns)
            {
                if (item.Value >= HoleSize)
                {
                    CompareDataWithHoleSize(StartDepthRuns, StartDepthOfHoleSize, item);
                    CompareDataWithHoleSize(EndDepthRuns, EndDepthOfHoleSize, item);
                    CompareDataWithHoleSize(MaxIncRuns, MaxIncOfHoleSize, item);
                    CompareDataWithHoleSize(MinIncRuns, MinIncOfHoleSize, item);
                    CompareDataWithHoleSize(MaxMudRuns, MaxMudOfHoleSize, item);
                    if (item.Key == RunNumbers[RunCount - 1])
                        break;
                }
            }
        }

        private void CompareDataWithHoleSize(
            Dictionary<string, double> dict,
            Dictionary<double, List<double>> dictOfHoleSize,
            KeyValuePair<string, double> item
            )
        {
            var curList = new List<double>();

            foreach (var i in dict)
            {
                if (item.Key == i.Key)
                {
                    if (!dictOfHoleSize.ContainsKey(item.Value))
                    {
                        curList.Add(i.Value);
                        dictOfHoleSize.Add(item.Value, curList);
                    }
                    else
                    {
                        curList.Add(i.Value);
                        dictOfHoleSize[item.Value].Add(i.Value);
                    }

                }
            }
        }

        /// <summary>
        /// Получает забой на начало рейса
        /// </summary>
        private void GetStartDepthHeader()
        {
            foreach (var item in StartDepthRuns)
            {
                if (item.Key == _runs[0])
                {
                    StartMD = item.Value;
                    break;
                }
            }
        }
        /// <summary>
        /// Получает конечный забой в рейсе
        /// </summary>
        private void GetEndDepthHeader()
        {
            foreach (var item in EndDepthRuns)
            {
                if (item.Key == _runs[_runs.Count - 1])
                {
                    EndMD = item.Value;
                    break;
                }
            }
        }
        /// <summary>
        /// Получает дату начала рейса
        /// </summary>
        private void GetStartDateHeader()
        {
            foreach (var item in StartDateRuns)
            {
                if (item.Key == _runs[0])
                {
                    StartDateHeader = item.Value[0..^6];
                    break;
                }
            }
        }
        /// <summary>
        /// Получает дату конца рейса
        /// </summary>
        private void GetEndDateHeader()
        {

            foreach (var item in EndDateRuns)
            {
                if (item.Key == _runs[_runs.Count - 1])
                {
                    EndDateHeader = item.Value[0..^6];
                    break;
                }
            }
        }

        /// <summary>
        /// Получает диаметр скважины
        /// </summary>
        private void GetHoleSize()
        {
            foreach (var item in HoleSizeRuns)
            {
                if (item.Key == _runs[0])
                {
                    HoleSize = item.Value;
                    break;
                }
            }
        }

        /// <summary>
        /// Переводит переводит принятую строку на русский язык
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string TranslateString(string str)
        {
            str = str.ToLower();
            bool doubleChar = false;
            string newString = "";
            Dictionary<string, string> templateString = new Dictionary<string, string>()
            {
                { "ya", "я" },
                { "ch", "ч"},
                { "kh", "х" },
                { "a", "а" },
                { "b", "б" },
                { "d", "д" },
                { "c", "ц" },
                { "e", "е" },
                { "f", "ф" },
                { "g", "г" },
                { "i", "и" },
                { "k", "к" },
                { "l", "л" },
                { "m", "м" },
                { "n", "н" },
                { "o", "о" },
                { "p", "п" },
                { "r", "р" },
                { "s", "с" },
                { "t", "т" },
                { "u", "у" },
                { "v", "в" },
                { "z", "з" },
                { "-", "-" }
            };

            for (int i = 0; i < str.Length; i++)
            {
                if (doubleChar)
                {
                    doubleChar = false;
                    continue;
                }
                foreach (var s in templateString)
                {
                    if (str[i] is 'y' or 'k' or 'c')
                    {
                        if (s.Key == str[i].ToString() + str[i + 1].ToString())
                        {
                            newString += s.Value;
                            doubleChar = true;
                            break;
                        }
                        else if (str[i].ToString() == s.Key)
                        {
                            newString += s.Value;
                            break;
                        }
                    }

                    else if (str[i].ToString() == s.Key)
                    {
                        newString += s.Value;
                        break;
                    }
                    else if (char.IsWhiteSpace(str[i]))
                    {
                        if (newString.EndsWith("ев"))
                        {
                            newString = newString.Replace("ев", "ьев");
                        }

                        newString += str[i];
                        break;
                    }
                }
            }


            string resultString = newString[0].ToString().ToUpper();

            for (int i = 1; i < newString.Length; i++)
            {
                if (newString[i - 1] == '-' || char.IsWhiteSpace(newString[i - 1]))
                {
                    resultString += newString[i].ToString().ToUpper();
                }
                else
                {
                    resultString += newString[i];
                }

            }

            return resultString;
        }

        /// <summary>
        /// Получает Склонение из файла Core
        /// </summary>
        /// <param name="run"></param>
        private void GetDeclination(string run)
        {
            var declination = _coreFile.GetElementsByTagName("Declination");
            string result = "";
            for (int i = 0; i < declination.Count; i++)
            {
                if (declination.Item(i).ParentNode.ParentNode.ChildNodes.Item(1).InnerText == run)
                    result = declination.Item(i).InnerText;
            }
            Declination = result + "°";
        }

        /// <summary>
        /// Получает Dip из файла Core
        /// </summary>
        /// <param name="run"></param>
        private void GetMagneticDip(string run)
        {
            var dip = _coreFile.GetElementsByTagName("Magnetic_DIP");
            string result = "";
            for (int i = 0; i < dip.Count; i++)
            {
                if (dip.Item(i).ParentNode.ParentNode.ChildNodes.Item(1).InnerText == run)
                    result = dip.Item(i).InnerText;
            }
            MagneticDip = result + "°";
        }
        /// <summary>
        /// Получает SSTVD из файла Core
        /// </summary>
        private void GetSSTVD()
        {
            string sstvd = _coreFile.GetElementsByTagName("Drill_Floor").Item(0).InnerText;
            if (string.IsNullOrEmpty(sstvd))
                sstvd = "0";
            double result = double.Parse(sstvd) / feetToMeter;
            SSTVD = result;
        }

        /// <summary>
        /// /Получает данные из текущего рейса
        /// </summary>
        private void GetDataFromCurrentRuns()
        {
            for (int i = 0; i < RunNumbers.Count; i++)
            {
                foreach (var item in _nozzlessSquare)
                {
                    if (item.Key == RunNumbers[i])
                    {
                        NozzlessSquare.Add(item.Value);
                    }
                }

                foreach (var item in _bitType)
                {
                    if (item.Key == RunNumbers[i])
                        BitType.Add(item.Value);
                }
            }
        }
        /// <summary>
        /// Конвертирует время из строки полученное в долях и переводит его в нормальное время
        /// </summary>
        /// <param name="time"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        private string TimeConverter(string time, bool start)
        {
            string hours = "";
            string minutes = "";
            double parseMinutes;
            if (start)
                time = (24 - double.Parse(time)).ToString("0.00");

            for (int i = 0; i < time.Length; i++)
            {
                if (i < 2 && time[i] != '.')
                {
                    hours += time[i];
                }
                else if (i >= 2 && time[i] != '.')
                {
                    minutes += time[i];
                }
            }
            if (minutes.Length < 2)
                minutes += "0";
            parseMinutes = Math.Round(double.Parse(minutes) * 0.6);
        
            string result = $"{hours}:{parseMinutes}";
            return result;
        }

        private string CalculateRmfc(string rm, string str = "rm", string run = "")
        {
            if(rm == "OBM")
            {
                return "n/a";
            }
            if (rm == "0")
                rm = "0.00@20";
            string[] splitStr = rm.Split('@');
            if (splitStr[0] == "" || splitStr[0].StartsWith("0.00"))
            {
                splitStr[0] = "0.2";
                splitStr[1] = "20";
            }

            double rmInStr = double.Parse(splitStr[0]);
            int tempInStr = int.Parse(splitStr[1]);
            if (str == "rmf")
            {
                rmInStr *= 0.75;
            }
            else if (str == "rmc")
            {
                rmInStr *= 1.5;
            }
            else if (str == "rmMaxTemp")
            {
                double maxTemp = double.Parse(Temp[run]);
                rmInStr = rmInStr * (21.5 + tempInStr) / (21.5 + maxTemp);
                return $"{rmInStr:f2}@{maxTemp}";
            }
            return $"{rmInStr:f2}@{tempInStr}"; ;

        }
        /// <summary>
        /// Получаем имя инженера на английском языке из строки
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string GetEngeneerName(string str)
        {
            string name = "";
            str = str.Trim();
            if (str.Contains("true") && str.Contains("WD"))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (char.IsWhiteSpace(str[i]))
                    {
                        name = name + str[i] + str[i + 1];
                        break;
                    }
                    name += str[i];
                }
            }
            return name;
        }
        private string FarenheitToCelsius(string farengeit)
        {
            string temp = ((double)5 / 9 * (double.Parse(farengeit) - 32)).ToString("0");
            return temp;
        }

        private void CasingShoeDepthCompare()
        {
            var tempList = new List<double>();
            
            foreach (var item in HoleSizeRuns)
            {
                if(!tempList.Contains(item.Value))
                    tempList.Add(item.Value);
                
            }

            foreach (var item in CasingShoeSize)
            {
                if(item.Key > HoleSize)
                {
                    for (int i = 0; i < tempList.Count; i++)
                    {
                        if (item.Key == tempList[i] && CasingShoeDepth.Count > 0)
                        {
                            CasingShoeDepthDict.Add(item.Value, CasingShoeDepth[i]);
                            break;
                        }
                    }
                }
                
            }

        }

        private bool ErrorsInRuns()
        {
            if (ErrorsInRunsDict.Count > 0)
                return true;
            return false;
            
        }

        private void ItemsInRuns(string item, string run, string itemValue)
        {
            if (ItemsDict.ContainsKey(item))
            {
                ItemsDict[item].Add(run, itemValue);
            }
        }
    }
}

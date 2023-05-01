using log4net;
using Microsoft.Office.Interop.Excel;
using System;

namespace FinalLog
{
    class RunSum
    {
        private const string MUD_NA = "n/a";

        private readonly Workbook _workbook;
        private readonly DataFromCore _data;
        private readonly string _activity;
        private readonly string _customerName;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public RunSum(Workbook workbook, DataFromCore data, string activity, string customerName)
        {
            _workbook = workbook;
            _data = data;
            _activity = activity;
            _customerName = customerName;
            CompleteRunSum();
        }

        private void CompleteRunSum()
        {
            try
            {
                double water = 20;
                double oil = 0;
                string solid = "";
                string sand = "";
                //открываем вкладку RunSum
                Worksheet worksheet = (Worksheet)_workbook.Sheets["RunSum"];

                //очищаем колонки
                worksheet.Columns["C:F"] = "";

                //заполняем данные на основе количества рейсов
                for (int i = 0; i < _data.RunNumbers.Count; i++)
                {
                    // номера рейсов
                    worksheet.Cells[2, 3 + i] = _data.RunNumbers[i];

                    //диаметр долота
                    worksheet.Cells[3, 3 + i] = _data.HoleSize.ToString("0.0");

                    //тип долота
                    worksheet.Cells[4, 3 + i] = _data.BitType[i];
                    // площадь насадок
                    worksheet.Cells[5, 3 + i] = _data.NozzlessSquare[i];

                    // бурение или запись
                    worksheet.Cells[6, 3 + i] = _activity;
                    // начальная глубина
                    worksheet.Cells[7, 3 + i] = _data.StartDepthRuns[_data.RunNumbers[i]];
                    // конечная глубина
                    worksheet.Cells[8, 3 + i] = _data.EndDepthRuns[_data.RunNumbers[i]];
                    // пробурено за рейс
                    worksheet.Cells[9, 3 + i] = _data.EndDepthRuns[_data.RunNumbers[i]] - _data.StartDepthRuns[_data.RunNumbers[i]];
                    // записано за рейс
                    if (_activity == "бурение")
                    {
                        worksheet.Cells[10, 3 + i] = 0;
                    }
                    else
                    {
                        worksheet.Cells[10, 3 + i] = 70;
                    }
                    //Начало рейса время
                    worksheet.Cells[11, 3 + i] = _data.StartTimeRuns[_data.RunNumbers[i]];
                    // начало рейса дата
                    worksheet.Cells[12, 3 + i] = _data.StartDateRuns[_data.RunNumbers[i]];
                    //конец рейса время 
                    worksheet.Cells[13, 3 + i] = _data.EndTimeRuns[_data.RunNumbers[i]];
                    //конец рейса дата
                    worksheet.Cells[14, 3 + i] = _data.EndDateRuns[_data.RunNumbers[i]];
                    // расход буравого раствора
                    worksheet.Cells[15, 3 + i] = _data.FlowRateRuns[_data.RunNumbers[i]];
                    // Мин. угол
                    worksheet.Cells[16, 3 + i] = _data.MinIncRuns[_data.RunNumbers[i]];
                    // Макс. угол
                    worksheet.Cells[17, 3 + i] = _data.MaxIncRuns[_data.RunNumbers[i]];
                    //тип бурового раствора
                    worksheet.Cells[19, 3 + i] = _data.MudType;
                    //плотность раствора
                    worksheet.Cells[20, 3 + i] = _data.MaxMudRuns[_data.RunNumbers[i]];
                    //вязкость раствора
                    worksheet.Cells[21, 3 + i] = _data.FunelViscosity[_data.RunNumbers[i]];
                    // ПВ / ДНС
                    worksheet.Cells[22, 3 + i] = $"{_data.PV[_data.RunNumbers[i]]} / {_data.YP[_data.RunNumbers[i]]}";
                    //Твердая фаза / песок
                    solid = _data.Solid[_data.RunNumbers[i]];
                    sand = _data.Sand[_data.RunNumbers[i]];
                    worksheet.Cells[23, 3 + i] = $"{solid} / {sand}";
                    //Хлориды
                    worksheet.Cells[24, 3 + i] = _data.Chlorides[_data.RunNumbers[i]];
                    //PH
                    worksheet.Cells[25, 3 + i] = _data.PH[_data.RunNumbers[i]];
                    //Смазка/вода
                    oil = _data.Oil[_data.RunNumbers[i]];

                    water = 100 - (oil + double.Parse(solid) + double.Parse(sand));
                    if (_data.MudType == "РУО")
                    {
                        //Сопративление раствора
                        worksheet.Cells[27, 3 + i] = MUD_NA;
                        worksheet.Cells[28, 3 + i] = MUD_NA;
                        worksheet.Cells[29, 3 + i] = MUD_NA;
                        //Сопротивление при максимальной температуре
                        worksheet.Cells[31, 3 + i] = MUD_NA;
                    }
                    else
                    {
                        //Сопративление раствора
                        worksheet.Cells[27, 3 + i] = _data.Rm[_data.RunNumbers[i]];
                        worksheet.Cells[28, 3 + i] = _data.Rmf[_data.RunNumbers[i]];
                        worksheet.Cells[29, 3 + i] = _data.Rmc[_data.RunNumbers[i]];
                        //Сопротивление при максимальной температуре
                        worksheet.Cells[31, 3 + i] = _data.RmMaxTemp[_data.RunNumbers[i]];
                    }
                    worksheet.Cells[26, 3 + i] = $"{oil} / {water}";
                    //Температура раствора
                    worksheet.Cells[30, 3 + i] = _data.Temp[_data.RunNumbers[i]];
                    
                    //Содержание KCL 
                    worksheet.Cells[32, 3 + i] = _data.KCL[_data.RunNumbers[i]];
                    //Представитель заказчика
                    worksheet.Cells[33, 3 + i] = _customerName;
                    //Инженер WFT
                    worksheet.Cells[34, 3 + i] = _data.Engeneer[_data.RunNumbers[i]];
                }
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
           
        }
    }
}

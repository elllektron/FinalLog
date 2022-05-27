using Microsoft.Office.Interop.Excel;
using System.Linq;

namespace FinalLog
{
    class HeaderInfo
    {
        private readonly Workbook _workbook;
        private readonly DataFromCore _data;

        public HeaderInfo(Workbook workbook, DataFromCore data)
        {
            _workbook = workbook;
            _data = data;
            CompleteHederInfo();
        }

        private void CompleteHederInfo()
        {
            Worksheet worksheet = (Worksheet)_workbook.Sheets["Header Info"];
            //Очищаем ячейки
            for (int i = 0; i < 8; i++)
            {
                worksheet.Rows[27 + i] = "";
                worksheet.Rows[38 + i] = "";
            }

            //компания
            worksheet.Cells[2, "B"] = $"{_data.Company}";

            //номер скважины
            worksheet.Cells[3, "B"] = $"{_data.WellName}#{_data.PadName} {_data.WellType}";
            
            //тип скважины
            worksheet.Cells[4, "B"] = _data.WellType;
            //Название месторождения
            worksheet.Cells[5, "B"] = _data.FieldName;
            //Тип буровой
            worksheet.Cells[6, "B"] = _data.RigType;
            //Номер работы
            worksheet.Cells[8, "B"] = _data.JobNumber;
            //Альтитуда
            worksheet.Cells[13, "B"] = $"{_data.SSTVD:f2} м";
            worksheet.Cells[16, "E"] = $"{_data.SSTVD:f2} м";
            worksheet.Cells[17, "E"] = $"{_data.SSTVD:f2} м";
            //Конечный забой
            worksheet.Cells[16, "B"] = $"{_data.EndMD:f1} м";
            worksheet.Cells[19, "B"] = $"{_data.EndMD:f1} м";
            //Диаметр скважины
            worksheet.Cells[17, "B"] = $"{_data.HoleSize:f1} мм";
            //Начальный забой
            worksheet.Cells[18, "B"] = $"{_data.StartMD:f1} м";
            //Дата начала бурения
            worksheet.Cells[20, "B"] = $"{_data.StartDateHeader}";
            worksheet.Cells[23, "B"] = $"{_data.StartDate}";
            //Дата конца бурения
            worksheet.Cells[21, "B"] = $"{_data.EndDateRuns[_data.RunNumbers[_data.RunCount - 1]]}";
            
            
            
            //Количество рейсов
            worksheet.Cells[22, "B"] = _data.RunCount;


            int num = 0;
            foreach (var item in _data.StartDepthOfHoleSize)
            {
                worksheet.Cells[27 + num, "A"] = $"{item.Key:f1} мм";
                worksheet.Cells[27 + num, "B"] = $"{item.Value.Min():f1} м";
                worksheet.Cells[38 + num, "A"] = $"{item.Key:f1} мм";
                worksheet.Cells[38 + num, "D"] = _data.MudType;
                worksheet.Cells[38 + num, "F"] = $"{item.Value.Min():f1} м";
                num++;
            }

            //Записи о скважине
            num = 0;
            foreach (var item in _data.EndDepthOfHoleSize)
            {
                worksheet.Cells[27 + num, "C"] = $"{item.Value.Last():f1} м";
                worksheet.Cells[38 + num, "G"] = $"{item.Value.Last():f1} м";
                num++;
            }

            //Записи об обсадной колонне
            num = 0;
            foreach (var item in _data.CasingShoeDepthDict)
            {
                worksheet.Cells[27 + num, "D"] = $"{item.Key:f1} мм";
                worksheet.Cells[27 + num, "E"] = "-";
                worksheet.Cells[27 + num, "F"] = "0.0 м";
                worksheet.Cells[27 + num, "G"] = $"{item.Value:f1} м";
                num++;
            }


            num = 0;
            //Максимальный угол за каждую секцию
            foreach (var item in _data.MaxIncOfHoleSize)
            {
                if (_data.RunCount < 2)
                {
                    worksheet.Cells[38 + num, "C"] = $"{item.Value.Last()}°";
                    num++;
                }
                else
                {
                    worksheet.Cells[38 + num, "C"] = $"{item.Value.Max()}°";
                    num++;
                }
            }

            num = 0;
            //Минимальный угол за каждую секцию
            foreach (var item in _data.MinIncOfHoleSize)
            {
                if (_data.RunCount < 2)
                {
                    worksheet.Cells[38 + num, "B"] = $"{item.Value.Last()}°";
                    num++;
                }
                else
                {
                    worksheet.Cells[38 + num, "B"] = $"{item.Value.Min()}°";
                    num++;
                }
               
            }


            //Информация по раствору
            num = 0;
            foreach (var item in _data.MaxMudOfHoleSize)
            {
                worksheet.Cells[38 + num++, "E"] = $"{item.Value.Max()}";
            }


            worksheet.Cells[2, "E"] = _data.Latitude;
            worksheet.Cells[3, "E"] = _data.Longitude;
            worksheet.Cells[6, "E"] = _data.Declination;
            worksheet.Cells[7, "E"] = _data.MagneticDip;

        }
    }
}

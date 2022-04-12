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
            //Конечный забой
            worksheet.Cells[16, "B"] = $"{_data.EndMD:f2} м";
            //Диаметр скважины
            worksheet.Cells[17, "B"] = $"{_data.HoleSize:f1} мм";
            //Начальный забой
            worksheet.Cells[18, "B"] = $"{_data.StartMD:f2} м";
            //Дата начала бурения
            worksheet.Cells[20, "B"] = $"{_data.StartDateHeader}";
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

            num = 0;
            foreach (var item in _data.EndDepthOfHoleSize)
            {
                worksheet.Cells[27 + num, "C"] = $"{item.Value.Max():f1} м";
                worksheet.Cells[38 + num, "G"] = $"{item.Value.Max():f1} м";
                num++;
            }


            for (int i = 0; i < _data.CasingShoeDepth.Count; i++)
            {
                if (_data.CasingShoeSize.Count > i)
                {
                    if (_data.CasingShoeSize[i] > _data.HoleSize)
                    {
                        worksheet.Cells[27 + i, "D"] = $"{_data.CasingShoeSize[i]:f1} мм";
                        worksheet.Cells[27 + i, "E"] = "-";
                        worksheet.Cells[27 + i, "F"] = "0.0 м";
                        worksheet.Cells[27 + i, "G"] = $"{_data.CasingShoeDepth[i]:f1} м";
                    }
                }

            }

            num = 0;
            foreach (var item in _data.MaxIncOfHoleSize)
            {
                worksheet.Cells[38 + num, "C"] = $"{item.Value.Min():f1}°";
                num++;
            }

            num = 0;
            foreach (var item in _data.MinIncOfHoleSize)
            {
                worksheet.Cells[38 + num, "B"] = $"{item.Value.Min():f1}°";
                num++;
            }

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

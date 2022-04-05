using Microsoft.Office.Interop.Excel;

namespace FinalLog
{
    class EquipmentSum
    {
        private readonly Workbook _workbook;
        private readonly DataFromCore _data;


        public EquipmentSum(Workbook workbook, DataFromCore data)
        {
            _workbook = workbook;
            _data = data;

            CompleteEquipSum();
        }

        private void CompleteEquipSum()
        {
            //Открываем вкладку EquipmentSummary
            Worksheet worksheet = (Worksheet)_workbook.Sheets["Equip Sum"];

            //очищаем колонки
            worksheet.Columns["B:E"] = "";

            //заполняем данные на основе номера рейса
            for (int i = 0; i < _data.RunNumbers.Count; i++)
            {
                // номера рейсов
                worksheet.Cells[2, 2 + i] = _data.RunNumbers[i];

                foreach (var item in _data.Tools[_data.RunNumbers[i]])
                {
                    if (item.Value == "IDS")
                        worksheet.Cells[3, 2 + i] = item.Key;
                    if (item.Value == "IDS2")
                        worksheet.Cells[4, 2 + i] = item.Key;
                    if (item.Value == "Driver Insert")
                        worksheet.Cells[5, 2 + i] = item.Key;
                    if (item.Value == "Pulser (AES)")
                        worksheet.Cells[6, 2 + i] = item.Key;
                    if (item.Value == "HAGR")
                        worksheet.Cells[7, 2 + i] = item.Key;
                    if (item.Value == "MFR Collar")
                        worksheet.Cells[8, 2 + i] = item.Key;
                    if (item.Value == "Neutron Insert")
                        worksheet.Cells[9, 2 + i] = item.Key;
                    if (item.Value == "Density Insert")
                        worksheet.Cells[10, 2 + i] = item.Key;
                    if (item.Value == "Am-241Be Source")
                        worksheet.Cells[23, 2 + i] = item.Key;
                    if (item.Value == "Cs-137 Source")
                        worksheet.Cells[24, 2 + i] = item.Key;
                }
                foreach (var item in _data.Offsets[_data.RunNumbers[i]])
                {
                    if (item.Key == worksheet.Cells[9, 2 + i].Text)
                    {
                        worksheet.Cells[13, 2 + i] = item.Value;
                    }
                    if (item.Key == worksheet.Cells[10, 2 + i].Text)
                    {
                        worksheet.Cells[14, 2 + i] = item.Value;
                    }
                    if (item.Key == worksheet.Cells[8, 2 + i].Text)
                    {
                        worksheet.Cells[15, 2 + i] = item.Value;
                    }
                    if (item.Key == worksheet.Cells[7, 2 + i].Text)
                    {
                        worksheet.Cells[16, 2 + i] = item.Value;
                    }
                    if (item.Key == worksheet.Cells[3, 2 + i].Text)
                    {
                        worksheet.Cells[17, 2 + i] = item.Value;
                    }
                    if (item.Key == worksheet.Cells[4, 2 + i].Text)
                    {
                        worksheet.Cells[18, 2 + i] = item.Value;
                    }
                }
                //длинна КНБК
                worksheet.Cells[26, 2 + i] = _data.LengthBHA[_data.RunNumbers[i]];
                // Тип КНБК
                worksheet.Cells[27, 2 + i] = _data.BHAType[_data.RunNumbers[i]];
                //Циркуляция за рейс
                worksheet.Cells[29, 2 + i] = _data.Circulation[_data.RunNumbers[i]];
                //Часы бурения за рейс
                worksheet.Cells[30, 2 + i] = _data.DrillingHours[_data.RunNumbers[i]];
            }
        }
    }
}

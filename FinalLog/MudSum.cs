using Microsoft.Office.Interop.Excel;

namespace FinalLog
{
    class MudSum
    {
        private readonly Workbook _workbook;
        private readonly DataFromCore _data;


        public MudSum(Workbook workbook, DataFromCore data)
        {
            _workbook = workbook;
            _data = data;

            CompleteMudSum();
        }

        private void CompleteMudSum()
        {
            //Открываем вкладку EquipmentSummary
            Worksheet worksheet = (Worksheet)_workbook.Sheets["Mud Sum"];

            //очищаем колонки
            worksheet.Rows["5:30"] = "";
            int j = 0;
            //заполняем данные на основе номера рейса
            for (int i = 0; i < _data.RunNumbers.Count; i++)
            {

                //дата замера сопротивления
                foreach (var item in _data.DailyMudSum[_data.RunNumbers[i]])
                {
                    //глубина
                    worksheet.Cells[5 + j, 9] = item.Key;

                    //дата
                    worksheet.Cells[5 + j, 1] = item.Value[0];
                    //плотность раствора
                    worksheet.Cells[5 + j, 10] = item.Value[1];
                    //номер рейса
                    worksheet.Cells[5 + j, 7] = _data.RunNumbers[i];
                    //KCL
                    worksheet.Cells[5 + j, 11] = _data.KCL[_data.RunNumbers[i]];
                    //Rm
                    worksheet.Cells[5 + j, 13] = _data.Rm[_data.RunNumbers[i]];
                    //Rmf
                    worksheet.Cells[5 + j, 16] = _data.Rmf[_data.RunNumbers[i]];
                    //Rmc
                    worksheet.Cells[5 + j, 19] = _data.Rmc[_data.RunNumbers[i]];
                    //Температура
                    worksheet.Cells[5 + j, 22] = item.Value[2];
                    j += 2;
                }
            }
        }
    }
}

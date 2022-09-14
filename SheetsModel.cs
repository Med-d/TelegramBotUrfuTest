using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace TGBot;

public class SheetsModel
{
    private const string SpreadsheetId = "1odRASl5YqIXf_xvSnUXOXSwFi0pn1vzq02xM1u-2MP8";
    private const string SheetName = "Main";
    private readonly SpreadsheetsResource.ValuesResource _googleSheetValues;

    private static class ItemMapper
    {
        public static string MapFromRangeData(IEnumerable<IList<object>> values)
        {
            return values.Select(value => value[0].ToString()).ToList()[0] ?? string.Empty;
        }

        public static IList<IList<object>> MapToRangeData(string value)
        {
            var list = (IList<object>)new List<object> { value };
            return new List<IList<object>> { list };
        }
    }

    public SheetsModel(GoogleSheetsHelper googleSheetsHelper)
    {
        _googleSheetValues = googleSheetsHelper.Service.Spreadsheets.Values;
    }

    public string GetItem(string column, string row)
    {
        var range = $"{SheetName}!{column}{row}";
        var request = _googleSheetValues.Get(SpreadsheetId, range);
        var response = request.Execute();
        var values = ItemMapper.MapFromRangeData(response.Values);
        return values;
    }

    public void SetItem(string column, string row, string value)
    {
        var range = $"{SheetName}!{column}{row}";
        var valueRange = new ValueRange
        {
            Values = (IList<IList<object>>)ItemMapper.MapToRangeData(value)
        };
        var request = _googleSheetValues.Update(valueRange, SpreadsheetId, range);
        request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        request.Execute();
    }
}
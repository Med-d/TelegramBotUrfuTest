using Deployf.Botf;
using TGBot;

BotfProgram.StartBot(
    args, 
    onConfigure: (services, configuration) =>
    {
        services.AddSingleton(typeof(GoogleSheetsHelper));
        services.AddSingleton(typeof(SheetsModel));
    });

class ControllerStateExample : BotController
{
    private readonly SheetsModel _sheetsModel;

    public ControllerStateExample(GoogleSheetsHelper googleSheetsHelper)
    {
        _sheetsModel = new SheetsModel(googleSheetsHelper);
    }

    [State] private Item? _data;

    [State]
    int _intField;

    int _nonStateIntField;

    [Action("/start", "start the bot")]
    public void Start()
    {
        _data ??= new Item("", "", "");
        PushL($"Hello!");
        PushL("This is a bot for working with this Google Sheet: ");
        PushL("https://docs.google.com/spreadsheets/d/1odRASl5YqIXf_xvSnUXOXSwFi0pn1vzq02xM1u-2MP8/edit#gid=0");

        PushL();
        PushL("Choose what you want to do");

        RowButton("Set value", Q(SetValue));
        RowButton("Get value", Q(GetValue));
        RowButton("Info", Q(ShowInfoTemplate));
    }

    [Action]
    private void ShowInfoTemplate()
    {
        PushL("Бот написан для тестового задания УрФУ ИРИТ-РТФ");
        PushL("");
        PushL("Команда:");
        PushL("Олег Гайдабура, РИ-300001");
        PushL("Логвинов Максим, РИ-300001");
        PushL("Удовенко Владислав, РИ-300016");
        PushL("Недогреева Яна, РИ-300016");
        RowButton("Back", "/start");
        Send();
    }

    [Action]
    public async Task SetValue()
    {
        await Send("Hurry up and set new value\nEnter column");
        var column = await AwaitText().ConfigureAwait(false);
        _data.Column = column.ToUpper();

        await Send("Enter row").ConfigureAwait(false);
        var row = await AwaitText().ConfigureAwait(false);
        _data.Row = row;

        await Send("Enter value").ConfigureAwait(false);
        var value = await AwaitText().ConfigureAwait(false);
        _data.Value = value;
        _sheetsModel.SetItem(_data.Column, _data.Row, _data.Value);

        PushL($"Value {_data.Value} set to cell {_data.Column}{_data.Row}");
        RowButton("New action", "/start");

        _data = null;
        await Send();
    }

    [Action]
    async Task GetValue()
    {
        await Send("Let`s go to pull some values\nEnter column").ConfigureAwait(false);
        var column = await AwaitText().ConfigureAwait(false);
        _data.Column = column.ToUpper();

        await Send("Enter row").ConfigureAwait(false);
        var row = await AwaitText().ConfigureAwait(false);
        _data.Row = row;

        _data.Value = _sheetsModel.GetItem(_data.Column, _data.Row);

        PushL($"Value {_data.Value} located in cell {_data.Column}{_data.Row}");
        RowButton("New action", "/start");

        _data = null;
        await Send();
    }
}

record Item(string Row, string Column, string Value)
{
    public string Row { get; set; } = Row;
    public string Column { get; set; } = Column;
    public string Value { get; set; } = Value;
}
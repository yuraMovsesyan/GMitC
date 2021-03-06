using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace GMitC;

class MainWindow : Window
{
    [UI]
    private Label mainLabel;

    private Calculator Cal;

    private double NumberA;
    private double? NumberB;

    private double Memory;

    private string AStr;

    private string NumberAStr
    {
        get => AStr;
        set
        {
            AStr = value;
            NumberA = Convert.ToDouble(value);
            mainLabel.Text = StrFormat.GetNum(value);
        }
    }

    private bool IsAC, IsCal, IsDot;

    public MainWindow() : this(new Builder("MainWindow.glade")) { }

    private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
    {
        builder.Autoconnect(this);
        DeleteEvent += Window_DeleteEvent;

        Cal = new();

        NumberA = 0;
        NumberB = null;

        Memory = 0;

        AStr = "0";

        IsAC = false;
        IsCal = false;
        IsDot = false;
    }

    private void Window_DeleteEvent(object sender, DeleteEventArgs a) => Application.Quit();

    public void OnNumClick (object sender, EventArgs e)
    {
        if (IsCal)
        {
            NumberB = NumberA;
            NumberA = 0;
            AStr = "0";
        }

        if (NumberAStr.Contains(","))
            IsDot = true;

        NumberAStr = StrFormat.AddEnd(
            in AStr,
            ((Button)sender).Label,
            IsDot
        );

        IsAC = false;
        IsCal = false;
    }

    public void OnBackspace (object sender, EventArgs e)
    {
        NumberAStr = StrFormat.RemoveEnd(
            in AStr
        );

        if (!NumberAStr.Contains(","))
            IsDot = false;
    }
    
    public void OnAC (object sender, EventArgs e)
    {
        if (IsAC) 
        {
            Cal.DelOperation();
            NumberB = null;
        }
        else
        {
            IsAC = true;
        }

        NumberAStr = "0";
        //NumberA = 0;

        IsDot = false;
    }
    public void OnAction (object sender, EventArgs e)
    {
        if (NumberB is not null && Cal.IsOperation()) NumberB = Cal.CalRes(NumberB ?? 0, NumberA); 
        else NumberB = NumberA;

        //NumberAStr = "0";//TODO check P&R?
        NumberA = 0;
        AStr = "0";

        mainLabel.Text = StrFormat.GetNum(NumberB.ToString());

        IsAC = false;
        IsCal = false;
        IsDot = false;
    }
    
    public void OnCal (object sender, EventArgs e)
    {
        NumberA = NumberB ?? 0;
        AStr = NumberA.ToString();

        NumberB = null;

        Cal.DelOperation();

        IsCal = true;
    }

    public void OnDot (object sender, EventArgs e)
    { 
        if (!IsDot)
        {
            if (!NumberAStr.Contains(","))
            {
                if (IsCal)
                {
                    NumberAStr = "0";
                }
                NumberAStr += ",";
            }
            IsDot = true;
        }
    }

    public void OnPercent (object sender, EventArgs e)
    { 
        if (NumberB is not null)
        {
            NumberAStr = Cal.CalResDuo(new Percent(), NumberA, NumberB ?? 0).ToString();
        }
        else
        {
            NumberAStr = Cal.CalResUno(new Percent(), NumberA).ToString();
        }
    }

    public void OnMC (object sender, EventArgs e) => Memory = 0;

    public void OnMR (object sender, EventArgs e) => NumberAStr = Memory.ToString();

    public void OnMAdd (object sender, EventArgs e) => Memory += NumberA;

    public void OnMSub (object sender, EventArgs e) => Memory -= NumberA;

    public void OnAdd (object sender, EventArgs e) => Cal.SetOperation(new Add());

    public void OnSub (object sender, EventArgs e) => Cal.SetOperation(new Sub());

    public void OnMul (object sender, EventArgs e) => Cal.SetOperation(new Mul());

    public void OnDiv (object sender, EventArgs e) => Cal.SetOperation(new Div());
}
namespace WinFormsIncorrectThreadExample;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        // new Thread(() =>
        // {
        //     Text = "New title";
        // }).Start();

        Task.Run(() =>
        {
            Parallel.For(0, 10000, (i) =>
            {
                Text = i.ToString();
                Thread.Sleep(1);
            });
        });
    }
}
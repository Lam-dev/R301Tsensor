using System;

namespace SmartCard
{
    public partial class Form1 : Form
    {
        //FingerprintSensor _fingerprintSensor = new FingerprintSensor();
       
        public Form1()
        {
            InitializeComponent();
            //_fingerprintSensor.VerifyPassword();
            //_fingerprintSensor.SetSecurityLevel(4);

        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    timer1.Start();


        //}

        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    var haveFinger = _fingerprintSensor.ReadImage();
        //    if (haveFinger)
        //    {
        //        _fingerprintSensor.ConvertImage(1);
        //        _fingerprintSensor.ReadImage();
        //        _fingerprintSensor.ConvertImage(2);
        //        _fingerprintSensor.CreateTemplate();

        //        _fingerprintSensor.ReadImage();
        //        _fingerprintSensor.ConvertImage(2);
        //        _fingerprintSensor.CreateTemplate();

        //        var characteristic = _fingerprintSensor.DownloadCharacteristics(1);
        //    }

        //}
    }
}
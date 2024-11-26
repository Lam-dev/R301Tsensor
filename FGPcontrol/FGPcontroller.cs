using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartCard;
using System.Threading;
using System.Resources;
using System.Reflection;

namespace FGPcontrol
{
    public delegate void GetFGPresult(bool result, string error, List<byte>charateristics);
    public delegate void FGPcheckResult(bool match,  int index);
    public partial class FGPcontroller : UserControl
    {
        public event GetFGPresult EventGetFGPresult;
        public event FGPcheckResult EventUserChecked;
        private static global::System.Globalization.CultureInfo resourceCulture;
        bool __flagLockFGPsensor = false;
        int __addFGPstep = 1;
        int __justFinishStep = 1;
        bool __fingerPrintHoldOn = false;
        int __numberWrongPrePos = 0;
        FingerprintSensor __fingerprintObj = null;
        int __numberFGPimageShow = 0;
        bool __flagSensorConnected = false;
        bool __textIsShow = true; //for blink text
        public FGPcontroller()
        {
            InitializeComponent();
        }

        public void ConnectSensor(string comport)
        {
            __fingerprintObj = new FingerprintSensor(comport);
            __fingerprintObj.ConnectSensor();
            __flagSensorConnected = true;

        }
        public bool StartGetFGP()
        {
            if (!__flagSensorConnected)
            {
                return false;
            }
            timer_checkFGP.Stop();
            __ReturnBeginAddFGPstep();
            timer_getFGPfeature.Start();
            return true;
        }

        void __BlinkText()
        {
            label_notificationText.Invoke((MethodInvoker)(() => { label_notificationText.Visible = __textIsShow;}));
            __textIsShow = !__textIsShow;
        }

        public void StopGetFGP()
        {
            timer_getFGPfeature.Stop();
        }

        public bool AddTemplate(int index, List<byte> charateristics)
        {
            timer_getFGPfeature.Stop();
            timer_checkFGP.Stop();
            if (__flagLockFGPsensor)
            {
                return false;
            }
            var result = __fingerprintObj.UploadCharacteristics(0x01, charateristics);
            if (result)
            {
                __fingerprintObj.StoreTemplate(index, 0x01);
            }
            return result;
        }

        public bool StartCheckFGP()
        {
            timer_getFGPfeature.Stop();
            timer_checkFGP.Start();
            return true;
        }

        
        void GetFGPstepByStep()
        {
            if (__flagLockFGPsensor)
            {
                return;
            }
            //__BlinkText();
            __flagLockFGPsensor = true;
            if (__addFGPstep == 1)
            {
                try
                {
                    StatusNotify(AddFingerprintNotificationEnum.PushOn);
                    if (__fingerprintObj.ReadImage())
                    {
                        try
                        {
                            __fingerprintObj.ConvertImage(0x01);
                            ShowFPGanimation(1);

                        }
                        catch (Exception ex)
                        {
                            __fingerPrintHoldOn = true;
                        }
                        __fingerPrintHoldOn = true;
                        __addFGPstep = 2;
                    }

                }
                catch
                {
                    __fingerPrintHoldOn = false;
                }
            }
            else if (__addFGPstep == 2)
            {
                try
                {
                    if (__fingerprintObj.ReadImage())
                    {
                        if (__fingerPrintHoldOn)
                        {
                            StatusNotify(AddFingerprintNotificationEnum.PushOff);
                        }
                        else
                        {
                            try
                            {
                                __fingerprintObj.ConvertImage(0x02);
                            }
                            catch
                            {
                                __fingerPrintHoldOn = true;
                            }
                            if (!__fingerprintObj.CreateTemplate())
                            {
                                __numberWrongPrePos += 1;
                                if (__numberWrongPrePos == 4)
                                {
                                    __addFGPstep = 1;
                                    ShowFPGanimation(1);
                                    __numberWrongPrePos = 0;
                                }
                                StatusNotify(AddFingerprintNotificationEnum.WrongPose);
                            }
                            else
                            {
                                ShowFPGanimation(2);
                                __fingerPrintHoldOn = true;
                                __addFGPstep = 3;

                            }
                        }
                    }
                    else
                    {
                        __fingerPrintHoldOn = false;
                        StatusNotify(AddFingerprintNotificationEnum.PushOn);
                    }
                }
                catch
                {

                }
            }
            else if (__addFGPstep == 3)
            {
                try
                {
                    if (__fingerprintObj.ReadImage())
                    {
                        if (__fingerPrintHoldOn)
                        {
                            StatusNotify(AddFingerprintNotificationEnum.PushOff);
                        }
                        else
                        {
                            try
                            {
                                __fingerprintObj.ConvertImage(0x02);
                            }
                            catch
                            {
                                __fingerPrintHoldOn = true;
                            }
                            if (!__fingerprintObj.CreateTemplate())
                            {
                                __numberWrongPrePos += 1;
                                if (__numberWrongPrePos == 4)
                                {
                                    __addFGPstep = 1;
                                    ShowFPGanimation(2);
                                    __numberWrongPrePos = 0;
                                }
                            }
                            else {
                                ShowFPGanimation(3);
                                __addFGPstep = 3;
                                __fingerPrintHoldOn = true;
                                DownloadCharacteristic();

                            }
                        }
                    }
                    else
                    {
                        __fingerPrintHoldOn = false;
                        StatusNotify(AddFingerprintNotificationEnum.PushOn);
                    }
                }
                catch {
                }
            }
            __flagLockFGPsensor = false;
        }
        void ShowFPGanimation(int step)
        {
            __justFinishStep = step;
            label_notificationText.Invoke((MethodInvoker)(() => timer_showFGPimageAnimation.Start()));

        }

        void __ReturnBeginAddFGPstep()
        {
            __addFGPstep = 1;
            __justFinishStep = 1;
            __numberWrongPrePos = 0;
            StatusNotify(AddFingerprintNotificationEnum.PushOn);
            pictureBox_showFGPicon.Image = Icon._0;
        }

        void DownloadCharacteristic()
        {
            var lstFGPfeature = __fingerprintObj.DownloadCharacteristics(0x01);
            if (EventGetFGPresult != null)
            {
                EventGetFGPresult(true, "", lstFGPfeature);
            }
            //__addFGPstep = 1;
            //__numberWrongPrePos = 0;
            //__numberFGPimageShow = 0;
            StatusNotify(AddFingerprintNotificationEnum.Ok);
            label_notificationText.Invoke((MethodInvoker)(() => timer_getFGPfeature.Stop()));
        }

        private void timer_showFGPimageAnimation_Tick(object sender, EventArgs e)
        {
            Console.WriteLine($"step = {__numberFGPimageShow}");
            if (__numberFGPimageShow > __justFinishStep * 5)
            {
                __numberFGPimageShow = (__justFinishStep - 1) * 5;
            }
            else if (__numberFGPimageShow == __justFinishStep * 5)
            {
                timer_showFGPimageAnimation.Stop();
            }
            else
            {
                pictureBox_showFGPicon.Invoke((MethodInvoker)(() => { pictureBox_showFGPicon.Image = (Bitmap)Icon.ResourceManager.GetObject($"_{__numberFGPimageShow}");}));
                __numberFGPimageShow += 1;
            }   
        }

        void StatusNotify(AddFingerprintNotificationEnum notificationCode)
        {
            if (notificationCode == AddFingerprintNotificationEnum.PushOn)
            {
                pictureBox_notificationIcon.Invoke((MethodInvoker)(() => pictureBox_notificationIcon.Image = Icon.put55));
                label_notificationText.Invoke((MethodInvoker)(() =>
                {
                    label_notificationText.Text = "ĐẶT TAY LÊN CẢM BIẾN";
                    label_notificationText.ForeColor = Color.Blue;
                }));
            }

            else if (notificationCode == AddFingerprintNotificationEnum.PushOff)
            {
                pictureBox_notificationIcon.Invoke((MethodInvoker)(() => pictureBox_notificationIcon.Image = Icon.notPut55));
                label_notificationText.Invoke((MethodInvoker)(() =>
                {
                    label_notificationText.Text = "NHẤC TAY RA KHỎI CẢM BIẾN";
                    label_notificationText.ForeColor = Color.Blue;

                }));
            }

            else if (notificationCode == AddFingerprintNotificationEnum.WrongPose)
            {
                pictureBox_notificationIcon.Invoke((MethodInvoker)(() => pictureBox_notificationIcon.Image = Icon.iconWarning));
                label_notificationText.Invoke((MethodInvoker)(() =>
                {
                    label_notificationText.Text = "KHÔNG THAY ĐỔI VỊ TRÍ NGÓN TAY, NHẤC TAY ĐẶT LẠI";
                    label_notificationText.ForeColor = Color.Red;
                }));
            }

            else if (notificationCode == AddFingerprintNotificationEnum.Ok)
            {
                pictureBox_notificationIcon.Invoke((MethodInvoker)(() => pictureBox_notificationIcon.Image = Icon.ok55));
                label_notificationText.Invoke((MethodInvoker)(() =>
                {
                    label_notificationText.Text = "ĐÃ LẤY VÂN TAY THÀNH CÔNG. XIN CẢM ƠN !";
                    label_notificationText.ForeColor = Color.Green;

                }));
            }

        }

        private void timer_getFGPfeature_Tick(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                GetFGPstepByStep();
            }).Start();
        }

        private void timer_checkFGP_Tick(object sender, EventArgs e)
        {
            new Thread(() => {
                try
                {
                    if (__fingerprintObj.ReadImage())
                    {
                        __fingerprintObj.ConvertImage(0x01);
                        var findIndex = __fingerprintObj.SearchTemplate();

                        if (EventUserChecked != null)
                        {
                            if (findIndex.Item1 == -1)
                            {
                                EventUserChecked(false, -1);
                            }
                            else
                            {
                                EventUserChecked(true, findIndex.Item1);
                            }
                        }
                    }
                }
                catch
                {

                }
            }).Start();
        }

    }

    enum AddFingerprintNotificationEnum
    {
        PushOn,
        PushOff,
        WrongPose,
        Ok
    }
}

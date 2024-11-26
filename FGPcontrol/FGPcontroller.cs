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
    public partial class FGPcontroller : UserControl
    {
        public event GetFGPresult EventGetFGPresult;
        private static global::System.Globalization.CultureInfo resourceCulture;
        bool __flagLockFGPsensor = false;
        int __addFGPstep = 1;
        bool __fingerPrintHoldOn = false;
        int __numberWrongPrePos = 0;
        FingerprintSensor __fingerprintObj = null;
        int __numberFGPimageShow = 0;
        bool __flagSensorConnected = false;
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
            timer_getFGPfeature.Start();
            return true;
        }

        public void StopGetFGP()
        {
            timer_getFGPfeature.Stop();
        }
        void GetFGPstepByStep()
        {
            if (__flagLockFGPsensor)
            {
                return;
            }
            __flagLockFGPsensor = true;
            if (__addFGPstep == 1)
            {
                try
                {
                    if (__fingerprintObj.ReadImage())
                    {
                        try
                        {
                            __fingerprintObj.ConvertImage(0x01);
                            label_notificationText.Invoke((MethodInvoker)(() => timer_showFGPimageAnimation.Start()));
                           
                        }
                        catch(Exception ex)
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
                                    label_notificationText.Invoke((MethodInvoker)(() => timer_showFGPimageAnimation.Start()));
                                    __numberWrongPrePos = 0;
                                }
                                StatusNotify(AddFingerprintNotificationEnum.WrongPose);
                            }
                            else
                            {
                                label_notificationText.Invoke((MethodInvoker)(() => timer_showFGPimageAnimation.Start()));
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
                                    label_notificationText.Invoke((MethodInvoker)(() => timer_showFGPimageAnimation.Start()));
                                    __numberWrongPrePos = 0;
                                }
                            }
                            else {
                                StatusNotify(AddFingerprintNotificationEnum.Ok);
                                __addFGPstep = 3;
                                label_notificationText.Invoke((MethodInvoker)(() => timer_showFGPimageAnimation.Start()));
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

        void DownloadCharacteristic()
        {
            var lstFGPfeature = __fingerprintObj.DownloadCharacteristics(0x01);
            if (EventGetFGPresult != null)
            {
                EventGetFGPresult(true, "", lstFGPfeature);
            }
            __addFGPstep = 0;
            __numberWrongPrePos = 0;
            __numberFGPimageShow = 0;
            label_notificationText.Invoke((MethodInvoker)(() => timer_getFGPfeature.Stop()));
        }

        private void timer_showFGPimageAnimation_Tick(object sender, EventArgs e)
        {
            Console.WriteLine($"step = {__addFGPstep}");
            if (__numberFGPimageShow > __addFGPstep * 5)
            {
                __numberFGPimageShow = (__addFGPstep - 1) * 5;
            }
            else if (__numberFGPimageShow == __addFGPstep * 5)
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
                    label_notificationText.Text = "TIẾP TỤC ĐẶT TAY LÊN CẢM BIẾN";

                }));
            }

            else if (notificationCode == AddFingerprintNotificationEnum.PushOff)
            {
                pictureBox_notificationIcon.Invoke((MethodInvoker)(() => pictureBox_notificationIcon.Image = Icon.notPut55));
                label_notificationText.Invoke((MethodInvoker)(() =>
                {
                    label_notificationText.Text = "NHẤC TAY RA KHỎI CẢM BIẾN";

                }));
            }

            else if (notificationCode == AddFingerprintNotificationEnum.WrongPose)
            {
                pictureBox_notificationIcon.Invoke((MethodInvoker)(() => pictureBox_notificationIcon.Image = Icon.iconWarning));
                label_notificationText.Invoke((MethodInvoker)(() =>
                {
                    label_notificationText.Text = "KHÔNG THAY ĐỔI VỊ TRÍ NGÓN TAY, NHẤC TAY ĐẶT LẠI";

                }));
            }

            else if (notificationCode == AddFingerprintNotificationEnum.Ok)
            {
                pictureBox_notificationIcon.Invoke((MethodInvoker)(() => pictureBox_notificationIcon.Image = Icon.ok55));
                label_notificationText.Invoke((MethodInvoker)(() =>
                {
                    label_notificationText.Text = "ĐÃ LẤY VÂN TAY THÀNH CÔNG. XIN CẢM ƠN !";

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
    }

    enum AddFingerprintNotificationEnum
    {
        PushOn,
        PushOff,
        WrongPose,
        Ok
    }
}

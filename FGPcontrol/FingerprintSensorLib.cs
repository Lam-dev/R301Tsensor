using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace SmartCard
{
    public class FingerprintSensor
    {
        private const ushort FINGERPRINT_STARTCODE = 0xEF01;

        private const byte FINGERPRINT_COMMANDPACKET = 0x01;
        private const byte FINGERPRINT_ACKPACKET = 0x07;
        private const byte FINGERPRINT_DATAPACKET = 0x02;
        private const byte FINGERPRINT_ENDDATAPACKET = 0x08;

        private const byte FINGERPRINT_VERIFYPASSWORD = 0x13;
        private const byte FINGERPRINT_SETPASSWORD = 0x12;
        private const byte FINGERPRINT_SETADDRESS = 0x15;
        private const byte FINGERPRINT_SETSYSTEMPARAMETER = 0x0E;
        private const byte FINGERPRINT_GETSYSTEMPARAMETERS = 0x0F;
        private const byte FINGERPRINT_TEMPLATEINDEX = 0x1F;
        private const byte FINGERPRINT_TEMPLATECOUNT = 0x1D;
        private const byte FINGERPRINT_READIMAGE = 0x01;
        private const byte FINGERPRINT_DOWNLOADIMAGE = 0x0A;
        private const byte FINGERPRINT_CONVERTIMAGE = 0x02;
        private const byte FINGERPRINT_CREATETEMPLATE = 0x05;
        private const byte FINGERPRINT_STORETEMPLATE = 0x06;
        private const byte FINGERPRINT_SEARCHTEMPLATE = 0x04;
        private const byte FINGERPRINT_LOADTEMPLATE = 0x07;
        private const byte FINGERPRINT_DELETETEMPLATE = 0x0C;
        private const byte FINGERPRINT_CLEARDATABASE = 0x0D;
        private const byte FINGERPRINT_GENERATERANDOMNUMBER = 0x14;
        private const byte FINGERPRINT_COMPARECHARACTERISTICS = 0x03;
        private const byte FINGERPRINT_UPLOADCHARACTERISTICS = 0x09;
        private const byte FINGERPRINT_DOWNLOADCHARACTERISTICS = 0x08;

        private const byte FINGERPRINT_SETSYSTEMPARAMETER_BAUDRATE = 4;
        private const byte FINGERPRINT_SETSYSTEMPARAMETER_SECURITY_LEVEL = 5;
        private const byte FINGERPRINT_SETSYSTEMPARAMETER_PACKAGE_SIZE = 6;

        private const byte FINGERPRINT_OK = 0x00;
        private const byte FINGERPRINT_ERROR_COMMUNICATION = 0x01;
        private const byte FINGERPRINT_ERROR_WRONGPASSWORD = 0x13;
        private const byte FINGERPRINT_ERROR_INVALIDREGISTER = 0x1A;
        private const byte FINGERPRINT_ERROR_NOFINGER = 0x02;
        private const byte FINGERPRINT_ERROR_READIMAGE = 0x03;
        private const byte FINGERPRINT_ERROR_MESSYIMAGE = 0x06;
        private const byte FINGERPRINT_ERROR_FEWFEATUREPOINTS = 0x07;
        private const byte FINGERPRINT_ERROR_INVALIDIMAGE = 0x15;
        private const byte FINGERPRINT_ERROR_CHARACTERISTICSMISMATCH = 0x0A;
        private const byte FINGERPRINT_ERROR_INVALIDPOSITION = 0x0B;
        private const byte FINGERPRINT_ERROR_FLASH = 0x18;
        private const byte FINGERPRINT_ERROR_NOTEMPLATEFOUND = 0x09;
        private const byte FINGERPRINT_ERROR_LOADTEMPLATE = 0x0C;
        private const byte FINGERPRINT_ERROR_DELETETEMPLATE = 0x10;
        private const byte FINGERPRINT_ERROR_CLEARDATABASE = 0x11;
        private const byte FINGERPRINT_ERROR_NOTMATCHING = 0x08;
        private const byte FINGERPRINT_ERROR_DOWNLOADIMAGE = 0x0F;
        private const byte FINGERPRINT_ERROR_DOWNLOADCHARACTERISTICS = 0x0D;

        private const byte FINGERPRINT_ADDRCODE = 0x20;
        private const byte FINGERPRINT_PASSVERIFY = 0x21;
        private const byte FINGERPRINT_PACKETRESPONSEFAIL = 0x0E;
        private const byte FINGERPRINT_ERROR_TIMEOUT = 0x01;
        private const byte FINGERPRINT_ERROR_BADPACKET = 0xFE;

        private const byte FINGERPRINT_CHARBUFFER1 = 0x01;
        private const byte FINGERPRINT_CHARBUFFER2 = 0x02;

        private uint _address;
        private uint _password;
        private SerialPort _serial;
        private string _port = "COM0";
        public bool flagSensorConnected = false;
        public FingerprintSensor(string port = "COM7", int baudRate = 57600, uint address = 0xFFFFFFFF, uint password = 0x00000000)
        {
            if (baudRate < 9600 || baudRate > 115200 || baudRate % 9600 != 0)
            {
                throw new ArgumentException("The given baudrate is invalid!");
            }

            if (address < 0x00000000 || address > 0xFFFFFFFF)
            {
                throw new ArgumentException("The given address is invalid!");
            }

            if (password < 0x00000000 || password > 0xFFFFFFFF)
            {
                throw new ArgumentException("The given password is invalid!");
            }

            _address = address;
            _password = password;
            _port = port;
            
        }

        public void ConnectSensor()
        {
            
            _serial = new SerialPort(_port, 57600);
            _serial.ReadTimeout = 2500;

            if (_serial.IsOpen)
            {
                _serial.Close();
            }

            _serial.Open();
            flagSensorConnected = true;
        }

        ~FingerprintSensor()
        {
            if (_serial != null && _serial.IsOpen)
            {
                _serial.Close();
            }
        }
        private byte RightShift(int n, int x)
        {
            return (byte)(n >> x & 0xFF);
        }

        private byte LeftShift(int n, int x)
        {
            return (byte)(n << x);
        }

        private int BitAtPosition(int n, int p)
        {
            int twoP = 1 << p;
            int result = n & twoP;
            return result > 0 ? 1 : 0;
        }

        private byte[] ByteToString(byte b)
        {
            return new byte[] { b };
        }

        private byte StringToByte(byte[] bytes)
        {
            return bytes[0];
        }

        private void WritePacket(byte packetType, byte[] packetPayload)
        {
            _serial.Write(ByteToString(RightShift(FINGERPRINT_STARTCODE, 8)), 0, 1);
            _serial.Write(ByteToString(RightShift(FINGERPRINT_STARTCODE, 0)), 0, 1);

            _serial.Write(ByteToString(RightShift((int)_address, 24)), 0, 1);
            _serial.Write(ByteToString(RightShift((int)_address, 16)), 0, 1);
            _serial.Write(ByteToString(RightShift((int)_address, 8)), 0, 1);
            _serial.Write(ByteToString(RightShift((int)_address, 0)), 0, 1);

            _serial.Write(ByteToString(packetType), 0, 1);

            int packetLength = packetPayload.Length + 2;

            _serial.Write(ByteToString(RightShift(packetLength, 8)), 0, 1);
            _serial.Write(ByteToString(RightShift(packetLength, 0)), 0, 1);

            int packetChecksum = packetType + RightShift(packetLength, 8) + RightShift(packetLength, 0);

            foreach (byte b in packetPayload)
            {
                _serial.Write(ByteToString(b), 0, 1);
                packetChecksum += b;
            }

            _serial.Write(ByteToString(RightShift(packetChecksum, 8)), 0, 1);
            _serial.Write(ByteToString(RightShift(packetChecksum, 0)), 0, 1);
        }

        private (byte, List<byte>) ReadPacket()
        {
            List<byte> receivedPacketData = new List<byte>();
            int i = 0;
            int timeReadNoData = 0;

            while (true)
            {
                byte[] receivedFragment = new byte[1];
                int bytesRead = _serial.Read(receivedFragment, 0, 1);
                if (bytesRead > 0)
                {
                    timeReadNoData = 0;
                   
                }
                else
                {
                    timeReadNoData++;
                    if (timeReadNoData == 3)
                    {
                        return (0, null);
                    }
                    continue;
                }
                receivedPacketData.Add(receivedFragment[0]);
                i += 1;

                if (i >= 12)
                {
                    if (receivedPacketData[0] != RightShift(FINGERPRINT_STARTCODE, 8) || receivedPacketData[1] != RightShift(FINGERPRINT_STARTCODE, 0))
                    {
                        _serial.ReadExisting();
                        throw new Exception("The received packet do not begin with a valid header!");
                       
                    }

                    int packetPayloadLength = LeftShift(receivedPacketData[7], 8) | LeftShift(receivedPacketData[8], 0);

                    if (i < packetPayloadLength + 9)
                    {
                        continue;
                    }
                
                    byte packetType = receivedPacketData[6];
                    int packetChecksum = packetType + receivedPacketData[7] + receivedPacketData[8];
                    List<byte> packetPayload = new List<byte>();

                    for (int j = 9; j < 9 + packetPayloadLength - 2; j++)
                    {
                        packetPayload.Add(receivedPacketData[j]);
                        packetChecksum += receivedPacketData[j];
                    }
                    //int receivedChecksum = LeftShift(receivedPacketData[i - 2], 8);
                    //receivedChecksum = receivedChecksum | LeftShift(receivedPacketData[i - 1], 0);
                    int receivedChecksum = receivedPacketData[i - 2] * 256 + receivedPacketData[i - 1];
                    if (receivedChecksum != packetChecksum)
                    {
                        throw new Exception("The received packet is corrupted (the checksum is wrong)!");
                    }

                    return (packetType, packetPayload);
                }
            }
        }

        public bool VerifyPassword()
        {
            byte[] packetPayload = {
            FINGERPRINT_VERIFYPASSWORD,
            RightShift((int)_password, 24),
            RightShift((int)_password, 16),
            RightShift((int)_password, 8),
            RightShift((int)_password, 0),
        };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                return true;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ADDRCODE)
            {
                throw new Exception("The address is wrong");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_WRONGPASSWORD)
            {
                return false;
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }

        public bool SetPassword(uint newPassword)
        {
            if (newPassword < 0x00000000 || newPassword > 0xFFFFFFFF)
            {
                throw new ArgumentException("The given password is invalid!");
            }

            byte[] packetPayload = {
            FINGERPRINT_SETPASSWORD,
            RightShift((int)newPassword, 24),
            RightShift((int)newPassword, 16),
            RightShift((int)newPassword, 8),
            RightShift((int)newPassword, 0),
        };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                _password = newPassword;
                return true;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }

        public bool SetAddress(uint newAddress)
        {
            if (newAddress < 0x00000000 || newAddress > 0xFFFFFFFF)
            {
                throw new ArgumentException("The given address is invalid!");
            }

            byte[] packetPayload = {
            FINGERPRINT_SETADDRESS,
            RightShift((int)newAddress, 24),
            RightShift((int)newAddress, 16),
            RightShift((int)newAddress, 8),
            RightShift((int)newAddress, 0),
        };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                _address = newAddress;
                return true;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }

        public bool SetSystemParameter(byte parameterNumber, byte parameterValue)
        {
            if (parameterNumber == FINGERPRINT_SETSYSTEMPARAMETER_BAUDRATE)
            {
                if (parameterValue < 1 || parameterValue > 12)
                {
                    throw new ArgumentException("The given baudrate parameter is invalid!");
                }
            }
            else if (parameterNumber == FINGERPRINT_SETSYSTEMPARAMETER_SECURITY_LEVEL)
            {
                if (parameterValue < 1 || parameterValue > 5)
                {
                    throw new ArgumentException("The given security level parameter is invalid!");
                }
            }
            else if (parameterNumber == FINGERPRINT_SETSYSTEMPARAMETER_PACKAGE_SIZE)
            {
                if (parameterValue < 0 || parameterValue > 3)
                {
                    throw new ArgumentException("The given package length parameter is invalid!");
                }
            }
            else
            {
                throw new ArgumentException("The given parameter number is invalid!");
            }

            byte[] packetPayload = {
            FINGERPRINT_SETSYSTEMPARAMETER,
            parameterNumber,
            parameterValue,
            };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                return true;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_INVALIDREGISTER)
            {
                throw new Exception("Invalid register number");
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }

        public void SetSecurityLevel(byte securityLevel)
        {
            SetSystemParameter(FINGERPRINT_SETSYSTEMPARAMETER_SECURITY_LEVEL, securityLevel);


        }

        public void SetBaudRate(int baudRate)
        {
            if (baudRate % 9600 != 0)
            {
                throw new ArgumentException("Invalid baudrate");
            }

            SetSystemParameter(FINGERPRINT_SETSYSTEMPARAMETER_BAUDRATE, (byte)(baudRate / 9600));
        }

        public void SetSecurityLevel(int securityLevel)
        {
            SetSystemParameter(FINGERPRINT_SETSYSTEMPARAMETER_SECURITY_LEVEL, (byte)securityLevel);
        }

        public void SetMaxPacketSize(int packetSize)
        {
            Dictionary<int, byte> packetSizes = new Dictionary<int, byte>
        {
            { 32, 0 },
            { 64, 1 },
            { 128, 2 },
            { 256, 3 }
        };

            if (!packetSizes.TryGetValue(packetSize, out byte packetMaxSizeType))
            {
                throw new ArgumentException("Invalid packet size");
            }

            SetSystemParameter(FINGERPRINT_SETSYSTEMPARAMETER_PACKAGE_SIZE, packetMaxSizeType);
        }

        public (ushort, ushort, ushort, ushort, uint, ushort, ushort) GetSystemParameters()
        {
            byte[] packetPayload = {
            FINGERPRINT_GETSYSTEMPARAMETERS,
            };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                ushort statusRegister = (ushort)((receivedPacketPayload[1] << 8) | receivedPacketPayload[2]);
                ushort systemID = (ushort)((receivedPacketPayload[3] << 8) | receivedPacketPayload[4]);
                ushort storageCapacity = (ushort)((receivedPacketPayload[5] << 8) | receivedPacketPayload[6]);
                ushort securityLevel = (ushort)((receivedPacketPayload[7] << 8) | receivedPacketPayload[8]);
                uint deviceAddress = (uint)((receivedPacketPayload[9] << 24) | (receivedPacketPayload[10] << 16) | (receivedPacketPayload[11] << 8) | receivedPacketPayload[12]);
                ushort packetLength = (ushort)((receivedPacketPayload[13] << 8) | receivedPacketPayload[14]);
                ushort baudRate = (ushort)((receivedPacketPayload[15] << 8) | receivedPacketPayload[16]);

                return (statusRegister, systemID, storageCapacity, securityLevel, deviceAddress, packetLength, baudRate);
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }
        public int GetStorageCapacity()
        {

            return GetSystemParameters().Item3;
        }

        public bool ReadImage()
        {
            byte[] packetPayload = {
            FINGERPRINT_READIMAGE,
            };
            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            //if (receivedPacket == null)
            //{
            //    return false;
            //}

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                return true;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_NOFINGER)
            {
                return false;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_READIMAGE)
            {
                throw new Exception("Could not read image");
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }

        public bool ConvertImage(byte charBufferNumber = FINGERPRINT_CHARBUFFER1)
        {
            if (charBufferNumber != FINGERPRINT_CHARBUFFER1 && charBufferNumber != FINGERPRINT_CHARBUFFER2)
            {
                throw new ArgumentException("The given charbuffer number is invalid!");
            }

            byte[] packetPayload = { FINGERPRINT_CONVERTIMAGE, charBufferNumber };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                return true;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_MESSYIMAGE)
            {
                throw new Exception("The image is too messy");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_FEWFEATUREPOINTS)
            {
                throw new Exception("The image contains too few feature points");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_INVALIDIMAGE)
            {
                throw new Exception("The image is invalid");
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }

        public bool CreateTemplate()
        {
            byte[] packetPayload = { FINGERPRINT_CREATETEMPLATE };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                return true;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_CHARACTERISTICSMISMATCH)
            {
                return false;
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }

        public List<byte> DownloadCharacteristics(byte charBufferNumber = FINGERPRINT_CHARBUFFER1)
        {
            if (charBufferNumber != FINGERPRINT_CHARBUFFER1 && charBufferNumber != FINGERPRINT_CHARBUFFER2)
            {
                throw new ArgumentException("The given charbuffer number is invalid!");
            }

            byte[] packetPayload = { FINGERPRINT_DOWNLOADCHARACTERISTICS, charBufferNumber };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);

            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                // Pass
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_DOWNLOADCHARACTERISTICS)
            {
                throw new Exception("Could not download characteristics");
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }

            var completePayload = new List<byte>();

            while (receivedPacketType != FINGERPRINT_ENDDATAPACKET)
            {
                receivedPacket = ReadPacket();

                receivedPacketType = receivedPacket.Item1;
                receivedPacketPayload = receivedPacket.Item2;

                if (receivedPacketType != FINGERPRINT_DATAPACKET && receivedPacketType != FINGERPRINT_ENDDATAPACKET)
                {
                    throw new Exception("The received packet is no data packet!");
                }

                completePayload.AddRange(receivedPacketPayload);
            }

            return completePayload;
        }
        
        public bool DeleteTemplate(int positionNumber, int count = 1)
        {
            int capacity = GetStorageCapacity();

            if (positionNumber < 0x0000 || positionNumber >= capacity)
            {
                throw new ArgumentException("The given position number is invalid!");
            }

            if (count < 0x0000 || count > capacity - positionNumber)
            {
                throw new ArgumentException("The given count is invalid!");
            }

            byte[] packetPayload = 
            {
            FINGERPRINT_DELETETEMPLATE,
            (byte)(positionNumber >> 8),
            (byte)(positionNumber),
            (byte)(count >> 8),
            (byte)(count)
            };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                return true;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_INVALIDPOSITION)
            {
                throw new Exception("Invalid position");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_DELETETEMPLATE)
            {
                return false;
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }

        public bool ClearDatabase()
        {
            var packetPayload = new List<byte>
        {
            FINGERPRINT_CLEARDATABASE
        };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload.ToArray());
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                return true;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_CLEARDATABASE)
            {
                return false;
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }

        public int CompareCharacteristics()
        {
            var packetPayload = new List<byte>
            {
            FINGERPRINT_COMPARECHARACTERISTICS
            };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload.ToArray());
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                int accuracyScore = (receivedPacketPayload[1] << 8) | receivedPacketPayload[2];
                return accuracyScore;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_NOTMATCHING)
            {
                return 0;
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }

        public int GetMaxPacketSize()
        {
            var packetMaxSizeType = GetSystemParameters().Item6;

            return packetMaxSizeType;

        }
        public List<bool> GetTemplateIndex(int page)
        {
            if (page < 0 || page > 3)
            {
                throw new ArgumentException("The given index page is invalid!");
            }

            var packetPayload = new byte[] { FINGERPRINT_TEMPLATEINDEX, (byte)page };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                var templateIndex = new List<bool>();
                var pageElements = receivedPacketPayload.GetRange(1, receivedPacketPayload.Count-1);

                foreach (var pageElement in pageElements)
                {
                    for (int p = 0; p <= 7; p++)
                    {
                        bool positionIsUsed = (BitAtPosition(pageElement, p) == 1);
                        templateIndex.Add(positionIsUsed);
                    }
                }

                return templateIndex;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }


        public bool UploadCharacteristics(byte charBufferNumber = FINGERPRINT_CHARBUFFER1, List<byte> characteristicsData = null)
        {
            if (charBufferNumber != FINGERPRINT_CHARBUFFER1 && charBufferNumber != FINGERPRINT_CHARBUFFER2)
            {
                throw new ArgumentException("The given charbuffer number is invalid!");
            }

            if (characteristicsData == null || characteristicsData.Count == 0)
            {
                throw new ArgumentException("The characteristics data is required!");
            }

            int maxPacketSize = GetMaxPacketSize();

            byte[] packetPayload = 
            {
                FINGERPRINT_UPLOADCHARACTERISTICS,
                charBufferNumber
            };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);

            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                // Pass
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_PACKETRESPONSEFAIL)
            {
                throw new Exception("Could not upload characteristics");
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }

            int packetNbr = characteristicsData.Count / maxPacketSize;

            if (packetNbr <= 1)
            {
                WritePacket(FINGERPRINT_ENDDATAPACKET, characteristicsData.ToArray());
            }
            else
            {
                for (int i = 1; i < packetNbr; i++)
                {
                    int lfrom = (i - 1) * maxPacketSize;
                    int lto = lfrom + maxPacketSize;
                    WritePacket(FINGERPRINT_DATAPACKET, characteristicsData.GetRange(lfrom, lto - lfrom).ToArray());
                }

                int lfromLast = (packetNbr - 1) * maxPacketSize;
                int ltoLast = lfromLast + maxPacketSize;
                WritePacket(FINGERPRINT_ENDDATAPACKET, characteristicsData.GetRange(lfromLast, ltoLast - lfromLast).ToArray());
            }

            var characterics = DownloadCharacteristics(charBufferNumber);
            return characterics.SequenceEqual(characteristicsData);
        }

        public int StoreTemplate(int positionNumber = -1, byte charBufferNumber = FINGERPRINT_CHARBUFFER1)
        {
            if (positionNumber == -1)
            {
                for (int page = 0; page < 4; page++)
                {
                    if (positionNumber >= 0)
                        break;

                    List<bool> templateIndex = GetTemplateIndex(page);

                    for (int i = 0; i < templateIndex.Count; i++)
                    {
                        if (!templateIndex[i])
                        {
                            positionNumber = (templateIndex.Count * page) + i;
                            break;
                        }
                    }
                }
            }

            if (positionNumber < 0x0000 || positionNumber >= GetStorageCapacity())
            {
                throw new ArgumentException("The given position number is invalid!");
            }

            if (charBufferNumber != FINGERPRINT_CHARBUFFER1 && charBufferNumber != FINGERPRINT_CHARBUFFER2)
            {
                throw new ArgumentException("The given charbuffer number is invalid!");
            }

            byte[] packetPayload = {
            FINGERPRINT_STORETEMPLATE,
            charBufferNumber,
            (byte)(positionNumber >> 8),
            (byte)(positionNumber >> 0),
        };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                return positionNumber;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_INVALIDPOSITION)
            {
                throw new Exception("Could not store template in that position");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_FLASH)
            {
                throw new Exception("Error writing to flash");
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }

        public Tuple<int, int> SearchTemplate()
        {
            byte charBufferNumber = FINGERPRINT_CHARBUFFER1;
            int positionStart = 0x0000;
            int templatesCount = GetStorageCapacity();

            byte[] packetPayload = {
            FINGERPRINT_SEARCHTEMPLATE,
            charBufferNumber,
            (byte)(positionStart >> 8),
            (byte)(positionStart >> 0),
            (byte)(templatesCount >> 8),
            (byte)(templatesCount >> 0),
        };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                int positionNumber = (receivedPacketPayload[1] << 8) | receivedPacketPayload[2];
                int accuracyScore = (receivedPacketPayload[3] << 8) | receivedPacketPayload[4];

                return Tuple.Create(positionNumber, accuracyScore);
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_NOTEMPLATEFOUND)
            {
                return Tuple.Create(-1, -1);
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }

        public bool LoadTemplate(int positionNumber, byte charBufferNumber = FINGERPRINT_CHARBUFFER1)
        {
            if (positionNumber < 0x0000 || positionNumber >= GetStorageCapacity())
            {
                throw new ArgumentException("The given position number is invalid!");
            }

            if (charBufferNumber != FINGERPRINT_CHARBUFFER1 && charBufferNumber != FINGERPRINT_CHARBUFFER2)
            {
                throw new ArgumentException("The given charbuffer number is invalid!");
            }

            byte[] packetPayload = {
            FINGERPRINT_LOADTEMPLATE,
            charBufferNumber,
            (byte)(positionNumber >> 8),
            (byte)(positionNumber >> 0),
        };

            WritePacket(FINGERPRINT_COMMANDPACKET, packetPayload);
            var receivedPacket = ReadPacket();

            byte receivedPacketType = receivedPacket.Item1;
            List<byte> receivedPacketPayload = receivedPacket.Item2;

            if (receivedPacketType != FINGERPRINT_ACKPACKET)
            {
                throw new Exception("The received packet is no ack packet!");
            }

            if (receivedPacketPayload[0] == FINGERPRINT_OK)
            {
                return true;
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_COMMUNICATION)
            {
                throw new Exception("Communication error");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_LOADTEMPLATE)
            {
                throw new Exception("The template could not be read");
            }
            else if (receivedPacketPayload[0] == FINGERPRINT_ERROR_INVALIDPOSITION)
            {
                throw new Exception("Could not load template from that position");
            }
            else
            {
                throw new Exception("Unknown error " + receivedPacketPayload[0].ToString("X"));
            }
        }
    }
}
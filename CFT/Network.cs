﻿using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace CerealFileTransfer {
    class Network {
        private String portName;
        private Int32 baudrate;
        private Int32 dataBits;
        private StopBits stopBits;  // Der Anzahl der Stoppbits (Stopbits.One, StopBits.OnePointFive, StopBits.Two)
        private Parity parity;      // Festlegung der Parität (Parity.Even, Parity.Mark, Parity.None, Parity.Odd, Parity.Space)
        private Int32 bufferSize;
        private Int32 packageSize;
        private SerialPort serial;

        public Network(Int32 baudrate, Int32 bufferSize, Int32 packageSize) {
            this.portName = SerialPort.GetPortNames()[0];
            this.baudrate = baudrate;
            this.dataBits = 8;
            this.stopBits = StopBits.One;
            this.parity = Parity.None;
            this.bufferSize = bufferSize;
            this.packageSize = packageSize;

            this.serial = new SerialPort() {
                PortName = this.portName,
                BaudRate = baudrate,
                DataBits = this.dataBits,
                StopBits = this.stopBits,
                Parity = this.parity,
                ReadBufferSize = bufferSize,
                WriteBufferSize = bufferSize / 2,
                DtrEnable = true           
            };
        }

        public void Open() {
            try {
                this.serial.Open();
            } catch (Exception ex) {
                Debug.Print(ex.Message);
            }
        }

        public Byte[][] GetPackage(Int32 count) {
            Byte[][] package = new Byte[count][];
            Byte[] buffer = new Byte[this.packageSize];

            for (Int32 i = 0; i < count; i++) {
                while (this.serial.BytesToRead < this.packageSize / 2) {
                    Debug.Print(Convert.ToString(this.serial.BytesToRead) + "/" + Convert.ToString(this.packageSize / 2));
                }
                this.serial.Read(buffer, 0, 1);
                package[i] = buffer;
            }

            return package;
        }

        public void SendPackage(Byte[][] package) {
            for (Int32 i = 0; i < package.Length - 1; i++) {
                this.serial.Write(package[i], 0, package[i].Length - 1);
            }
        }
 
        public Boolean IsDataAvailable() {
            try {
                if (this.serial.BytesToRead > 0) {
                    return true;
                } else {
                    return false;
                }
            } catch (InvalidOperationException ex) {
                Debug.Print(ex.Message);
            }

            return false;
        }
    }
}

﻿using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using __Cereal__;

namespace CerealFileTransfer {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private String portName;
        private Int32 baudrate;
        private Int32 dataBits;
        private StopBits stopBits;
        private Parity parity;
        private Cereal rs232;

        public MainWindow() {
            InitializeComponent();
            this.portName = "COM1";
            this.baudrate = 9600;
            this.dataBits = 8;
            this.stopBits = StopBits.One;
            this.parity = Parity.None;
            this.rs232 = new Cereal(portName, baudrate, dataBits, stopBits, parity);
        }

        // convert control and data to a package ready to send
        Byte[] StringToPackage(String control, String data) {
            Byte[] controlBytes = new Byte[4];
            System.Buffer.BlockCopy(control.ToCharArray(), 0,
                                    controlBytes, 0, 4);
            Int32 dataSize = data.Length * sizeof(Char);
            Byte[] dataSizeBytes = new Byte[4];
            dataSizeBytes = BitConverter.GetBytes(dataSize);
            Byte[] dataBytes = new Byte[dataSize];
            System.Buffer.BlockCopy(data.ToCharArray(), 0,
                                    dataBytes, 0, 4);

            Byte[] package = new Byte[4 + 4 + dataSize];
            package = controlBytes.Concat(dataSizeBytes.Concat(dataBytes).ToArray()).ToArray();
            /*package = controlBytes;
            package.AddRange(dataSizeBytes);
            package.AddRange(dataBytes);*/

            return package;
        }

        private void CFT_Loaded(Object sender, RoutedEventArgs e) {
            /* open COM1
            Display a Error MessageBox when open() return false
                OK to retry opening COM1
                CANCEL to close the program
            */
            //bool retryCOM1 = true;
            //while (retryCOM1) {
            while(true) {
                if(!rs232.Open()) {
                    switch(MessageBox.Show("ERROR: Can't open " + portName + ".\n" +
                                            "Maybe " + portName + " is in use or does not exist\n" +
                                            "Retry opening " + portName + "?",
                                            "Error opening " + portName,
                                            MessageBoxButton.OKCancel,
                                            MessageBoxImage.Error,
                                            MessageBoxResult.OK)) {
                        case MessageBoxResult.OK:
                            break;
                        case MessageBoxResult.Cancel:
                            return;
                    }
                } else { break; }
            }
            rtb_Log.AppendText("[  OK  ] Port " + portName + " opened\n");

            rs232.SetDTR(true); // we're ready to work
            rtb_Log.AppendText("[ INFO ] Terminal ready\n");
            // are you ready too?
            while(!rs232.IsDCD()) {
                switch(MessageBox.Show("ERROR: Cannot connect with Partner\n" +
                                       "Maybe the cable is not connected correctly?",
                                       "Error connecting to Partner",
                                       MessageBoxButton.OKCancel,
                                       MessageBoxImage.Error,
                                       MessageBoxResult.OK)) {
                    case MessageBoxResult.OK:
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                } // we're waiting
            }
            rtb_Log.AppendText("[  OK  ] Partner ready\n");
            // now it is the GUIs turn, we're done here
        }
    }
}

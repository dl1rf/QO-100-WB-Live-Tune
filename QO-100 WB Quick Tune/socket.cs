﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace QO_100_WB_Quick_Tune
{




    class socket
    {
        public Action<ushort[]> callback;

        private WebSocket ws;       //websocket client
        public bool connected;
        private ushort[] fft_data;

        public DateTime lastdata;

        public socket()
        {
            connected = false;
            
        }

        public void start()
        {

            if (!connected)
            {
                Console.WriteLine(connected);
                Console.WriteLine("Try connect..\n");
               // System.Threading.Thread.Sleep(500);     //can't catch exception from websocket!?, slow down retries if no network

                ws = new WebSocket("wss://eshail.batc.org.uk/wb/fft", "fft_m0dtslivetune");
                ws.OnMessage += (ss, ee) => NewData(ee.RawData);
                ws.OnOpen += (ss, ee) => { connected = true; Console.WriteLine("Connected.\n"); };
                ws.OnClose += (ss, ee) => { connected = false; };
                ws.Connect();
                lastdata = DateTime.Now;
            }
            
        }

        public void stop()
        {
            if (connected)
            {
                ws.Close();
                connected = false;
            }

        }



            private void NewData(byte[] data)
        {
            //Console.WriteLine("newdata\n");
            //Console.WriteLine(data[0]);

            lastdata = DateTime.Now;

            fft_data = new UInt16[data.Length / 2];


            //unpack bytes to unsigned short int values
            int n = 0;
            byte[] buf = new byte[2];

            for (int i = 0; i < data.Length; i += 2)
            {
                buf[0] = data[i];
                buf[1] = data[i + 1];
                fft_data[n] = BitConverter.ToUInt16(buf, 0);
                n++;
            }
            callback(fft_data);
            //Console.WriteLine(".");

       
        }
    }

    

}

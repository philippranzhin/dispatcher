

namespace DispatcherDesktop.Domain.Driver
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;
    using System.Threading;
    using Modbus.IO;

    class SerialAdapter : IStreamResource, IDisposable
    {
        private SerialPort serialPort;

        private readonly List<byte> wroteBytes = new List<byte>();

        public SerialAdapter(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            this.serialPort.NewLine = "\r\n";
        }

        public int InfiniteTimeout => -1;

        public int ReadTimeout
        {
            get => this.serialPort.ReadTimeout;
            set => this.serialPort.ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => this.serialPort.WriteTimeout;
            set => this.serialPort.WriteTimeout = value;
        }

        public void DiscardInBuffer()
        {
            try
            {
                this.serialPort.DiscardInBuffer();
            }
            catch
            {
                // ignored
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            Thread.Sleep(200);

            try
            {
                var internalCount = count + this.wroteBytes.Count;

                var internalBuffer = new byte[internalCount];

                this.serialPort.Read(internalBuffer, 0, internalCount);
                var enchoback = false;

                if (this.wroteBytes.Any())
                {
                    enchoback = true;
                    var i = 0;

                    foreach (var @byte in this.wroteBytes)
                    {
                        if (internalBuffer[i] != @byte)
                        {
                            enchoback = false;
                            break;
                        }

                        i++;
                    }
                }

                var startPosition = enchoback ? this.wroteBytes.Count : 0;

                for (var i = offset; i < count; i++)
                {
                    buffer[i] = internalBuffer[startPosition + i];
                }

                return count;
            }
            catch
            {
                return 0;
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            this.wroteBytes.Clear();

            try
            {
                var length = offset + count;

                for (var i = offset; i < length; i++)
                {
                    if (buffer.Length > i)
                    {
                        this.wroteBytes.Add(buffer[i]);
                    }
                    else
                    {
                        break;
                    }
                }

                this.serialPort.Write(buffer, offset, count);
            }
            catch
            {
                // Ignore
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.serialPort?.Dispose();
            this.serialPort = (SerialPort)null;
        }
    }
}

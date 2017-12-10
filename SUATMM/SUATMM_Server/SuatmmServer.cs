using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using SuatmmApi;
using SuatmmApi.Formats;
using SuatmmApi.Serialize;
using SuatmmServer.Processing;

namespace SUATMM
{
    enum SuatmmServerStatus
    {
        Stopped, Starting, Running, Stopping
    }

    class SuatmmServer : IDisposable
    {
        /// <summary>
        /// HTTP прослушиватель
        /// </summary>
        private HttpListener listener;

        /// <summary>
        /// Состояние сервера
        /// </summary>
        public SuatmmServerStatus Status { get; private set; }

        /// <summary>
        /// Узел
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// Порт
        /// </summary>
        public string Port { get; private set; }

        /// <summary>
        /// Поток для вывода текстовой информации
        /// </summary>
        public TextWriter Out { get; internal set; } = TextWriter.Null;


        public SuatmmServer(string host = "localhost", string port = "13000")
        {
            if (String.IsNullOrWhiteSpace(host))
                throw new ArgumentException();

            if (String.IsNullOrWhiteSpace(port))
                throw new ArgumentException();

            Host = host;
            Port = port;

            Status = SuatmmServerStatus.Stopped;

            listener = new HttpListener();
        }




        internal void Run()
        {
            Out.WriteLine("Starting...");
            Status = SuatmmServerStatus.Starting;

            //Нужна проверка адекватности итоговой строки. Пока предполагается, что администратор
            // не совсем глуп и понимает с какими параметрами запускает сервер
            string prefix = String.Format("http://{0}:{1}/", Host, Port);

            listener.Prefixes.Add(prefix);

            Out.WriteLine("Start listening on {0}", prefix);
            try
            {
                listener.Start();
            }
            catch (HttpListenerException exception)
            {
                switch (exception.ErrorCode)
                {
                    case 5: //Отказано в доступе
                        Out.WriteLine(exception.Message);
                        Out.WriteLine("Попробуйте запустить программу с правами администратора, либо явно задать разрешение для прослушивания URL: netsh http add urlacl url={0} user=ВСЕ", prefix);
                        break;

                    default:
                        Out.WriteLine(exception.Message);
                        break;
                }

                Status = SuatmmServerStatus.Stopped;

                return;
            }

            Status = SuatmmServerStatus.Running;

            listener.BeginGetContext(AcceptNewConnection, null);

            Out.WriteLine("Started");
        }

        internal void Stop()
        {
            Out.WriteLine("Stopping");
            Status = SuatmmServerStatus.Stopping;
            listener.Stop();
            Status = SuatmmServerStatus.Stopped;
            Out.WriteLine("Stopped");

        }

        void AcceptNewConnection(IAsyncResult result)
        {
            if (listener.IsListening)
                listener.BeginGetContext(AcceptNewConnection, null); //Сразу продолжаем прием новых запросов
            else
                return;

            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            Out.WriteLine("{0} {1} {2}", DateTime.Now.ToString(), request.RemoteEndPoint, request.HttpMethod);

            try
            {

                PacketSerializer serializer = PacketSerializer.Create(request.ContentType);

                if (serializer == null)
                    throw new NotSupportedException(); //вместо исключения должен быть корректный ответ об ошибке


                //Читаем что нам прислали
                Packet<IPacketContent> packet = serializer.Deserialize<IPacketContent>(request.InputStream);

                if (packet == null)
                    throw new ArgumentException();

                //Подберем обработчик
                PacketHandler handler = PacketHandler.Create(packet.Version);
                handler.Out = Out;

                //Получаем результат
                Packet<IPacketContent> responsePacket = handler.Handle(packet);

                //И отправляем обратно
                using (MemoryStream mstream = new MemoryStream())
                {
                    serializer.Serialize(responsePacket, mstream);

                    response.ContentLength64 = mstream.Length;
                    response.StatusCode = 200;

                    mstream.Position = 0;
                    mstream.CopyTo(response.OutputStream);
                }
            }
            catch (Exception ex)
            {
                Out.Write("General Exception");
                Out.Write(ex.Message);
                Out.Write(ex.StackTrace);
                response.StatusCode = 500;
            }
            finally
            {
                response.OutputStream.Close();
            }
        }


        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    listener.Close();
                    Out.Close();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion




    }
}

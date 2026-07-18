using System;
using System.IO;
using System.Text;
using System.Threading;

namespace CapaNegocio.Logging
{
    public class FileLogger
    {
        private readonly string _ruta;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public FileLogger(string rutaArchivo = null)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
                rutaArchivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bitacora.log");

            _ruta = rutaArchivo;
            try
            {
                var dir = Path.GetDirectoryName(_ruta);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            catch
            {
                // No lanzar; si falla la creación de carpeta, los intentos de escritura fallarán y serán manejados por el llamante.
            }
        }

        public void LogLine(string linea)
        {
            try
            {
                _lock.EnterWriteLock();
                using (var fs = new FileStream(_ruta, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
                {
                    sw.WriteLine(linea);
                    sw.Flush();
                    fs.Flush(true);
                }
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public string[] ReadLastLines(int maxLines = 200)
        {
            try
            {
                _lock.EnterReadLock();
                if (!File.Exists(_ruta)) return new string[0];

                var lines = new System.Collections.Generic.List<string>();
                using (var fs = new FileStream(_ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    while (!sr.EndOfStream)
                    {
                        lines.Add(sr.ReadLine());
                        if (lines.Count > maxLines) lines.RemoveAt(0);
                    }
                }
                return lines.ToArray();
            }
            catch
            {
                return new string[0];
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }
    }
}

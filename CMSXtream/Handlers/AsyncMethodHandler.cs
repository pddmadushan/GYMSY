using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMSXtream.Handlers
{
    public class AsyncMethodHandler
    {

        // Store last scan times to avoid duplicate entries
        private readonly Dictionary<string, DateTime> _lastScanTimes = new Dictionary<string, DateTime>();
        private readonly object _lockObj = new object();

        // Event Handler for Attendance Transaction
        private void axCZKEM1_OnAttTransactionEx(
            string sEnrollNumber,
            int iIsInValid,
            int iAttState,
            int iVerifyMethod,
            int iYear,
            int iMonth,
            int iDay,
            int iHour,
            int iMinute,
            int iSecond,
            int iWorkCode)
        {
            // Build timestamp
            DateTime scanTime;
            try
            {
                scanTime = new DateTime(iYear, iMonth, iDay, iHour, iMinute, iSecond);
            }
            catch
            {
                // Fallback if timestamp is invalid
                scanTime = DateTime.Now;
            }

            // Debounce logic: avoid duplicate scans in a short period
            lock (_lockObj)
            {
                if (_lastScanTimes.TryGetValue(sEnrollNumber, out DateTime lastTime))
                {
                    if ((scanTime - lastTime).TotalSeconds < 10) // Adjust debounce window if needed
                        return;
                }

                _lastScanTimes[sEnrollNumber] = scanTime;
            }

            // Offload processing to avoid blocking SDK event thread
            Task.Run(() => ProcessAttendance(sEnrollNumber, scanTime, iVerifyMethod, iAttState, iIsInValid));
        }

        private async Task ProcessAttendance(string userId, DateTime scanTime, int verifyMethod, int attState, int isInvalid)
        {
            try
            {
                // Simulate async operation (e.g., database save, API call)
                await SaveToDatabaseAsync(userId, scanTime, verifyMethod, attState, isInvalid);

                // Optional: update UI (must be on UI thread)
                //this.Invoke((MethodInvoker)(() =>
                //{
                //    lblStatus.Text = $"User {userId} verified at {scanTime:HH:mm:ss}";
                //    lblStatus.ForeColor = Color.Green;
                //}));
            }
            catch (Exception ex)
            {
                // Log exception
                //Console.WriteLine($"[ERROR] Failed to process attendance: {ex.Message}");
            }
        }

        private async Task SaveToDatabaseAsync(string userId, DateTime scanTime, int verifyMethod, int attState, int isInvalid)
        {
            // Simulate async DB delay
            await Task.Delay(100);

            // Here you can add your logic to store attendance in DB
            //Console.WriteLine($"[LOG] Attendance: {userId} at {scanTime}");
        }

        //private void LogToFile(string message)
        //{
        //    string path = "attendance_log.txt";
        //    File.AppendAllText(path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n");
        //}

    }
}

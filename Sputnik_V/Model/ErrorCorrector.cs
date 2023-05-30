using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Sputnik_V.Model
{
    public class ErrorCorrector
    {
        private double _lastX;
        private double _lastW;
        private int _mode;
        private int _errorType;
        private int _tError;
        private int _fixStatus;
        private bool _flag;
        private double _timeOut;
        private double _T;
        private double _dt;
        private List<double> _listX = new List<double>();
        private List<double> _listW = new List<double>();
        private List<double> _listX_T = new List<double>();
        private List<double> _listW_T = new List<double>();
        private FlyWheelEngine _engine;

        public ErrorCorrector(FlyWheelEngine engine, int mode = 0, int isStick = 0, int t_error = 60, int fix_status = 0, double timeout = 10, double t = 2, double dt = 0.01)
        {
            _engine = engine;
            _mode = mode;
            _errorType = isStick;
            _tError = t_error;
            _fixStatus = fix_status;
            _timeOut = timeout = 60;
            _T = t;
            _dt = dt;
        }

        public (double, double, double) CorrectError(double x, double dx, double w, double dw, double prevX, double prevDx, double prevW, double t, double dt, int steps, Dictionary<string, List<double>> valuesDict)
        {
            var (t_x, t_tx) = CorrectX(x, dx, w, dw, t, prevX);
            var (t_w, t_tw) = CorrectW(x, dx, w, dw, t, prevW);
            Console.WriteLine(t_tx == t_tw);
            return (t_x, t_w, t_tx);
        }

        private (double, double) CorrectX(double x, double dx, double w, double dw, double t, double prevX)
        {
            if (_listX.Count < 1)
            {
                _listX.Append(x);
                _listX_T.Append(t);
                return (x, t);
            }
            if (_listX.Count > 1 && Math.Abs(((x - _listX.Last()) / (t - _listX_T.Last()))) >
                Math.Abs(((_listX.Last() - _listX[_listX.Count - 2]) / (_listX_T.Last() - _listX_T[_listX_T.Count - 2]))) * 3)
            {
                _engine.Overclocking(ref prevX, ref dx, ref t, _dt, ref w, ref dw, (int)(_T / _dt), null, false);
                _listX.RemoveAt(0);
                _listX_T.RemoveAt(0);
                _listX.Append(prevX);
                _listX_T.Append(t);
                return (prevX, t);
            }
            if (_listX.Count >= 3 && x == _listX.Last() && x == _listX[_listX.Count - 2])
            {
                _engine.Overclocking(ref prevX, ref dx, ref t, _dt, ref w, ref dw, (int)(_T / _dt), null, false);
                _listX.RemoveAt(0);
                _listX_T.RemoveAt(0);
                _listX.Append(prevX);
                _listX_T.Append(t);
                return (prevX, t);
            }

            if (_listX.Count < 10)
            {
                _listX.Append(x);
                _listX_T.Append(t);
            }
            else
            {
                _listX.RemoveAt(0);
                _listX_T.RemoveAt(0);
                _listX.Append(x);
                _listX_T.Append(t);
            }
            return (x, t);
        }

        private (double, double) CorrectW(double x, double dx, double w, double dw, double t, double prevW)
        {
            if (_listW.Count < 1)
            {
                _listW.Append(w);
                _listW_T.Append(t);
                return (w, t);
            }
            if (_listW.Count > 1 && Math.Abs(((w - _listW.Last()) / (t - _listW_T.Last()))) >
                    Math.Abs(((_listW.Last() - _listW[_listW.Count - 2]) / (_listW_T.Last() - _listW_T[_listW_T.Count - 2]))) * 3)
            {
                _engine.Overclocking(ref x, ref dx, ref t, _dt, ref prevW, ref dw, (int)(_T / _dt), null, false);
                _listW.RemoveAt(0);
                _listW_T.RemoveAt(0);
                _listW.Append(prevW);
                _listW_T.Append(t);
                return (prevW, t);
            }
            if (_listW.Count >= 3 && w == _listW.Last() && w == _listW[_listW.Count - 2])
            {
                _engine.Overclocking(ref x, ref dx, ref t, _dt, ref prevW, ref dw, (int)(_T / _dt), null, false);
                _listW.RemoveAt(0);
                _listW_T.RemoveAt(0);
                _listW.Append(prevW);
                _listW_T.Append(t);
                return (prevW, t);
            }
            if (_listW.Count < 10)
            {
                _listW.Append(w);
                _listW_T.Append(t);
            }
            else
            {
                _listW.RemoveAt(0);
                _listW_T.RemoveAt(0);
                _listW.Append(w);
                _listW_T.Append(t);
            }
            return (w, t);
        }
    }
}

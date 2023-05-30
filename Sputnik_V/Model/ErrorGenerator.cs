using OxyPlot;
using Sputnik_V.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sputnik_V.Model
{
    public class ErrorGenerator
    {
        private double _lastX;
        private double _lastW;
        private int _mode;
        private int _errorType;
        private int _tError;
        private bool _flag;
        private double _timeOut;

        public ErrorGenerator(int mode = 0, int isStick = 0, int t_error = 60, int fix_status = 0, double timeout = 10)
        {
            _mode = mode;
            _errorType = isStick;
            _tError = t_error;
            _timeOut = timeout = 60;
        }
        public (double, double) GenerateError(double x, double w, double currentTime, Dictionary<string, List<double>> valuesDict) 
        {
            double x_base = 0, w_base = 0;
            if (_mode == ErrorModeConsts.None || currentTime < _tError || currentTime > _tError + _timeOut || _flag)
            {
                _lastX = x_base = x;
                _lastW = w_base = w;
                return (x, w);
            }
            else if (_mode == ErrorModeConsts.AngPosSensorError && currentTime <= _tError + _timeOut)
            {
                if(_errorType == ErrorTypeConsts.ZeroingType)
                    x_base = 0;
                if (_errorType == ErrorTypeConsts.StickingType)
                    x_base = _lastX;
                if (_errorType == ErrorTypeConsts.BigSingleErrType && !_flag)
                {
                    x_base = 0.2;
                    _flag = true;
                }
                _lastX = x_base;
                _lastW = w_base = w;
            }
            else if (_mode == ErrorModeConsts.AngVelSensorError && currentTime <= _tError + _timeOut)
            {
                if (_errorType == ErrorTypeConsts.ZeroingType)
                    w_base = 0;
                if (_errorType == ErrorTypeConsts.StickingType)
                    w_base = _lastW;
                if (_errorType == ErrorTypeConsts.BigSingleErrType && !_flag)
                {
                    w_base = 0.2;
                    _flag = true;
                }
                _lastX = x_base = x;
                _lastW = w_base;
            }
            
            //if (_mode == ErrorModeConsts.AngPosSensorError && _fixStatus == 1 && currentTime >= _tError + _timeOut && _errorType != 2)
            //    x_base = valuesDict["cx"].Last();
            //if (_mode == ErrorModeConsts.AngVelSensorError && _fixStatus == 1 && currentTime >= _tError + _timeOut && _errorType != 2)
            //    w_base = valuesDict["cdx"].Last();
            return (x_base, w_base);
        }
    }
}

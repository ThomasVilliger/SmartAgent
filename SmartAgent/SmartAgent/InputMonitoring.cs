using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml;
using DataAccess;

namespace SmartAgent
{
    // this class handels the inpunt monitoring
    public class InputMonitoring
    {
        private IOutput _output;
        private InputMonitoringConfiguration _config;
        private ThreadPoolTimer _timerCycleTimeOut1;
        private ThreadPoolTimer _timerCycleTimeOut2;
        private IDevice _device;

        public InputMonitoring(IDevice device, InputMonitoringConfiguration inputSignalMonitoringConfiguration)
        {
            _device = device;
            _config = inputSignalMonitoringConfiguration;

            _device.Inputs[_config.InputPin].InputChanged += InputInterpretation;

            _output = _device.Outputs[_config.OutputPin];


            _timerCycleTimeOut1 = ThreadPoolTimer.CreatePeriodicTimer(MachineNotAlive,
                                       TimeSpan.FromHours(24));

            _timerCycleTimeOut2 = ThreadPoolTimer.CreatePeriodicTimer(ReciveNoCycles,
                                      TimeSpan.FromSeconds(15));
        }

        private void MachineNotAlive(ThreadPoolTimer timer)
        {
            _timerCycleTimeOut1.Cancel();
            _timerCycleTimeOut1 = ThreadPoolTimer.CreatePeriodicTimer(MachineNotAlive,
                                        TimeSpan.FromHours(24));
        }

        private void ReciveNoCycles(ThreadPoolTimer timer)
        {
            SetOutput(false);
        }

        private void InputInterpretation(IInput input)
        {
            SetOutput(true);
            ResetTimerCycleTimeOut();
        }

        public void ResetTimerCycleTimeOut()
        {
            _timerCycleTimeOut2.Cancel();
            _timerCycleTimeOut2 = ThreadPoolTimer.CreatePeriodicTimer(ReciveNoCycles,
                                        TimeSpan.FromSeconds(15));

            _timerCycleTimeOut1.Cancel();
            _timerCycleTimeOut1 = ThreadPoolTimer.CreatePeriodicTimer(MachineNotAlive,
                                        TimeSpan.FromHours(24));
        }

        private void SetOutput(bool outputValue)
        {
            _output.State = outputValue;
        }

        public void StopInputMonitoring()
        {
            _device.Inputs[_config.InputPin].InputChanged -= InputInterpretation;
            _timerCycleTimeOut1.Cancel();
            _timerCycleTimeOut2.Cancel();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml;
using DataStorageLibrary;

namespace PiFace_II
{
  public  class InputMonitoring 
    {

        private IOutput output;

        private InputMonitoringConfiguration config;

        private ThreadPoolTimer timerCycleTimeOut1;
        private ThreadPoolTimer timerCycleTimeOut2;
        private IDevice _device;

        

        public InputMonitoring(IDevice device, InputMonitoringConfiguration inputSignalMonitoringConfiguration)
        {
            _device = device;
            config = inputSignalMonitoringConfiguration;

            _device.Inputs[config.InputPin].InputChanged += InputInterpretation;

            output = _device.Outputs[config.OutputPin];

   
            timerCycleTimeOut1 = ThreadPoolTimer.CreatePeriodicTimer(machineNotAlive,
                                       TimeSpan.FromHours(24));

            timerCycleTimeOut2 = ThreadPoolTimer.CreatePeriodicTimer(reciveNoCycles,
                                       TimeSpan.FromSeconds(15));

        }


        private void machineNotAlive(ThreadPoolTimer timer)
        {
            sendEmailToSpecifiedAdressList();

            timerCycleTimeOut1.Cancel();
            timerCycleTimeOut1 = ThreadPoolTimer.CreatePeriodicTimer(machineNotAlive,
                                        TimeSpan.FromHours(24));
        }

        private void reciveNoCycles(ThreadPoolTimer timer)
        {
            setOutput(false);
        }


        private void InputInterpretation(IInput input) 
        {
            setOutput(true);
            ResetTimerCycleTimeOut();
        }


        public void ResetTimerCycleTimeOut()
        {
            timerCycleTimeOut2.Cancel();
            timerCycleTimeOut2 = ThreadPoolTimer.CreatePeriodicTimer(reciveNoCycles,
                                        TimeSpan.FromSeconds(15));

            timerCycleTimeOut1.Cancel();
            timerCycleTimeOut1 = ThreadPoolTimer.CreatePeriodicTimer(machineNotAlive,
                                        TimeSpan.FromHours(24));
        }

        private void setOutput(bool outputValue)
        {
            output.State = outputValue;
        }


        public void StopInputMonitoring()
        {
            _device.Inputs[config.InputPin].InputChanged -= InputInterpretation;
            timerCycleTimeOut1.Cancel();
            timerCycleTimeOut2.Cancel();
        }

            private void sendEmailToSpecifiedAdressList()
        {
            // not implemented
        }
    }
}

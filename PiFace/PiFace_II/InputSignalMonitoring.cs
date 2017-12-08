using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml;

namespace PiFace_II
{
  public  class InputSignalMonitoring 
    {

        private IOutput output;

        private InputSignalMonitoringConfiguration config;

        private ThreadPoolTimer timerCycleTimeOut1;
        private ThreadPoolTimer timerCycleTimeOut2;

        

        public InputSignalMonitoring(IDevice device, InputSignalMonitoringConfiguration inputSignalMonitoringConfiguration)
        {
            config = inputSignalMonitoringConfiguration;

            device.Inputs[config.InputPinToMonitor].InputChanged += InputInterpretation;

            output = device.Outputs[config.OutputPinForNotification];

   
            timerCycleTimeOut1 = ThreadPoolTimer.CreatePeriodicTimer(cycleMachineNotAlive,
                                       TimeSpan.FromHours(24));

            timerCycleTimeOut2 = ThreadPoolTimer.CreatePeriodicTimer(reciveNoCycles,
                                       TimeSpan.FromSeconds(15));

        }


        private void cycleMachineNotAlive(ThreadPoolTimer timer)
        {
            sendEmailToSpecifiedAdressList();

            timerCycleTimeOut1.Cancel();
            timerCycleTimeOut1 = ThreadPoolTimer.CreatePeriodicTimer(cycleMachineNotAlive,
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
            timerCycleTimeOut1 = ThreadPoolTimer.CreatePeriodicTimer(cycleMachineNotAlive,
                                        TimeSpan.FromHours(24));
        }

        private void setOutput(bool outputValue)
        {
            output.State = outputValue;
        }

            private void sendEmailToSpecifiedAdressList()
        {
            // not implemented
        }
    }
}

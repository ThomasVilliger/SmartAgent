using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml;

namespace PiFace_II
{
  public  class InputMonitoring 
    {

        private IOutput output;

        private InputMonitoringConfiguration config;

        private ThreadPoolTimer timerCycleTimeOut1;
        private ThreadPoolTimer timerCycleTimeOut2;

        

        public InputMonitoring(IDevice device, InputMonitoringConfiguration inputSignalMonitoringConfiguration)
        {
            config = inputSignalMonitoringConfiguration;

            device.Inputs[config.InputPin].InputChanged += InputInterpretation;

            output = device.Outputs[config.OutputPin];

   
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

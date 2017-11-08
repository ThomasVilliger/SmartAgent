using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace PIFace_Digital_II
{
  public  class InputSignalMonitoring : InputSignalMonitoringConfiguration
    {
        private DispatcherTimer timerCycleTimeOut1;
        private DispatcherTimer timerCycleTimeOut2;
        private IOutput iOutput;

        public InputSignalMonitoring(IDevice iDevice, InputSignalMonitoringConfiguration inputSignalMonitoringConfiguration)
        {
            InputPinToMonitor = inputSignalMonitoringConfiguration.InputPinToMonitor;
            EmailAddressListForNotification = inputSignalMonitoringConfiguration.EmailAddressListForNotification;
            OutputPinForNotification = inputSignalMonitoringConfiguration.OutputPinForNotification;

            iDevice.Inputs[InputPinToMonitor].InputChanged += InputInterpretation;

            iOutput = iDevice.Outputs[OutputPinForNotification];


            timerCycleTimeOut1 = new DispatcherTimer();
            timerCycleTimeOut1.Interval = TimeSpan.FromHours(24); //after xHours no Cycle send Mail
            timerCycleTimeOut1.Tick += cycleMachineNotAlive;
            timerCycleTimeOut1.Start();
            

            timerCycleTimeOut2 = new DispatcherTimer();
            timerCycleTimeOut2.Interval = TimeSpan.FromSeconds(15); //after xSeconds no Cycle set Output false
            timerCycleTimeOut2.Tick += reciveNoCycles;
            timerCycleTimeOut2.Start();

        }


        private void cycleMachineNotAlive(object sender, object e)
        {
            sendEmailToSpecifiedAdressList();

            timerCycleTimeOut1.Stop();
            timerCycleTimeOut1.Start();
        }

        private void reciveNoCycles(object sender, object e)
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
            timerCycleTimeOut1.Stop();
            timerCycleTimeOut1.Start();

            timerCycleTimeOut2.Stop();
            timerCycleTimeOut2.Start();
        }

        private void setOutput(bool outputValue)
        {
            iOutput.OutputValue = outputValue;
        }


        private void sendEmailToSpecifiedAdressList()
        {

        }
    }
}

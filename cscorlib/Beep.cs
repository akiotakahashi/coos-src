using System;
using CooS.Architectures.IA32;
using CooS.Drivers.Controllers;

namespace CooS {
    
	public class Beep {
		// ビープ音を鳴らします。

		public static void TurnOn(int frequency) {
			IntervalTimer.Beep.SetFrequency((uint)frequency);
			IntervalTimer.SpeakerEnabled = true;
		}
		public static void TurnOff() {
			IntervalTimer.SpeakerEnabled = false;
		}
		public static void Play(int frequency) {
			TurnOn(frequency);
			// Wait
			Instruction.hlt();
			Instruction.hlt();
			Instruction.hlt();
			Instruction.hlt();
			Instruction.hlt();
			Instruction.hlt();
			Instruction.hlt();
			Instruction.hlt();
			//
			TurnOff();
		}
		public static void PlayWakeupTone() {
			TurnOn(1046);
			Instruction.hlt();
			Instruction.hlt();
			TurnOn(1318);
			Instruction.hlt();
			Instruction.hlt();
			TurnOn(1760);
			Instruction.hlt();
			Instruction.hlt();
			TurnOff();
		}
	}

}

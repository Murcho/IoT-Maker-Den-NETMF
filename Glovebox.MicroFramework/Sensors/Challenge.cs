using System;
using Microsoft.SPOT;
using Glovebox.MicroFramework.Base;
using Glovebox.MicroFramework.IoT;

namespace Glovebox.MicroFramework.Sensors {
    public class Challenge : SensorBase {
        IotBase iotItem;

        public Challenge(IotBase iotItem, string userDefinedType)
            : base(userDefinedType, "n", ValuesPerSample.One, 60000, "challenge") {
                this.iotItem = iotItem;

            StartMeasuring();
        }

        protected override void Measure(double[] value) {
            //value[0] = IotList.ActionCountByName(iotName);
            value[0] = iotItem.TotalActionCount;
        }

        protected override string GeoLocation() {
            return string.Empty;
        }

        public override double Current {
            get { return TotalActionCount; }
        }

        protected override void SensorCleanup() {
        }
    }
}

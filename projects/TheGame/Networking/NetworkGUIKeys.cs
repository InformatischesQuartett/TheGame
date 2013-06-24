using Fusee.Engine;

namespace Examples.TdM.Networking
{
    static class NetworkGUIKeys
    {
        public static string KeyInput(string oldIp)
        {
            var key = "";

            if (Input.Instance.IsKeyDown(KeyCodes.D0) || Input.Instance.IsKeyDown(KeyCodes.NumPad0))
                key = "0";
            
            if (Input.Instance.IsKeyDown(KeyCodes.D1) || Input.Instance.IsKeyDown(KeyCodes.NumPad1))
                key = "1";
            
            if (Input.Instance.IsKeyDown(KeyCodes.D2) || Input.Instance.IsKeyDown(KeyCodes.NumPad2))
                key = "2";
            
            if (Input.Instance.IsKeyDown(KeyCodes.D3) || Input.Instance.IsKeyDown(KeyCodes.NumPad3))
                key = "3";
            
            if (Input.Instance.IsKeyDown(KeyCodes.D4) || Input.Instance.IsKeyDown(KeyCodes.NumPad4))
                key = "4";
            
            if (Input.Instance.IsKeyDown(KeyCodes.D5) || Input.Instance.IsKeyDown(KeyCodes.NumPad5))
                key = "5";
            
            if (Input.Instance.IsKeyDown(KeyCodes.D6) || Input.Instance.IsKeyDown(KeyCodes.NumPad6))
                key = "6";

            if (Input.Instance.IsKeyDown(KeyCodes.D7) || Input.Instance.IsKeyDown(KeyCodes.NumPad7))
                key = "7";

            if (Input.Instance.IsKeyDown(KeyCodes.D8) || Input.Instance.IsKeyDown(KeyCodes.NumPad8))
                key = "8";

            if (Input.Instance.IsKeyDown(KeyCodes.D9) || Input.Instance.IsKeyDown(KeyCodes.NumPad9))
                key = "9";

            if (Input.Instance.IsKeyDown(KeyCodes.OemPeriod))
                key = ".";

            if (Input.Instance.IsKeyDown(KeyCodes.Back))
                if (oldIp.Length > 0)
                    oldIp = (oldIp == "Discovery?") ? "" : oldIp.Remove(oldIp.Length - 1);
            
            if (key != "")
                if (oldIp == "Discovery?")
                    oldIp = key;
                else
                    oldIp += key;

            return oldIp;
        }
    }
}

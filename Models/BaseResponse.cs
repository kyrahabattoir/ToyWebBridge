/* SPDX-License-Identifier: CC-BY-NC-SA-4.0 */
using System;

namespace ToyWebBridge.Models
{
    public class BasedResponse //yes.
    {
        public string Action { get; }
        public BasedResponse(Type action)
        {
            Action = action.Name;
        }
        public BasedResponse(string action)
        {
            Action = action;
        }
    }
}

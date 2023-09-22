// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// DEPRECATED. Will be removed from headers at version v49.
  public class CalApplicationFinalized
  {
    /// Number of milliseconds to wait before launching the app. Launcher should
    /// display a countdown to the user while waiting.
    public readonly int CountdownMS;
    /// ID of the application we should launch into.
    public readonly UInt64 ID;
    /// Launch argument generated by the CAL system. This must be passed unmodified
    /// to the application as an intent extra or command line argument
    public readonly string LaunchDetails;


    public CalApplicationFinalized(IntPtr o)
    {
      CountdownMS = CAPI.ovr_CalApplicationFinalized_GetCountdownMS(o);
      ID = CAPI.ovr_CalApplicationFinalized_GetID(o);
      LaunchDetails = CAPI.ovr_CalApplicationFinalized_GetLaunchDetails(o);
    }
  }

}
